# 🏗️ Arquitectura del Sistema AdminConstruct

## Diagrama de Arquitectura

```mermaid
graph TB
    subgraph "Frontend Layer"
        CLIENT["Cliente SPA<br/>React + TypeScript<br/>Port: 3000"]
    end
    
    subgraph "Backend Layer"
        API["API REST<br/>ASP.NET Core 8.0<br/>JWT Auth<br/>Port: 5228"]
        WEB["Panel Admin<br/>Razor Pages<br/>Port: 5005"]
    end
    
    subgraph "Data Layer"
        DB[("PostgreSQL<br/>Database<br/>Port: 5432")]
    end
    
    subgraph "Testing"
        TESTS["xUnit Tests<br/>Automated Testing"]
    end
    
    CLIENT -->|HTTP/REST + JWT| API
    WEB -->|Entity Framework| DB
    API -->|Entity Framework| DB
    TESTS -.->|Validates| API
    TESTS -.->|Validates| WEB
    
    style CLIENT fill:#61dafb,stroke:#333,stroke-width:2px,color:#000
    style API fill:#512bd4,stroke:#333,stroke-width:2px,color:#fff
    style WEB fill:#512bd4,stroke:#333,stroke-width:2px,color:#fff
    style DB fill:#336791,stroke:#333,stroke-width:2px,color:#fff
    style TESTS fill:#5fa04e,stroke:#333,stroke-width:2px,color:#fff
```

## Patrón de Arquitectura

### 🎯 Arquitectura de 3 Capas

1. **Capa de Presentación**
   - Cliente SPA (React)
   - Panel Admin (Razor Pages)

2. **Capa de Lógica de Negocio**
   - API REST con JWT
   - Servicios de negocio
   - Validaciones

3. **Capa de Datos**
   - PostgreSQL
   - Entity Framework Core
   - Migraciones automáticas

### 🔐 Seguridad

```mermaid
sequenceDiagram
    participant C as Cliente
    participant A as API
    participant DB as Database
    
    C->>A: POST /api/auth/login
    A->>DB: Validar credenciales
    DB-->>A: Usuario válido
    A-->>C: JWT Token
    
    C->>A: GET /api/products (+ JWT)
    A->>A: Validar JWT
    A->>DB: Query productos
    DB-->>A: Datos
    A-->>C: Productos JSON
```

### 📦 Contenedorización

```mermaid
graph LR
    subgraph "Docker Compose"
        T[Tests Container]
        D[PostgreSQL Container]
        A[API Container]
        W[Web Container]
        C[Client Container]
    end
    
    T -->|Must Pass| D
    D -->|Health Check| A
    D -->|Health Check| W
    A -->|Ready| C
    
    style T fill:#5fa04e,stroke:#333,stroke-width:2px
    style D fill:#336791,stroke:#333,stroke-width:2px
    style A fill:#512bd4,stroke:#333,stroke-width:2px
    style W fill:#512bd4,stroke:#333,stroke-width:2px
    style C fill:#61dafb,stroke:#333,stroke-width:2px
```

## Flujo de Datos

### Compra de Productos

```mermaid
sequenceDiagram
    participant U as Usuario
    participant C as Cliente SPA
    participant A as API
    participant DB as Database
    
    U->>C: Ver catálogo
    C->>A: GET /api/products?page=1
    A->>DB: SELECT * FROM Products
    DB-->>A: Lista paginada
    A-->>C: JSON con productos
    C-->>U: Mostrar productos
    
    U->>C: Agregar al carrito
    C->>C: Guardar en localStorage
    
    U->>C: Checkout
    C->>A: POST /api/orders (+ JWT)
    A->>DB: INSERT INTO Orders
    A->>DB: UPDATE Products (stock)
    DB-->>A: Confirmación
    A-->>C: Orden creada
    C-->>U: Confirmación
```

### Alquiler de Maquinaria

