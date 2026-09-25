# API_CON_DB

API REST para administrar el catálogo de café, desarrollada con ASP.NET Core 10, Entity Framework Core y MySQL.

La guía detallada de arquitectura, configuración, ejecución y uso está en [DOCUMENTACION_PROYECTO.md](./DOCUMENTACION_PROYECTO.md). La referencia de endpoints y ejemplos para Postman está en [API_CON_DB_Documentacion.md](./API_CON_DB_Documentacion.md).

## Inicio rápido

1. Inicia MySQL y confirma que existe la base `tienda_cafe` con la tabla `cafe`.
2. Configura `ConnectionStrings__DefaultConnection` en las variables de entorno de Windows o en el archivo local ignorado `API_CON_DB/appsettings.secrets.json`.
3. Abre `API_CON_DB.slnx` en Visual Studio, establece el proyecto `API_CON_DB` como proyecto de inicio y ejecuta el perfil `http` con F5. Desde PowerShell también puedes ejecutar:

   ```powershell
   dotnet run --project .\API_CON_DB\API_CON_DB.csproj
   ```

4. Prueba `http://localhost:5031/api/cafe` desde Postman.

No ejecutes las migraciones antiguas de `Productos` contra la base `tienda_cafe`; la tabla `cafe` ya fue creada con el esquema nuevo.
