# Entity Relationship Diagram (ERD)

## Database Schema Overview

**Restaurant API** uses a relational database design with the following entities:

## Core Entities

### User
- **UserID** (PK, Identity)
- **UserEmail** (Unique, Required)
- **Password** (Required)
- **Usercode** (Unique API Key, Required)

### Restaurant
- **RestaurantID** (PK, Identity)
- **RestaurantName** (Required)
- **Address** (Required)
- **Type** (Required)
- **ParkingLot** (Boolean)

### Item
- **ItemID** (PK, Identity)
- **ItemName** (Required)
- **ItemDescription** (Required)
- **ItemPrice** (Decimal, Required)
- **RestaurantID** (FK to Restaurant)
- **ImageUrl** (Optional)

### MasterOrder
- **MasterID** (PK, Identity)
- **UserID** (FK to User.UserEmail)
- **RestaurantID** (FK to Restaurant)
- **GrandTotal** (Decimal)
- **OrderDate** (DateTime, Auto-generated)

### Order
- **OrderID** (PK, Identity)
- **UserID** (FK to User.UserEmail)
- **ItemName** (Required)
- **ItemPrice** (Decimal)
- **Quantity** (Integer)
- **TotalPrice** (Decimal)
- **MasterID** (FK to MasterOrder)

### Cart
- **CartID** (PK, Identity)
- **UserID** (FK to User.Usercode)
- **ItemID** (FK to Item)
- **ItemName** (Required)
- **Quantity** (Integer)

## Relationships

```
User ||--o{ MasterOrder : "places"
User ||--o{ Order : "creates"
User ||--o{ Cart : "has"

Restaurant ||--o{ Item : "contains"
Restaurant ||--o{ MasterOrder : "receives"

Item ||--o{ Cart : "added to"

MasterOrder ||--o{ Order : "contains"
```

## Database Constraints

- **Primary Keys**: All entities have auto-incrementing identity primary keys
- **Foreign Keys**: Proper referential integrity maintained
- **Unique Constraints**: UserEmail and Usercode are unique
- **Required Fields**: All essential business fields are non-nullable
- **Data Types**: Appropriate data types for each field (decimal for prices, etc.)

## Indexing Strategy

- Primary keys are automatically indexed
- Foreign keys should be indexed for performance
- UserEmail and Usercode should have unique indexes
- RestaurantName should be indexed for search performance

---

[⬅️ Back to Main README](../README.md)