# 📚 دليل Course Management API

## جدول المحتويات
1. [مقدمة](#مقدمة)
2. [الهيكل المعماري](#الهيكل-المعماري)
3. [Fluent API](#fluent-api)
4. [ER Diagram](#er-diagram)
5. [التثبيت والتشغيل](#التثبيت-والتشغيل)
6. [اختبار الـ API](#اختبار-الـ-api)

---

## مقدمة

هذا المشروع هو **API شامل لإدارة الدورات التعليمية** يتضمن:

✅ نظام تسجيل والدخول مع JWT  
✅ إدارة المستخدمين والأدوار  
✅ إدارة الدورات والدروس  
✅ نظام تسجيل الطلاب  
✅ قاعدة بيانات محسنة مع Fluent API  

---

## الهيكل المعماري

### أطبقة المشروع

```
┌─────────────────────────────────────────────────┐
│              API Controllers                     │
│  (UserController, CourseController, ...)       │
└─────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────┐
│              DTOs & Mapping                      │
│  (UserDto, CourseDto, MappingService)          │
└─────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────┐
│              Services                           │
│  (JwtService, MappingService)                  │
└─────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────┐
│         Entity Framework Core                    │
│         (DbContext + Fluent API)                │
└─────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────┐
│          SQL Server Database                    │
│  (Users, Courses, Lessons, Enrollments)        │
└─────────────────────────────────────────────────┘
```

---

## Fluent API

### ما هي Fluent API؟

**Fluent API** هي طريقة برمجية لتكوين نماذج Entity Framework Core بدلاً من استخدام Data Annotations.

### مثال من المشروع:

```csharp
// في CMSdbContext.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // تكوين User Entity
    modelBuilder.Entity<User>(entity =>
    {
        entity.HasKey(e => e.Id);  // تحديد Primary Key
        
        entity.Property(e => e.Username)
            .IsRequired()           // حقل إلزامي
            .HasMaxLength(100);     // الحد الأقصى للطول
        
        entity.HasIndex(e => e.Username)
            .IsUnique();            // فهرس فريد
        
        // العلاقات
        entity.HasMany(e => e.Enrollments)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);  // حذف متسلسل
    });
}
```

### الفوائد:

1. **المرونة** - تكوين معقد بدون تعليقات
2. **الوضوح** - الكود قابل للقراءة
3. **الأداء** - تحكم كامل على الفهارس والقيود
4. **إعادة الاستخدام** - توافق مع الأنماط

---

## ER Diagram

### الجداول والعلاقات

```
┌──────────────┐
│   USER       │
├──────────────┤
│ PK: Id       │
│ Username     │──┐
│ Password     │  │
│ Role         │  │ (1:M)
│ IsDeleted    │  │
└──────────────┘  │
                  │
    ┌─────────────┴──────────────┐
    │                            │
    ↓                            ↓
┌──────────────┐        ┌──────────────┐
│ ENROLLMENT   │        │   COURSE     │
├──────────────┤        ├──────────────┤
│ PK: Id       │        │ PK: Id       │
│ FK: UserId   │        │ Title        │ (1:M)
│ FK: CourseId │        │ Description  │────┐
│ EnrollDate   │        │ Price        │    │
└──────────────┘        │ IsDeleted    │    │
                        └──────────────┘    │
                                            │
                                            ↓
                                    ┌──────────────┐
                                    │   LESSON     │
                                    ├──────────────┤
                                    │ PK: Id       │
                                    │ FK: CourseId │
                                    │ Title        │
                                    │ Content      │
                                    └──────────────┘
```

### الأرقام في العلاقات:

- **1** = واحد
- **M** = متعدد (Many)
- **1:M** = واحد له علاقة مع متعدد

---

## التثبيت والتشغيل

### متطلبات النظام

```
✓ .NET 10 SDK
✓ SQL Server 2019+
✓ Visual Studio 2022 / VS Code
✓ Postman (اختياري للاختبار)
```

### خطوات التثبيت

#### 1️⃣ Clone Repository
```bash
git clone https://github.com/kareem18012002/CourseManagementAPI.git
cd CourseManagementAPI
```

#### 2️⃣ تعديل Connection String
**ملف**: `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=CourseManagementAPI;Integrated Security=true;TrustServerCertificate=True"
  }
}
```

#### 3️⃣ تطبيق Migrations

**الطريقة الأولى (الموصى بها):**
```bash
dotnet ef database update
```

**الطريقة الثانية (يدويًا):**
- افتح `Database_Setup.sql` في SQL Server Management Studio
- نفذ الـ script

#### 4️⃣ تشغيل المشروع
```bash
dotnet run
```

#### 5️⃣ الوصول للـ API
```
🌐 Swagger: https://localhost:7001/swagger
📝 API Base: https://localhost:7001/api
```

---

## اختبار الـ API

### الطريقة 1: استخدام Postman

1. **Import Collection**
   - افتح Postman
   - `File → Import`
   - اختر `CourseManagementAPI.postman_collection.json`

2. **تسجيل الدخول كـ Admin**
   ```
   POST /api/auth/login
   {
     "username": "admin",
     "password": "admin123"
   }
   ```
   - نسخ `token` من الرد
   - ضع في Postman Variables: `admin_token`

3. **اختبر الـ Endpoints**
   - اختر أي endpoint من Collection
   - استبدل المتغيرات بقيمك
   - أرسل الطلب

### الطريقة 2: استخدام cURL

```bash
# تسجيل الدخول
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'

# احصل على Token من الرد

# اختبر endpoint
curl -X GET https://localhost:7001/api/course \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### الطريقة 3: استخدام Swagger UI

1. افتح `https://localhost:7001/swagger`
2. اضغط "Authorize" 
3. أدخل Token
4. اختبر الـ endpoints مباشرة

---

## البيانات النموذجية (Sample Data)

### المستخدمون:
```
┌──────────────┬─────────────────┬─────────┐
│ Username     │ Password        │ Role    │
├──────────────┼─────────────────┼─────────┤
│ admin        │ admin123        │ Admin   │
│ instructor   │ instructor123   │ Instructor│
│ student1     │ student123      │ Student │
│ student2     │ student123      │ Student │
│ student3     │ student123      │ Student │
└──────────────┴─────────────────┴─────────┘
```

### الدورات:
```
┌─────────────────────────┬──────────┐
│ Title                   │ Price    │
├─────────────────────────┼──────────┤
│ C# Basics               │ $49.99   │
│ ASP.NET Core            │ $79.99   │
│ Entity Framework Core   │ $59.99   │
│ SQL Server              │ $89.99   │
└─────────────────────────┴──────────┘
```

---

## الأخطاء الشائعة وحلولها

### ❌ خطأ: Connection String
**المشكلة**: `System.Data.SqlClient.SqlException`

**الحل**:
```json
// تحقق من server name و database name
"DefaultConnection": "Server=YOUR_SERVER;Database=CourseManagementAPI;..."
```

### ❌ خطأ: JWT Token Invalid
**المشكلة**: `401 Unauthorized`

**الحل**:
- تأكد من إدخال Token بشكل صحيح
- Format: `Authorization: Bearer YOUR_TOKEN`
- تحقق من انتهاء صلاحية Token

### ❌ خطأ: Role Not Authorized
**المشكلة**: `403 Forbidden`

**الحل**:
- تحقق من دور المستخدم
- Admin يمكنه كل شيء
- Instructor يمكنه إنشاء الدورات
- Student يمكنه التسجيل فقط

---

## ملفات مهمة

| الملف | الوصف |
|------|-------|
| `Program.cs` | التكوين الرئيسي |
| `Models/CMSdbContext.cs` | Fluent API |
| `Controllers/*` | API Endpoints |
| `DTOs/*` | Data Transfer Objects |
| `Services/JwtService.cs` | JWT Tokens |
| `appsettings.json` | الإعدادات |
| `Database_Setup.sql` | SQL Script |
| `README.md` | التوثيق الكامل |
| `DATABASE_SCHEMA.md` | شرح القاعدة |

---

## الأوامر المفيدة

```bash
# إنشاء migration جديد
dotnet ef migrations add MigrationName

# عرض الـ migrations
dotnet ef migrations list

# حذف آخر migration
dotnet ef migrations remove

# تحديث قاعدة البيانات
dotnet ef database update

# إنشاء SQL script
dotnet ef migrations script

# تنظيف المشروع
dotnet clean

# إعادة بناء
dotnet build

# تشغيل
dotnet run
```

---

## نصائح للإنتاج

1. **قم بتشفير Passwords**
   ```csharp
   // استخدم BCrypt أو Argon2
   password = BCrypt.Net.BCrypt.HashPassword(password);
   ```

2. **استخدم Environment Variables**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "${DATABASE_CONNECTION_STRING}"
   }
   ```

3. **أضف HTTPS**
   ```csharp
   app.UseHttpsRedirection();
   ```

4. **أضف CORS**
   ```csharp
   builder.Services.AddCors(options => {
       options.AddPolicy("AllowAll", 
           builder => builder.AllowAnyOrigin()...);
   });
   ```

5. **Log & Monitor**
   ```csharp
   builder.Services.AddLogging();
   ```

---

## الخلاصة

لقد تم إنشاء:

✅ **4 Controllers** مع CRUD operations  
✅ **4 Models** مع Navigation Properties  
✅ **Fluent API** للتكوين المتقدم  
✅ **JWT Authentication** و Authorization  
✅ **DTOs** للأمان والفصل  
✅ **ER Diagram** واضح  
✅ **SQL Script** لإنشاء قاعدة البيانات  
✅ **Postman Collection** للاختبار  
✅ **Documentation** شاملة  

---

## المساعدة والدعم

- 📚 [README.md](./README.md) - توثيق شامل
- 📊 [DATABASE_SCHEMA.md](./DATABASE_SCHEMA.md) - شرح قاعدة البيانات
- 📋 [PROJECT_INFO.md](./PROJECT_INFO.md) - معلومات المشروع
- 🐛 [GitHub Issues](https://github.com/kareem18012002/CourseManagementAPI/issues)

---

**أنت الآن جاهز لبدء العمل مع Course Management API!** 🚀
