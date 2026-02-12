# 🎓 Course Management API

A comprehensive RESTful API for managing online courses, lessons, and student enrollments with JWT authentication and role-based access control.

---

## ✨ Features

- ✅ **User Authentication**: JWT-based authentication system
- ✅ **Role-Based Access Control**: Admin, Instructor, Student roles
- ✅ **Course Management**: Create, read, update, delete courses
- ✅ **Lesson Management**: Organize lessons within courses
- ✅ **Enrollment System**: Students can enroll in courses
- ✅ **Soft Delete**: Preserve data with soft deletion
- ✅ **Data Transfer Objects**: DTOs for API security and separation of concerns
- ✅ **Fluent API Configuration**: Entity Framework Core configuration
- ✅ **Swagger Documentation**: Interactive API documentation
- ✅ **Error Handling**: Comprehensive error responses

---

## 🏗️ Architecture

### Layers
```
┌─────────────────────────────────┐
│      API Controllers             │ ◄─ HTTP Requests/Responses
├─────────────────────────────────┤
│      DTOs & Services             │ ◄─ Business Logic
├─────────────────────────────────┤
│      Database Context (EF Core)  │ ◄─ Data Access Layer
├─────────────────────────────────┤
│      SQL Server Database         │ ◄─ Data Persistence
└─────────────────────────────────┘
```

### Design Patterns Used
- **Repository Pattern** (via DbContext)
- **Dependency Injection**
- **Data Transfer Objects (DTO)**
- **Fluent API Configuration**

---

## 📊 Database Schema

See [DATABASE_SCHEMA.md](./DATABASE_SCHEMA.md) for complete ER Diagram and schema details.

### Quick Overview

```
USER (1) ──────── (M) ENROLLMENT ────────── (1) COURSE ──────── (1) LESSON
  │                                             │
  └─ Id (PK)                                   └─ Id (PK)
  └─ Username (Unique)                        └─ Title
  └─ Password                                 └─ Description
  └─ Role (Admin/Instructor/Student)          └─ Price
  └─ IsDeleted
```

---

## 🔐 Authentication & Authorization

### Roles
```
┌──────────────┬─────────────────────┬──────────────────┐
│ Role         │ Course Operations   │ User Operations  │
├──────────────┼─────────────────────┼──────────────────┤
│ Admin        │ Create/Edit/Delete  │ Full Access      │
│ Instructor   │ Create/Edit         │ Own Profile Only │
│ Student      │ View/Enroll         │ Own Profile Only │
└──────────────┴─────────────────────┴──────────────────┘
```

### JWT Token
```json
{
  "sub": "1",
  "name": "ahmed",
  "role": "Student",
  "iat": 1704067200,
  "exp": 1704070800
}
```

---

## 📝 API Documentation

### Base URL
```
https://localhost:7001/api
```

### Authentication Header
```
Authorization: Bearer {token}
```

---

## 🔌 API Endpoints

### 🔑 Authentication Endpoints

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "ahmed",
  "password": "password123",
  "role": "Student"
}
```

**Response (201 Created)**
```json
{
  "id": 1,
  "username": "ahmed",
  "role": "Student"
}
```

---

#### Login User
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "ahmed",
  "password": "password123"
}
```

**Response (200 OK)**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "ahmed",
    "role": "Student"
  }
}
```

---

### 👤 User Endpoints

#### Create User (Admin Only)
```http
POST /api/user
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "username": "john_doe",
  "password": "secure_password",
  "role": "Instructor"
}
```

#### Get All Users (Admin Only)
```http
GET /api/user
Authorization: Bearer {admin_token}
```

#### Get User by ID
```http
GET /api/user/1
Authorization: Bearer {token}
```

#### Update User
```http
PUT /api/user/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "username": "ahmed_updated",
  "password": "new_password",
  "role": "Student"
}
```

#### Delete User (Admin Only)
```http
DELETE /api/user/1
Authorization: Bearer {admin_token}
```

---

### 📚 Course Endpoints

#### Create Course (Admin, Instructor Only)
```http
POST /api/course
Authorization: Bearer {instructor_token}
Content-Type: application/json

{
  "title": "C# Basics",
  "description": "Learn C# from scratch",
  "price": 49.99
}
```

#### Get All Courses
```http
GET /api/course
Authorization: Bearer {token}
```

#### Get Course by ID
```http
GET /api/course/1
Authorization: Bearer {token}
```

**Response**
```json
{
  "id": 1,
  "title": "C# Basics",
  "description": "Learn C# from scratch",
  "price": 49.99,
  "lessons": [
    {
      "id": 1,
      "title": "Introduction",
      "content": "...",
      "courseId": 1
    }
  ]
}
```

#### Update Course
```http
PUT /api/course/1
Authorization: Bearer {instructor_token}
Content-Type: application/json

{
  "id": 1,
  "title": "C# Advanced",
  "description": "Advanced C# concepts",
  "price": 99.99
}
```

#### Delete Course (Admin Only)
```http
DELETE /api/course/1
Authorization: Bearer {admin_token}
```

---

### 📖 Lesson Endpoints

#### Create Lesson (Admin, Instructor Only)
```http
POST /api/lesson
Authorization: Bearer {instructor_token}
Content-Type: application/json

