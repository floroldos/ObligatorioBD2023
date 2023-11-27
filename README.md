# Obligatorio Bases de Datos - segundo semestre 2023 - Formulario Carnet de Salud de Funcionarios
Integrantes:
Matías Varela, León Salvo, Florencia Roldós, Floriana Locatelli

## Descripción del Proyecto
La aplicación permitirá a los usuarios cargar los carnets de salud de los funcionarios y almacenar la información en una base de datos. Utiliza la tecnología Blazor para el desarrollo de la interfaz de usuario y ASP.NET Core para la lógica del servidor.

## Características Principales

- **Formulario de Carga:** Permite a los usuarios cargar y asociar los carnets de salud de los funcionarios.
- **Base de Datos:** Utiliza una base de datos para almacenar la información de los carnets de salud.
- **Seguridad:** Implementa medidas de seguridad para proteger la información sensible.

## Requisitos 

- [Docker](https://www.docker.com/get-started)
- [.NET SDK](https://dotnet.microsoft.com/download) versión 6.

## Configuración del Proyecto

Clona este repositorio: `https://github.com/floroldos/ObligatorioBD2023.git`

## Uso

1. Iniciar la Aplicación con Docker Compose
Dirigirse a la carpeta *sql-scripts*, `cd sql-scripts`.
Ejecutar `docker-compose up` para levantar la aplicación y su base de datos.

3. Compilar y Ejecutar con dotnet watch.
  Dentro del directorio de la aplicación, ejecutar:
  `dotnet watch`
  Este comando compilará la aplicación y la ejecutará. La opción watch permitirá que la aplicación se reinicie automáticamente cuando se realicen cambios en el código.

4. Acceder a la Aplicación
Abrir navegador y acceder a la aplicación en http://localhost:5199.


