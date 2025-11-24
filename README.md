# AdminConstruct 🏗️

Sistema completo de gestión para empresas de construcción con panel administrativo, API REST y aplicación cliente.

[![Docker](https://img.shields.io/badge/Docker-Ready-blue)](https://www.docker.com/)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-blue)](https://reactjs.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)](https://www.postgresql.org/)

## 🚀 Inicio Rápido (Solo necesitas Docker)

```bash
# 1. Clonar el repositorio
git clone https://github.com/tu-usuario/Riwi-AdminConstruct.git
cd Riwi-AdminConstruct

# 2. Levantar todo el sistema
docker compose up --build

# 3. Acceder a los servicios
# Cliente:  http://localhost:3000
# API:      http://localhost:5228
- Protección de rutas

### 📦 Gestión de Productos
- CRUD completo
- Catálogo paginado
- Control de inventario
- Carga de imágenes

### 🚜 Gestión de Maquinaria
- Alquiler por días
- Disponibilidad en tiempo real
- Cálculo automático de costos

### 🛒 Carrito de Compras
- Agregar productos
- Alquilar maquinaria
- Cálculo de IVA
- Checkout

### 📊 Panel Administrativo
- Dashboard con estadísticas
- Gestión de inventario
- Reportes de alquileres
- Exportación PDF/Excel

### 🧪 Pruebas Automatizadas
- Tests unitarios con xUnit
- Integración en Docker
- CI/CD ready

## 🏗️ Arquitectura

```
┌─────────────────┐
│  Cliente (SPA)  │  React + TypeScript + TailwindCSS
│  Port: 3000     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   API REST      │  ASP.NET Core 8.0 + JWT
│   Port: 5228    │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  PostgreSQL     │  Base de datos
│  Port: 5432     │
└─────────────────┘

┌─────────────────┐
│  Admin Panel    │  Razor Pages
│  Port: 5005     │
└─────────────────┘
```

## 🛠️ Tecnologías

| Categoría | Tecnologías |
|-----------|-------------|
| **Backend** | .NET 8, Entity Framework Core, JWT |
| **Frontend** | React 18, TypeScript, Vite, TailwindCSS |
| **Base de Datos** | PostgreSQL 16 |
| **Testing** | xUnit, Moq |
| **DevOps** | Docker, Docker Compose |

## 📸 Capturas de Pantalla

### Cliente
- Catálogo de productos con paginación
- Alquiler de maquinaria
- Carrito de compras

### Panel Admin
- Dashboard con métricas
- Gestión de inventario
- Reportes

## 🧪 Pruebas

```bash
# Ejecutar pruebas localmente
dotnet test

# Ejecutar pruebas en Docker
docker build -f Dockerfile.tests -t tests .
docker run tests
```

## 🤝 Contribuir

1. Fork el proyecto
2. Crea una rama (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📝 Licencia

Proyecto académico desarrollado para **Riwi** - 2025

## 👥 Autor

Desarrollado como proyecto final del programa de formación Riwi

## 📞 Soporte

¿Problemas con el despliegue? Consulta la [Guía de Despliegue](DEPLOYMENT.md)

---

⭐ Si este proyecto te fue útil, dale una estrella en GitHub