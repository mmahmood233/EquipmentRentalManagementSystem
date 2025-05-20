# Equipment Rental Management System

## Overview
The Equipment Rental Management System is a comprehensive solution for managing equipment rentals, user accounts, and rental transactions. It consists of both a web application and a Windows Forms desktop application, providing flexible access options for different types of users.

## Features

### User Management
- Role-based access control (Administrator, Manager, Customer)
- User registration and authentication
- User profile management
- Administrative user management interface

### Equipment Management
- Categorized equipment inventory
- Equipment availability tracking
- Condition status monitoring
- Pricing management

### Rental Process
- Rental request submission
- Request approval workflow
- Transaction processing
- Return processing with condition assessment

### Additional Features
- Dashboard with key metrics
- Notification system
- Document management
- User feedback collection
- Activity logging

## System Architecture

### Web Application (ASP.NET Core MVC)
- Modern responsive interface
- Role-specific dashboards
- Complete rental management functionality

### Desktop Application (Windows Forms)
- Streamlined interface for staff operations
- Local access for administrative tasks
- Integration with the central database

### Database
- SQL Server database
- Entity Framework Core for data access
- Comprehensive data model for all system entities

## Getting Started

### Prerequisites
- .NET 6.0 SDK or later
- SQL Server (2019 recommended)
- Visual Studio 2022 (recommended)

### Installation
1. Clone the repository
2. Open the solution in Visual Studio
3. Run the database migrations or execute the SQL scripts in the DataAccess/Scripts folder
4. Build and run the application

### Sample Data
The system includes a sample data script (`SampleData.sql`) that populates the database with test data, including:
- User accounts with different roles
- Equipment categories and items
- Sample rental transactions
- Test notifications and logs

### Sample User Credentials
| Email               | Password    | Role        |
|---------------------|-------------|-------------|
| admin@rental.com    | admin123    | Admin       |
| manager@rental.com  | manager123  | Manager     |
| customer@rental.com | customer123 | Customer    |

## Project Structure

### EquipmentRental.DataAccess
- Data models and database context
- Entity Framework configurations
- Database migrations
- SQL scripts for database setup and sample data

### EquipmentRental.Web
- ASP.NET Core MVC web application
- Controllers, views, and view models
- Authentication and authorization logic
- Web-specific services

### EquipmentRental.Forms
- Windows Forms desktop application
- Administrative interface
- Local data operations

## Development Guidelines

### Coding Standards
- Follow C# coding conventions
- Use meaningful names for variables, methods, and classes
- Include XML documentation for public APIs
- Write unit tests for critical functionality

### Database Changes
- Use Entity Framework migrations for schema changes
- Document any direct SQL modifications
- Maintain backward compatibility when possible

## License
This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments
- Developed as part of Advanced Programming course
- Built with ASP.NET Core and Entity Framework Core

## Group Members

- Mohammed Taha 202200948 
- Abdullah Bakhsh 202201356 
- Ali Juma  202200673 
- Yusuf Husain 202102772 
