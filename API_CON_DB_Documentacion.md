# Documentación de la API de café

## Descripción

API REST ASP.NET Core 10 conectada a MySQL mediante Entity Framework Core. Administra los registros de la tabla `cafe` en la base `tienda_cafe`.

## Requisitos

- .NET SDK 10.
- MySQL en ejecución.
- Base de datos `tienda_cafe` y tabla `cafe` creadas.
- El usuario MySQL debe tener permisos de lectura, inserción, actualización y eliminación sobre esa tabla.

## Esquema esperado

La aplicación mapea la tabla `cafe` y estas columnas:

| Columna | Tipo esperado | Reglas |
|---|---|---|
| `id` | `INT` | Clave primaria autoincremental |
| `cafe` | `VARCHAR(120)` | Obligatorio |
| `especialidad` | `VARCHAR(120)` | Obligatorio |
| `presentacion` | `VARCHAR(80)` | Obligatorio |
| `origen` | `VARCHAR(120)` | Obligatorio |
| `cantidad` | `INT` | Cero o mayor |
| `valor` | `DECIMAL(10,2)` | Cero o mayor |
| `descripcion` | `TEXT` | Opcional |

Los nombres y tipos deben coincidir con la tabla que se creó en MySQL Workbench. La columna `id` debe generar el valor automáticamente.

## Configurar conexión

La cadena de conexión se lee desde la variable de entorno `ConnectionStrings__DefaultConnection`. En Windows se puede guardar para el usuario desde PowerShell:

```powershell
setx ConnectionStrings__DefaultConnection "Server=localhost;Database=tienda_cafe;User=root;Password=TU_CLAVE;SslMode=Disabled;AllowPublicKeyRetrieval=True;"
```

Reemplaza `TU_CLAVE` por la contraseña local de MySQL. `setx` aplica en terminales y aplicaciones abiertas después de configurarla; cierra y vuelve a abrir Visual Studio o la terminal. No guardes una contraseña real en `appsettings.json` ni la subas al repositorio.

Como alternativa local, copia `API_CON_DB/appsettings.example.json` a `API_CON_DB/appsettings.secrets.json` y cambia la contraseña. El archivo de secretos está excluido de Git. La variable de entorno tiene prioridad sobre los JSON.

## Ejecutar

### Visual Studio

1. Abre `API_CON_DB.slnx`.
2. Establece `API_CON_DB` como proyecto de inicio.
3. Selecciona el perfil `http`.
4. Presiona F5 o Ctrl+F5.

### PowerShell

Desde la carpeta raíz del repositorio:

```powershell
dotnet restore
dotnet run --project .\API_CON_DB\API_CON_DB.csproj
```

La API queda disponible en `http://localhost:5031`. Mantén la aplicación ejecutándose mientras haces peticiones desde Postman.

## Endpoints

| Método | URL | Resultado |
|---|---|---|
| GET | `/api/cafe` | Lista todos los cafés |
| GET | `/api/cafe/{id}` | Devuelve un registro o 404 |
| POST | `/api/cafe` | Crea un registro y responde 201 |
| PUT | `/api/cafe/{id}` | Actualiza el registro y responde 204 |
| DELETE | `/api/cafe/{id}` | Elimina el registro y responde 204 |

Base URL local: `http://localhost:5031`.

## Uso con Postman

Para `POST` y `PUT`, selecciona **Body → raw → JSON**. Postman debe enviar `Content-Type: application/json`.

### Crear — POST `/api/cafe`

```json
{
  "cafe": "Café de prueba",
  "especialidad": "Arábica",
  "presentacion": "Bolsa de 500 g",
  "origen": "Antioquia",
  "cantidad": 10,
  "valor": 3500.00,
  "descripcion": "Café tostado de prueba"
}
```

`id` se genera en MySQL; no hace falta incluirlo al crear.

### Actualizar — PUT `/api/cafe/1`

Incluye el mismo registro y el `id` de la ruta en el cuerpo:

```json
{
  "id": 1,
  "cafe": "Café especial",
  "especialidad": "Arábica lavado",
  "presentacion": "Bolsa de 500 g",
  "origen": "Antioquia",
  "cantidad": 12,
  "valor": 3800.00,
  "descripcion": "Actualizado"
}
```

El `id` del cuerpo debe coincidir con el de la URL.

## Respuestas habituales

- `200 OK`: consulta exitosa.
- `201 Created`: registro creado.
- `204 No Content`: actualización o eliminación exitosa.
- `400 Bad Request`: JSON no válido, campos requeridos ausentes o IDs distintos.
- `404 Not Found`: no existe un registro con ese ID.
- `500 Internal Server Error`: revisa el error de conexión/esquema en la consola de la API.

## Notas de base de datos

Las migraciones incluidas en `API_CON_DB/Migrations` pertenecen al CRUD anterior de `Productos`. La tabla `cafe` ya se creó manualmente; no ejecutes `dotnet ef database update` con esas migraciones. Si en el futuro se adoptan migraciones para `cafe`, primero hay que preparar una línea base que coincida con la base existente.
