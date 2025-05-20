-- Sample Data for Equipment Rental Management System

-- Clear existing data (optional)
DELETE FROM Log;
DELETE FROM Notification;
DELETE FROM Document;
DELETE FROM Feedback;
DELETE FROM ReturnRecord;
DELETE FROM RentalTransaction;
DELETE FROM RentalRequest;
DELETE FROM Equipment;
DELETE FROM Category;
DELETE FROM [User];
DELETE FROM [UserRole];

-- Reset identity columns (optional)
DBCC CHECKIDENT ('Log', RESEED, 0);
DBCC CHECKIDENT ('Notification', RESEED, 0);
DBCC CHECKIDENT ('Document', RESEED, 0);
DBCC CHECKIDENT ('Feedback', RESEED, 0);
DBCC CHECKIDENT ('ReturnRecord', RESEED, 0);
DBCC CHECKIDENT ('RentalTransaction', RESEED, 0);
DBCC CHECKIDENT ('RentalRequest', RESEED, 0);
DBCC CHECKIDENT ('Equipment', RESEED, 0);
DBCC CHECKIDENT ('Category', RESEED, 0);
DBCC CHECKIDENT ('[User]', RESEED, 0);
DBCC CHECKIDENT ('[UserRole]', RESEED, 0);

-- 1. Seeding Roles
INSERT INTO [UserRole] (RoleName) VALUES
('Administrator'),
('Manager'),
('Customer');

-- 2. Seeding Users (using a simple hash for demo purposes)
INSERT INTO [User] (FullName, Email, PasswordHash, RoleId, IsActive, CreatedAt)
VALUES
-- Administrators
('John Admin', 'admin@rental.com', 'AQAAAAEAACcQAAAAEKXr0NNS3RUMPDy4WhDEQSvn4ButYb8QrBNPwXBJjUL5K5jfU5eFiVxiQxPtRcYpfg==', 1, 1, '2025-01-01'),
('Sarah Admin', 'sarah@rental.com', 'AQAAAAEAACcQAAAAEKXr0NNS3RUMPDy4WhDEQSvn4ButYb8QrBNPwXBJjUL5K5jfU5eFiVxiQxPtRcYpfg==', 1, 1, '2025-01-15'),

-- Managers
('Mike Manager', 'manager@rental.com', 'AQAAAAEAACcQAAAAEKXr0NNS3RUMPDy4WhDEQSvn4ButYb8QrBNPwXBJjUL5K5jfU5eFiVxiQxPtRcYpfg==', 2, 1, '2025-01-10'),
('Lisa Manager', 'lisa@rental.com', 'AQAAAAEAACcQAAAAEKXr0NNS3RUMPDy4WhDEQSvn4ButYb8QrBNPwXBJjUL5K5jfU5eFiVxiQxPtRcYpfg==', 2, 1, '2025-02-01'),
('David Manager', 'david@rental.com', 'AQAAAAEAACcQAAAAEKXr0NNS3RUMPDy4WhDEQSvn4ButYb8QrBNPwXBJjUL5K5jfU5eFiVxiQxPtRcYpfg==', 2, 1, '2025-02-15'),

-- Customers
('Alice Customer', 'customer@rental.com', 'AQAAAAEAACcQAAAAEKXr0NNS3RUMPDy4WhDEQSvn4ButYb8QrBNPwXBJjUL5K5jfU5eFiVxiQxPtRcYpfg==', 3, 1, '2025-02-05'),
('Bob Smith', 'bob@example.com', 'AQAAAAEAACcQAAAAEKXr0NNS3RUMPDy4WhDEQSvn4ButYb8QrBNPwXBJjUL5K5jfU5eFiVxiQxPtRcYpfg==', 3, 1, '2025-02-10'),
('Carol Johnson', 'carol@example.com', 'AQAAAAEAACcQAAAAEKXr0NNS3RUMPDy4WhDEQSvn4ButYb8QrBNPwXBJjUL5K5jfU5eFiVxiQxPtRcYpfg==', 3, 1, '2025-02-15'),
('Daniel Wilson', 'daniel@example.com', 'AQAAAAEAACcQAAAAEKXr0NNS3RUMPDy4WhDEQSvn4ButYb8QrBNPwXBJjUL5K5jfU5eFiVxiQxPtRcYpfg==', 3, 1, '2025-02-20'),
('Emma Brown', 'emma@example.com', 'AQAAAAEAACcQAAAAEKXr0NNS3RUMPDy4WhDEQSvn4ButYb8QrBNPwXBJjUL5K5jfU5eFiVxiQxPtRcYpfg==', 3, 1, '2025-03-01'),
('Frank Davis', 'frank@example.com', 'AQAAAAEAACcQAAAAEKXr0NNS3RUMPDy4WhDEQSvn4ButYb8QrBNPwXBJjUL5K5jfU5eFiVxiQxPtRcYpfg==', 3, 0, '2025-03-05'); -- Inactive user

