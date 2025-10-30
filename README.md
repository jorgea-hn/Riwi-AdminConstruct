# Firmeza - AdminConstruct

Aplicativo administrativo para venta de insumos de construcción y renta de vehículos industriales.

## Proyectos
- AdminConstruct.Ryzor: Panel administrativo (Razor, Tailwind, EF Core, Identity, EPPlus, QuestPDF)
- AdminConstruct.API: API para el cliente (pendiente de ampliar)
- AdminConstruct.Blazor: Front cliente
- AdminConstruct.Test: Pruebas xUnit (pendiente)

## Requisitos
- .NET 8 SDK
- Node.js 18+
- PostgreSQL 16 (si corres local) o Docker Compose

## Variables de entorno (Docker y local)
- DB_HOST, DB_PORT, DB_NAME, DB_USER, DB_PASSWORD
- ADMIN_EMAIL, ADMIN_PASSWORD

## Ejecución local
```bash
# restaurar dependencias
 dotnet restore

# migraciones (ya creadas) y actualizar DB
 dotnet ef database update -p AdminConstruct.Ryzor -s AdminConstruct.Ryzor

# construir CSS (Tailwind)
 cd AdminConstruct.Ryzor && npm install && npm run build:css

# ejecutar panel admin
 dotnet run --project ../AdminConstruct.Ryzor --urls "http://localhost:5095;https://localhost:7177"
```
- Admin: Email admin@firmeza.local / Password Admin123$

## Docker Compose
```bash
docker compose up --build
```
- Admin: http://localhost:8080
- API: http://localhost:8081
- Blazor: http://localhost:8082
- PostgreSQL: localhost:5432 (postgres/postgres)
- Recibos PDF: `wwwroot/recibos` montado en volumen

## Funcionalidades clave (Razor Admin)
- Autenticación y roles (Admin/Cliente); acceso a panel solo Admin
- Dashboard con métricas (productos, clientes, ventas)
- CRUD Productos (validaciones + filtro nombre y rango precio)
- CRUD Clientes (validaciones correo/documento/teléfono + búsqueda)
- Importación Excel desnormalizada (EPPlus):
  - Archivos mezclados (producto, cliente, venta)
  - Normaliza en memoria, valida campos y realiza upsert/insert
  - Log de errores por fila
  - Muestra ejemplo descargable
- Exportación a Excel (productos, clientes, ventas)
- Ventas: registro y generación de recibo PDF (QuestPDF) en `wwwroot/recibos`

## Arquitectura Limpia (propuesta de evolución)
Separar en capas y proyectos:
- Core (Dominio): entidades, value objects, reglas, interfaces de repositorio
- Application: casos de uso (handlers), DTOs, validaciones, orquestación
- Infrastructure: EF Core (DbContext, configs, migraciones), repositorios, servicios (Excel/PDF)
- Web.Admin (Razor), Web.API, Web.Blazor: interacción con usuario; sin depender de EF directamente

Pasos sugeridos:
1. Mover `Models` a `Core`; crear interfaces `IProductRepository`, `ICustomerRepository`, etc.
2. Crear `Infrastructure` con `ApplicationDbContext`, mapeos y repos impl.
3. `Application` con casos de uso (ej. CreateProductHandler) y validaciones.
4. `Web.*` inyecta casos de uso; UI deja de usar DbContext directo.

## Diagramas
- Modelo ER (simplificado): ver `docs/er.md`
- Diagrama de clases (simplificado): ver `docs/classes.md`

## Importación Excel - Formato ejemplo
Descarga desde el panel (Importar Excel → Descargar ejemplo). Columnas reconocidas de forma flexible:
- Producto | Price/Precio | Stock
- Cliente | Documento | Email | Telefono
- Fecha | Cantidad | UnitPrice/PrecioUnitario

Cada fila puede tener solo producto, solo cliente o una venta completa (requiere producto+cliente+fecha+cantidad+precio). El sistema normaliza y registra.

## Pruebas (pendiente de ampliar)
- xUnit para reglas de dominio (cálculo totales/IVA, validaciones, normalización Excel)

## Seguridad
- Cookie paths configurados (Login/AccessDenied)
- Middleware que redirige usuarios no-Admin fuera del panel

## Notas
- EPPlus en contexto NoCommercial para desarrollo
- QuestPDF Community License