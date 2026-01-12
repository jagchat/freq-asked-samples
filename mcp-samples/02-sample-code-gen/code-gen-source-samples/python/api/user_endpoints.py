"""
User API endpoints

This module defines FastAPI endpoints for user operations.
"""

from typing import List
from fastapi import APIRouter, HTTPException, status, Depends
from models.user import User, UserCreate, UserUpdate
from services.user_service import UserService


# Create router
router = APIRouter(
    prefix="/api/users",
    tags=["users"],
    responses={404: {"description": "Not found"}},
)


def get_user_service() -> UserService:
    """
    Dependency injection for UserService.

    Returns:
        UserService instance
    """
    # This would typically be injected from a dependency container
    # For now, returning a placeholder
    pass


@router.get("/", response_model=List[User], summary="Get all users")
async def get_all_users(service: UserService = Depends(get_user_service)):
    """
    Retrieve all users.

    Returns:
        List of all users

    Raises:
        HTTPException: If an error occurs during retrieval
    """
    try:
        users = await service.get_all()
        return users
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error retrieving users: {str(e)}"
        )


@router.get("/{user_id}", response_model=User, summary="Get user by ID")
async def get_user_by_id(
    user_id: int,
    service: UserService = Depends(get_user_service)
):
    """
    Retrieve a user by ID.

    Args:
        user_id: The user's ID

    Returns:
        User details

    Raises:
        HTTPException: If user not found or error occurs
    """
    try:
        user = await service.get_by_id(user_id)
        return user
    except ValueError as e:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=str(e)
        )
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error retrieving user: {str(e)}"
        )


@router.post(
    "/",
    response_model=User,
    status_code=status.HTTP_201_CREATED,
    summary="Create a new user"
)
async def create_user(
    user_data: UserCreate,
    service: UserService = Depends(get_user_service)
):
    """
    Create a new user.

    Args:
        user_data: User creation data

    Returns:
        Created user details

    Raises:
        HTTPException: If validation fails or error occurs
    """
    try:
        user = await service.create(user_data)
        return user
    except ValueError as e:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail=str(e)
        )
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error creating user: {str(e)}"
        )


@router.put("/{user_id}", response_model=User, summary="Update a user")
async def update_user(
    user_id: int,
    user_data: UserUpdate,
    service: UserService = Depends(get_user_service)
):
    """
    Update an existing user.

    Args:
        user_id: The user's ID
        user_data: Updated user data

    Returns:
        Updated user details

    Raises:
        HTTPException: If user not found or validation fails
    """
    try:
        user = await service.update(user_id, user_data)
        return user
    except ValueError as e:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=str(e)
        )
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error updating user: {str(e)}"
        )


@router.delete(
    "/{user_id}",
    status_code=status.HTTP_204_NO_CONTENT,
    summary="Delete a user"
)
async def delete_user(
    user_id: int,
    service: UserService = Depends(get_user_service)
):
    """
    Delete a user.

    Args:
        user_id: The user's ID

    Raises:
        HTTPException: If user not found or error occurs
    """
    try:
        await service.delete(user_id)
        return None
    except ValueError as e:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=str(e)
        )
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error deleting user: {str(e)}"
        )
