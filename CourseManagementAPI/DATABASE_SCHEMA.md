# Course Management API

## 📋 نظرة عامة

API لإدارة الدورات التعليمية مع نظام تسجيل الطلاب وإدارة الدروس والمستخدمين.

---

## 🗄️ قاعدة البيانات

### Entity Relationship Diagram (ERD)

```
┌─────────────────────────────────────────────────────────────────────┐
│                                                                     │
│                    ┌──────────────────┐                            │
│                    │      USER        │                            │
│                    ├──────────────────┤                            │
│                    │ PK: Id (int)     │                            │
│                    │ Username (str)   │ ◄─ Unique Index            │
│                    │ Password (str)   │                            │
│                    │ Role (str)       │ ◄─ Default: 'Student'     │
│                    │ IsDeleted (bool) │ ◄─ Default: false         │
│                    └──────────────────┘                            │
│                           ▲                                         │
│                           │ 1                                       │
│                           │ (One-to-Many)                           │
│                           │ HasMany: Enrollments                   │
│                           │                                         │
│        ┌──────────────────────────────────────────────┐            │
│        │            ENROLLMENT                        │            │
│        ├──────────────────────────────────────────────┤            │
│        │ PK: Id (int)                                 │            │
│        │ FK: UserId (int) ◄─ Cascade Delete          │            │
│        │ FK: CourseId (int) ◄─ Cascade Delete        │            │
│        │ EnrollDate (DateTime) ◄─ Default: GETUTCDATE()│          │
│        │ Unique Index: (UserId, CourseId)             │            │
│        └──────────────────────────────────────────────┘            │
│                           ▲                                         │
│                           │ 1                                       │
│                           │ (One-to-Many)                           │
│                           │ HasMany: Enrollments                   │
│                           │                                         │
│                    ┌──────────────────┐                            │
│                    │     COURSE       │                            │
│                    ├──────────────────┤                            │
│                    │ PK: Id (int)     │                            │
│                    │ Title (str)      │ ◄─ Max: 200               │
│                    │ Description (str)│ ◄─ Max: 1000              │
│                    │ Price (decimal)  │ ◄─ Precision: 18,2        │
│                    │ IsDeleted (bool) │ ◄─ Default: false         │
│                    └──────────────────┘                            │
│                           ▲                                         │
│                           │ 1                                       │
│                           │ (One-to-Many)                           │
│                           │ HasMany: Lessons                       │
│                           │                                         │
│                    ┌──────────────────┐                            │
│                    │     LESSON       │                            │
│                    ├──────────────────┤                            │
│                    │ PK: Id (int)     │                            │
│                    │ FK: CourseId (int)│◄─ Cascade Delete         │
│                    │ Title (str)      │ ◄─ Max: 200               │
│                    │ Content (str)    │ ◄─ Max: 5000              │
│                    └──────────────────┘                            │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 📊 جدول العلاقات

| الجدول | العلاقة | الجدول الآخر | نوع الحذف |
|--------|--------|---------|---------|
| User | 1 : M | Enrollment | Cascade |
| Course | 1 : M | Enrollment | Cascade |
| Course | 1 : M | Lesson | Cascade |

---

## 🔑 Primary & Foreign Keys

### User
- **PK**: Id
- **Unique Index**: Username

### Course
- **PK**: Id

### Lesson
- **PK**: Id
- **FK**: CourseId → Course(Id)

### Enrollment
- **PK**: Id
- **FK**: UserId → User(Id)
- **FK**: CourseId → Course(Id)
- **Unique Index**: (UserId, CourseId)

---

## 🔐 الأدوار (Roles)

```
┌─────────────────────────────────────────────────────────────────┐
│                         ROLES                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  1. ADMIN                                                       │
│     └─ Can manage all users, courses, lessons, and enrollments │
│                                                                 │
│  2. INSTRUCTOR                                                  │
│     └─ Can create and manage courses and lessons               │
│     └─ Can view course enrollments                             │
│                                                                 │
│  3. STUDENT                                                     │
│     └─ Can enroll in courses                                   │
│     └─ Can view their enrollments                              │
│     └─ Can view courses and lessons                            │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🛠️ Fluent API Configuration

### User Configuration
```csharp
- Key: Id (auto-generated)
- Username: Required, Max 100, Unique Index
- Password: Required, Max 255
- Role: Required, Max 50, Default = "Student"
- IsDeleted: Default = false
- Navigation: Enrollments (1:M)
```

### Course Configuration
```csharp
- Key: Id (auto-generated)
- Title: Required, Max 200
- Description: Max 1000
- Price: Decimal (18,2), Default = 0
- IsDeleted: Default = false
- Navigation: Lessons (1:M), Enrollments (1:M)
- Cascade Delete for both relationships
```