-- 3. Seeding Categories
INSERT INTO Category (CategoryName, Description) VALUES
('Power Tools', 'Tools for construction and home improvement projects'),
('Cameras & Photography', 'Photography and video equipment for professionals and hobbyists'),
('Event Equipment', 'Equipment for events, parties, and gatherings'),
('Gardening Tools', 'Tools for gardening and landscaping'),
('Camping Gear', 'Equipment for outdoor camping adventures'),
('Audio Equipment', 'Sound systems and audio recording equipment'),
('Home Improvement', 'Tools and equipment for home renovation projects'),
('Sports Equipment', 'Equipment for various sports activities');

-- 4. Seeding Equipment
INSERT INTO Equipment (Name, Description, CategoryId, RentalPrice, AvailabilityStatus, ConditionStatus, CreatedAt)
VALUES
-- Power Tools
('Bosch Drill Machine', 'Professional electric drill with multiple speed settings', 1, 15.99, 'Available', 'New', '2025-01-15'),
('DeWalt Power Saw', 'Heavy-duty circular saw for construction projects', 1, 25.50, 'Available', 'Good', '2025-01-20'),
('Makita Hammer Drill', 'Hammer drill for concrete and masonry work', 1, 18.75, 'Under Maintenance', 'Good', '2025-01-25'),
('Milwaukee Impact Driver', 'Cordless impact driver with lithium-ion battery', 1, 22.00, 'Available', 'New', '2025-02-01'),
('Ryobi Angle Grinder', 'Angle grinder for cutting and grinding metal', 1, 16.50, 'Available', 'Good', '2025-02-05'),

-- Cameras & Photography
('Canon EOS 5D Mark IV', 'Professional DSLR camera with 30.4MP sensor', 2, 75.00, 'Available', 'Excellent', '2025-01-10'),
('Sony Alpha a7 III', 'Mirrorless camera with 24.2MP sensor', 2, 65.00, 'Available', 'New', '2025-01-15'),
('Nikon D850', 'DSLR camera with 45.7MP sensor', 2, 70.00, 'Unavailable', 'Good', '2025-01-20'),
('DJI Ronin Gimbal', 'Professional 3-axis gimbal stabilizer', 2, 45.00, 'Available', 'Good', '2025-02-01'),
('GoPro Hero 10', 'Action camera for outdoor adventures', 2, 35.00, 'Available', 'Excellent', '2025-02-10'),

-- Event Equipment
('JBL Party Speaker', 'Powerful speaker system with Bluetooth connectivity', 3, 50.00, 'Available', 'Good', '2025-01-05'),
('LED Stage Lights', 'Colorful LED lights for stage performances', 3, 30.00, 'Available', 'Good', '2025-01-15'),
('Folding Tables (set of 5)', 'Portable tables for events', 3, 25.00, 'Available', 'Good', '2025-01-25'),
('Portable Projector', 'HD projector with HDMI and USB connections', 3, 40.00, 'Available', 'Excellent', '2025-02-05'),
('Karaoke Machine', 'Complete karaoke system with microphones', 3, 35.00, 'Under Maintenance', 'Fair', '2025-02-15'),

