# AGENTS.md

# ServiControl - Backend Context

## Descripción general

ServiControl es una aplicación web desarrollada como Trabajo Integrador Final (TIF) de la Tecnicatura Universitaria en Programación.

El sistema está orientado a técnicos independientes que realizan servicios domiciliarios, como:

- Plomeros
- Electricistas
- Técnicos en aire acondicionado
- Técnicos de mantenimiento

El objetivo principal es digitalizar la gestión operativa del técnico, permitiendo administrar:

- Clientes
- Trabajos
- Estados
- Costos
- Métricas

---

# Documentación del proyecto

Antes de implementar funcionalidades, leer la documentación ubicada en:

```txt
/docs
```

Los documentos dentro de `/docs` contienen:

- Propuesta del proyecto
- DER
- UML
- Diccionario de datos
- Reglas de negocio
- Casos de uso
- Módulos del sistema

No asumir reglas de negocio que no estén documentadas.

---

# Stack tecnológico

## Backend

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Code First
- Clean Architecture

## Frontend

- React
- Mobile First

---

# Arquitectura obligatoria

El proyecto debe implementarse utilizando Clean Architecture.

La solución debe dividirse en:

```txt
ServiControl.Domain
ServiControl.Application
ServiControl.Infrastructure
ServiControl.Api
```

---

# Responsabilidad de cada capa

## Domain

Contiene:

- Entidades
- Enums
- Reglas simples del dominio

NO debe depender de:

- ASP.NET Core
- Entity Framework Core
- SQL Server
- Infrastructure
- Api

Debe ser una capa pura.

---

## Application

Contiene:

- Services
- Interfaces
- DTOs
- Casos de uso
- Validaciones

Puede depender únicamente de:

```txt
Domain
```

Toda la lógica de negocio debe implementarse acá.

---

## Infrastructure

Contiene:

- AppDbContext
- Entity Framework Core
- SQL Server
- Repositories
- Migraciones
- JWT
- Hashing
- Servicios externos

Puede depender de:

```txt
Application
Domain
```

---

## Api

Contiene:

- Controllers
- Program.cs
- Swagger
- Middlewares
- Dependency Injection

Puede depender de:

```txt
Application
Infrastructure
```

Los Controllers NO deben contener lógica de negocio.

---

# Regla de dependencias

Las dependencias deben ir siempre hacia adentro:

```txt
Api → Application
Api → Infrastructure

Infrastructure → Application
Infrastructure → Domain

Application → Domain

Domain → ninguna capa
```

Nunca hacer:

```txt
Domain → Infrastructure
Domain → Api

Application → Api
Application → Infrastructure
```

---

# Estructura sugerida

```txt
ServiControl.Domain
│
├── Entities
└── Enums

ServiControl.Application
│
├── DTOs
├── Interfaces
└── Services

ServiControl.Infrastructure
│
├── Persistence
│   ├── Context
│   ├── Migrations
│   └── Repositories
│
├── Authentication
└── Security

ServiControl.Api
│
├── Controllers
└── Program.cs
```

---

# Entidades principales

Las entidades principales del sistema son:

- User
- Client
- Job
- Cost
- Metric
- ServiceCategory

---

# Roles del sistema

Roles válidos:

```txt
Admin
Technician
Assistant
```

---

# Estados de trabajo

Estados válidos:

```txt
Pending
InProgress
Finished
Cancelled
```

## Reglas

- Todo trabajo nuevo inicia en Pending.
- Solo trabajos Finished impactan métricas.
- Trabajos Cancelled no generan ingresos.
- Un trabajo Cancelled no puede finalizarse.
- Un trabajo Finished no debe volver a Pending.

---

# Reglas generales

- No colocar lógica de negocio en Controllers.
- No devolver entidades directamente desde Controllers.
- Usar DTOs para requests y responses.
- Usar async/await.
- No crear endpoints innecesarios.
- No agregar paquetes innecesarios.
- No modificar migraciones anteriores manualmente.
- Mantener código simple y mantenible.
- Priorizar legibilidad sobre sobreingeniería.

