# 🚀 Guía de CI/CD con GitHub Actions

Esta guía explica cómo funciona el despliegue automático con GitHub Actions.

## 📋 Arquitectura del Sistema

### Componentes

```
┌─────────────────────────────────────────────────────┐
│                   CLIENTE (SPA)                      │
│   React + TypeScript + TailwindCSS                  │
│   Puerto: 3000                                       │
└──────────────────┬──────────────────────────────────┘
                   │ HTTP/REST (JWT)
                   ▼
┌─────────────────────────────────────────────────────┐
│                  API REST                            │
│   ASP.NET Core 8.0 + Entity Framework               │
│   Puerto: 5228                                       │
└──────────────────┬──────────────────────────────────┘
                   │ ORM
                   ▼
┌─────────────────────────────────────────────────────┐
│              PostgreSQL Database                     │
│   Puerto: 5432                                       │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│            PANEL ADMIN (Razor Pages)                 │
│   ASP.NET Core 8.0                                  │
│   Puerto: 5005                                       │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
         (Misma Base de Datos)
```

### Patrón de Arquitectura

- **Frontend**: SPA (Single Page Application) con React
- **Backend**: API REST con autenticación JWT
- **Admin**: MVC con Razor Pages
- **Database**: PostgreSQL con Entity Framework Core
- **Testing**: xUnit con Moq
- **Containerization**: Docker multi-stage builds
- **Orchestration**: Docker Compose

## 🔄 Flujo de CI/CD

### Trigger (Disparador)

El pipeline se ejecuta automáticamente cuando:
- Haces `git push` a la rama `main`
- Haces `git push` a la rama `develop`
- Creas un Pull Request a `main`

### Pipeline Stages

```
┌─────────────┐
│   PUSH      │
│  to GitHub  │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  1. TEST    │  ← Ejecuta pruebas xUnit
└──────┬──────┘
       │ ✅ Pass
       ▼
┌─────────────┐
│  2. BUILD   │  ← Construye imágenes Docker
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  3. PUSH    │  ← Sube a GitHub Container Registry
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  4. DEPLOY  │  ← Despliega (opcional)
└─────────────┘
```

## 🛠️ Configuración Inicial

### 1. Habilitar GitHub Container Registry

Las imágenes Docker se almacenan en GitHub Container Registry (GHCR) automáticamente.

**No necesitas configurar nada adicional** - GitHub Actions tiene permisos automáticos.

### 2. Hacer tu primer Push

```bash
# 1. Agregar archivos
git add .

# 2. Commit
git commit -m "Add CI/CD pipeline"

# 3. Push a GitHub
git push origin main
```

### 3. Ver el Pipeline

1. Ve a tu repositorio en GitHub
2. Click en la pestaña **Actions**
3. Verás el pipeline ejecutándose

## 📦 Imágenes Docker

Después de cada push exitoso, se crean 3 imágenes:

```
ghcr.io/tu-usuario/riwi-adminconstruct-api:latest
ghcr.io/tu-usuario/riwi-adminconstruct-web:latest
ghcr.io/tu-usuario/riwi-adminconstruct-client:latest
```

## 🚀 Despliegue en Servidor

### Opción 1: Servidor con Docker (Recomendado)

En tu servidor (VPS, EC2, DigitalOcean, etc.):

```bash
# 1. Instalar Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# 2. Login a GitHub Container Registry
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# 3. Clonar el repo (solo docker-compose.prod.yml)
git clone https://github.com/tu-usuario/Riwi-AdminConstruct.git
cd Riwi-AdminConstruct

# 4. Configurar variables de entorno
cp .env.example .env
nano .env  # Editar con tus valores

# 5. Desplegar
docker compose -f docker-compose.prod.yml pull
docker compose -f docker-compose.prod.yml up -d
```

### Opción 2: GitHub Actions Deploy Automático

Para despliegue automático, descomenta la sección de deploy en `.github/workflows/ci-cd.yml`:

#### Para AWS ECS:

```yaml
- name: Deploy to AWS ECS
  uses: aws-actions/amazon-ecs-deploy-task-definition@v1
  with:
    task-definition: task-definition.json
    service: adminconstruct-service
    cluster: adminconstruct-cluster
```

**Secrets necesarios:**
- `AWS_ACCESS_KEY_ID`
- `AWS_SECRET_ACCESS_KEY`

#### Para Azure:

```yaml
- name: Deploy to Azure
  uses: azure/webapps-deploy@v2
  with:
    app-name: adminconstruct
    images: ghcr.io/${{ github.repository }}-api:latest
```

**Secrets necesarios:**
- `AZURE_CREDENTIALS`

#### Para DigitalOcean:

```yaml
- name: Deploy to DigitalOcean
  uses: digitalocean/action-doctl@v2
  with:
    token: ${{ secrets.DIGITALOCEAN_ACCESS_TOKEN }}
```

**Secrets necesarios:**
- `DIGITALOCEAN_ACCESS_TOKEN`

### Agregar Secrets en GitHub

1. Ve a tu repositorio en GitHub
2. Settings → Secrets and variables → Actions
3. Click en "New repository secret"
4. Agrega los secrets necesarios

## 🔐 Variables de Entorno

Crea un archivo `.env` en producción:

```env
# Base de datos
POSTGRES_DB=AdminConstructDb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=tu_password_seguro_aqui

# JWT
JWT_SECRET_KEY=tu_clave_super_segura_de_al_menos_32_caracteres

# GitHub (para pull de imágenes)
GITHUB_REPOSITORY=tu-usuario/riwi-adminconstruct
```

## 📊 Monitoreo

### Ver logs en producción

```bash
# Todos los servicios
docker compose -f docker-compose.prod.yml logs -f

# Servicio específico
docker compose -f docker-compose.prod.yml logs -f api
```

### Estado de los contenedores

```bash
docker compose -f docker-compose.prod.yml ps
```

## 🔄 Actualizar en Producción

Cada vez que hagas push a `main`:

```bash
# En tu servidor
cd Riwi-AdminConstruct
docker compose -f docker-compose.prod.yml pull
docker compose -f docker-compose.prod.yml up -d
```

O automatiza con un webhook o cron job.

## 🧪 Testing en CI/CD

El pipeline ejecuta automáticamente:
- ✅ Pruebas unitarias (xUnit)
- ✅ Build de todos los proyectos
- ✅ Validación de Dockerfiles

Si alguna prueba falla, el pipeline se detiene y NO se despliega.

## 🌐 Servicios en la Nube Recomendados

### Opción 1: Railway (Más Fácil)
- Deploy directo desde GitHub
- Base de datos PostgreSQL incluida
- SSL automático
- **Costo**: ~$5-10/mes

### Opción 2: DigitalOcean App Platform
- Deploy desde GitHub
- Managed PostgreSQL
- SSL automático
- **Costo**: ~$12-20/mes

### Opción 3: AWS (Más Escalable)
- ECS + RDS
- CloudFront + S3 para el frontend
- Route 53 para DNS
- **Costo**: ~$20-50/mes

### Opción 4: Azure
- Container Instances
- Azure Database for PostgreSQL
- **Costo**: ~$15-30/mes

## 🔧 Troubleshooting

### Error: "permission denied while trying to connect to the Docker daemon"

```bash
sudo usermod -aG docker $USER
newgrp docker
```

### Error: "pull access denied"

```bash
# Login a GitHub Container Registry
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin
```

### Pipeline falla en tests

Revisa los logs en GitHub Actions y corre las pruebas localmente:

```bash
dotnet test
```

## 📝 Resumen

1. **Haces push a GitHub** → GitHub Actions se ejecuta automáticamente
2. **Se ejecutan las pruebas** → Si fallan, se detiene
3. **Se construyen las imágenes Docker** → Multi-stage builds
4. **Se suben a GitHub Container Registry** → Disponibles públicamente
5. **Se despliegan (opcional)** → Según configuración

**Todo es automático después del primer push.**