-- Gardening Tools
('Lawn Mower', 'Gas-powered lawn mower for medium to large lawns', 4, 35.00, 'Available', 'Good', '2025-02-01'),
('Hedge Trimmer', 'Electric hedge trimmer for precise cutting', 4, 20.00, 'Available', 'Good', '2025-02-10'),
('Garden Tiller', 'Rotary tiller for soil preparation', 4, 30.00, 'Available', 'Good', '2025-02-15'),
('Pressure Washer', 'High-pressure cleaner for outdoor surfaces', 4, 28.00, 'Available', 'Excellent', '2025-02-20'),
('Leaf Blower', 'Powerful leaf blower with vacuum function', 4, 18.00, 'Unavailable', 'Fair', '2025-03-01'),

-- Camping Gear
('4-Person Tent', 'Spacious tent with weather protection', 5, 25.00, 'Available', 'Good', '2025-02-05'),
('Sleeping Bag Set', 'Set of 2 warm sleeping bags', 5, 15.00, 'Available', 'Good', '2025-02-10'),
('Portable Grill', 'Compact propane grill for camping', 5, 20.00, 'Available', 'Good', '2025-02-15'),
('Camping Chairs (set of 4)', 'Comfortable folding chairs for outdoor use', 5, 18.00, 'Available', 'Good', '2025-02-20'),
('Cooler Box', 'Large insulated cooler for food and drinks', 5, 12.00, 'Available', 'Excellent', '2025-03-01');

-- 5. Seeding Rental Requests
INSERT INTO RentalRequest (CustomerId, EquipmentId, RentalStartDate, RentalEndDate, TotalCost, Status, Description, CreatedAt)
VALUES
-- Completed/Approved Requests
(6, 1, '2025-03-10', '2025-03-12', 31.98, 'Approved', 'Need for home renovation project', '2025-03-05'),
(7, 6, '2025-03-15', '2025-03-18', 225.00, 'Approved', 'For wedding photography', '2025-03-10'),
(8, 11, '2025-03-20', '2025-03-21', 50.00, 'Approved', 'Birthday party', '2025-03-15'),
(9, 16, '2025-04-01', '2025-04-02', 35.00, 'Approved', 'Spring lawn maintenance', '2025-03-25'),
(10, 21, '2025-04-05', '2025-04-07', 50.00, 'Approved', 'Weekend camping trip', '2025-03-30'),
(6, 7, '2025-04-10', '2025-04-12', 130.00, 'Approved', 'Product photography for online store', '2025-04-05'),
(7, 2, '2025-04-15', '2025-04-16', 25.50, 'Approved', 'Home improvement project', '2025-04-10'),

-- Pending Requests
(8, 12, '2025-05-01', '2025-05-03', 60.00, 'Pending', 'School graduation party', '2025-04-20'),
(9, 17, '2025-05-05', '2025-05-06', 20.00, 'Pending', 'Hedge trimming for spring', '2025-04-25'),
(10, 22, '2025-05-10', '2025-05-12', 45.00, 'Pending', 'Family camping weekend', '2025-04-30'),
(6, 3, '2025-05-15', '2025-05-16', 18.75, 'Pending', 'Bathroom renovation', '2025-05-10'),

-- Rejected Requests
(7, 8, '2025-04-20', '2025-04-25', 350.00, 'Rejected', 'Too long rental period', '2025-04-15'),
(8, 13, '2025-04-22', '2025-04-23', 25.00, 'Rejected', 'Equipment not available on requested dates', '2025-04-17');

-- 6. Seeding Rental Transactions
INSERT INTO RentalTransaction (RentalRequestId, EquipmentId, CustomerId, RentalStartDate, RentalEndDate, RentalPeriod, RentalFee, Deposit, PaymentStatus, CreatedAt)
VALUES
-- Completed Transactions
(1, 1, 6, '2025-03-10 09:00:00', '2025-03-12 17:00:00', 2, 31.98, 20.00, 'Paid', '2025-03-06'),
(2, 6, 7, '2025-03-15 10:00:00', '2025-03-18 16:00:00', 3, 225.00, 100.00, 'Paid', '2025-03-11'),
(3, 11, 8, '2025-03-20 14:00:00', '2025-03-21 14:00:00', 1, 50.00, 30.00, 'Paid', '2025-03-16'),
(4, 16, 9, '2025-04-01 08:00:00', '2025-04-02 18:00:00', 1, 35.00, 20.00, 'Paid', '2025-03-26'),
(5, 21, 10, '2025-04-05 09:00:00', '2025-04-07 18:00:00', 2, 50.00, 25.00, 'Paid', '2025-03-31'),

