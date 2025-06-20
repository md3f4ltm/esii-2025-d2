# Job Proposals and Talents Search & Filter API Documentation

This document describes the search and filter functionality added to the Job Proposals and Talents API endpoints.

## Overview

The system now provides powerful search and filtering capabilities for both Job Proposals and Talents, including:
- Text-based search
- Multiple filter criteria
- Sorting options
- Pagination support
- Server-side filtering for performance

## Job Proposals Search API

### Endpoints

#### POST `/api/JobProposal/search`
Search job proposals using POST request with JSON body.

#### GET `/api/JobProposal/search`
Search job proposals using GET request with query parameters.

### Search Parameters

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| `SearchText` | string | Search in name and description | "Web Developer" |
| `SkillId` | int? | Filter by specific skill | 5 |
| `TalentCategoryId` | int? | Filter by talent category | 2 |
| `CustomerId` | string? | Filter by customer | "customer-123" |
| `MinTotalHours` | int? | Minimum total hours | 10 |
| `MaxTotalHours` | int? | Maximum total hours | 100 |
| `Page` | int | Page number (1-based) | 1 |
| `PageSize` | int | Items per page (1-100) | 20 |
| `SortBy` | string | Sort field | "Name" |
| `SortDirection` | string | Sort direction (asc/desc) | "asc" |

### Sort Fields for Job Proposals
- `name` - Sort by proposal name
- `totalhours` - Sort by total hours
- `skill` - Sort by skill name
- `category` - Sort by talent category name
- `customer` - Sort by customer name

### Example Requests

#### POST Request (JSON Body)
```json
{
  "searchText": "React",
  "skillId": 5,
  "minTotalHours": 20,
  "maxTotalHours": 80,
  "page": 1,
  "pageSize": 10,
  "sortBy": "name",
  "sortDirection": "asc"
}
```

#### GET Request (Query Parameters)
```
GET /api/JobProposal/search?searchText=React&skillId=5&minTotalHours=20&maxTotalHours=80&page=1&pageSize=10&sortBy=name&sortDirection=asc
```

### Response Format
```json
{
  "page": 1,
  "pageSize": 10,
  "totalItems": 25,
  "totalPages": 3,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "items": [
    {
      "id": 1,
      "name": "React Developer Position",
      "description": "Looking for React developer...",
      "totalHours": 40,
      "skillId": 5,
      "customerId": "customer-123",
      "talentCategoryId": 2,
      "skill": {
        "id": 5,
        "name": "React"
      },
      "talentCategory": {
        "id": 2,
        "name": "Frontend Development"
      },
      "customer": {
        "id": "customer-123",
        "company": "Tech Company"
      }
    }
  ]
}
```

## Talents Search API

### Endpoints

#### POST `/api/Talent/search`
Search talents using POST request with JSON body.

#### GET `/api/Talent/search`
Search talents using GET request with query parameters.

### Search Parameters

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| `SearchText` | string | Search in name, country, and email | "John" |
| `TalentCategoryId` | int? | Filter by talent category | 2 |
| `SkillId` | int? | Filter by specific skill | 5 |
| `Country` | string? | Filter by country | "Portugal" |
| `MinHourlyRate` | decimal? | Minimum hourly rate | 25.00 |
| `MaxHourlyRate` | decimal? | Maximum hourly rate | 100.00 |
| `IsPublic` | bool? | Filter by public status | true |
| `Page` | int | Page number (1-based) | 1 |
| `PageSize` | int | Items per page (1-100) | 20 |
| `SortBy` | string | Sort field | "Name" |
| `SortDirection` | string | Sort direction (asc/desc) | "asc" |

### Sort Fields for Talents
- `name` - Sort by talent name
- `country` - Sort by country
- `hourlyrate` - Sort by hourly rate
- `category` - Sort by talent category name
- `email` - Sort by email

### Example Requests

#### POST Request (JSON Body)
```json
{
  "searchText": "John",
  "talentCategoryId": 2,
  "country": "Portugal",
  "minHourlyRate": 30.00,
  "maxHourlyRate": 80.00,
  "isPublic": true,
  "page": 1,
  "pageSize": 15,
  "sortBy": "hourlyrate",
  "sortDirection": "desc"
}
```

#### GET Request (Query Parameters)
```
GET /api/Talent/search?searchText=John&talentCategoryId=2&country=Portugal&minHourlyRate=30.00&maxHourlyRate=80.00&isPublic=true&page=1&pageSize=15&sortBy=hourlyrate&sortDirection=desc
```

### Response Format
```json
{
  "page": 1,
  "pageSize": 15,
  "totalItems": 8,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false,
  "items": [
    {
      "id": 1,
      "name": "John Smith",
      "country": "Portugal",
      "email": "john@example.com",
      "hourlyRate": 75.00,
      "isPublic": true,
      "talentCategoryId": 2,
      "talentCategory": {
        "id": 2,
        "name": "Frontend Development"
      },
      "talentSkills": [
        {
          "skillId": 5,
          "skill": {
            "id": 5,
            "name": "React"
          }
        }
      ]
    }
  ]
}
```

## Filter Options API

### Get Job Proposal Filter Options
#### GET `/api/JobProposal/filter-options`

Returns available options for filtering job proposals.

**Response:**
```json
{
  "skills": [
    { "id": 1, "name": "JavaScript" },
    { "id": 2, "name": "React" }
  ],
  "talentCategories": [
    { "id": 1, "name": "Frontend Development" },
    { "id": 2, "name": "Backend Development" }
  ],
  "customers": [
    { "id": "customer-1", "name": "Tech Company A" },
    { "id": "customer-2", "name": "Startup B" }
  ]
}
```

### Get Talent Filter Options
#### GET `/api/Talent/filter-options`

Returns available options for filtering talents.

**Response:**
```json
{
  "talentCategories": [
    { "id": 1, "name": "Frontend Development" },
    { "id": 2, "name": "Backend Development" }
  ],
  "skills": [
    { "id": 1, "name": "JavaScript" },
    { "id": 2, "name": "React" }
  ],
  "countries": [
    "Portugal",
    "Spain",
    "Brazil"
  ]
}
```

## Authentication

- Job Proposal search endpoints require authentication (Bearer token)
- Talent search endpoints are publicly accessible
- Filter options endpoints are publicly accessible

## Error Handling

All endpoints return appropriate HTTP status codes:

- `200 OK` - Successful request
- `400 Bad Request` - Invalid parameters or validation errors
- `401 Unauthorized` - Authentication required
- `500 Internal Server Error` - Server error

**Error Response Format:**
```json
{
  "message": "Error description",
  "error": "Detailed error information"
}
```

## Usage Tips

1. **Performance**: Use pagination for large datasets to improve performance
2. **Text Search**: Text search is case-insensitive and uses `CONTAINS` matching
3. **Validation**: All parameters are validated server-side
4. **Sorting**: Default sorting is by name in ascending order
5. **Filtering**: Multiple filters can be combined for precise results

## Frontend Integration Example

```javascript
// Search for React developers in Portugal
const searchTalents = async () => {
  const searchParams = {
    searchText: "React",
    country: "Portugal",
    minHourlyRate: 30,
    isPublic: true,
    page: 1,
    pageSize: 20,
    sortBy: "hourlyrate",
    sortDirection: "desc"
  };

  const response = await fetch('/api/Talent/search', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(searchParams)
  });

  const result = await response.json();
  return result;
};

// Get filter options for dropdowns
const getFilterOptions = async () => {
  const response = await fetch('/api/Talent/filter-options');
  const options = await response.json();
  return options;
};
```
