-- Equipment Rental Management System - Full Database Script

-- 1. Create Database
IF DB_ID('EquipmentRentalDataBase') IS NOT NULL
    DROP DATABASE EquipmentRentalDataBase;
GO

CREATE DATABASE EquipmentRentalDataBase;
GO

USE EquipmentRentalDataBase;
GO

-- 2. Drop tables if they exist (for re-runs, in correct order for FKs)
IF OBJECT_ID('Log', 'U') IS NOT NULL DROP TABLE Log;
IF OBJECT_ID('Notification', 'U') IS NOT NULL DROP TABLE Notification;
IF OBJECT_ID('Document', 'U') IS NOT NULL DROP TABLE Document;
IF OBJECT_ID('Feedback', 'U') IS NOT NULL DROP TABLE Feedback;
IF OBJECT_ID('ReturnRecord', 'U') IS NOT NULL DROP TABLE ReturnRecord;
IF OBJECT_ID('RentalTransaction', 'U') IS NOT NULL DROP TABLE RentalTransaction;
IF OBJECT_ID('RentalRequest', 'U') IS NOT NULL DROP TABLE RentalRequest;
IF OBJECT_ID('Equipment', 'U') IS NOT NULL DROP TABLE Equipment;
IF OBJECT_ID('Category', 'U') IS NOT NULL DROP TABLE Category;
IF OBJECT_ID('[User]', 'U') IS NOT NULL DROP TABLE [User];
IF OBJECT_ID('[UserRole]', 'U') IS NOT NULL DROP TABLE [UserRole];
GO

-- 3. User and Role Tables
CREATE TABLE [UserRole] (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE -- Administrator, Manager, Customer
);

CREATE TABLE [User] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL, -- Store hashed password
    RoleId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (RoleId) REFERENCES [UserRole](RoleId)
);

-- 4. Category Table
CREATE TABLE Category (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255)
);

-- 5. Equipment Table
CREATE TABLE Equipment (
    EquipmentId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    CategoryId INT NOT NULL,
    RentalPrice DECIMAL(10,2) NOT NULL,
    AvailabilityStatus NVARCHAR(50) NOT NULL, -- Available, Unavailable, Under Maintenance, etc.
    ConditionStatus NVARCHAR(50) NOT NULL,    -- New, Good, Damaged, etc.
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId)
);

-- 6. Rental Request Table
CREATE TABLE RentalRequest (
    RentalRequestId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    EquipmentId INT NOT NULL,
    RentalStartDate DATE NOT NULL,
    RentalEndDate DATE NOT NULL,
    TotalCost DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(50) NOT NULL, -- Pending, Approved, Rejected, etc.
    Description NVARCHAR(255),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CustomerId) REFERENCES [User](UserId),
    FOREIGN KEY (EquipmentId) REFERENCES Equipment(EquipmentId)
);

-- 7. Rental Transaction Table
CREATE TABLE RentalTransaction (
    RentalTransactionId INT IDENTITY(1,1) PRIMARY KEY,
    RentalRequestId INT NOT NULL,
    EquipmentId INT NOT NULL,
    CustomerId INT NOT NULL,
    RentalStartDate DATETIME NOT NULL,
    RentalEndDate DATETIME NOT NULL,
    RentalPeriod INT NOT NULL, -- in days
    RentalFee DECIMAL(10,2) NOT NULL,
    Deposit DECIMAL(10,2) NOT NULL,
    PaymentStatus NVARCHAR(50) NOT NULL, -- Paid, Pending, Overdue
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (RentalRequestId) REFERENCES RentalRequest(RentalRequestId),
    FOREIGN KEY (EquipmentId) REFERENCES Equipment(EquipmentId),
    FOREIGN KEY (CustomerId) REFERENCES [User](UserId)
);

-- 8. Return Record Table
CREATE TABLE ReturnRecord (
    ReturnRecordId INT IDENTITY(1,1) PRIMARY KEY,
    RentalTransactionId INT NOT NULL,
    ActualReturnDate DATETIME NOT NULL,
    ReturnCondition NVARCHAR(50) NOT NULL, -- Good, Damaged, Lost
    LateReturnFee DECIMAL(10,2) NOT NULL DEFAULT 0,
    AdditionalCharges DECIMAL(10,2) NOT NULL DEFAULT 0,
    Notes NVARCHAR(255),
    FOREIGN KEY (RentalTransactionId) REFERENCES RentalTransaction(RentalTransactionId)
);