-- Active Transactions
(6, 7, 6, '2025-04-10 10:00:00', '2025-04-12 17:00:00', 2, 130.00, 50.00, 'Paid', '2025-04-06'),
(7, 2, 7, '2025-04-15 09:00:00', '2025-04-16 18:00:00', 1, 25.50, 15.00, 'Pending', '2025-04-11');

-- 7. Seeding Return Records
INSERT INTO ReturnRecord (RentalTransactionId, ActualReturnDate, ReturnCondition, LateReturnFee, AdditionalCharges, Notes)
VALUES
(1, '2025-03-12 16:30:00', 'Good', 0.00, 0.00, 'Returned on time in good condition'),
(2, '2025-03-18 17:15:00', 'Good', 0.00, 0.00, 'Slight delay but no additional charges applied'),
(3, '2025-03-21 13:45:00', 'Good', 0.00, 0.00, 'Returned early'),
(4, '2025-04-02 18:30:00', 'Good', 5.00, 0.00, 'Returned 30 minutes late'),
(5, '2025-04-07 17:45:00', 'Damaged', 0.00, 15.00, 'Tent has a small tear, repair fee charged');

-- 8. Seeding Feedback
INSERT INTO Feedback (EquipmentId, UserId, CommentText, Rating, CreatedAt, IsVisible)
VALUES
(1, 6, 'Excellent drill, worked perfectly for my project!', 5, '2025-03-13', 1),
(6, 7, 'Camera was in great condition, produced amazing photos.', 5, '2025-03-19', 1),
(11, 8, 'Speaker was loud and clear, perfect for our party.', 4, '2025-03-22', 1),
(16, 9, 'Lawn mower worked well but was a bit difficult to start.', 3, '2025-04-03', 1),
(21, 10, 'Tent was spacious but had a small tear when set up.', 2, '2025-04-08', 1),
(2, 7, 'Power saw worked great for my project.', 4, '2025-04-17', 1),
(7, 6, 'Camera was amazing but battery life was shorter than expected.', 4, '2025-04-13', 0);

-- 9. Seeding Documents
INSERT INTO Document (RentalTransactionId, UserId, FileName, FileType, StoragePath, UploadedAt)
VALUES
(1, 6, 'rental_agreement_1.pdf', 'PDF', '/documents/agreements/rental_agreement_1.pdf', '2025-03-06'),
(1, 6, 'id_verification_6.jpg', 'Image', '/documents/verifications/id_verification_6.jpg', '2025-03-06'),
(2, 7, 'rental_agreement_2.pdf', 'PDF', '/documents/agreements/rental_agreement_2.pdf', '2025-03-11'),
(2, 7, 'equipment_condition_6.jpg', 'Image', '/documents/conditions/equipment_condition_6.jpg', '2025-03-11'),
(3, 8, 'rental_agreement_3.pdf', 'PDF', '/documents/agreements/rental_agreement_3.pdf', '2025-03-16'),
(4, 9, 'rental_agreement_4.pdf', 'PDF', '/documents/agreements/rental_agreement_4.pdf', '2025-03-26'),
(5, 10, 'rental_agreement_5.pdf', 'PDF', '/documents/agreements/rental_agreement_5.pdf', '2025-03-31'),
(5, 10, 'damage_report_21.pdf', 'PDF', '/documents/damages/damage_report_21.pdf', '2025-04-08'),
(6, 6, 'rental_agreement_6.pdf', 'PDF', '/documents/agreements/rental_agreement_6.pdf', '2025-04-06'),
(7, 7, 'rental_agreement_7.pdf', 'PDF', '/documents/agreements/rental_agreement_7.pdf', '2025-04-11');

