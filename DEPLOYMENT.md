# 🚀 Guía de Despliegue - AdminConstruct

Esta guía explica cómo cualquier persona puede ejecutar el proyecto completo sin configuración manual.

## ✅ Requisitos Previos

Solo necesitas tener instalado:
- **Docker Desktop** (incluye Docker y Docker Compose)
  - Windows/Mac: https://www.docker.com/products/docker-desktop
  - Linux: `sudo apt install docker.io docker-compose`

**¡Eso es todo!** No necesitas instalar:
- ❌ .NET SDK
- ❌ Node.js
- ❌ PostgreSQL
- ❌ Visual Studio Code

## 📦 Clonar el Repositorio

```bash
git clone https://github.com/tu-usuario/Riwi-AdminConstruct.git
cd Riwi-AdminConstruct
```

## 🐳 Despliegue con Docker (Método Recomendado)

### Opción 1: Despliegue Completo (con pruebas)

```bash
docker compose up --build
```

Este comando:
1. ✅ Ejecuta las pruebas automatizadas
2. 🗄️ Crea la base de datos PostgreSQL automáticamente
3. 🔌 Levanta la API
4. 👨‍💼 Levanta el panel de administración
5. 💻 Levanta el cliente web

**Si las pruebas fallan, el despliegue se detiene automáticamente.**

### Opción 2: Despliegue sin Pruebas (más rápido)

```bash
docker compose up --build api web client db
```

### Acceder a los Servicios

Una vez que Docker termine de construir (puede tardar 2-5 minutos la primera vez):

| Servicio | URL | Descripción |
|----------|-----|-------------|
| 🌐 **Cliente** | http://localhost:3000 | Aplicación para usuarios finales |
| 🔌 **API** | http://localhost:5228 | API REST |
| 📚 **API Docs** | http://localhost:5228/swagger | Documentación interactiva |
| 👨‍💼 **Admin** | http://localhost:5005 | Panel administrativo |

## 🗄️ Base de Datos

### ¿Necesito instalar PostgreSQL?

**NO.** Docker crea automáticamente un contenedor con PostgreSQL. La base de datos:
- ✅ Se crea automáticamente
- ✅ Se inicializa con las migraciones
- ✅ Persiste los datos en un volumen Docker
- ✅ No requiere configuración manual

### Credenciales de la Base de Datos

Las credenciales están en `docker-compose.yml`:

```yaml
POSTGRES_DB: AdminConstructDb
POSTGRES_USER: postgres
POSTGRES_PASSWORD: admin123
```

**Para producción:** Cambia estas credenciales antes de desplegar.

### Acceder a la Base de Datos

Si necesitas conectarte directamente a PostgreSQL:

```bash
docker exec -it adminconstruct-db psql -U postgres -d AdminConstructDb
```

O usa cualquier cliente PostgreSQL con:
- Host: `localhost`
- Puerto: `5432`
- Usuario: `postgres`
- Password: `admin123`
- Database: `AdminConstructDb`

## 🔧 Configuración (Opcional)

### Variables de Entorno

Si necesitas cambiar configuraciones, edita `docker-compose.yml`:

```yaml
environment:
  # Cambiar clave JWT (IMPORTANTE para producción)
  - Jwt__SecretKey=TU_CLAVE_SUPER_SECRETA_AQUI
  
  # Configurar email (opcional)
  - SmtpSettings__Server=smtp.gmail.com
  - SmtpSettings__Port=587
  - SmtpSettings__Username=tu-email@gmail.com
  - SmtpSettings__Password=tu-app-password
```

### Puertos

Si los puertos están ocupados, cámbialos en `docker-compose.yml`:

```yaml
ports:
  - "3000:80"    # Cliente: cambia 3000 por otro puerto
  - "5228:8080"  # API: cambia 5228 por otro puerto
  - "5005:8080"  # Admin: cambia 5005 por otro puerto
```

## 🛑 Detener los Servicios

```bash
# Detener sin eliminar datos
docker compose down

# Detener y eliminar volúmenes (borra la base de datos)
docker compose down -v
```

## 🔄 Actualizar el Proyecto

Si hay cambios en el código:

```bash
git pull
docker compose down
docker compose up --build
```

## 🧪 Ejecutar Solo las Pruebas

```bash
docker build -f Dockerfile.tests -t adminconstruct-tests .
docker run adminconstruct-tests
```

## 📊 Ver Logs

```bash
# Todos los servicios
docker compose logs -f

# Servicio específico
docker compose logs -f api
docker compose logs -f web
docker compose logs -f client
docker compose logs -f db
```

## 🐛 Solución de Problemas

### Error: "port is already allocated"

Otro servicio está usando el puerto. Opciones:
1. Detén el otro servicio
2. Cambia el puerto en `docker-compose.yml`

### Error: "no space left on device"

Docker se quedó sin espacio. Limpia:

```bash
docker system prune -a
```

### La base de datos no se inicializa

```bash
# Eliminar volúmenes y recrear
docker compose down -v
docker compose up --build
```

### Las pruebas fallan

Verifica los logs:

```bash
docker compose logs tests
```

### El frontend no carga estilos

Espera a que termine el build completo. El frontend tarda más en compilar.

## 📱 Usuarios de Prueba

El sistema crea automáticamente roles:
- **Administrador**: Acceso al panel admin
- **Cliente**: Acceso al catálogo

Para crear usuarios, usa el registro en:
- Cliente: http://localhost:3000/register
- Admin: http://localhost:5005/Account/Register

## 🌐 Despliegue en Producción

### Cambios Necesarios

1. **Cambiar credenciales de BD** en `docker-compose.yml`
2. **Cambiar JWT SecretKey** (mínimo 32 caracteres)
3. **Configurar SMTP** para emails
4. **Usar HTTPS** (agregar certificados SSL)
5. **Configurar CORS** en la API para tu dominio

### Servicios en la Nube

El proyecto está listo para desplegarse en:
- **AWS**: ECS + RDS
- **Azure**: Container Instances + Azure Database
- **Google Cloud**: Cloud Run + Cloud SQL
- **DigitalOcean**: App Platform
- **Railway**: Despliegue directo desde GitHub

## 📞 Soporte

Si tienes problemas:
1. Verifica que Docker Desktop esté corriendo
2. Revisa los logs: `docker compose logs -f`
3. Intenta reconstruir: `docker compose up --build --force-recreate`

## 📄 Licencia

Proyecto académico - Riwi 2025
