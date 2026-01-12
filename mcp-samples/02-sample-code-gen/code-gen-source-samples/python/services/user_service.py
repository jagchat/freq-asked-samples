"""
User service module

This module contains business logic for user operations.
"""

from typing import List, Optional
from datetime import datetime
from models.user import User, UserCreate, UserUpdate


class UserService:
    """Service class for managing user operations"""

    def __init__(self, repository):
        """
        Initialize the user service.

        Args:
            repository: Repository instance for data access
        """
        self.repository = repository

    async def get_all(self) -> List[User]:
        """
        Retrieve all users.

        Returns:
            List of all users
        """
        return await self.repository.get_all()

    async def get_by_id(self, user_id: int) -> User:
        """
        Retrieve a user by ID.

        Args:
            user_id: The user's ID

        Returns:
            User object

        Raises:
            ValueError: If user not found
        """
        user = await self.repository.get_by_id(user_id)
        if not user:
            raise ValueError(f"User with ID {user_id} not found")
        return user

    async def create(self, user_data: UserCreate) -> User:
        """
        Create a new user.

        Args:
            user_data: User creation data

        Returns:
            Created user object

        Raises:
            ValueError: If validation fails or email already exists
        """
        # Check if email already exists
        existing_user = await self.repository.find_by_email(user_data.email)
        if existing_user:
            raise ValueError(f"User with email {user_data.email} already exists")

        # Validate name
        if not user_data.name or not user_data.name.strip():
            raise ValueError("Name is required")

        return await self.repository.create(user_data)

    async def update(self, user_id: int, user_data: UserUpdate) -> User:
        """
        Update an existing user.

        Args:
            user_id: The user's ID
            user_data: Updated user data

        Returns:
            Updated user object

        Raises:
            ValueError: If user not found or validation fails
        """
        # Check if user exists
        existing_user = await self.repository.get_by_id(user_id)
        if not existing_user:
            raise ValueError(f"User with ID {user_id} not found")

        # If email is being updated, check for conflicts
        if user_data.email and user_data.email != existing_user.email:
            email_exists = await self.repository.find_by_email(user_data.email)
            if email_exists:
                raise ValueError(f"Email {user_data.email} is already in use")

        return await self.repository.update(user_id, user_data)

    async def delete(self, user_id: int) -> None:
        """
        Delete a user.

        Args:
            user_id: The user's ID

        Raises:
            ValueError: If user not found
        """
        user = await self.repository.get_by_id(user_id)
        if not user:
            raise ValueError(f"User with ID {user_id} not found")

        await self.repository.delete(user_id)

    async def find_by_email(self, email: str) -> Optional[User]:
        """
        Find a user by email address.

        Args:
            email: Email address to search for

        Returns:
            User object if found, None otherwise
        """
        return await self.repository.find_by_email(email)
