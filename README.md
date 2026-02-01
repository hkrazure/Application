# Author Notes

I took the liberty to enrich the domain with an Actor and Person entity that contains the FirstName and LastName properties.

## Out of scope

The solution could be improved with several additional implementations including:
- Global exception handling middleware
- Transfer and Deposit entities
- Component, Integration and API tests

## Domain Model

The application implements a banking domain with the following structure:

### Entities

- **Actor** (Abstract Base Entity)
  - Base class for all actors in the system
  - Properties: `InternalId`, `PublicId`
  
- **Person** (Inherits from Actor)
  - Represents individual account holders
  - Properties: `FirstName`, `LastName`

- **Account**
  - Represents a bank account owned by an Actor
  - Properties: `PublicId`, `AccountNumber`, `CurrencyType`, `Balance`, `ActorId`
  - Operations: `Deposit(Amount)`, `Withdraw(Amount)`

### Value Objects

- **Amount**
  - Encapsulates monetary value with currency
  - Properties: `Value` (decimal), `Currency` (enum)

- **AccountNumber**
  - Unique identifier for an account
  - Auto-generated GUID-based value

### Enums

- **Currency**
  - `DKK` (Danish Krone)
  - `Undefined`

### Domain Diagram

```
┌──────────────────────┐
│      Actor           │
│  (Abstract Base)     │
├──────────────────────┤
│ + InternalId: int    │
│ + PublicId: Guid     │
└──────────┬───────────┘
           │
           │ inherits
           ▼
┌──────────────────────┐
│      Person          │
├──────────────────────┤
│ + FirstName: string  │
│ + LastName: string   │
└──────────────────────┘
           │
           │ has
           │
           ▼
┌─────────────────────────────┐         ┌──────────────────────┐
│         Account             │────────>│    AccountNumber     │        
├─────────────────────────────┤         ├──────────────────────┤
│ + PublicId: Guid            │         │ + Value: string      │
│ + AccountNumber: Value Obj  │         └──────────────────────┘
│ + CurrencyType: Currency    │
│ + Balance: decimal          │         ┌──────────────────────┐
│ + ActorId: int              │────────>│       Amount         │
├─────────────────────────────┤         ├──────────────────────┤
│ + Deposit(Amount)           │         │ + Value: decimal     │
│ + Withdraw(Amount)          │         │ + Currency: Currency │
└─────────────────────────────┘         └──────────────────────┘
```

### Business Rules

1. **Deposit**: Amount must be positive and match account currency
2. **Withdrawal**: Amount must not exceed balance and match account currency
3. **Transfer**: Validates both source and destination accounts, ensures sufficient balance
4. **Currency Validation**: All operations enforce currency matching

---

# Application API

A .NET 9.0 Web API for managing bank accounts with support for deposits and transfers.

## Prerequisites

- .NET 9.0 SDK or later
- A terminal or command prompt

## Running the Application

### Using the .NET CLI

Navigate to the API project directory and run:

```bash
cd Application.API
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5011`
- HTTPS: `https://localhost:7246`

### Using Visual Studio

1. Open `Application.sln`
2. Set `Application.API` as the startup project
3. Press F5 or click "Run"

### Using IIS Express

```bash
dotnet run --launch-profile "IIS Express"
```

### Accessing Swagger UI

Once the application is running, navigate to:
- `https://localhost:7246/swagger` (HTTPS)
- `http://localhost:5011/swagger` (HTTP)

## Seeded Data

The application automatically seeds three actors on startup for testing purposes:

| Actor ID                               | First Name | Last Name |
| -------------------------------------- | ---------- | --------- |
| `11111111-1111-1111-1111-111111111111` | John       | Smith     |
| `22222222-2222-2222-2222-222222222222` | Emma       | Johnson   |
| `33333333-3333-3333-3333-333333333333` | Michael    | Williams  |

You can use these actor IDs when creating accounts.

## API Endpoints

All endpoints are versioned under `/api/v1.0/accounts`

### 1. Create Account

Creates a new bank account for a specific actor with a specified currency.

**Endpoint:** `POST /api/v1.0/accounts`

**Request Body:**
```json
{
  "actorId": "11111111-1111-1111-1111-111111111111",
  "currencyType": "DKK"
}
```

**cURL Command:**
```bash
curl -X POST "http://localhost:5011/api/v1.0/accounts" ^
  -H "Content-Type: application/json" ^
  -d "{\"actorId\":\"11111111-1111-1111-1111-111111111111\",\"currencyType\":\"DKK\"}"
```

**Response (200 OK):**
```json
{
  "id": "a1b2c3d4-5678-90ab-cdef-1234567890ab",
  "actorId": "11111111-1111-1111-1111-111111111111",
  "balance": {
    "value": 0,
    "currency": "DKK"
  }
}
```

### 2. Get Account Balance

Retrieves the balance for a specific account.

**Endpoint:** `GET /api/v1.0/accounts/{id}/balance`

**cURL Command:**
```bash
curl -X GET "http://localhost:5011/api/v1.0/accounts/a1b2c3d4-5678-90ab-cdef-1234567890ab/balance" ^
  -H "accept: application/json"
```

**Response (200 OK):**
```json
{
  "accountId": "a1b2c3d4-5678-90ab-cdef-1234567890ab",
  "balance": {
    "value": 1000.50,
    "currency": "DKK"
  }
}
```

