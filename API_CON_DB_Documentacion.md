# Documentación del Proyecto: API_CON_DB

## 1. Descripción General

**API_CON_DB** es una API REST desarrollada en **ASP.NET Core (.NET 10)** que expone un CRUD de productos, conectada a una base de datos **MySQL** mediante **Entity Framework Core**.

| Ítem | Detalle |
|---|---|
| Framework | .NET 10 |
| Lenguaje | C# |
| ORM | Entity Framework Core |
| Proveedor de base de datos | MySQL (paquete `MySql.EntityFrameworkCore` — Oracle) |
| Base de datos | `Tienda` |
| Servidor | `localhost` |
| Puerto de la API | `5031` (HTTP) |

---

## 2. Estructura del Proyecto

```
API_CON_DB/
├── Controllers/
│   ├── ProductosController.cs      # Endpoints CRUD de Producto
│   └── WeatherForecastController.cs # Controlador de ejemplo (plantilla por defecto)
├── DB/
│   ├── AppDbContext.cs              # Contexto de Entity Framework
│   └── DesignTimeDbContextFactory.cs # Fábrica para comandos de diseño (migraciones)
├── Migrations/
│   ├── 20260904233641_initialcreate.cs
│   ├── 20260905001918_RenameProductoTable.cs
│   └── AppDbContextModelSnapshot.cs
├── Models/
│   └── Productos.cs                 # Entidad Producto
├── Properties/
│   └── launchSettings.json
├── appsettings.json                 # Configuración y cadena de conexión
├── Program.cs                       # Punto de entrada y configuración de servicios
└── API_CON_DB.csproj
```

---

## 3. Configuración de la Base de Datos

### 3.1 Cadena de conexión (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Tienda;User=root;Password=REPLACE_WITH_LOCAL_PASSWORD;SslMode=none;AllowPublicKeyRetrieval=True;"
  }
}
```

> ⚠️ **Nota de seguridad:** la contraseña queda expuesta en texto plano en este archivo. Para un entorno de producción, mover la cadena de conexión a variables de entorno o a un gestor de secretos (por ejemplo, `dotnet user-secrets` en desarrollo).

**Parámetros relevantes:**
- `SslMode=none`: desactiva la conexión cifrada (válido para desarrollo local).
- `AllowPublicKeyRetrieval=True`: necesario porque MySQL 8+ usa por defecto el plugin de autenticación `caching_sha2_password`, que exige recuperar la clave pública RSA incluso en conexiones sin SSL. Sin este parámetro, las operaciones de escritura (`INSERT`, `UPDATE`) fallan con el error:
  ```
  MySqlException: Retrieval of the RSA public key is not enabled for insecure connections
  ```

### 3.2 Paquete NuGet utilizado

El proyecto usa el proveedor de **Oracle** para MySQL:

```
MySql.EntityFrameworkCore
```

> Durante el desarrollo se probó inicialmente con **Pomelo.EntityFrameworkCore.MySql**, pero se descartó por conflicto de versiones con `Microsoft.EntityFrameworkCore.Relational` en .NET 10 (Pomelo 9.0.0 requiere una versión de Relational entre 9.0.0 y 9.0.999, incompatible con la 10.0.11 instalada). El paquete Pomelo fue desinstalado con:
> ```
> dotnet remove package Pomelo.EntityFrameworkCore.MySql
> ```

**Diferencia clave entre ambos proveedores:**

| Proveedor | Método de configuración | Requiere `ServerVersion`? |
|---|---|---|
| Oracle `MySql.EntityFrameworkCore` | `options.UseMySQL(connectionString)` | No |
| Pomelo `Pomelo.EntityFrameworkCore.MySql` | `options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))` | Sí |

---

## 4. Modelo de Datos

### 4.1 `Models/Productos.cs`

```csharp
namespace API_CON_DB.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public int Cantidad { get; set; }
        public double Valor { get; set; }
    }
}
```

| Campo | Tipo | Descripción |
|---|---|---|
| `Id` | `int` | Clave primaria autogenerada |
| `Nombre` | `string` (obligatorio) | Nombre del producto |
| `Cantidad` | `int` | Cantidad en inventario |
| `Valor` | `double` | Precio unitario |

> El modificador `required` obliga a inicializar `Nombre` al crear una instancia, evitando la advertencia del compilador `CS8618` sobre propiedades no anulables sin valor por defecto.

### 4.2 `DB/AppDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using API_CON_DB.Models;

