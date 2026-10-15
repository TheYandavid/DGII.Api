# DGII.Api

Aplicación web para consultar contribuyentes y comprobantes fiscales. Incluye una API REST en ASP.NET Core, una interfaz en React y persistencia en SQL Server mediante Entity Framework Core.

## Características

- Listado paginado de contribuyentes, con búsqueda por nombre, RNC/cédula, tipo y estatus.
- Consulta de comprobantes fiscales con filtros por RNC/cédula, NCF y estado, además de paginación.
- Vista de comprobantes filtrada al seleccionar un contribuyente desde la interfaz.
- Presentación del monto e ITBIS de los comprobantes y totales para los registros mostrados en la página actual.
- Reportes de ITBIS por contribuyente y resumen general de contribuyentes y comprobantes activos.
- Validación numérica del RNC/cédula en la interfaz, con un máximo de 11 dígitos.
- Respuestas de error con Problem Details, códigos HTTP adecuados y `traceId` para correlación con los logs.
- Datos de muestra opcionales para el entorno de desarrollo.

## Tecnologías

### Backend

- .NET 9 / ASP.NET Core Web API.
- Entity Framework Core 9 con proveedor SQL Server.
- Swagger y OpenAPI para explorar la API en desarrollo.
- Inyección de dependencias para servicios e interfaces.

### Frontend

- React y Vite.
- HTML y CSS responsivo.
- Las llamadas a la API usan rutas relativas `/api`.

## Estructura

```text
Controllers/       Endpoints HTTP
Data/              DbContext, catálogo/seeder y migraciones EF Core
DTOs/              Modelos de respuesta de la API
Exceptions/        Excepciones de aplicación
Interfaces/        Contratos de servicios
Middleware/        Manejo global de errores
Models/            Entidades de dominio/persistencia
Services/          Consultas y lógica de aplicación
Validators/        Validaciones compartidas
Tests/             Pruebas unitarias, integración y E2E
frontend/src/      Componentes y estilos React
wwwroot/           Build estático servido por ASP.NET Core
```

## Requisitos previos

- SDK de .NET 9.
- Node.js y npm.
- SQL Server; la configuración local predeterminada apunta a `localhost\SQLEXPRESS`.

## Configuración de base de datos

Revisa `ConnectionStrings:DefaultConnection` en `appsettings.json` y cámbiala si tu instancia de SQL Server es diferente. Las migraciones están en `Data/Migrations`; aplícalas antes de iniciar la API si la base todavía no tiene el esquema.

En desarrollo, `appsettings.Development.json` habilita `SeedData:Enabled`. Si está activado, el seeder agrega los registros de muestra faltantes al iniciar. Para desactivarlo, cambia esa opción a `false`.

## Ejecutar en desarrollo

1. Desde la raíz, inicia el backend con `dotnet run`.
2. En otra terminal, entra a `frontend` y ejecuta `npm install` (o `npm ci` para usar exactamente el lockfile).
3. Ejecuta `npm run dev` desde `frontend`.
4. Abre `http://localhost:5173` para usar React. Vite reenvía las peticiones `/api` a `http://localhost:5039`.

Los perfiles HTTP/HTTPS están definidos en `Properties/launchSettings.json`. Swagger y OpenAPI están disponibles en el entorno Development.

## Compilar y servir el frontend con ASP.NET Core

Desde `frontend`, ejecuta `npm run build`. Vite genera los archivos compilados en `wwwroot`. Después, ejecuta o publica la API con `dotnet run` o `dotnet publish`; ASP.NET Core sirve el frontend desde esa carpeta. Hay que volver a generar el build cuando se cambie React.

## Endpoints principales

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/health` | Estado básico de la API. |
| `GET` | `/api/contribuyentes?page=1&pageSize=10` | Listado paginado y agregados de comprobantes activos. |
| `GET` | `/api/contribuyentes/buscar?nombre=Juan` | Búsqueda por nombre, RNC/cédula, tipo o estatus. |
| `GET` | `/api/contribuyentes/{rncCedula}` | Detalle y agregados del contribuyente. |
| `GET` | `/api/contribuyentes/{rncCedula}/comprobantes` | Todos los comprobantes del contribuyente, incluidos los inactivos. |
| `GET` | `/api/comprobantes?page=1&pageSize=10` | Listado paginado con filtros opcionales por RNC/cédula, NCF y estado (`VIGENTE` o `VENCIDO`). |
| `GET` | `/api/reportes/itbis-por-contribuyente` | ITBIS y monto agrupados por contribuyente; permite filtrar por tipo y estatus. |
| `GET` | `/api/reportes/resumen` | Resumen global de contribuyentes y comprobantes activos. |

Los endpoints paginados limitan el tamaño máximo de página a 100. Las búsquedas de contribuyentes y los reportes no están paginados actualmente.

## Manejo de errores y logs

`Middleware/ApiExceptionHandler.cs` centraliza el manejo de excepciones de las solicitudes HTTP. Las excepciones de validación se convierten en `400`, las de recurso inexistente en `404` y las inesperadas en `500`. Las respuestas siguen Problem Details e incluyen un `traceId`; los detalles internos de errores `500` solo se registran en el servidor, no se devuelven al cliente.

Los errores inesperados se registran como Error y las excepciones esperadas como Warning. Si falla el seeder durante el arranque, el programa registra el error como Critical y vuelve a lanzar la excepción para no iniciar en un estado incompleto.

## Pruebas

<<<<<<< HEAD
La suite está en `Tests/DGII.Api.Tests.csproj` y usa xUnit. Las pruebas de integración y E2E usan SQLite en memoria, así que no requieren conectarse a SQL Server ni alteran la base de desarrollo.

Ejecuta toda la suite desde la raíz del repositorio:

```powershell
dotnet test Tests/DGII.Api.Tests.csproj
```

Las categorías pueden ejecutarse por separado:

```powershell
dotnet test Tests/DGII.Api.Tests.csproj --filter "Category=Unit"
dotnet test Tests/DGII.Api.Tests.csproj --filter "Category=Integration"
dotnet test Tests/DGII.Api.Tests.csproj --filter "Category=EndToEnd"
```

- **Unit:** validador de RNC/cédula y servicio de salud.
- **Integration:** servicios de contribuyentes/comprobantes con EF Core y SQLite.
- **EndToEnd:** solicitudes HTTP al host ASP.NET Core de prueba; verifica rutas, filtros, paginación y respuestas de error.

`DGII.Api.http` también conserva una petición manual de ejemplo para comprobar el endpoint de salud.
=======
El repositorio no contiene actualmente proyectos de pruebas automatizadas. `DGII.Api.http` incluye una petición HTTP de ejemplo para comprobar el endpoint de salud.
>>>>>>> bdd0e686a8b912222560b64121f40af0019c93d1