**Response (404 Not Found):** Account not found

### 3. Deposit Amount

Deposits money into a specific account.

**Endpoint:** `POST /api/v1.0/accounts/{id}/deposits`

**Request Body:**
```json
{
  "amount": {
    "value": 500.00,
    "currency": "DKK"
  }
}
```

**cURL Command:**
```bash
curl -X POST "http://localhost:5011/api/v1.0/accounts/a1b2c3d4-5678-90ab-cdef-1234567890ab/deposits" ^
  -H "Content-Type: application/json" ^
  -d "{\"amount\":{\"value\":500.00,\"currency\":\"DKK\"}}"
```

**Response:** `204 No Content`

### 4. Transfer Amount

Transfers money from one account to another.

**Endpoint:** `POST /api/v1.0/accounts/{id}/transfers`

**Request Body:**
```json
{
  "toAccountId": "b2c3d4e5-6789-01bc-def2-234567890abc",
  "amount": {
    "value": 250.00,
    "currency": "DKK"
  }
}
```

**cURL Command:**
```bash
curl -X POST "http://localhost:5011/api/v1.0/accounts/a1b2c3d4-5678-90ab-cdef-1234567890ab/transfers" ^
  -H "Content-Type: application/json" ^
  -d "{\"toAccountId\":\"b2c3d4e5-6789-01bc-def2-234567890abc\",\"amount\":{\"value\":250.00,\"currency\":\"DKK\"}}"
```

**Response:** `204 No Content`

## Complete Workflow Example

Here's a complete workflow showing how to create accounts and perform transactions:

```bash
# 1. Create first account for John Smith
curl -X POST "http://localhost:5011/api/v1.0/accounts" ^
  -H "Content-Type: application/json" ^
  -d "{\"actorId\":\"11111111-1111-1111-1111-111111111111\",\"currencyType\":\"DKK\"}"

# Save the returned account ID (e.g., a1b2c3d4-5678-90ab-cdef-1234567890ab)

# 2. Create second account for Emma Johnson
curl -X POST "http://localhost:5011/api/v1.0/accounts" ^
  -H "Content-Type: application/json" ^
  -d "{\"actorId\":\"22222222-2222-2222-2222-222222222222\",\"currencyType\":\"DKK\"}"

# Save the returned account ID (e.g., b2c3d4e5-6789-01bc-def2-234567890abc)

# 3. Deposit money to first account
curl -X POST "http://localhost:5011/api/v1.0/accounts/a1b2c3d4-5678-90ab-cdef-1234567890ab/deposits" ^
  -H "Content-Type: application/json" ^
  -d "{\"amount\":{\"value\":1000.00,\"currency\":\"DKK\"}}"

# 4. Check balance of first account
curl -X GET "http://localhost:5011/api/v1.0/accounts/a1b2c3d4-5678-90ab-cdef-1234567890ab/balance" ^
  -H "accept: application/json"

# 5. Transfer money from first account to second account
curl -X POST "http://localhost:5011/api/v1.0/accounts/a1b2c3d4-5678-90ab-cdef-1234567890ab/transfers" ^
  -H "Content-Type: application/json" ^
  -d "{\"toAccountId\":\"b2c3d4e5-6789-01bc-def2-234567890abc\",\"amount\":{\"value\":250.00,\"currency\":\"DKK\"}}"

# 6. Check balance of second account
curl -X GET "http://localhost:5011/api/v1.0/accounts/b2c3d4e5-6789-01bc-def2-234567890abc/balance" ^
  -H "accept: application/json"
```

## Notes

- **Currency:** Currently only `DKK` (Danish Krone) is supported.
- **GUIDs:** Replace example GUIDs with actual IDs returned from API responses.
- **Windows:** The cURL examples use `^` for line continuation. On Linux/Mac, use `\` instead.
- **HTTPS:** For production, use HTTPS (`https://localhost:7246`) and add the `-k` flag to bypass SSL certificate validation in development.

## Linux/Mac cURL Commands

For Linux or Mac users, replace `^` with `\` in the cURL commands:

```bash
curl -X POST "http://localhost:5011/api/v1.0/accounts" \
  -H "Content-Type: application/json" \
  -d '{"actorId":"11111111-1111-1111-1111-111111111111","currencyType":"DKK"}'
```

## Project Structure

- **Application.API**: Web API project with controllers and DTOs
- **Application.Application**: Application layer with commands, queries, and handlers
- **Domain**: Domain entities, value objects, and enums
- **Infrastructure**: Data access and infrastructure concerns
- **UnitTests**: Unit tests
- **ComponentTests**: Component/integration tests

## Architecture

The application follows Clean Architecture principles with:
- **CQRS** pattern using MediatR
- **API Versioning** (v1.0)
- **Swagger/OpenAPI** documentation
- **In-memory data seeding** for development

## Troubleshooting

### Port Already in Use

If ports 5011 or 7246 are already in use, you can specify different ports:

```bash
dotnet run --urls "https://localhost:7247;http://localhost:5012"
```

### Trust Development Certificate

If you get SSL errors, trust the development certificate:

```bash
dotnet dev-certs https --trust
```

## API Versioning

The API uses URL-based versioning. Current version: `v1.0`

To use a specific version, include it in the URL:
- `/api/v1.0/accounts`
