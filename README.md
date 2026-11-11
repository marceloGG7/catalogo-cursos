# Catálogo de cursos de programación

Aplicación desarrollada como prueba técnica. Este primer avance incluye
una API REST con ASP.NET Core, SQL Server y Entity Framework Core.

## Estado del proyecto

Backend funcional. Pendientes: frontend en React, integración visual,
diseño adaptable y validación final del flujo completo.

## Funcionalidades implementadas

- Consulta de categorías.
- Consulta de cursos y filtro por categoría.
- Consulta de un curso por identificador.
- Creación, actualización y eliminación de cursos.
- Inicio y cierre de sesión mediante cookies.
- Roles admin y user.
- Restricción de modificaciones al administrador.
- Almacenamiento de contraseñas mediante hash.
- Validaciones mediante DTOs y comprobación de categorías.
- Documentación interactiva con Swagger UI.

## Tecnologías

- C# y ASP.NET Core sobre .NET 10.
- Entity Framework Core.
- SQL Server LocalDB.
- Swagger UI mediante Swashbuckle.

## Requisitos para ejecutar localmente

- Windows con SDK de .NET 10.
- SQL Server Express LocalDB con la instancia MSSQLLocalDB.
- SQL Server Management Studio para ejecutar el script SQL.
- Git para clonar el repositorio.

También es posible utilizar otra instancia de SQL Server ajustando
la cadena de conexión.

## Configuración de la base de datos

1. Abrir SQL Server Management Studio.
2. Conectarse a `(localdb)\MSSQLLocalDB` con autenticación de Windows.
3. Abrir y ejecutar `database/01_crear_base_datos.sql`.

El script crea CatalogoCursosDb, sus tablas y los datos iniciales
de categorías y cursos. No modifica el esquema de tablas ya existentes.

La conexión se configura en:

`CatalogoCursos.Api/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CatalogoCursosDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
  }
}
```

Este fragmento muestra únicamente la sección de conexión.
La configuración de confianza del certificado está destinada al entorno local.

## Ejecutar la API

Desde la carpeta principal del repositorio:

```powershell
cd CatalogoCursos.Api
dotnet restore
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --no-launch-profile --urls http://localhost:5050
```

Abrir Swagger UI:

http://localhost:5050/swagger

## Cuentas de demostración

Al iniciar en Development, la API crea las cuentas si no existen.

| Correo | Contraseña de demostración | Rol |
|---|---|---|
| admin@catalogo.test | AdminDemo2026! | admin |
| usuario@catalogo.test | UsuarioDemo2026! | user |

Son cuentas de prueba local. No deben utilizarse como credenciales
de un entorno de producción.

## Probar la autenticación en Swagger

1. Ejecutar POST /api/auth/login con una cuenta de demostración.
2. Ejecutar GET /api/auth/me para consultar la sesión.
3. Probar las operaciones de cursos.
4. Ejecutar POST /api/auth/logout para cerrar la sesión.

Swagger y la API comparten el mismo origen, por lo que el navegador
envía automáticamente la cookie. No es necesario introducir un JWT.

Las sesiones de PowerShell y del navegador son independientes.

## Endpoints

| Método | Ruta | Acceso |
|---|---|---|
| GET | /api/categorias | Público |
| GET | /api/cursos | Público |
| GET | /api/cursos?categoriaId=1 | Público |
| GET | /api/cursos/{id} | Público |
| POST | /api/cursos | admin |
| PUT | /api/cursos/{id} | admin |
| DELETE | /api/cursos/{id} | admin |
| POST | /api/auth/login | Público |
| GET | /api/auth/me | Usuario autenticado |
| POST | /api/auth/logout | Usuario autenticado |

La documentación interactiva muestra los cuerpos de las solicitudes.

## Base de datos y normalización

- Categorias: identificador y nombre único.
- Cursos: identificador, categoría, título, descripción, imagen y precio.
- Usuarios: identificador, nombre, correo único, hash de contraseña y rol.

Relación: una categoría puede tener varios cursos; cada curso pertenece
a una categoría mediante Cursos.CategoriaId.

Las tablas tienen claves primarias y atributos atómicos. Los atributos
describen a la entidad identificada por su clave. El nombre de la categoría
se almacena en Categorias y no se repite en Cursos, evitando esa
dependencia transitiva y manteniendo el diseño en tercera forma normal.

La base de datos restringe los roles a admin y user, impide precios
negativos y exige una categoría existente para cada curso.

## Organización

- Controllers: endpoints HTTP.
- DTOs: datos recibidos y sus validaciones.
- Models: entidades de la base de datos.
- Data: contexto de Entity Framework y cuentas iniciales.
- database: script de creación y datos iniciales.

## Verificaciones manuales realizadas

- Consulta de categorías y cursos.
- Filtro de cursos por categoría.
- Creación, actualización y eliminación de un curso temporal.
- Inicio de sesión y consulta de la sesión desde Swagger.
- DELETE sin sesión: 401.
- DELETE con rol user: 403.
- DELETE con rol admin sobre un ID inexistente: 404.

El resultado 404 confirma que el administrador supera la autorización;
las operaciones del CRUD también se verificaron antes de agregar
las restricciones por rol.

## Pendientes

- Implementar frontend en React basado en el wireframe seleccionado.
- Integrar catálogo, filtros, login y administración.
- Verificar diseño adaptable.
- Repetir el flujo de modificaciones con la sesión de administrador.
- Preparar video demostrativo y documentación final.