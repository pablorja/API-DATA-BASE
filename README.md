# 🛒 API_CON_DB

**API REST en ASP.NET Core (.NET 10) conectada a MySQL**, con operaciones CRUD completas sobre un catálogo de productos.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?style=for-the-badge&logo=mysql&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity%20Framework-Core-68217A?style=for-the-badge&logo=nuget&logoColor=white)
![License](https://img.shields.io/badge/status-en%20desarrollo-yellow?style=for-the-badge)

---

## 📋 Descripción

Este proyecto implementa una API REST que gestiona un inventario de productos, utilizando **Entity Framework Core** como ORM y **MySQL** como motor de base de datos. Fue desarrollado como parte del curso de Diseño Web (TEC-UPB).

## ✨ Características

- ✅ CRUD completo de productos (`GET`, `POST`, `PUT`, `DELETE`)
- ✅ Conexión a MySQL mediante Entity Framework Core
- ✅ Migraciones versionadas de base de datos
- ✅ Documentación de endpoints y arquitectura incluida
- ✅ Probado end-to-end con Postman

## 🛠️ Tecnologías

| Tecnología | Uso |
|---|---|
| **ASP.NET Core (.NET 10)** | Framework de la API |
| **Entity Framework Core** | ORM |
| **MySql.EntityFrameworkCore** | Proveedor de conexión a MySQL |
| **MySQL** | Base de datos relacional |
| **Postman** | Pruebas de endpoints |

## 📂 Estructura del proyecto

```
API_CON_DB/
├── Controllers/
│   └── ProductosController.cs
├── DB/
│   ├── AppDbContext.cs
│   └── DesignTimeDbContextFactory.cs
├── Migrations/
├── Models/
│   └── Productos.cs
├── appsettings.json
└── Program.cs
```

## 🚀 Cómo ejecutar el proyecto

```bash
# 1. Clonar el repositorio
git clone https://github.com/pablorja/API-DATA-BASE.git
cd API-DATA-BASE/API_CON_DB

# 2. Restaurar dependencias
dotnet restore

# 3. Configurar la cadena de conexión en appsettings.json
#    (ver sección de configuración más abajo)

# 4. Aplicar migraciones
dotnet ef database update

# 5. Ejecutar la API
dotnet run
```

La API quedará disponible en `http://localhost:5031`.

## ⚙️ Configuración de la base de datos

En `appsettings.json`, ajusta la cadena de conexión con tus propios datos:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Tienda;User=root;Password=TU_PASSWORD;SslMode=none;AllowPublicKeyRetrieval=True;"
  }
}
```

> ⚠️ No subas contraseñas reales a un repositorio público. Usa `dotnet user-secrets` o variables de entorno en proyectos reales.

## 📡 Endpoints disponibles

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/productos` | Lista todos los productos |
| `GET` | `/api/productos/{id}` | Obtiene un producto por id |
| `POST` | `/api/productos` | Crea un nuevo producto |
| `PUT` | `/api/productos/{id}` | Actualiza un producto existente |
| `DELETE` | `/api/productos/{id}` | Elimina un producto |

**Ejemplo de body (POST/PUT):**
```json
{
  "nombre": "Producto de prueba",
  "cantidad": 10,
  "valor": 25.50
}
```

## 📖 Documentación completa

Para el detalle técnico completo (modelos, migraciones, decisiones de arquitectura y solución de errores comunes), consulta:

📄 [**API_CON_DB_Documentacion.md**](./API_CON_DB_Documentacion.md)

## 👤 Autor

**Pablo Rodríguez**
Proyecto académico — TEC-UPB, Diseño Web

---

<p align="center">Hecho con ☕ y muchas migraciones de Entity Framework</p>
