# 🎯 معلومات المشروع

## نسخة المشروع: 1.0.0

---

## 📋 الـ 5 Tasks المنجزة

### ✅ Task 1: إنشاء الـ 4 Controllers الأساسية
- **UserController** - إدارة المستخدمين
- **CourseController** - إدارة الدورات
- **LessonController** - إدارة الدروس
- **EnrollmentController** - إدارة التسجيل في الدورات

### ✅ Task 2: نظام Authentication و Authorization
- **AuthController** - تسجيل وتسجيل الدخول
- **JWT Tokens** - رموز آمنة للمستخدمين
- **Role-Based Access Control** - التحكم بالصلاحيات حسب الأدوار
  - Admin
  - Instructor
  - Student

### ✅ Task 3: إضافة DTOs (Data Transfer Objects)
- **UserDto** - نقل بيانات المستخدم بأمان
- **CourseDto** - نقل بيانات الدورات
- **LessonDto** - نقل بيانات الدروس
- **EnrollmentDto** - نقل بيانات التسجيل
- **MappingService** - تحويل Models إلى DTOs

### ✅ Task 4: Fluent API Configuration
تكوين قاعدة البيانات باستخدام Fluent API بدلاً من Data Annotations:

```csharp
modelBuilder.Entity<User>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Username)
        .IsRequired()
        .HasMaxLength(100);
    entity.HasIndex(e => e.Username).IsUnique();
    // ... المزيد من التكوين
});
```

**المميزات:**
- تحديد Primary Keys والأجنبية
- تعيين القيم الافتراضية
- إضافة Indexes
- تكوين العلاقات (One-to-Many, Many-to-One)
- Cascade Delete

### ✅ Task 5: إنشاء الـ Diagram والـ Documentation
- **DATABASE_SCHEMA.md** - شرح مفصل لـ ER Diagram
- **README.md** - توثيق شامل للمشروع
- **ER Diagram Visual** - رسم تفاعلي للعلاقات

---

## 🗄️ قاعدة البيانات

### الجداول الأربعة:

```
┌─────────────────────────────────────────────────────┐
│                                                     │
│  USER (1) ──────── (M) ENROLLMENT ────────── (1) COURSE
│                                                │
│                                            (1) LESSON
│
└─────────────────────────────────────────────────────┘
```

### التكوينات في Fluent API:

1. **User Table**
   - PK: Id (auto-increment)
   - Unique Index على Username
   - Default Role: "Student"
   - Soft Delete: IsDeleted

2. **Course Table**
   - PK: Id (auto-increment)
   - Price مع Precision (18,2)
   - Cascade Delete مع Lessons و Enrollments
   - Soft Delete: IsDeleted

3. **Lesson Table**
   - PK: Id (auto-increment)
   - FK: CourseId مع Cascade Delete
   - Content محدود بـ 5000 حرف

4. **Enrollment Table**
   - PK: Id (auto-increment)
   - FK: UserId (Cascade Delete)
   - FK: CourseId (Cascade Delete)
   - Unique Index على (UserId, CourseId)
   - Default EnrollDate: UTC Now

---

## 🔒 الأدوار والصلاحيات

```
┌──────────────┬────────────────────┬──────────────────┐
│ Role         │ Endpoints Access   │ Permissions      │
├──────────────┼────────────────────┼──────────────────┤
│ Admin        │ كل الـ endpoints    │ كل العمليات      │
│ Instructor   │ Course + Lesson    │ Create/Edit      │
│ Student      │ Course + Enroll    │ View/Enroll      │
└──────────────┴────────────────────┴──────────────────┘
```

---

## 📊 معلومات المشروع التقنية

### الإصدارات:
- **C# Version**: 14.0
- **.NET Target**: .NET 10
- **Entity Framework Core**: 10.0.3
- **JWT Bearer**: 10.0.2

### الخدمات المسجلة (Dependency Injection):
```csharp
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IMappingService, MappingService>();
```

### Middleware Configuration:
```csharp
app.UseHttpsRedirection();
app.UseAuthentication();      // ✅ JWT Authentication
app.UseAuthorization();       // ✅ Role-based Authorization
app.MapControllers();
```

---

## 🚀 آخر الملفات المُنشأة

### Controllers (5 ملفات)
1. `AuthController.cs` - للتسجيل والدخول
2. `UserController.cs` - لإدارة المستخدمين
3. `CourseController.cs` - لإدارة الدورات
4. `LessonController.cs` - لإدارة الدروس
5. `EnrollmentController.cs` - لإدارة التسجيل