namespace API_CON_DB.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
    }
}
```

> **Historial:** el `DbSet` se llamó inicialmente `Producto` (singular), lo que generó la tabla `Producto` en la primera migración. Al renombrarlo a `Productos` (plural, siguiendo la convención estándar), fue necesario generar una nueva migración (`RenameProductoTable`) para reflejar el cambio en la base de datos; de lo contrario, la API fallaba con `Table 'tienda.productos' doesn't exist`.

### 4.3 `DB/DesignTimeDbContextFactory.cs`

Necesario para que las herramientas de línea de comandos de EF Core (`dotnet ef migrations`, `dotnet ef database update`) puedan crear una instancia de `AppDbContext` en tiempo de diseño, leyendo la cadena de conexión directamente desde `appsettings.json`:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace API_CON_DB.DB
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySQL(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
```

---

## 5. Configuración de la Aplicación (`Program.cs`)

```csharp
using API_CON_DB.DB;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString)
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## 6. Controlador — `ProductosController.cs`

Expone un CRUD completo sobre `/api/productos`:

| Método HTTP | Ruta | Acción | Código de éxito |
|---|---|---|---|
| `GET` | `/api/productos` | Lista todos los productos | `200 OK` |
| `GET` | `/api/productos/{id}` | Obtiene un producto por id | `200 OK` / `404 Not Found` |
| `POST` | `/api/productos` | Crea un nuevo producto | `201 Created` |
| `PUT` | `/api/productos/{id}` | Actualiza un producto existente | `204 No Content` |
| `DELETE` | `/api/productos/{id}` | Elimina un producto | `204 No Content` |

```csharp
using API_CON_DB.DB;
using API_CON_DB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_CON_DB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();
            return producto;
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.Id) return BadRequest();

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
```

### Ejemplo de body para `POST` / `PUT`

```json
{
  "nombre": "Producto de prueba",
  "cantidad": 10,
  "valor": 25.50
}
```

---

## 7. Migraciones aplicadas

| Migración | Descripción |
|---|---|
| `20260904233641_initialcreate` | Creación inicial de la base de datos con la tabla `Producto` (singular) |
| `20260905001918_RenameProductoTable` | Renombra la tabla a `Productos` (plural), tras actualizar el `DbSet` en `AppDbContext` |

**Comandos utilizados:**

```bash
# Crear una migración
dotnet ef migrations add <NombreMigracion>

# Aplicar migraciones pendientes a la base de datos
dotnet ef database update
```

---

## 8. Guía de ejecución local

```bash
# 1. Ubicarse en la carpeta del proyecto (dentro de la carpeta de la solución)
cd API_CON_DB

# 2. Restaurar dependencias
dotnet restore

# 3. Compilar
dotnet build

# 4. Aplicar migraciones (si hay cambios pendientes)
dotnet ef database update

# 5. Ejecutar la API
dotnet run
```

Al iniciar correctamente, la consola debe mostrar:
```
Now listening on: http://localhost:5031
Application started. Press Ctrl+C to shut down.
```

---

## 9. Pruebas con Postman

1. Crear una nueva petición en Postman.
2. Seleccionar el método HTTP correspondiente (`GET`, `POST`, `PUT`, `DELETE`) en el desplegable — **no** escribirlo dentro del campo de URL.
3. Usar como URL base: `http://localhost:5031/api/productos`.
4. Para `POST` y `PUT`: en la pestaña **Body**, seleccionar **raw** → **JSON**, y pegar el body de ejemplo (ver sección 6).
5. Enviar (`Send`) y verificar el código de estado y la respuesta JSON.

