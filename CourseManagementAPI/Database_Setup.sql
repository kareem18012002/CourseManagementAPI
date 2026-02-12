-- ============================================
-- Course Management API - Database Creation Script
-- ============================================
-- This script creates the database structure
-- Alternative to using EF Core migrations
-- ============================================

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CourseManagementAPI')
BEGIN
    CREATE DATABASE CourseManagementAPI;
END
GO

USE CourseManagementAPI;
GO

-- ============================================
-- 1. Create Users Table
-- ============================================
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(100) NOT NULL UNIQUE,
        Password NVARCHAR(255) NOT NULL,
        Role NVARCHAR(50) NOT NULL DEFAULT 'Student',
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT CHK_Role CHECK (Role IN ('Admin', 'Instructor', 'Student'))
    );

    CREATE INDEX IX_Users_Username ON dbo.Users(Username);
    CREATE INDEX IX_Users_IsDeleted ON dbo.Users(IsDeleted);
END
GO

-- ============================================
-- 2. Create Courses Table
-- ============================================
IF OBJECT_ID('dbo.Courses', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Courses
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000) NULL,
        Price DECIMAL(18,2) NOT NULL DEFAULT 0,
        IsDeleted BIT NOT NULL DEFAULT 0
    );

    CREATE INDEX IX_Courses_IsDeleted ON dbo.Courses(IsDeleted);
    CREATE INDEX IX_Courses_Title ON dbo.Courses(Title);
END
GO

-- ============================================
-- 3. Create Lessons Table
-- ============================================
IF OBJECT_ID('dbo.Lessons', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Lessons
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CourseId INT NOT NULL,
        Title NVARCHAR(200) NOT NULL,
        Content NVARCHAR(5000) NOT NULL,
        CONSTRAINT FK_Lessons_Courses FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_Lessons_CourseId ON dbo.Lessons(CourseId);
    CREATE INDEX IX_Lessons_Title ON dbo.Lessons(Title);
END
GO

-- ============================================
-- 4. Create Enrollments Table
-- ============================================
IF OBJECT_ID('dbo.Enrollments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Enrollments
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        CourseId INT NOT NULL,
        EnrollDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Enrollments_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Enrollments_Courses FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_Enrollments_UserCourse UNIQUE (UserId, CourseId)
    );

    CREATE INDEX IX_Enrollments_UserId ON dbo.Enrollments(UserId);
    CREATE INDEX IX_Enrollments_CourseId ON dbo.Enrollments(CourseId);
    CREATE INDEX IX_Enrollments_EnrollDate ON dbo.Enrollments(EnrollDate);
END
GO

-- ============================================
-- 5. Insert Sample Data
-- ============================================

-- Clear existing data
DELETE FROM dbo.Enrollments;
DELETE FROM dbo.Lessons;
DELETE FROM dbo.Courses;
DELETE FROM dbo.Users;

-- Reset identity seeds
DBCC CHECKIDENT ('dbo.Users', RESEED, 0);
DBCC CHECKIDENT ('dbo.Courses', RESEED, 0);
DBCC CHECKIDENT ('dbo.Lessons', RESEED, 0);
DBCC CHECKIDENT ('dbo.Enrollments', RESEED, 0);

-- Insert Users
INSERT INTO dbo.Users (Username, Password, Role, IsDeleted) VALUES
('admin', 'admin123', 'Admin', 0),
('instructor', 'instructor123', 'Instructor', 0),
('student1', 'student123', 'Student', 0),
('student2', 'student123', 'Student', 0),
('student3', 'student123', 'Student', 0);

-- Insert Courses
INSERT INTO dbo.Courses (Title, Description, Price, IsDeleted) VALUES
('C# Basics', 'Learn C# from scratch', 49.99, 0),
('ASP.NET Core', 'Build web applications with ASP.NET Core', 79.99, 0),
('Entity Framework Core', 'Database access with EF Core', 59.99, 0),
('SQL Server', 'Advanced SQL Server concepts', 89.99, 0);

-- Insert Lessons
INSERT INTO dbo.Lessons (CourseId, Title, Content) VALUES
(1, 'Introduction to C#', 'Welcome to C# programming. In this lesson, you will learn the basics...'),
(1, 'Variables and Data Types', 'In this lesson, we cover int, string, bool, decimal, and more...'),
(1, 'Control Flow', 'Learn about if-else, switch, and loops in C#...'),
(2, 'Getting Started with ASP.NET Core', 'Introduction to ASP.NET Core framework...'),
(2, 'Building APIs', 'Create RESTful APIs with ASP.NET Core...'),
(3, 'DbContext and Models', 'Understanding DbContext and Entity Models...'),
(3, 'LINQ Queries', 'Learn LINQ for database queries...'),
(4, 'Creating Tables and Indexes', 'Database design and optimization...');

-- Insert Enrollments
INSERT INTO dbo.Enrollments (UserId, CourseId, EnrollDate) VALUES
(3, 1, GETUTCDATE()),
(3, 2, GETUTCDATE()),
(4, 1, GETUTCDATE()),
(4, 3, GETUTCDATE()),
(5, 1, GETUTCDATE()),
(5, 4, GETUTCDATE());

GO

-- ============================================
-- 6. Verify Data
-- ============================================

PRINT '--- Users ---'
SELECT * FROM dbo.Users;

PRINT '--- Courses ---'
SELECT * FROM dbo.Courses;

PRINT '--- Lessons ---'
SELECT * FROM dbo.Lessons;

PRINT '--- Enrollments ---'
SELECT * FROM dbo.Enrollments;

GO

-- ============================================
-- Query: Get User with their Enrollments
-- ============================================
SELECT 
    u.Id as UserId,
    u.Username,
    u.Role,
    c.Id as CourseId,
    c.Title,
    c.Price,
    e.EnrollDate
FROM dbo.Users u
LEFT JOIN dbo.Enrollments e ON u.Id = e.UserId
LEFT JOIN dbo.Courses c ON e.CourseId = c.Id
WHERE u.IsDeleted = 0
ORDER BY u.Id, c.Id;

GO

-- ============================================
-- Query: Get Course with its Lessons and Enrollments
-- ============================================
SELECT 
    c.Id as CourseId,
    c.Title as CourseName,
    c.Description,
    c.Price,
    l.Id as LessonId,
    l.Title as LessonTitle,
    COUNT(e.Id) as TotalEnrollments
FROM dbo.Courses c
LEFT JOIN dbo.Lessons l ON c.Id = l.CourseId
LEFT JOIN dbo.Enrollments e ON c.Id = e.CourseId
WHERE c.IsDeleted = 0
GROUP BY c.Id, c.Title, c.Description, c.Price, l.Id, l.Title
ORDER BY c.Id;

GO