```mermaid
sequenceDiagram
    participant U as Usuario
    participant C as Cliente SPA
    participant A as API
    participant DB as Database
    
    U->>C: Seleccionar maquinaria
    C->>A: GET /api/machinery/{id}
    A->>DB: SELECT disponibilidad
    DB-->>A: Maquinaria disponible
    A-->>C: Datos + disponibilidad
    
    U->>C: Seleccionar fechas
    C->>C: Calcular costo
    
    U->>C: Confirmar alquiler
    C->>A: POST /api/rentals (+ JWT)
    A->>DB: Validar disponibilidad
    A->>DB: INSERT INTO Rentals
    A->>DB: UPDATE Machinery (stock)
    DB-->>A: Confirmación
    A-->>C: Alquiler creado
    C-->>U: Confirmación + email
```

## Tecnologías por Capa

### Frontend (Cliente SPA)
- **Framework**: React 18
- **Language**: TypeScript
- **Build Tool**: Vite
- **Styling**: TailwindCSS
- **Routing**: React Router
- **HTTP Client**: Axios
- **State**: localStorage (carrito)

### Backend (API REST)
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core
- **Auth**: JWT Bearer
- **Validation**: Data Annotations
- **Documentation**: Swagger/OpenAPI
- **Email**: SMTP (configurable)

### Backend (Panel Admin)
- **Framework**: ASP.NET Core MVC
- **View Engine**: Razor Pages
- **Auth**: Cookie Authentication
- **Export**: iTextSharp (PDF), EPPlus (Excel)

### Database
- **RDBMS**: PostgreSQL 16
- **Migrations**: EF Core Migrations
- **Connection Pooling**: Npgsql

### Testing
- **Framework**: xUnit
- **Mocking**: Moq
- **Database**: InMemory Provider

### DevOps
- **Containerization**: Docker
- **Orchestration**: Docker Compose
- **CI/CD**: GitHub Actions
- **Registry**: GitHub Container Registry

## Escalabilidad

### Horizontal Scaling

```mermaid
graph TB
    LB[Load Balancer]
    
    subgraph "API Instances"
        API1[API Instance 1]
        API2[API Instance 2]
        API3[API Instance 3]
    end
    
    subgraph "Database"
        MASTER[(PostgreSQL Master)]
        REPLICA1[(Replica 1)]
        REPLICA2[(Replica 2)]
    end
    
    LB --> API1
    LB --> API2
    LB --> API3
    
    API1 --> MASTER
    API2 --> MASTER
    API3 --> MASTER
    
    MASTER -.->|Replication| REPLICA1
    MASTER -.->|Replication| REPLICA2
    
    API1 -.->|Read| REPLICA1
    API2 -.->|Read| REPLICA2
```

### Caching Strategy (Futuro)

```mermaid
graph LR
    C[Cliente] --> API[API]
    API --> REDIS[Redis Cache]
    API --> DB[(PostgreSQL)]
    
    REDIS -.->|Cache Miss| DB
    DB -.->|Update Cache| REDIS
```

## Seguridad Implementada

- ✅ **Autenticación JWT** con tokens firmados
- ✅ **Roles** (Administrador, Cliente)
- ✅ **HTTPS** ready (configurar certificados)
- ✅ **CORS** configurado
- ✅ **SQL Injection** protección via EF Core
- ✅ **XSS** protección via React
- ✅ **Password Hashing** con Identity
- ✅ **Rate Limiting** (recomendado agregar)

## Monitoreo (Recomendado)

Para producción, considera agregar:

- **Application Insights** (Azure)
- **Sentry** (Error tracking)
- **Prometheus + Grafana** (Métricas)
- **ELK Stack** (Logs)

## Próximos Pasos de Arquitectura

1. **Microservicios**: Separar en servicios independientes
2. **Event-Driven**: Implementar mensajería (RabbitMQ/Kafka)
3. **CQRS**: Separar comandos de consultas
4. **API Gateway**: Agregar Kong o Ocelot
5. **Service Mesh**: Implementar Istio
