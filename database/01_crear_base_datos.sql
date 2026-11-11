-- Catálogo de cursos
-- Ejecutar en SQL Server Management Studio.
-- Crea la base de datos y agrega datos iniciales sin duplicarlos.

USE master;
GO

IF DB_ID(N'CatalogoCursosDb') IS NULL
BEGIN
    CREATE DATABASE CatalogoCursosDb;
END;
GO

USE CatalogoCursosDb;
GO

-- 1. Categorías
IF OBJECT_ID(N'dbo.Categorias', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categorias (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        CONSTRAINT UQ_Categorias_Nombre UNIQUE (Nombre)
    );
END;
GO

-- 2. Usuarios
-- Las cuentas de demostración se crean desde la API
-- al ejecutarla en el entorno Development.
-- Las contraseñas se almacenan como hash.
IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Correo NVARCHAR(150) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        Rol NVARCHAR(10) NOT NULL,
        CONSTRAINT UQ_Usuarios_Correo UNIQUE (Correo),
        CONSTRAINT CK_Usuarios_Rol
            CHECK (Rol IN ('admin', 'user'))
    );
END;
GO

-- 3. Cursos
IF OBJECT_ID(N'dbo.Cursos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cursos (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CategoriaId INT NOT NULL,
        Titulo NVARCHAR(150) NOT NULL,
        Descripcion NVARCHAR(1000) NOT NULL,
        ImagenUrl NVARCHAR(500) NULL,
        Precio DECIMAL(10,2) NOT NULL,
        CONSTRAINT FK_Cursos_Categorias
            FOREIGN KEY (CategoriaId)
            REFERENCES dbo.Categorias(Id),
        CONSTRAINT CK_Cursos_Precio
            CHECK (Precio >= 0)
    );
END;
GO

-- 4. Categorías iniciales
INSERT INTO dbo.Categorias (Nombre)
SELECT datos.Nombre
FROM (
    VALUES
        (N'Frontend'),
        (N'Backend'),
        (N'Bases de datos')
) AS datos(Nombre)
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.Categorias existente
    WHERE existente.Nombre = datos.Nombre
);
GO

-- 5. Cursos iniciales
-- Se busca cada categoría por nombre para no depender
-- de identificadores específicos.
INSERT INTO dbo.Cursos (
    CategoriaId,
    Titulo,
    Descripcion,
    ImagenUrl,
    Precio
)
SELECT
    categoria.Id,
    datos.Titulo,
    datos.Descripcion,
    NULL,
    datos.Precio
FROM (
    VALUES
        (
            N'Frontend',
            N'HTML y CSS desde cero',
            N'Aprende a crear páginas web adaptables.',
            499.00
        ),
        (
            N'Frontend',
            N'Introducción a React',
            N'Crea interfaces utilizando componentes.',
            799.00
        ),
        (
            N'Backend',
            N'APIs con ASP.NET Core',
            N'Desarrolla servicios web con C#.',
            899.00
        ),
        (
            N'Bases de datos',
            N'SQL Server básico',
            N'Aprende consultas y relaciones entre tablas.',
            599.00
        ),
        (
            N'Backend',
            N'C# desde cero',
            N'Aprende variables, condiciones, ciclos y clases.',
            699.00
        )
) AS datos(Categoria, Titulo, Descripcion, Precio)
INNER JOIN dbo.Categorias categoria
    ON categoria.Nombre = datos.Categoria
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.Cursos existente
    WHERE existente.Titulo = datos.Titulo
);
GO

-- 6. Verificación
SELECT Id, Nombre
FROM dbo.Categorias
ORDER BY Id;

SELECT
    curso.Id,
    curso.Titulo,
    curso.Descripcion,
    curso.Precio,
    categoria.Nombre AS Categoria
FROM dbo.Cursos curso
INNER JOIN dbo.Categorias categoria
    ON categoria.Id = curso.CategoriaId
ORDER BY curso.Id;
GO