-- 9. Feedback Table
CREATE TABLE Feedback (
    FeedbackId INT IDENTITY(1,1) PRIMARY KEY,
    EquipmentId INT NOT NULL,
    UserId INT NOT NULL,
    CommentText NVARCHAR(255),
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsVisible BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (EquipmentId) REFERENCES Equipment(EquipmentId),
    FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- 10. Document Table
CREATE TABLE Document (
    DocumentId INT IDENTITY(1,1) PRIMARY KEY,
    RentalTransactionId INT,
    UserId INT,
    FileName NVARCHAR(255) NOT NULL,
    FileType NVARCHAR(50) NOT NULL,
    StoragePath NVARCHAR(255) NOT NULL,
    UploadedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (RentalTransactionId) REFERENCES RentalTransaction(RentalTransactionId),
    FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- 11. Notification Table
CREATE TABLE Notification (
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Message NVARCHAR(255) NOT NULL,
    Type NVARCHAR(50) NOT NULL, -- RentalApproval, ReturnReminder, etc.
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- 12. Log Table
CREATE TABLE Log (
    LogId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    Action NVARCHAR(255) NOT NULL,
    Exception NVARCHAR(255),
    Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
    Source NVARCHAR(50) NOT NULL, -- Web, Desktop
    FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- 13. Seeding Roles
INSERT INTO [UserRole] (RoleName) VALUES
('Administrator'),
('Manager'),
('Customer');

-- 14. Seeding Users (passwords should be hashed in real system)
INSERT INTO [User] (FullName, Email, PasswordHash, RoleId)
VALUES
('Admin User', 'admin@rental.com', 'hashed_admin_pw', 1),
('Manager User', 'manager@rental.com', 'hashed_manager_pw', 2),
('Customer User', 'customer@rental.com', 'hashed_customer_pw', 3);

-- 15. Seeding Categories
INSERT INTO Category (CategoryName, Description) VALUES
('Power Tools', 'Tools for construction and repair'),
('Cameras', 'Photography and video equipment'),
('Event Supplies', 'Equipment for events');

-- 16. Seeding Equipment
INSERT INTO Equipment (Name, Description, CategoryId, RentalPrice, AvailabilityStatus, ConditionStatus)
VALUES
('Drill Machine', 'Electric drill', 1, 15.00, 'Available', 'New'),
('DSLR Camera', 'Canon EOS', 2, 25.00, 'Available', 'Good'),
('Stage Light', 'LED stage light', 3, 10.00, 'Available', 'Good');

-- 17. Seeding Rental Requests
INSERT INTO RentalRequest (CustomerId, EquipmentId, RentalStartDate, RentalEndDate, TotalCost, Status, Description)
VALUES
(3, 1, '2025-04-20', '2025-04-22', 30.00, 'Pending', 'Need for weekend project');

-- 18. Seeding Rental Transactions
INSERT INTO RentalTransaction (RentalRequestId, EquipmentId, CustomerId, RentalStartDate, RentalEndDate, RentalPeriod, RentalFee, Deposit, PaymentStatus)
VALUES
(1, 1, 3, '2025-04-20', '2025-04-22', 2, 30.00, 10.00, 'Pending');

-- 19. Seeding Return Records
INSERT INTO ReturnRecord (RentalTransactionId, ActualReturnDate, ReturnCondition, LateReturnFee, AdditionalCharges, Notes)
VALUES
(1, '2025-04-22', 'Good', 0, 0, 'Returned on time');

-- 20. Seeding Feedback
INSERT INTO Feedback (EquipmentId, UserId, CommentText, Rating)
VALUES
(1, 3, 'Worked perfectly!', 5);

-- 21. Seeding Documents
INSERT INTO Document (RentalTransactionId, UserId, FileName, FileType, StoragePath)
VALUES
(1, 3, 'agreement.pdf', 'PDF', '/docs/agreement.pdf');

-- 22. Seeding Notifications
INSERT INTO Notification (UserId, Message, Type)
VALUES
(3, 'Your rental request has been received.', 'RentalRequest');

-- 23. Seeding Logs
INSERT INTO Log (UserId, Action, Exception, Source)
VALUES
(1, 'Login', NULL, 'Web'),
(3, 'Submitted rental request', NULL, 'Web');

-- End of Script