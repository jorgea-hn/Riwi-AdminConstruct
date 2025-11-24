# AdminConstruct API

Esta API gestiona las operaciones del negocio para AdminConstruct, incluyendo Productos, Clientes y Ventas/Alquileres.

## Requisitos

- .NET 8 SDK
- PostgreSQL
- Cuenta de Gmail para envío de correos (SMTP)

## Configuración

1.  **Base de Datos**: Asegúrate de que la cadena de conexión en `appsettings.json` apunte a tu base de datos PostgreSQL.
2.  **Correo**: Configura las credenciales de Gmail en `appsettings.json` bajo la sección `Smtp`.
    ```json
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": 587,
      "Username": "TU_CORREO@gmail.com",
      "Password": "TU_APP_PASSWORD"
    }
    ```
3.  **JWT**: La clave secreta JWT está configurada en `appsettings.json`.

## Ejecución

Para ejecutar la API:

```bash
cd AdminConstruct.API
dotnet run
```

La API estará disponible en `http://localhost:5000` (o el puerto configurado).

## Documentación (Swagger)

Una vez ejecutada la API, puedes acceder a la documentación interactiva en:

`http://localhost:5000/swagger`

## Endpoints Principales

-   **POST /api/auth/register**: Registrar un nuevo cliente. Envía correo de bienvenida.
-   **POST /api/auth/login**: Iniciar sesión y obtener token JWT.
-   **GET /api/products**: Listar productos.
-   **POST /api/sales**: Crear una venta/alquiler. Envía correo de confirmación.

## Pruebas

Para ejecutar las pruebas unitarias:

```bash
dotnet test AdminConstruct.Test
```
