# Fleet Module API Contract

## Base URL
```
/api/vehicles
```

## Authentication
All endpoints require JWT Bearer token in `Authorization` header:
```
Authorization: Bearer {jwt_token}
```

## Endpoints

### 1. Create Vehicle
**POST** `/api/vehicles`

**Request Body:**
```json
{
  "registrationNumber": "ABC123",
  "make": "Volvo",
  "model": "FH16",
  "year": 2023
}
```

**Success Response (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "registrationNumber": "ABC123",
  "make": "Volvo",
  "model": "FH16",
  "year": 2023,
  "status": "Available"
}
```

**Error Response (400 Bad Request):**
```json
{
  "error": "Validation error message"
}
```

---

### 2. Get Vehicle by ID
**GET** `/api/vehicles/{id}`

**Path Parameters:**
- `id` (Guid) – Vehicle ID

**Success Response (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "registrationNumber": "ABC123",
  "make": "Volvo",
  "model": "FH16",
  "year": 2023,
  "status": "Available"
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "Vehicle not found."
}
```

---

### 3. List Vehicles (Paginated)
**GET** `/api/vehicles?page=1&pageSize=20`

**Query Parameters:**
- `page` (int, default=1) – Page number (1-indexed)
- `pageSize` (int, default=20) – Items per page (clamped to 1-100)

**Success Response (200 OK):**
```json
{
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "registrationNumber": "ABC123",
      "make": "Volvo",
      "model": "FH16",
      "year": 2023,
      "status": "Available"
    },
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "registrationNumber": "DEF456",
      "make": "Scania",
      "model": "R440",
      "year": 2022,
      "status": "InService"
    }
  ],
  "totalCount": 15,
  "page": 1,
  "pageSize": 20
}
```

**Error Response (400 Bad Request):**
```json
{
  "error": "Validation error message"
}
```

---

### 4. Update Vehicle
**PUT** `/api/vehicles/{id}`

**Path Parameters:**
- `id` (Guid) – Vehicle ID

**Request Body (all fields optional):**
```json
{
  "registrationNumber": "ABC124",
  "make": "Volvo",
  "model": "FH16",
  "year": 2023
}
```

**Success Response (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "registrationNumber": "ABC124",
  "make": "Volvo",
  "model": "FH16",
  "year": 2023,
  "status": "Available"
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "Vehicle not found."
}
```

---

## Response Models

### VehicleResponseDto
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "registrationNumber": "ABC123",
  "make": "Volvo",
  "model": "FH16",
  "year": 2023,
  "status": "Available"
}
```

### PagedResult<VehicleResponseDto>
```json
{
  "items": [VehicleResponseDto],
  "totalCount": 100,
  "page": 1,
  "pageSize": 20
}
```

---

## Status Codes
- **200 OK** – Request successful
- **201 Created** – Resource created successfully
- **400 Bad Request** – Validation error
- **401 Unauthorized** – Missing or invalid JWT token
- **404 Not Found** – Resource not found
- **500 Internal Server Error** – Server error

---

## Multi-Tenancy
All vehicles are automatically scoped to the tenant extracted from the JWT token. 
- A user from Tenant A cannot see vehicles from Tenant B
- Cross-tenant write attempts are rejected with 403 Forbidden

---

## cURL Examples

### Create Vehicle
```bash
curl -X POST https://localhost:5001/api/vehicles \
  -H "Authorization: Bearer {jwt_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "registrationNumber": "ABC123",
    "make": "Volvo",
    "model": "FH16",
    "year": 2023
  }'
```

### Get Vehicle
```bash
curl -X GET https://localhost:5001/api/vehicles/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer {jwt_token}"
```

### List Vehicles
```bash
curl -X GET "https://localhost:5001/api/vehicles?page=1&pageSize=20" \
  -H "Authorization: Bearer {jwt_token}"
```

### Update Vehicle
```bash
curl -X PUT https://localhost:5001/api/vehicles/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer {jwt_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "registrationNumber": "ABC124"
  }'
```

---

## Validation Rules
- `registrationNumber` – Required, max 50 chars
- `make` – Required, max 100 chars
- `model` – Required, max 100 chars
- `year` – Required, positive integer
- `registrationNumber` must be unique per tenant
