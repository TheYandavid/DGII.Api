# Frontend DGII (React)

La interfaz está hecha con React y Vite. El build se publica directamente en `../wwwroot`, que ASP.NET Core sirve como archivos estáticos.

## Desarrollo

1. Inicia la API ASP.NET Core en `http://localhost:5039`.
2. Desde esta carpeta ejecuta `npm install` y después `npm run dev`.
3. Abre `http://localhost:5173`. Vite reenvía las peticiones `/api` a la API local.

## Publicación

Ejecuta `npm run build` antes de compilar/publicar la API. Los archivos listos para producción se generan en `../wwwroot`.
