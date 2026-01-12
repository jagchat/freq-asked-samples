"""
Configuration Module

This module loads configuration from environment variables and creates
the appropriate LLM client based on the selected provider.

Supported providers:
- OpenAI (GPT-4, GPT-3.5-turbo)
- Anthropic (Claude)
- Ollama (Local models)
- LiteLLM (Unified interface for 100+ providers)
"""

import os
from typing import Literal
from pydantic_settings import BaseSettings
from langchain_core.language_models.chat_models import BaseChatModel
from langchain_openai import ChatOpenAI
from langchain_anthropic import ChatAnthropic
from langchain_ollama import ChatOllama
from langchain_community.chat_models import ChatLiteLLM
import logging

logger = logging.getLogger(__name__)


# ============================================================================
# Settings Model
# ============================================================================

class Settings(BaseSettings):
    """
    Application settings loaded from environment variables.

    Pydantic automatically reads from:
    1. Environment variables
    2. .env file (if python-dotenv is configured)

    - Pydantic automatically validates types (ensures llm_provider is one of the three allowed values)
    - Provides defaults if variables are not set (type checking ensures you can't set invalid values)

    Example .env file:
        LLM_PROVIDER=openai
        OPENAI_API_KEY=sk-...
        OPENAI_MODEL=gpt-4
    """

    # LLM Provider Selection
    llm_provider: Literal["openai", "anthropic", "ollama", "litellm"] = "openai"

    # OpenAI Configuration
    openai_api_key: str | None = None
    openai_model: str = "gpt-4"

    # Anthropic Configuration
    anthropic_api_key: str | None = None
    anthropic_model: str = "claude-3-5-sonnet-20241022"

    # Ollama Configuration
    ollama_base_url: str = "http://localhost:11434"
    ollama_model: str = "codellama"

    # LiteLLM Configuration
    litellm_api_key: str | None = None
    litellm_model: str = "gpt-4"
    litellm_base_url: str | None = None  # Optional: custom proxy URL

    # Server Configuration
    server_host: str = "127.0.0.1"
    server_port: int = 8000

    # Generation Settings
    max_examples_tokens: int = 6000
    llm_temperature: float = 0.2
    examples_path: str = "../code-gen-source-samples"  # Path to code examples library

    class Config:
        # Tell Pydantic to read from .env file
        env_file = ".env"
        env_file_encoding = "utf-8"
        # Make field names case-insensitive when reading from env
        case_sensitive = False


# ============================================================================
# Global Settings Instance
# ============================================================================

# Load settings once when module is imported
# This reads from environment variables and .env file
settings = Settings()


# ============================================================================
# LLM Client Factory
# ============================================================================

def get_llm() -> BaseChatModel:
    """
    Create and return the appropriate LLM client based on configuration.

    This function:
    1. Checks which provider is configured (settings.llm_provider)
    2. Creates the corresponding LangChain chat model
    3. Configures it with API keys, model name, and temperature

    Returns:
        BaseChatModel: A LangChain chat model ready to use

    Raises:
        ValueError: If provider is invalid or required config is missing
    """

    provider = settings.llm_provider.lower()
    temperature = settings.llm_temperature

    logger.info(f"Initializing LLM provider: {provider}")

    # -------------------------------------------------------------------------
    # OpenAI Provider
    # -------------------------------------------------------------------------
    if provider == "openai":
        if not settings.openai_api_key:
            raise ValueError(
                "OpenAI API key not found. "
                "Set OPENAI_API_KEY in your .env file or environment variables."
            )

        logger.info(f"Using OpenAI model: {settings.openai_model}")

        return ChatOpenAI(
            api_key=settings.openai_api_key,
            model=settings.openai_model,
            temperature=temperature,
        )

    # -------------------------------------------------------------------------
    # Anthropic Provider
    # -------------------------------------------------------------------------
    elif provider == "anthropic":
        if not settings.anthropic_api_key:
            raise ValueError(
                "Anthropic API key not found. "
                "Set ANTHROPIC_API_KEY in your .env file or environment variables."
            )

        logger.info(f"Using Anthropic model: {settings.anthropic_model}")

        return ChatAnthropic(
            api_key=settings.anthropic_api_key,
            model=settings.anthropic_model,
            temperature=temperature,
        )

    # -------------------------------------------------------------------------
    # Ollama Provider (Local)
    # -------------------------------------------------------------------------
    elif provider == "ollama":
        logger.info(f"Using Ollama model: {settings.ollama_model}")
        logger.info(f"Ollama base URL: {settings.ollama_base_url}")

        return ChatOllama(
            base_url=settings.ollama_base_url,
            model=settings.ollama_model,
            temperature=temperature,
        )

    # -------------------------------------------------------------------------
    # LiteLLM Provider (Unified interface for 100+ providers)
    # -------------------------------------------------------------------------
    elif provider == "litellm":
        logger.info(f"Using LiteLLM model: {settings.litellm_model}")

        # Build kwargs for ChatLiteLLM
        litellm_kwargs = {
            "model": settings.litellm_model,
            "temperature": temperature,
        }

        # Add API key if provided
        if settings.litellm_api_key:
            litellm_kwargs["api_key"] = settings.litellm_api_key

        # Add custom base URL if provided (for proxies)
        if settings.litellm_base_url:
            litellm_kwargs["api_base"] = settings.litellm_base_url
            logger.info(f"LiteLLM base URL: {settings.litellm_base_url}")

        return ChatLiteLLM(**litellm_kwargs)

    # -------------------------------------------------------------------------
    # Invalid Provider
    # -------------------------------------------------------------------------
    else:
        raise ValueError(
            f"Invalid LLM provider: {provider}. "
            f"Must be one of: openai, anthropic, ollama, litellm"
        )