---

## 10. Problemas encontrados durante el desarrollo y sus soluciones

| # | Error | Causa | Solución |
|---|---|---|---|
| 1 | `CS0103: El nombre 'ServerVersion' no existe` / `CS1061: 'DbContextOptionsBuilder' no contiene 'UseMySql'` | Se mezcló la sintaxis de Pomelo (`UseMySql`, minúsculas) con el paquete de Oracle instalado (`MySql.EntityFrameworkCore`) | Usar `UseMySQL` (mayúsculas) sin `ServerVersion`, sintaxis del paquete Oracle |
| 2 | `warning CS8618` en `Nombre` | Propiedad `string` no anulable sin inicializar | Agregar el modificador `required` |
| 3 | `Requested value 'None' was not found` al correr `dotnet ef migrations add` | Valor `SslMode=None` no reconocido por el enum interno del proveedor | Ajustar el valor de `SslMode` en la cadena de conexión |
| 4 | `CS0101` / `CS0111`: clase `DesignTimeDbContextFactory` duplicada | El código de `DesignTimeDbContextFactory.cs` fue pegado por error dentro de `Program.cs` | Restaurar el contenido correcto de `Program.cs` |
| 5 | `NU1608`: conflicto de versión Pomelo vs `Microsoft.EntityFrameworkCore.Relational` | Paquete Pomelo instalado junto al de Oracle, versiones incompatibles con .NET 10 | `dotnet remove package Pomelo.EntityFrameworkCore.MySql` |
| 6 | `CS1061: 'AppDbContext' no contiene una definición para 'Productos'` | El `DbSet` se llamaba `Producto` (singular) en el contexto, pero el controlador usaba `Productos` (plural) | Renombrar el `DbSet` a `Productos` en `AppDbContext.cs` |
| 7 | `MySqlException: Table 'tienda.productos' doesn't exist` | La tabla física seguía llamándose `Producto` tras el cambio del `DbSet`; faltaba aplicar una nueva migración | `dotnet ef migrations add RenameProductoTable` + `dotnet ef database update` |
| 8 | `No se ha podido encontrar un proyecto para ejecutar` al correr `dotnet run` | El comando se ejecutó desde la carpeta de la solución en lugar de la carpeta del proyecto | `cd API_CON_DB` antes de `dotnet run` |
| 9 | `MySqlException: Retrieval of the RSA public key is not enabled for insecure connections` | MySQL 8+ exige SSL o permiso explícito para recuperar la clave RSA en conexiones sin cifrar | Agregar `AllowPublicKeyRetrieval=True` a la cadena de conexión |
| 10 | `404 Not Found` al probar en Postman | Se escribió el verbo HTTP (`GET`) dentro del campo de URL en lugar de seleccionarlo en el desplegable | Corregir la URL dejando solo la dirección, sin el verbo |

---

## 11. Pendientes / Mejoras sugeridas

- [ ] Mover la cadena de conexión (usuario y contraseña) a `dotnet user-secrets` o variables de entorno.
- [ ] Eliminar el controlador de ejemplo `WeatherForecastController.cs` si no se usa.
- [ ] Agregar validaciones de datos (`DataAnnotations`) en el modelo `Producto` (ej. `[Required]`, `[Range]`).
- [ ] Configurar CORS si la API será consumida desde un frontend en otro origen.
- [ ] Agregar manejo de errores centralizado (middleware de excepciones).
- [ ] Documentar la API con Swagger UI visible en desarrollo.
- [ ] Evaluar habilitar SSL real en la conexión a MySQL para entornos de producción, en lugar de `SslMode=none`.

## 12. Autor

Pablo Santamaria
