# DGII.Api

API ASP.NET Core con Entity Framework Core y SQL Server, junto con una interfaz React.

## Estructura

- `Controllers`: rutas HTTP.
- `Services` e `Interfaces`: lógica de aplicación y sus contratos.
- `Data/Migrations`: contexto, datos de desarrollo y migraciones de EF Core.
- `Models` y `DTOs`: entidades de persistencia y respuestas de la API.
- `frontend/src`: aplicación React.
- `wwwroot`: archivos compilados que ASP.NET Core sirve al navegador.

## Ejecutar en desarrollo

1. Inicia la API con `dotnet run` desde la raíz del proyecto.
2. En otra terminal, entra a `frontend` y ejecuta `npm install` una vez.
3. Ejecuta `npm run dev` desde `frontend` y abre `http://localhost:5173`.

Vite reenvía las rutas `/api` a `http://localhost:5039`; ese puerto corresponde al perfil HTTP de `Properties/launchSettings.json`.

## Preparar la interfaz para ASP.NET Core

Ejecuta `npm run build` desde `frontend`. Vite genera los archivos estáticos en `wwwroot`; luego inicia o publica la API normalmente. Se conserva `wwwroot` en el repositorio porque es el contenido que sirve la aplicación .NET.

## Errores y registros

Las excepciones de las solicitudes HTTP se centralizan en `Middleware/ApiExceptionHandler.cs`. Las respuestas usan Problem Details: errores de validación como `400`, recursos inexistentes como `404` y errores inesperados como `500`. Los detalles internos de errores `500` no se envían al cliente; la respuesta incluye un `traceId` para relacionarla con el registro del servidor. Los errores inesperados se registran como `Error`, y las excepciones esperadas como `Warning`.

Si falla la carga de datos de desarrollo durante el inicio, la aplicación lo registra como `Critical` y se detiene para evitar arrancar en un estado incompleto.