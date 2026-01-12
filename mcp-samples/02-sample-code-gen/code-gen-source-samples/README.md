# Code Generation Source Samples

This directory contains code examples that the LangGraph workflow uses as learning patterns for generating new code.

## Purpose

When you request code generation (e.g., "Create a payments service with CRUD operations"), the LangGraph workflow:

1. **Loads relevant examples** from this directory based on your stack (.NET, Python, JavaScript)
2. **Analyzes patterns** in the example code (naming conventions, structure, dependencies)
3. **Generates new code** following the same patterns and conventions

## Directory Structure

```
code-gen-source-samples/
├── dotnet/              # .NET C# examples
│   ├── services/        # Business logic services
│   │   └── UserService.cs          ✅ Complete CRUD service with validation
│   ├── controllers/     # API controllers
│   │   └── UserController.cs       ✅ REST API controller with error handling
│   ├── repositories/    # Data access repositories
│   │   └── UserRepository.cs       ✅ Entity Framework repository pattern
│   └── models/          # Domain models and DTOs
│       └── User.cs                 ✅ Model with data annotations
│
├── python/              # Python examples
│   ├── services/        # Business logic services
│   │   └── user_service.py         ✅ Service with async operations
│   ├── api/             # FastAPI endpoints
│   │   └── user_endpoints.py       ✅ FastAPI router with dependency injection
│   └── models/          # Pydantic models
│       └── user.py                 ✅ Pydantic models with validation
│
└── javascript/          # JavaScript/TypeScript examples
    ├── services/        # Business logic services
    │   └── userService.js          ✅ Service class with error handling
    ├── controllers/     # Express/NestJS controllers
    │   └── userController.js       ✅ Express router with REST endpoints
    └── models/          # Data models
        └── User.js                 ✅ Model class with validation methods
```

## Available Examples Inventory

### .NET (C#) - 4 Files

| File | Location | Description | Features |
|------|----------|-------------|----------|
| **UserService.cs** | `dotnet/services/` | Business logic service | Full CRUD operations, validation, error handling, interface definition, dependency injection |
| **UserRepository.cs** | `dotnet/repositories/` | Data access layer | Entity Framework integration, async operations, CRUD methods, custom queries |
| **UserController.cs** | `dotnet/controllers/` | REST API controller | ASP.NET Core Web API, HTTP methods (GET/POST/PUT/DELETE), error responses, model binding |
| **User.cs** | `dotnet/models/` | Domain model | Data annotations, validation attributes, properties with proper typing |

### Python - 3 Files

| File | Location | Description | Features |
|------|----------|-------------|----------|
| **user_service.py** | `python/services/` | Business logic service | Async/await, full CRUD, validation, error handling, type hints |
| **user_endpoints.py** | `python/api/` | FastAPI endpoints | FastAPI router, dependency injection, HTTP status codes, comprehensive error handling |
| **user.py** | `python/models/` | Pydantic models | Pydantic BaseModel, validation, multiple schemas (Create/Update/Full), type annotations |

### JavaScript (Node.js) - 3 Files

| File | Location | Description | Features |
|------|----------|-------------|----------|
| **userService.js** | `javascript/services/` | Business logic service | Class-based, async/await, full CRUD, validation, error handling |
| **userController.js** | `javascript/controllers/` | Express router | Express.js routes, REST API, error handling, parameter validation |
| **User.js** | `javascript/models/` | Model class | Class-based model, static validation methods, JSON serialization |

## Pattern Highlights

Each stack demonstrates consistent patterns that the LLM will learn and replicate:

### .NET Patterns
- **PascalCase** naming (UserService, GetById)
- **Interface definitions** (IUserService, IUserRepository)
- **Dependency injection** via constructor
- **Async/await** with Task<T>
- **Data annotations** for validation
- **XML documentation** comments

### Python Patterns
- **snake_case** naming (user_service, get_by_id)
- **Type hints** with typing module
- **Pydantic models** for validation
- **Async/await** with asyncio
- **FastAPI patterns** (router, Depends, HTTPException)
- **Docstrings** for documentation

### JavaScript Patterns
- **camelCase** naming (userService, getById)
- **Class-based** architecture
- **JSDoc comments** for documentation
- **Async/await** with Promises
- **Express.js patterns** (router, middleware)
- **Error-first** error handling

## Adding Examples

To improve code generation quality, add real, well-written code examples:

### For .NET:
```bash
code-gen-source-samples/dotnet/services/UserService.cs
code-gen-source-samples/dotnet/repositories/UserRepository.cs
code-gen-source-samples/dotnet/models/User.cs
```