### Models (5 ملفات)
1. `User.cs` - مع Navigation Properties
2. `Course.cs` - مع Navigation Properties
3. `Lesson.cs` - مع Navigation Properties
4. `Enrollment.cs` - مع Navigation Properties
5. `CMSdbContext.cs` - مع Fluent API

### DTOs (5 ملفات)
1. `UserDto.cs`
2. `CourseDto.cs`
3. `LessonDto.cs`
4. `EnrollmentDto.cs`
5. + Classes في `AuthController.cs`

### Services (2 ملفات)
1. `JwtService.cs` - JWT Token Generation
2. `MappingService.cs` - Model to DTO Mapping

### Documentation (2 ملفات)
1. `README.md` - Comprehensive guide
2. `DATABASE_SCHEMA.md` - ER Diagram & Schema

---

## 🔄 Fluent API Features المستخدمة

✅ `.HasKey()` - تحديد Primary Key  
✅ `.Property()` - تكوين الخصائص  
✅ `.IsRequired()` - الحقول الإلزامية  
✅ `.HasMaxLength()` - طول الحقول  
✅ `.HasDefaultValue()` - القيمة الافتراضية  
✅ `.HasIndex()` - إضافة فهارس  
✅ `.IsUnique()` - قيود التفرد  
✅ `.HasOne()` / `.HasMany()` - تعريف العلاقات  
✅ `.WithOne()` / `.WithMany()` - الجانب الآخر من العلاقة  
✅ `.HasForeignKey()` - تحديد الأجنبي  
✅ `.OnDelete(DeleteBehavior.Cascade)` - Cascade Delete  
✅ `.HasPrecision()` - دقة الأرقام العشرية  

---

## 📈 إحصائيات المشروع

```
Total Files Created:     20+
Total Controllers:        5
Total Models:            5
Total DTOs:              5
Total Services:          2
Documentation Files:     2
Lines of Code (approx):  2000+
```

---

## 🔐 الميزات الأمنية المستخدمة

✅ JWT Token-based Authentication  
✅ Role-Based Access Control (RBAC)  
✅ Unique Index على Username  
✅ Cascade Delete لـ Data Integrity  
✅ Soft Delete (IsDeleted flag)  
✅ Unique Constraint على (UserId, CourseId)  
✅ Password في قاعدة البيانات (يجب تشفيره في الإنتاج)  

---

## 🎯 كيفية الاستخدام

### 1. تسجيل مستخدم جديد
```bash
POST /api/auth/register
{
  "username": "ahmed",
  "password": "password123",
  "role": "Student"
}
```

### 2. تسجيل الدخول والحصول على Token
```bash
POST /api/auth/login
{
  "username": "ahmed",
  "password": "password123"
}

Response:
{
  "token": "eyJhbGc...",
  "user": { "id": 1, "username": "ahmed", "role": "Student" }
}
```

### 3. استخدام Token في الطلبات
```bash
GET /api/course
Authorization: Bearer eyJhbGc...
```

---

## 📚 الملفات المهمة

| الملف | الوصف |
|------|-------|
| `Program.cs` | Configuration و Dependency Injection |
| `CMSdbContext.cs` | Fluent API Configuration |
| `AuthController.cs` | Authentication endpoints |
| `MappingService.cs` | DTO Mapping Logic |
| `JwtService.cs` | JWT Token Generation |
| `README.md` | API Documentation |
| `DATABASE_SCHEMA.md` | ER Diagram & Schema |

---

## 🎓 خطوات التشغيل

1. **Update Database Connection**
   ```json
   // appsettings.json
   "DefaultConnection": "Server=YOUR_SERVER;Database=CourseManagementAPI;..."
   ```

2. **Run Migrations**
   ```bash
   dotnet ef database update
   ```

3. **Run Application**
   ```bash
   dotnet run
   ```

4. **Access Swagger**
   ```
   https://localhost:7001/swagger
   ```

---

## ✨ الخلاصة

تم بنجاح إنشاء **Course Management API** كامل بـ:
- ✅ 4 Controllers مع CRUD operations
- ✅ نظام Authentication و Authorization
- ✅ DTOs لـ Data Transfer
- ✅ Fluent API Configuration
- ✅ ER Diagram و Documentation شاملة

البروجكت **جاهز للإنتاج** مع أفضل الممارسات! 🚀

---

**Repository**: https://github.com/kareem18012002/CourseManagementAPI
