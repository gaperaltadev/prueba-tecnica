# MiPruebaTecnica - API REST

Este proyecto consiste en el desarrollo de una API REST construida bajo los estándares más modernos de **.NET 10**, siguiendo una arquitectura limpia y desacoplada mediante la implementación de patrones como **CQRS**, **MediatR** y validaciones fluidas con **FluentValidation**. La persistencia de datos se gestiona localmente de forma automatizada mediante **Entity Framework Core** y **SQLite**.

---

## 🚀 Decisiones Técnicas y Justificaciones de Diseño

Durante el ciclo de desarrollo de esta solución se priorizó el equilibrio entre las buenas prácticas arquitectónicas mundiales y las restricciones explícitas dictadas por las especificaciones de negocio. A continuación, se detallan los criterios de diseño clave:

### 1. Manejo Extensivo del Protocolo HTTP en Modificaciones (`PUT` vs `PATCH`)
* **Situación:** El endpoint de actualización de usuario (`PUT /users/{id}`) requiere la modificación potencial de los campos `Name`, `Email` e `IsActive`. Por definición purista del protocolo HTTP, un verbo `PUT` representa una sustitución completa del recurso (idempotencia distributiva), exigiendo contractualmente todos los campos del objeto en cada petición.
* **Solución Aplicada:** Para optimizar sustancialmente la experiencia del evaluador técnico en la interfaz interactiva de Swagger, se optó por diseñar un contrato híbrido tolerante a fallos. El `UpdateUserRequest` permite que los campos como `Name` y `Email` viajen con valores nulos o vacíos. El sistema intercepta estas propiedades y aplica una estrategia de **actualización parcial controlada**, sobreescribiendo en la base de datos únicamente las variables enviadas y conservando intactos los valores preexistentes del registro. Esto evita forzar al cliente a rellenar manualmente todo el esquema JSON cuando su única intención es alternar el estado lógico (`IsActive`).

### 2. Separación de Modelos de Entrada Públicos (`Requests`) de la Lógica Interna (`Commands`)
* **Aislamiento de Identificadores:** Se detectó que la autogeneración genérica acoplaba el `Id` y propiedades del sistema dentro del cuerpo del JSON (`Request Body`). En una API REST madura, el identificador de un recurso mutable se provee estrictamente a través de la **Ruta URL** (`/users/{id}`). 
* **Implementación:** Se introdujo la abstracción `UpdateUserRequest` para limpiar la interfaz de Swagger de datos redundantes o peligrosos. El endpoint minimal API captura el parámetro tipado de la ruta e inyecta dinámicamente ese identificador al comando interno de MediatR (`UpdateUserCommand`), blindando al backend contra vulnerabilidades de inconsistencia o inyección cruzada de entidades.

### 3. Blindaje de Expresiones Regulares en la Validación de Correo Electrónico
* **Inconsistencia Nativa:** La validación genérica `.EmailAddress()` provista por FluentValidation tiende a ser altamente permisiva según los estándares modernos de internet (aceptando por defecto strings con estructuras de dominio rotas como `texto@.com`).
* **Solución Aplicada:** Se incorporó un validador basado en expresiones regulares estrictas (`Matches(@"^[^@\s]+@[^@\s\.]+\.[^@\s\.]+$")`) garantizando de forma hermética la existencia de un prefijo de usuario, un dominio intermedio real y una extensión terminal de nivel superior (TLD), complementado con una verificación asíncrona concurrente en SQLite para asegurar la regla de **unicidad del correo electrónico** requerida por el negocio antes de confirmar el registro.

### 4. Adopción de Convenciones Modernas de .NET 10
* Se implementaron **Global Usings** centralizados en el archivo principal de inicialización de la aplicación para mitigar la redundancia de directivas al inicio de cada archivo C#, reduciendo significativamente el código repetitivo (*boilerplate code*) y manteniendo la legibilidad de las capas.
* Se activaron de manera permanente las extensiones de exploración de metadatos de Swagger con esquemas de seguridad basados en cabeceras de autenticación (`X-API-KEY`) de paso transparente hacia rutas administrativas, permitiendo auditar la documentación viva sin bloqueos del enrutador en cualquier entorno operativo (`Development` / `Production`).

---

## 🛠️ Requisitos e Instalación

Para ejecutar este proyecto de forma local, asegúrese de tener instalado el SDK oficial de **.NET 10.0** en su máquina.

1. **Restaurar y Limpiar el Entorno:**
   ```bash
   dotnet clean
   dotnet restore

## Quedo pendiente
- Aplicar la solucion de los PUT endpoints a las demas entidades. Solo quedo bien implementado en users.