### For Python:
```bash
code-gen-source-samples/python/services/user_service.py
code-gen-source-samples/python/api/user_endpoints.py
code-gen-source-samples/python/models/user.py
```

### For JavaScript:
```bash
code-gen-source-samples/javascript/services/userService.js
code-gen-source-samples/javascript/controllers/userController.js
code-gen-source-samples/javascript/models/User.js
```

## Best Practices

1. **Use Real Code**: Add actual working code from your projects, not simplified examples
2. **Be Consistent**: Follow consistent naming conventions within each stack
3. **Include Comments**: Well-commented code helps the LLM understand intent
4. **Show Patterns**: Include examples of common operations (CRUD, validation, error handling)
5. **Keep It Modular**: Each file should have a single, clear responsibility

## How the LLM Uses These Examples

The LangGraph workflow will:
- **Extract patterns**: File structure, naming conventions, import statements
- **Learn conventions**: Error handling, validation, dependency injection
- **Generate similar code**: New entities following the same patterns as examples
- **Adapt context**: Customize based on your specific request

## Configuration

The path to this directory is configured in `langgraph-engine/.env`:

```env
EXAMPLES_PATH=../code-gen-source-samples
```

You can change this to point to a different location if needed.

## Usage Examples

### Example 1: Generate .NET Payment Service

**Request:**
```
"Create a payments service with CRUD operations"
Stack: dotnet
```

**LangGraph Workflow:**
1. Loads: `UserService.cs`, `UserRepository.cs`, `UserController.cs`, `User.cs`
2. Analyzes patterns: PascalCase, interfaces, dependency injection
3. Generates: `PaymentService.cs`, `PaymentRepository.cs`, `PaymentController.cs`, `Payment.cs`

**Generated Code Follows:**
- Same namespace structure (`YourProject.Services`, `YourProject.Repositories`)
- Same interface pattern (`IPaymentService`, `IPaymentRepository`)
- Same error handling (throwing `KeyNotFoundException`, `ArgumentException`)
- Same async patterns (`Task<T>`, `async/await`)

### Example 2: Generate Python Order API

**Request:**
```
"Create an order service with CRUD"
Stack: python
```

**LangGraph Workflow:**
1. Loads: `user_service.py`, `user_endpoints.py`, `user.py`
2. Analyzes patterns: snake_case, Pydantic, FastAPI router
3. Generates: `order_service.py`, `order_endpoints.py`, `order.py`

**Generated Code Follows:**
- Same naming convention (`get_by_id`, `create`, `update`, `delete`)
- Same Pydantic schemas (`OrderBase`, `OrderCreate`, `OrderUpdate`, `Order`)
- Same FastAPI patterns (router, dependency injection, HTTP status codes)
- Same error handling (raising `ValueError` with descriptive messages)

### Example 3: Generate JavaScript Product Service

**Request:**
```
"Create a product service"
Stack: javascript
```

**LangGraph Workflow:**
1. Loads: `userService.js`, `userController.js`, `User.js`
2. Analyzes patterns: camelCase, class-based, Express router
3. Generates: `productService.js`, `productController.js`, `Product.js`

**Generated Code Follows:**
- Same naming convention (`getById`, `create`, `update`, `delete`)
- Same class structure (constructor with repository injection)
- Same Express patterns (router with REST endpoints)
- Same error handling (try-catch with descriptive error messages)

## Tips for Better Code Generation

1. **Add More Examples**: The more examples you add, the better the LLM understands your patterns
2. **Use Consistent Patterns**: Keep naming conventions and code structure consistent across examples
3. **Include Edge Cases**: Show how your code handles errors, edge cases, and validation
4. **Document Well**: Comments and documentation help the LLM understand intent
5. **Keep Examples Current**: Update examples to match your current best practices

## Quality Checklist

When adding new examples, ensure they demonstrate:

- ✅ **Proper naming conventions** for the stack
- ✅ **Error handling patterns** (try-catch, exceptions, error responses)
- ✅ **Input validation** (data annotations, Pydantic models, custom validators)
- ✅ **Async operations** where appropriate
- ✅ **Dependency injection** or service composition
- ✅ **CRUD operations** (Create, Read, Update, Delete)
- ✅ **Documentation** (XML comments, docstrings, JSDoc)
- ✅ **RESTful patterns** for controllers/endpoints
- ✅ **Data access patterns** for repositories/data layers