---

# Convenciones

## Controllers

```txt
ClientsController
JobsController
AuthController
UsersController
```

## Services

```txt
ClientService
JobService
AuthService
UserService
```

## Interfaces

```txt
IClientService
IJobService
IAuthService
IUserService
```

## DTOs Request

```txt
CreateClientRequest
UpdateClientRequest

CreateJobRequest
UpdateJobRequest
UpdateJobStatusRequest

RegisterFinalCostRequest

LoginRequest
RegisterRequest
```

## DTOs Response

```txt
ClientResponse
JobResponse
CostResponse
MetricResponse
AuthResponse
UserResponse
```

---

# Módulos principales

## Auth

Responsabilidad:

- Registro
- Login
- JWT
- Autenticación

---

## Users

Responsabilidad:

- Gestión de usuarios
- Gestión de roles

---

## Clients

Responsabilidad:

- Gestión de clientes

Operaciones:

- Crear cliente
- Obtener clientes
- Obtener cliente por id
- Actualizar cliente
- Eliminar cliente

---

## Jobs

Responsabilidad:

- Gestión de trabajos técnicos

Operaciones:

- Crear trabajo
- Actualizar estado
- Cancelar trabajo
- Finalizar trabajo
- Obtener trabajos

---

## Costs

Responsabilidad:

- Gestión de costos

Operaciones:

- Registrar costo estimado
- Registrar costo final
- Actualizar costos

---

## Metrics

Responsabilidad:

- Generación de métricas

Operaciones:

- Calcular ingresos
- Calcular trabajos finalizados
- Calcular trabajos pendientes
- Calcular ticket promedio

---

# Reglas de negocio importantes

- Todo trabajo debe pertenecer a un cliente.
- Todo trabajo debe pertenecer a un usuario.
- El costo final no puede ser negativo.
- El costo estimado no puede ser negativo.
- Un trabajo puede existir sin costos inicialmente.
- El email de usuario debe ser único.
- Solo usuarios autenticados pueden acceder al sistema.
- Solo trabajos Finished generan métricas.
- Las métricas deben calcularse por rango de fechas.

---

# Reglas para Entity Framework Core

- Usar Code First.
- Configurar relaciones correctamente.
- Configurar precisión decimal.
- Configurar Email como único.
- Crear migraciones con nombres claros.
- No modificar migraciones anteriores manualmente.

---

# Flujo principal

Flujo principal del sistema:

1. Crear cliente.
2. Crear trabajo asociado a cliente.
3. Registrar costo estimado.
4. Actualizar estado del trabajo.
5. Finalizar trabajo.
6. Registrar costo final.
7. Actualizar métricas.

---

# Forma de trabajo esperada

Antes de escribir código:

1. Explicar qué archivos se van a crear o modificar.
2. Explicar por qué se necesitan.
3. Explicar qué reglas de negocio aplican.

Después de escribir código:

1. Explicar qué se implementó.
2. Explicar cómo probarlo.
3. Explicar endpoints involucrados.
4. Explicar si requiere migración.

---

# Restricciones para el agente

- No implementar todo el backend de una sola vez.
- Trabajar módulo por módulo.
- No cambiar arquitectura sin autorización.
- No agregar paquetes innecesarios.
- No crear endpoints fuera del alcance de la tarea.
- No crear entidades nuevas sin justificar.
- No duplicar lógica.
- No mezclar responsabilidades entre capas.
- No asumir reglas de negocio que no estén documentadas.

---

# Primer objetivo técnico

El primer objetivo del proyecto es:

1. Crear la solución con Clean Architecture.
2. Configurar proyectos y referencias.
3. Configurar Entity Framework Core.
4. Configurar SQL Server.
5. Crear entidades principales.
6. Crear AppDbContext.
7. Crear primera migración.

NO implementar todavía:

- JWT
- Autorización
- Módulos completos
- Frontend