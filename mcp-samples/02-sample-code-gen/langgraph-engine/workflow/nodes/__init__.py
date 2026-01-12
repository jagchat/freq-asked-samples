"""
Workflow Nodes Package

This package contains all the LangGraph workflow nodes for code generation.

Each node is a function that:
1. Takes the current state as input
2. Performs a specific operation (parse intent, select examples, etc.)
3. Returns updates to add to the state

Nodes:
- parse_intent: Extracts structured info from user's natural language request
- select_examples: Loads relevant code examples from the library
- plan_generation: Decides which files to generate
- generate_code: Creates actual code using LLM and examples
"""

from workflow.nodes.parse_intent import parse_intent
from workflow.nodes.select_examples import select_examples
from workflow.nodes.plan_generation import plan_generation
from workflow.nodes.generate_code import generate_code

__all__ = [
    "parse_intent",
    "select_examples",
    "plan_generation",
    "generate_code",
]
