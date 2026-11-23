# Cristal Beauty Clear - Backend .NET

A lightweight, scalable RESTful backend API for the **Cristal Beauty Clear** e-commerce platform, built with **.NET 10 (ASP.NET Core)** and **MongoDB**.

---

## 🚀 Tech Stack

- **Framework**: [.NET 10 (C#)](https://dotnet.microsoft.com/)
- **Database**: [MongoDB](https://www.mongodb.com/) (using MongoDB C# Driver)
- **Authentication**: JWT (JSON Web Tokens) & BCrypt password hashing
- **Email Service**: [MailKit](https://github.com/jstedfast/MailKit) (SMTP / OTP email delivery)
- **API Spec**: Microsoft.AspNetCore.OpenApi

---

## 📁 Project Architecture

The application follows a clean **Controller-Service-Repository** layered architecture:

```
CristalBeautyAPI/
├── Controllers/       # REST API endpoints (User, Product, Order, Message)
├── Services/          # Business logic layer and service contracts
├── Repositories/      # Data access layer interfacing directly with MongoDB
├── Models/            # MongoDB entity models and DTOs
├── Middleware/         # Custom middleware (JWT validation & authentication)
├── Utility/           # Constants and shared utility helpers
└── Properties/        # Launch profiles & environment configuration
```

---

## 🛠 Features & Endpoints

### 🔐 User & Authentication (`/api/user`)
- `POST /api/user` — Register a new account
- `POST /api/user/login` — Account login & JWT issuance
- `POST /api/user/google` — Google OAuth authentication
- `GET /api/user/current` — Get authenticated user details
- `POST /api/user/sendMail` — Send OTP for password reset
- `POST /api/user/changePassword` — Reset password using OTP
- `PATCH /api/user/block/{userEmail}` / `unblock/{userEmail}` — Admin account moderation
- `GET`, `POST`, `DELETE /api/user/wishlist` — Wishlist management

### 🛍 Products (`/api/product`)
- `GET /api/product` — Retrieve product catalog
- `GET /api/product/{id}` / `search/{id}` — Find product by ID or query
- `GET /api/product/categories` — List available product categories
- `GET /api/product/filter` — Filter products by criteria
- `POST /api/product` — Add new product (Admin)
- `PUT /api/product/{productID}` — Update existing product (Admin)
- `DELETE /api/product/{productID}` — Remove product (Admin)

### 📦 Orders (`/api/order`)
- `GET /api/order` — Fetch orders
- `POST /api/order` — Place a new order
- `PUT /api/order/{orderId}` — Update order details/status

### ✉️ Contact Messages (`/api/message`)
- `POST /api/message` — Submit customer inquiry/message
- `GET /api/message` — View customer messages
- `POST /api/message/{messageId}/reply` — Reply to customer inquiry
- `DELETE /api/message/{messageId}` — Delete message

---

## ⚙️ Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A running [MongoDB](https://www.mongodb.com/) instance or MongoDB Atlas cluster

### Configuration
Update the configuration in `CristalBeautyAPI/appsettings.json` (or use user secrets / environment variables):

```json
{
  "MongoDb": {
    "ConnectionString": "<your-mongodb-connection-string>",
    "DatabaseName": "test"
  },
  "Jwt": {
    "Key": "<your-secure-jwt-secret-key>"
  },
  "Email": {
    "SenderEmail": "<your-smtp-email>",
    "AppPassword": "<your-app-password>"
  }
}
```

### Running Locally

1. Clone or navigate to the repository:
   ```bash
   cd Backend-.NET
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Start the API server:
   ```bash
   dotnet run --project CristalBeautyAPI
   ```

By default, the API will be available at:
`http://localhost:3000`

---

## 📄 License
This project is for educational purposes only.