-- 10. Seeding Notifications
INSERT INTO Notification (UserId, Message, Type, IsRead, CreatedAt)
VALUES
-- Customer Notifications
(6, 'Your rental request for Bosch Drill Machine has been approved.', 'RentalApproval', 1, '2025-03-06'),
(6, 'Your rental for Bosch Drill Machine starts tomorrow.', 'RentalReminder', 1, '2025-03-09'),
(6, 'Your rental for Bosch Drill Machine ends today. Please return by 5:00 PM.', 'ReturnReminder', 1, '2025-03-12'),
(6, 'Thank you for returning Bosch Drill Machine. Your deposit has been refunded.', 'DepositRefund', 1, '2025-03-13'),
(6, 'Your rental request for Sony Alpha a7 III has been approved.', 'RentalApproval', 1, '2025-04-06'),
(6, 'Your rental for Sony Alpha a7 III starts tomorrow.', 'RentalReminder', 0, '2025-04-09'),

(7, 'Your rental request for Canon EOS 5D Mark IV has been approved.', 'RentalApproval', 1, '2025-03-11'),
(7, 'Your rental for Canon EOS 5D Mark IV starts tomorrow.', 'RentalReminder', 1, '2025-03-14'),
(7, 'Your rental for Canon EOS 5D Mark IV ends today. Please return by 4:00 PM.', 'ReturnReminder', 1, '2025-03-18'),
(7, 'Your rental request for Nikon D850 has been rejected.', 'RentalRejection', 1, '2025-04-16'),
(7, 'Your rental request for DeWalt Power Saw has been approved.', 'RentalApproval', 0, '2025-04-11'),

(8, 'Your rental request for JBL Party Speaker has been approved.', 'RentalApproval', 1, '2025-03-16'),
(8, 'Your rental for JBL Party Speaker starts tomorrow.', 'RentalReminder', 1, '2025-03-19'),
(8, 'Your rental request for Folding Tables has been rejected.', 'RentalRejection', 1, '2025-04-18'),
(8, 'Your rental request for LED Stage Lights is pending approval.', 'RentalRequest', 0, '2025-04-20'),

(9, 'Your rental request for Lawn Mower has been approved.', 'RentalApproval', 1, '2025-03-26'),
(9, 'Your rental for Lawn Mower starts tomorrow.', 'RentalReminder', 1, '2025-03-31'),
(9, 'Your rental for Lawn Mower ends today. Please return by 6:00 PM.', 'ReturnReminder', 1, '2025-04-02'),
(9, 'Late fee of $5.00 has been charged for late return of Lawn Mower.', 'LateFee', 1, '2025-04-03'),
(9, 'Your rental request for Hedge Trimmer is pending approval.', 'RentalRequest', 0, '2025-04-25'),

(10, 'Your rental request for 4-Person Tent has been approved.', 'RentalApproval', 1, '2025-03-31'),
(10, 'Your rental for 4-Person Tent starts tomorrow.', 'RentalReminder', 1, '2025-04-04'),
(10, 'Your rental for 4-Person Tent ends today. Please return by 6:00 PM.', 'ReturnReminder', 1, '2025-04-07'),
(10, 'Additional charge of $15.00 has been applied for damage to 4-Person Tent.', 'DamageCharge', 1, '2025-04-08'),
(10, 'Your rental request for Portable Grill is pending approval.', 'RentalRequest', 0, '2025-04-30'),

-- Manager Notifications
(3, 'New rental request from Alice Customer for Bosch Drill Machine.', 'NewRequest', 1, '2025-03-05'),
(3, 'New rental request from Bob Smith for Canon EOS 5D Mark IV.', 'NewRequest', 1, '2025-03-10'),
(3, 'New rental request from Carol Johnson for JBL Party Speaker.', 'NewRequest', 1, '2025-03-15'),
(3, 'New rental request from Daniel Wilson for Lawn Mower.', 'NewRequest', 1, '2025-03-25'),
(3, 'New rental request from Emma Brown for 4-Person Tent.', 'NewRequest', 1, '2025-03-30'),
(3, 'Equipment maintenance required for Makita Hammer Drill.', 'MaintenanceAlert', 0, '2025-04-15'),
(3, 'New rental request from Carol Johnson for LED Stage Lights.', 'NewRequest', 0, '2025-04-20'),
(3, 'New rental request from Daniel Wilson for Hedge Trimmer.', 'NewRequest', 0, '2025-04-25'),
(3, 'New rental request from Emma Brown for Portable Grill.', 'NewRequest', 0, '2025-04-30'),

