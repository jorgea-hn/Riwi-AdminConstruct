# AdminConstruct Client

Cliente web desarrollado con Vite + React + TypeScript para consumir la API de AdminConstruct.

## Características

- ✅ Autenticación JWT (Login/Register)
- ✅ Catálogo de Productos con paginación
- ✅ Catálogo de Maquinaria con paginación
- ✅ Sistema de alquiler de maquinaria con selección de fechas
- ✅ Carrito de compras
- ✅ Checkout y confirmación por email
- ✅ Diseño responsive con TailwindCSS

## Requisitos

- Node.js 18+
- npm o yarn

## Instalación

```bash
# Instalar dependencias
npm install

# Ejecutar en desarrollo
npm run dev

# Build para producción
npm run build
```

## Configuración

La URL de la API está configurada en `src/services/api.ts`:

```typescript
const API_URL = 'http://localhost:5228/api';
```

## Estructura del Proyecto

```
src/
├── components/       # Componentes reutilizables
│   ├── Navbar.tsx
│   └── ProtectedRoute.tsx
├── pages/           # Páginas de la aplicación
│   ├── Login.tsx
│   ├── Register.tsx
│   ├── Home.tsx
│   ├── Products.tsx
│   ├── Machinery.tsx
│   └── Cart.tsx
├── services/        # Servicios y lógica de negocio
│   ├── api.ts
│   ├── authService.ts
│   └── productService.ts
└── App.tsx         # Componente principal con rutas
```

## Uso

1. **Registro**: Crear una cuenta nueva en `/register`
2. **Login**: Iniciar sesión en `/login`
3. **Productos**: Ver y agregar productos al carrito
4. **Maquinaria**: Alquilar maquinaria seleccionando fechas
5. **Carrito**: Revisar y finalizar la compra

## Tecnologías

- **Vite**: Build tool y dev server
- **React 18**: Framework UI
- **TypeScript**: Tipado estático
- **React Router**: Navegación
- **TailwindCSS**: Estilos
- **Axios**: Cliente HTTP
- **Lucide React**: Iconos