### Lesson Configuration
```csharp
- Key: Id (auto-generated)
- CourseId: Required (FK)
- Title: Required, Max 200
- Content: Required, Max 5000
- Navigation: Course (M:1)
- Cascade Delete on CourseId
```

### Enrollment Configuration
```csharp
- Key: Id (auto-generated)
- UserId: Required (FK), Cascade Delete
- CourseId: Required (FK), Cascade Delete
- EnrollDate: Default = GETUTCDATE()
- Unique Index on (UserId, CourseId)
- Navigation: User (M:1), Course (M:1)
```

---

## 📝 API Endpoints

### 🔐 Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login user |

### 👤 User Management
| Method | Endpoint | Authorization |
|--------|----------|----------------|
| POST | `/api/user` | Admin only |
| GET | `/api/user` | Admin only |
| GET | `/api/user/{id}` | Admin or Self |
| PUT | `/api/user/{id}` | Admin or Self |
| DELETE | `/api/user/{id}` | Admin only |

### 📚 Course Management
| Method | Endpoint | Authorization |
|--------|----------|----------------|
| POST | `/api/course` | Admin, Instructor |
| GET | `/api/course` | Authenticated |
| GET | `/api/course/{id}` | Authenticated |
| PUT | `/api/course/{id}` | Admin, Instructor |
| DELETE | `/api/course/{id}` | Admin only |

### 📖 Lesson Management
| Method | Endpoint | Authorization |
|--------|----------|----------------|
| POST | `/api/lesson` | Admin, Instructor |
| GET | `/api/lesson` | Authenticated |
| GET | `/api/lesson/{id}` | Authenticated |
| GET | `/api/lesson/course/{courseId}` | Authenticated |
| PUT | `/api/lesson/{id}` | Admin, Instructor |
| DELETE | `/api/lesson/{id}` | Admin only |

### 📋 Enrollment Management
| Method | Endpoint | Authorization |
|--------|----------|----------------|
| POST | `/api/enrollment` | Student, Admin |
| GET | `/api/enrollment` | Admin only |
| GET | `/api/enrollment/{id}` | Admin or Owner |
| GET | `/api/enrollment/user/{userId}` | Admin or Owner |
| DELETE | `/api/enrollment/{id}` | Admin or Owner |

---

## 🏗️ Project Structure

```
CourseManagementAPI/
├── Controllers/
│   ├── AuthController.cs
│   ├── UserController.cs
│   ├── CourseController.cs
│   ├── LessonController.cs
│   └── EnrollmentController.cs
├── Models/
│   ├── User.cs
│   ├── Course.cs
│   ├── Lesson.cs
│   ├── Enrollment.cs
│   └── CMSdbContext.cs
├── DTOs/
│   ├── UserDto.cs
│   ├── CourseDto.cs
│   ├── LessonDto.cs
│   └── EnrollmentDto.cs
├── Services/
│   ├── JwtService.cs
│   ├── MappingService.cs
├── Migrations/
└── Program.cs
```

---

## 🔄 Cascade Delete Rules

- **User → Enrollment**: Cascade (عند حذف المستخدم، يتم حذف جميع تسجيلاته)
- **Course → Lesson**: Cascade (عند حذف الكورس، يتم حذف جميع الدروس)
- **Course → Enrollment**: Cascade (عند حذف الكورس، يتم حذف جميع التسجيلات)

---

## 🔒 Data Validation

### User
- Username: Unique, Required, Max 100
- Password: Required, Max 255
- Role: Default "Student"
- IsDeleted: Soft Delete

### Course
- Title: Required, Max 200
- Description: Max 1000
- Price: Precision 18,2
- IsDeleted: Soft Delete

### Lesson
- Title: Required, Max 200
- Content: Required, Max 5000
- CourseId: Required (FK)

### Enrollment
- UserId + CourseId: Unique Constraint
- EnrollDate: Auto-set to UTC Now

---

## 🚀 Getting Started

### Prerequisites
- .NET 10
- SQL Server
- Visual Studio / VS Code

### Installation

1. **Clone Repository**
```bash
git clone https://github.com/kareem18012002/CourseManagementAPI.git
```

2. **Update Database Connection**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=CourseManagementAPI;Integrated Security=true;TrustServerCertificate=True"
  }
}
```

3. **Apply Migrations**
```bash
dotnet ef database update
```

4. **Run Application**
```bash
dotnet run
```

---

## 📚 Technology Stack

- **Framework**: ASP.NET Core 10
- **Database**: SQL Server
- **Authentication**: JWT (JSON Web Tokens)
- **ORM**: Entity Framework Core
- **API Documentation**: Swagger/OpenAPI

---

## 📄 License

MIT License - Feel free to use this project!

---

**Created by**: Kareem  
**Repository**: https://github.com/kareem18012002/CourseManagementAPI