# ============================================================================
# Configuration Getter
# ============================================================================

def get_config() -> Settings:
    """
    Get the global settings instance.

    Returns:
        Settings: The global settings object with all configuration

    This function provides access to the settings object for use in other modules.
    It's particularly useful for accessing examples_path and max_examples_tokens
    in the workflow nodes.

    Example:
        from config import get_config
        config = get_config()
        print(config.examples_path)
        print(config.max_examples_tokens)
    """
    return settings


# ============================================================================
# Configuration Summary
# ============================================================================

def print_config_summary():
    """
    Print a summary of the current configuration.
    Useful for debugging and verifying settings at startup.
    """
    logger.info("=" * 60)
    logger.info("Configuration Summary")
    logger.info("=" * 60)
    logger.info(f"LLM Provider: {settings.llm_provider}")

    if settings.llm_provider == "openai":
        logger.info(f"OpenAI Model: {settings.openai_model}")
        logger.info(f"API Key: {'✓ Set' if settings.openai_api_key else '✗ Missing'}")
    elif settings.llm_provider == "anthropic":
        logger.info(f"Anthropic Model: {settings.anthropic_model}")
        logger.info(f"API Key: {'✓ Set' if settings.anthropic_api_key else '✗ Missing'}")
    elif settings.llm_provider == "ollama":
        logger.info(f"Ollama Model: {settings.ollama_model}")
        logger.info(f"Base URL: {settings.ollama_base_url}")
    elif settings.llm_provider == "litellm":
        logger.info(f"LiteLLM Model: {settings.litellm_model}")
        logger.info(f"API Key: {'✓ Set' if settings.litellm_api_key else '✗ Missing'}")
        if settings.litellm_base_url:
            logger.info(f"Base URL: {settings.litellm_base_url}")

    logger.info(f"Temperature: {settings.llm_temperature}")
    logger.info(f"Max Examples Tokens: {settings.max_examples_tokens}")
    logger.info(f"Examples Path: {settings.examples_path}")
    logger.info(f"Server: {settings.server_host}:{settings.server_port}")
    logger.info("=" * 60)


# ============================================================================
# Module-level test
# ============================================================================

if __name__ == "__main__":
    """
    Test the configuration module.
    Run with: python config.py
    """
    logging.basicConfig(level=logging.INFO)

    print("\n" + "=" * 60)
    print("Testing Configuration Module")
    print("=" * 60 + "\n")

    # Print configuration summary
    print_config_summary()

    # Try to create LLM client
    print("\nAttempting to create LLM client...\n")
    try:
        llm = get_llm()
        logger.info(f"✓ Successfully created LLM client: {type(llm).__name__}")
        logger.info(f"✓ Model: {llm.model_name if hasattr(llm, 'model_name') else llm.model}")
    except Exception as e:
        logger.error(f"✗ Failed to create LLM client: {str(e)}")
        logger.info("\nMake sure you have:")
        logger.info("1. Created a .env file (copy from .env.example)")
        logger.info("2. Set the appropriate API keys")
        logger.info("3. Selected a valid LLM_PROVIDER")


"""
Usage Examples:
---------------

1. In your code, import the settings and LLM factory:

   from config import settings, get_llm

   # Access settings
   print(f"Using provider: {settings.llm_provider}")

   # Create LLM client
   llm = get_llm()
   response = llm.invoke("Hello, how are you?")

2. Change provider by setting environment variable:

   # In .env file:
   LLM_PROVIDER=anthropic
   ANTHROPIC_API_KEY=sk-ant-...
   ANTHROPIC_MODEL=claude-3-5-sonnet-20241022

3. Test configuration:

   python config.py
"""