-- Admin Notifications
(1, 'Monthly rental summary report is available.', 'ReportAvailable', 1, '2025-04-01'),
(1, 'New manager account created for David Manager.', 'AccountCreation', 1, '2025-02-15'),
(1, 'System maintenance scheduled for May 1, 2025.', 'SystemMaintenance', 0, '2025-04-25');

-- 11. Seeding Logs
INSERT INTO Log (UserId, Action, Exception, Timestamp, Source)
VALUES
-- User Logins
(1, 'User login', NULL, '2025-04-01 08:30:00', 'Web'),
(1, 'User login', NULL, '2025-04-05 09:15:00', 'Web'),
(2, 'User login', NULL, '2025-04-02 10:00:00', 'Web'),
(3, 'User login', NULL, '2025-04-01 08:45:00', 'Web'),
(3, 'User login', NULL, '2025-04-03 09:30:00', 'Web'),
(3, 'User login', NULL, '2025-04-05 08:15:00', 'Web'),
(6, 'User login', NULL, '2025-04-02 14:30:00', 'Web'),
(6, 'User login', NULL, '2025-04-06 11:45:00', 'Web'),
(7, 'User login', NULL, '2025-04-03 16:00:00', 'Web'),
(7, 'User login', NULL, '2025-04-07 10:30:00', 'Web'),
(8, 'User login', NULL, '2025-04-04 13:15:00', 'Web'),
(9, 'User login', NULL, '2025-04-05 15:45:00', 'Web'),
(10, 'User login', NULL, '2025-04-06 12:30:00', 'Web'),

-- Failed Logins
(NULL, 'Failed login attempt for user: unknown@example.com', 'Invalid credentials', '2025-04-02 11:30:00', 'Web'),
(NULL, 'Failed login attempt for user: frank@example.com', 'Account inactive', '2025-04-03 14:45:00', 'Web'),
(NULL, 'Failed login attempt for user: admin@rental.com', 'Invalid credentials', '2025-04-04 09:30:00', 'Web'),

-- Rental Actions
(3, 'Approved rental request #1', NULL, '2025-03-06 10:15:00', 'Web'),
(3, 'Approved rental request #2', NULL, '2025-03-11 11:30:00', 'Web'),
(3, 'Approved rental request #3', NULL, '2025-03-16 09:45:00', 'Web'),
(3, 'Approved rental request #4', NULL, '2025-03-26 14:00:00', 'Web'),
(3, 'Approved rental request #5', NULL, '2025-03-31 10:30:00', 'Web'),
(3, 'Approved rental request #6', NULL, '2025-04-06 11:15:00', 'Web'),
(3, 'Approved rental request #7', NULL, '2025-04-11 13:45:00', 'Web'),
(3, 'Rejected rental request #8', NULL, '2025-04-16 15:30:00', 'Web'),
(3, 'Rejected rental request #9', NULL, '2025-04-18 14:15:00', 'Web'),

-- Return Actions
(3, 'Processed return for transaction #1', NULL, '2025-03-12 16:45:00', 'Web'),
(3, 'Processed return for transaction #2', NULL, '2025-03-18 17:30:00', 'Web'),
(3, 'Processed return for transaction #3', NULL, '2025-03-21 14:00:00', 'Web'),
(3, 'Processed return for transaction #4', NULL, '2025-04-02 18:45:00', 'Web'),
(3, 'Processed return for transaction #5', NULL, '2025-04-07 18:00:00', 'Web'),

-- System Actions
(1, 'Generated monthly report', NULL, '2025-04-01 09:00:00', 'Web'),
(1, 'Updated system settings', NULL, '2025-04-10 11:30:00', 'Web'),
(1, 'Backup database', NULL, '2025-04-15 08:00:00', 'Web'),
(2, 'Added new equipment category', NULL, '2025-04-05 10:15:00', 'Web'),
(2, 'Updated equipment prices', NULL, '2025-04-12 14:30:00', 'Web');

-- Sample User Credentials for Testing
-- ===================================
-- | Email               | Password    | Role        |
-- |---------------------|-------------|-------------|
-- | admin@rental.com    | admin123    | Admin       |
-- | manager@rental.com  | manager123  | Manager     |
-- | customer@rental.com | customer123 | Customer    |
-- ===================================
