# AdminConstruct 🏗️

A complete construction company management system with an administrative panel, REST API, and client application.

[![Docker](https://img.shields.io/badge/Docker-Ready-blue)](https://www.docker.com/)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-blue)](https://reactjs.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)](https://www.postgresql.org/)

## 📋 Table of Contents

- [Quick Start](#-quick-start-docker-only)
- [Project Overview](#-project-overview)
- [Features](#-features)
- [Architecture](#-architecture)
- [Technology Stack](#-technology-stack)
- [Environment Setup](#-environment-setup)
- [Default Credentials](#-default-credentials)
- [Deployment Guide](#-deployment-guide)
- [Testing](#-testing)
- [Troubleshooting](#-troubleshooting)
- [Project Structure](#-project-structure)
- [Contributing](#-contributing)
- [License](#-license)

## 🚀 Quick Start (Docker Only)

**Prerequisites:** Only [Docker Desktop](https://www.docker.com/products/docker-desktop) is required.

```bash
# 1. Clone the repository
git clone https://github.com/your-username/Riwi-AdminConstruct.git
cd Riwi-AdminConstruct

# 2. Set up environment variables
# Copy the content from env_configuration.txt to a new file named .env
# On Windows (PowerShell): Copy-Item env_configuration.txt .env
# On Linux/Mac: cp env_configuration.txt .env
cp env_configuration.txt .env

# 3. Start all services
docker compose up --build

# 4. Access the applications
# Client App: http://localhost:3000
# Admin Panel: http://localhost:5005
# API Docs: http://localhost:5228/swagger
```

That's it! The system will automatically:
- ✅ Create and configure the PostgreSQL database
- ✅ Run database migrations
- ✅ Seed initial data
- ✅ Start all services

## 📖 Project Overview

AdminConstruct is a full-stack web application designed for construction companies to manage their inventory, sales, and machinery rentals. The system consists of three main components:

### 1. **Client Application (SPA)** 
A modern React-based storefront where customers can:
- Browse construction products and machinery
- Add items to shopping cart
- Rent machinery by selecting date ranges
- Complete purchases with automatic tax calculation
- View purchase and rental history

### 2. **Admin Panel (Web)**
A server-side rendered administrative interface for:
- Dashboard with real-time statistics
- Product and machinery inventory management
- Sales and rental tracking
- PDF and Excel report generation
- User management

### 3. **REST API**
A robust .NET Core API providing:
- JWT-based authentication
- RESTful endpoints for all operations
- Swagger/OpenAPI documentation
- Email notifications for user registration
- Role-based access control

## ✨ Features

### 🛍️ Product Management
- Complete CRUD operations
- Image upload and management
- Stock tracking
- Category organization

### 🚜 Machinery Rental System
- Daily rental pricing
- Real-time availability checking
- Automatic cost calculation
- Conflict detection for rental dates
- Return tracking

### 🛒 Shopping Cart
- Add products and machinery
- Rental date selection
- Automatic tax (IVA) calculation
- Checkout process

### 📊 Admin Dashboard
- Sales statistics
- Revenue tracking
- Active rental monitoring
- Overdue rental alerts
- Export to PDF/Excel

### 🔐 Authentication & Authorization
- JWT token-based authentication
- Role-based access (Admin/Customer)
- Email verification for registration
- Secure password hashing

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     CLIENT LAYER                             │
│  ┌─────────────────┐              ┌─────────────────┐       │
│  │  Client SPA     │              │  Admin Panel    │       │
│  │  React + TS     │              │  Razor Pages    │       │
│  │  Port: 3000     │              │  Port: 5005     │       │
│  └────────┬────────┘              └────────┬────────┘       │
└───────────┼──────────────────────────────┼─────────────────┘
            │                               │
            └───────────────┬───────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────┐
│                     API LAYER                                │
│  ┌──────────────────────────────────────────────────────┐   │
│  │         ASP.NET Core 8.0 REST API                    │   │
│  │         JWT Authentication                           │   │
│  │         Port: 5228                                   │   │
│  │         Swagger Documentation: /swagger              │   │
│  └─────────────────────────┬────────────────────────────┘   │
└────────────────────────────┼─────────────────────────────────┘
                             │
┌────────────────────────────▼─────────────────────────────────┐
│                   DATA LAYER                                 │
│  ┌──────────────────────────────────────────────────────┐   │
│  │         PostgreSQL 16                                │   │
│  │         Entity Framework Core                        │   │
│  │         Port: 5432                                   │   │
│  └──────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────┘
```

## 🛠️ Technology Stack

| Category | Technologies |
|----------|-------------|
| **Backend** | .NET 8, ASP.NET Core, Entity Framework Core |
| **Frontend - Client** | React 18, TypeScript, Vite, TailwindCSS |
| **Frontend - Admin** | Razor Pages, Bootstrap, jQuery |
| **Database** | PostgreSQL 16 |
| **Authentication** | JWT (JSON Web Tokens) |
| **Email** | SMTP (Gmail compatible) |
| **Testing** | xUnit, Moq |
| **DevOps** | Docker, Docker Compose |
| **Documentation** | Swagger/OpenAPI |

## ⚙️ Environment Setup

### Option 1: Quick Setup (Recommended)

1. Copy the content from `env_configuration.txt` to a new file named `.env`:
   ```bash
   cp env_configuration.txt .env
   ```

2. The file contains pre-configured values that work out of the box.

### Option 2: Custom Configuration

1. Copy `.env.example` to `.env`:
   ```bash
   cp .env.example .env
   ```

2. Edit `.env` and replace the placeholder values:

```env
# Database Configuration
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_secure_password
POSTGRES_DB=AdminConstructDb
DB_HOST=db
DB_PORT=5432

# Connection String
CONNECTION_STRING=Host=db;Port=5432;Database=AdminConstructDb;Username=postgres;Password=your_secure_password

# JWT Configuration (minimum 32 characters)
JWT_KEY=your_super_secret_jwt_key_at_least_32_characters_long
JWT_ISSUER=AdminConstructAPI
JWT_AUDIENCE=AdminConstructClients

# SMTP Configuration (Optional - for email notifications)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your_email@gmail.com
SMTP_PASSWORD=your_gmail_app_password
```

### Environment Variables Explained

| Variable | Description | Required |
|----------|-------------|----------|
| `POSTGRES_USER` | PostgreSQL username | Yes |
| `POSTGRES_PASSWORD` | PostgreSQL password | Yes |
| `POSTGRES_DB` | Database name | Yes |
| `CONNECTION_STRING` | Full database connection string | Yes |
| `JWT_KEY` | Secret key for JWT tokens (min 32 chars) | Yes |
| `JWT_ISSUER` | JWT token issuer identifier | Yes |
| `JWT_AUDIENCE` | JWT token audience identifier | Yes |
| `SMTP_HOST` | SMTP server hostname | No* |
| `SMTP_PORT` | SMTP server port | No* |
| `SMTP_USERNAME` | SMTP username/email | No* |
| `SMTP_PASSWORD` | SMTP password | No* |

*SMTP configuration is optional. If not provided, email notifications will be disabled.

## 🔑 Default Credentials

The system automatically creates two default users on first startup:

### Administrator Account
- **Email:** `admin@adminconstruct.com`
- **Password:** `Admin123!`
- **Access:** Admin Panel (http://localhost:5005)
- **Permissions:** Full system administration, reports, inventory management

### Customer Account
- **Email:** `client@example.com`
- **Password:** `Client123!`
- **Access:** Client App (http://localhost:3000)
- **Permissions:** Product browsing, shopping cart, machinery rental

> **Note:** You can create additional users through the registration pages in both applications.

## 📦 Deployment Guide

### Docker Deployment Options

#### Option 1: Full Deployment (with tests)

```bash
docker compose up --build
```

This command:
1. ✅ Runs automated tests
2. 🗄️ Creates PostgreSQL database automatically
3. 🔌 Starts the API
4. 👨‍💼 Starts the admin panel
5. 💻 Starts the client web app

**If tests fail, deployment stops automatically.**

#### Option 2: Deployment without Tests (faster)

```bash
docker compose up --build api web client db
```

### Database Management

#### Do I need to install PostgreSQL?

**NO.** Docker automatically creates a PostgreSQL container. The database:
- ✅ Is created automatically
- ✅ Is initialized with migrations
- ✅ Persists data in a Docker volume
- ✅ Requires no manual configuration

#### Accessing the Database

If you need to connect directly to PostgreSQL:

```bash
# Using Docker CLI
docker exec -it adminconstruct-db psql -U postgres -d AdminConstructDb
```

Or use any PostgreSQL client with:
- **Host:** `localhost`
- **Port:** `5432`
- **Username:** `postgres`
- **Password:** `admin123` (or your custom password)
- **Database:** `AdminConstructDb`

### Production Deployment

#### Required Changes for Production

1. **Change database credentials** in `.env`
2. **Change JWT SecretKey** (minimum 32 characters, cryptographically secure)
3. **Configure SMTP** for email notifications
4. **Use HTTPS** (add SSL certificates)
5. **Configure CORS** in the API for your domain
6. **Set strong passwords** for all default users
7. **Enable database backups**

#### Cloud Deployment Options

The project is ready to deploy on:
- **AWS:** ECS + RDS
- **Azure:** Container Instances + Azure Database
- **Google Cloud:** Cloud Run + Cloud SQL
- **DigitalOcean:** App Platform
- **Railway:** Direct deployment from GitHub

## 🧪 Testing

### Run All Tests

```bash
# Using Docker (recommended)
docker compose up tests

# Or build and run separately
docker build -f Dockerfile.tests -t adminconstruct-tests .
docker run adminconstruct-tests
```

### Run Tests Locally

```bash
# Requires .NET 8 SDK installed
dotnet test
```

## 🐛 Troubleshooting

### Port Already in Use

If you get a "port is already allocated" error:
1. Stop the conflicting service, or
2. Change the port mapping in `docker-compose.yml`:
   ```yaml
   services:
     client:
       ports:
         - "3001:80"    # Change to 3001
   ```

### Database Connection Issues

```bash
# Reset database (WARNING: deletes all data)
docker compose down -v
docker compose up --build
```

### Email Not Sending

- Verify SMTP credentials in `.env`
- For Gmail, ensure you're using an App Password (not your regular password)
- Check that 2-Step Verification is enabled

### Build Failures

```bash
# Clean Docker cache
docker system prune -a

# Rebuild from scratch
docker compose up --build --force-recreate
```

### API returns 401 Unauthorized

1. Verify JWT configuration in `.env`
2. Ensure `JWT_KEY` is at least 32 characters
3. Check that the API container restarted after changing `.env`

## 📁 Project Structure

```
Riwi-AdminConstruct/
├── AdminConstruct.API/          # REST API (.NET 8)
│   ├── Controllers/             # API endpoints
│   ├── Models/                  # Data models
│   ├── Services/                # Business logic
│   └── Dockerfile               # API container config
│
├── AdminConstruct.Web/          # Admin Panel (Razor Pages)
│   ├── Controllers/             # MVC controllers
│   ├── Views/                   # Razor views
│   ├── wwwroot/                 # Static assets
│   └── Dockerfile               # Web container config
│
├── admin-construct-client/      # Client SPA (React)
│   ├── src/
│   │   ├── components/          # React components
│   │   ├── pages/               # Page components
│   │   ├── services/            # API services
│   │   └── utils/               # Utilities
│   └── Dockerfile               # Client container config
│
├── AdminConstruct.Test/         # Test project
│   └── Tests/                   # Unit & integration tests
│
├── docker-compose.yml           # Docker orchestration
├── .env.example                 # Environment template
├── env_configuration.txt        # Pre-configured environment
└── README.md                    # This file
```

## 🤝 Contributing

1. Fork the project
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

Academic project developed for **Riwi** - 2025

## 👥 Author

Developed as a final project for the Riwi training program.

---

⭐ If you find this project useful, please give it a star on GitHub!