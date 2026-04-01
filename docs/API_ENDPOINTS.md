# 📝 API Endpoints Reference

This document provides a comprehensive reference for all available endpoints in **Restaurant API**.

> **Base URL**: `http://localhost:5124`  
> **Auth Header**: `apiKey: <your-user-code>` *(required on protected routes)*

---

## 🏪 Restaurant Endpoints

| Method | Endpoint | Auth | Description |
| :--- | :--- | :---: | :--- |
| `GET` | `/api/restaurant` | ❌ | List all restaurants (supports filtering) |
| `GET` | `/api/restaurant/{id}` | ❌ | Get a restaurant with its full menu |
| `POST` | `/api/restaurant` | ✅ | Create a new restaurant |
| `PUT` | `/api/restaurant/{id}` | ✅ | Update restaurant details |
| `DELETE` | `/api/restaurant/{id}` | ✅ | Delete a restaurant |

### Query Parameters — `GET /api/restaurant`

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `name` | `string` | Filter by restaurant name |
| `address` | `string` | Filter by restaurant address |
| `category` | `string` | Filter by restaurant category |

---

## 👤 User Endpoints

| Method | Endpoint | Auth | Description |
| :--- | :--- | :---: | :--- |
| `POST` | `/api/user/register` | ❌ | Register a new user account |
| `POST` | `/api/user/login` | ❌ | Authenticate and retrieve API key |
| `GET` | `/api/user/profile` | ✅ | Get the authenticated user's profile |
| `PUT` | `/api/user/profile` | ✅ | Update profile information |

---

## 🛒 Cart Endpoints

| Method | Endpoint | Auth | Description |
| :--- | :--- | :---: | :--- |
| `GET` | `/api/cart` | ✅ | View all items in the user's cart |
| `POST` | `/api/cart` | ✅ | Add an item to the cart |
| `PUT` | `/api/cart/{cartId}` | ✅ | Update item quantity in the cart |
| `DELETE` | `/api/cart/{cartId}` | ✅ | Remove a specific item from the cart |
| `DELETE` | `/api/cart` | ✅ | Clear the entire cart |

### Request Body — `POST /api/cart`

```json
{
  "itemId": 1,
  "quantity": 2
}
```

---

## 📦 Order Endpoints

| Method | Endpoint | Auth | Description |
| :--- | :--- | :---: | :--- |
| `POST` | `/api/order` | ✅ | Place a new order from the current cart |
| `GET` | `/api/order` | ✅ | Get the user's full order history |
| `GET` | `/api/order/{masterId}` | ✅ | Get details of a specific master order |

---

## 🖼️ Image Endpoints

| Method | Endpoint | Auth | Description |
| :--- | :--- | :---: | :--- |
| `POST` | `/api/restaurant/{id}/image` | ✅ | Upload an image file for a restaurant item |
| `POST` | `/api/restaurant/{id}/image/base64` | ✅ | Upload a base64-encoded image |

---

## 📋 Response Format

All endpoints return a consistent JSON envelope:

```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": { ... }
}
```

### Error Response

```json
{
  "success": false,
  "message": "Descriptive error message.",
  "data": null
}
```

---

[⬅️ Back to Main README](../README.md)