{
  "title": "Variables and Data Types",
  "content": "In this lesson, we'll learn about...",
  "courseId": 1
}
```

#### Get All Lessons
```http
GET /api/lesson
Authorization: Bearer {token}
```

#### Get Lesson by ID
```http
GET /api/lesson/1
Authorization: Bearer {token}
```

#### Get Lessons by Course
```http
GET /api/lesson/course/1
Authorization: Bearer {token}
```

#### Update Lesson
```http
PUT /api/lesson/1
Authorization: Bearer {instructor_token}
Content-Type: application/json

{
  "id": 1,
  "title": "Variables and Data Types",
  "content": "Updated content...",
  "courseId": 1
}
```

#### Delete Lesson (Admin Only)
```http
DELETE /api/lesson/1
Authorization: Bearer {admin_token}
```

---

### 📋 Enrollment Endpoints

#### Enroll in Course
```http
POST /api/enrollment
Authorization: Bearer {student_token}
Content-Type: application/json

{
  "userId": 1,
  "courseId": 1
}
```

#### Get All Enrollments (Admin Only)
```http
GET /api/enrollment
Authorization: Bearer {admin_token}
```

#### Get Enrollment by ID
```http
GET /api/enrollment/1
Authorization: Bearer {token}
```

#### Get User Enrollments
```http
GET /api/enrollment/user/1
Authorization: Bearer {token}
```

**Response**
```json
[
  {
    "id": 1,
    "userId": 1,
    "courseId": 1,
    "enrollDate": "2024-01-15T10:30:00",
    "user": {
      "id": 1,
      "username": "ahmed",
      "role": "Student"
    },
    "course": {
      "id": 1,
      "title": "C# Basics",
      "description": "...",
      "price": 49.99,
      "lessons": []
    }
  }
]
```

#### Cancel Enrollment
```http
DELETE /api/enrollment/1
Authorization: Bearer {token}
```

---

## 🛠️ Setup & Installation

### Prerequisites
- .NET 10 SDK
- SQL Server 2019 or higher
- Visual Studio 2022 / VS Code

### Step 1: Clone Repository
```bash
git clone https://github.com/kareem18012002/CourseManagementAPI.git
cd CourseManagementAPI
```

### Step 2: Configure Database
Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=CourseManagementAPI;Integrated Security=true;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "ThisIsAVerySecureSecretKeyFor256BitEncryption12345",
    "Issuer": "CourseManagementAPI",
    "Audience": "CourseManagementAPIUsers",
    "DurationInMinutes": 60
  }
}
```

### Step 3: Apply Migrations
```bash
dotnet ef database update
```

### Step 4: Run Application
```bash
dotnet run
```

### Step 5: Access API
- **Swagger UI**: https://localhost:7001/swagger/index.html
- **API Base URL**: https://localhost:7001/api

---

## 📦 Dependencies

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.2" />
  <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.2" />
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.3" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.3" />
  <PackageReference Include="Swashbuckle.AspNetCore" Version="10.1.2" />
</ItemGroup>
```

---

## 🧪 Testing

### Create Test User (Admin)
```bash
curl -X POST https://localhost:7001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123","role":"Admin"}'
```

### Login
```bash
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

---

## 🚀 Performance Considerations

- ✅ Async/Await for all database operations
- ✅ Eager loading with `.Include()` to prevent N+1 queries
- ✅ Indexed columns for faster lookups (Username, Unique Enrollments)
- ✅ DTO mapping to reduce data transfer
- ✅ Soft delete to maintain referential integrity

---

## 🔒 Security Features

- ✅ JWT Token-based authentication
- ✅ Password stored (should be hashed in production)
- ✅ Role-based access control
- ✅ Unique constraints on Username and User-Course enrollments
- ✅ Cascade delete prevention for data integrity

---

## 📚 Project Structure

```
CourseManagementAPI/
├── Controllers/
│   ├── AuthController.cs          # Authentication endpoints
│   ├── UserController.cs          # User management
│   ├── CourseController.cs        # Course management
│   ├── LessonController.cs        # Lesson management
│   └── EnrollmentController.cs    # Enrollment management
├── Models/
│   ├── User.cs
│   ├── Course.cs
│   ├── Lesson.cs
│   ├── Enrollment.cs
│   └── CMSdbContext.cs            # Fluent API configuration
├── DTOs/
│   ├── UserDto.cs
│   ├── CourseDto.cs
│   ├── LessonDto.cs
│   └── EnrollmentDto.cs
├── Services/
│   ├── JwtService.cs              # JWT token generation
│   └── MappingService.cs          # Model to DTO mapping
├── Migrations/
├── appsettings.json
├── Program.cs                      # Dependency injection & configuration
├── DATABASE_SCHEMA.md             # Database documentation
└── README.md                      # This file
```

---

## 📈 Future Enhancements

- [ ] Add payment processing (Stripe/PayPal)
- [ ] Email notifications for enrollment
- [ ] Course ratings and reviews
- [ ] Progress tracking for students
- [ ] Certificate generation
- [ ] Advanced search and filtering
- [ ] Caching layer (Redis)
- [ ] Unit tests
- [ ] Integration tests
- [ ] API versioning

---

## 🤝 Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

---

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## 👤 Author

**Kareem**  
- GitHub: [@kareem18012002](https://github.com/kareem18012002)
- Repository: [CourseManagementAPI](https://github.com/kareem18012002/CourseManagementAPI)

---

## 📞 Support

For issues and questions, please open an issue on GitHub.

---

**Last Updated**: January 2025  
**Version**: 1.0.0
