# adminConstruct

Administrative Web Application for Construction Supplies and Industrial Vehicle Rentals

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [Features](#features)
3. [Technologies](#technologies)
4. [User Roles](#user-roles)
5. [Project Structure](#project-structure)
6. [Database](#database)
7. [Installation and Setup](#installation-and-setup)
8. [Excel and PDF Operations](#excel-and-pdf-operations)
9. [Unit Testing](#unit-testing)
10. [Docker Deployment](#docker-deployment)
11. [Documentation](#documentation)
12. [Screenshots / UI Overview](#screenshots--ui-overview)
13. [Contributing](#contributing)
14. [License](#license)

---

## Project Overview
**adminConstruct** is a web application built with **ASP.NET Core Razor Pages** designed for managing a business in **construction supplies** and **industrial vehicle rentals**.  
The application allows administrators to:

- Manage products, clients, and sales.
- Perform bulk data import/export via **Excel (EPPlus)**.
- Generate **PDF receipts** and reports.
- Maintain a clean, navigable administrative dashboard.

---

## Features

- **Admin Dashboard**: Quick overview of total products, clients, and sales.
- **CRUD Operations**: Products, clients, and sales management.
- **Excel Import**: Bulk import from unstructured `.xlsx` files with automatic normalization.
- **Excel/PDF Export**: Export any dataset, including sales receipts.
- **Authentication & Authorization**: Role-based access (Administrator vs. Client).
- **Error Handling**: Friendly messages, data validation, logging.
- **Unit Testing**: Ensures stability using **xUnit**.
- **Docker Ready**: Preconfigured for PostgreSQL and web application deployment.

---

## Technologies

| Layer              | Technology                     |
|-------------------|--------------------------------|
| Backend           | ASP.NET Core 8, Razor Pages     |
| Database          | PostgreSQL, EF Core             |
| Authentication    | ASP.NET Core Identity           |
| Excel Operations  | EPPlus                          |
| PDF Generation    | QuestPDF / iTextSharp           |
| Frontend          | Bootstrap 5 / Tailwind CSS      |
| Testing           | xUnit                           |
| Containerization  | Docker, Docker Compose          |

---

## User Roles

| Role           | Access Level                                      |
|----------------|--------------------------------------------------|
| Administrator  | Full access to Razor panel (Dashboard, CRUDs)   |
| Client         | Access via Blazor only, no Razor panel access   |

---

## Project Structure





---

## Database

- PostgreSQL with **EF Core migrations only** (no manual scripts)
- Entities include:
    - `Product` (Name, Description, Price, Stock)
    - `Client` (Name, Document, Email, Phone)
    - `Sale` (SaleDate, Client, SaleDetails)
    - `SaleDetail` (Product, Quantity, Price)

**Automatic migration example:**
```bash
dotnet ef migrations add InitialCreate --project adminConstruct.Web
dotnet ef database update --project adminConstruct.Web
```


## Installation and Setup

1. Clone the repository
```bash
git clone https://github.com/yourusername/adminConstruct.git
cd adminConstruct
```

2. Update appsettings.json with your PostgresSQL credentials:

```bash
"ConnectionStrings": {
"DefaultConnection": "Host=localhost;Database=adminconstruct;Username=postgres;Password=your_password"
}
```

3. Apply database migrations
```bash
dotnet ef database update --project adminConstruct.Web
```

4. Run the web applications
```bash
dotnet run --project adminConstruct.Web
```

5. Acces the application:
```bash
https://localhost:5001
```

## Excel and PDF operations
* Excel import
  * Import multiple denormalized dataset in .xlsx
  * Automatic normalization and validation.
  * Logs inconsistencies or errors.
* Export
  * Export Products, Clients, Sales to Excel
  * Generate PDF receipt with:
    * Client data
    * Sale date and number
    * Product list
    * Totals and taxes