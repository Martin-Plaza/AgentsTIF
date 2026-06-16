# Flujo backend de ServiControl

Este documento resume como circulan las solicitudes dentro del backend y sirve como guia para defensa oral del TPI.

## Capas

1. Domain
   Contiene entidades, enums y reglas propias del dominio. No depende de ASP.NET, EF Core, SQL Server ni otras capas.

2. Application
   Contiene DTOs, interfaces y servicios de caso de uso. Coordina reglas de aplicacion y depende solo de Domain.

3. Infrastructure
   Implementa persistencia, hashing, JWT y servicios externos. Depende de Application e implementa sus interfaces.

4. Presentation
   Expone la Web API, controllers, Swagger, CORS y autenticacion HTTP. Depende de Application para usar DTOs e interfaces, y de Infrastructure solo para registrar implementaciones concretas.

Clean Architecture se usa para que las reglas centrales del sistema no dependan de frameworks ni detalles externos. Controllers llaman servicios, servicios usan interfaces de repositorio, repositorios usan `ApplicationDbContext` y EF Core persiste en SQL Server.

## Referencias entre proyectos

```txt
ServiControl.Presentation -> ServiControl.Application
ServiControl.Presentation -> ServiControl.Infrastructure
ServiControl.Infrastructure -> ServiControl.Application
ServiControl.Application -> ServiControl.Domain
ServiControl.Domain -> ninguna capa
```

En `Program.cs`, Presentation registra las interfaces de Application con sus servicios de caso de uso y llama `AddInfrastructure(...)` para registrar EF Core, repositorios, JWT, hashing y servicios externos. Los controllers no usan `ApplicationDbContext`, repositorios concretos ni clases concretas de Infrastructure.

## Registro de usuario

1. `POST /api/auth/register` recibe nombre, email, password y rol.
2. `AuthController` delega en `IAuthService`.
3. `AuthService` valida email unico mediante `IUsuarioRepository`.
4. `AuthService` solicita el hash a `IPasswordHasher`.
5. `BCryptPasswordHasher` genera un hash BCrypt.
6. `UsuarioRepository` guarda el usuario con EF Core.
7. La API devuelve datos basicos del usuario sin exponer la password.

## Login con JWT

1. `POST /api/auth/login` recibe email y password.
2. `AuthService` busca el usuario por email.
3. `BCryptPasswordHasher` verifica la password contra el hash guardado.
4. `JwtTokenGenerator` genera un token con `IdUsuario`, `Email` y `Rol`.
5. La API devuelve token, id, nombre, email y rol.

## Crear cliente

1. El cliente HTTP envia `POST /api/clientes` con Bearer token.
2. JWT valida el token antes de entrar al controller.
3. `ClientesController` delega en `IClienteService`.
4. `ClienteService` crea la entidad `Cliente`.
5. `ClienteRepository` persiste con EF Core.

## Crear trabajo

1. `POST /api/trabajos` recibe cliente, usuario, categoria, descripcion, fecha y direccion.
2. `TrabajoService` valida que existan cliente y usuario.
3. La entidad `Trabajo` nace en estado `Pendiente`.
4. `TrabajoRepository` guarda el trabajo.

## Cambiar estado de trabajo

1. `PUT /api/trabajos/{id}/estado`, `/finalizar` o `/cancelar` reciben la accion.
2. `TrabajoService` carga el trabajo.
3. La entidad `Trabajo` aplica reglas de dominio:
   - Un trabajo cancelado no puede finalizarse.
   - Un trabajo finalizado no puede modificarse.
   - No se vuelve a `Pendiente`.
4. `TrabajoRepository` guarda el cambio.

## Registrar costo final

1. `PUT /api/costos/{id}/final` recibe el monto final.
2. `CostoService` valida que exista el trabajo.
3. La entidad `Costo` valida que el costo no sea negativo.
4. `CostoRepository` actualiza o crea el costo asociado.

## Generar metricas

1. `GET /api/metricas?usuarioId=...&periodoInicio=...&periodoFin=...`.
2. `MetricaService` obtiene trabajos del usuario en el rango.
3. Solo considera trabajos `Finalizado` para ingresos.
4. Suma costos finales y calcula promedio.
5. La entidad `Metrica` valida que `periodoInicio <= periodoFin`.
6. `MetricaRepository` guarda la metrica.

## Flujo de persistencia con EF Core

1. Application define interfaces como `IClienteRepository` e `IGenericRepository<T>`.
2. Infrastructure implementa esas interfaces.
3. `GenericRepository<T>` centraliza CRUD comun.
4. Repositorios especificos agregan consultas propias.
5. `ApplicationDbContext` configura tablas, relaciones, indices, precision decimal y enums mediante Fluent API.
6. La connection string se lee desde configuracion externa. En Azure debe configurarse como connection string del App Service.

## Flujo de autorizacion con JWT

1. El usuario hace login y recibe un token.
2. Swagger o el cliente HTTP envia `Authorization: Bearer {token}`.
3. `UseAuthentication()` valida firma, issuer, audience y expiracion.
4. `[Authorize]` bloquea endpoints protegidos si no hay token valido.
5. Endpoints publicos: `POST /api/auth/register` y `POST /api/auth/login`.

## Servicio externo de feriados

1. `GET /api/feriados/{anio}` esta protegido con JWT.
2. `FeriadosController` llama `IFeriadoService`.
3. `FeriadoService` usa `HttpClientFactory` para consumir `https://api.argentinadatos.com/v1/feriados/{anio}`.
4. La respuesta se mapea a `FeriadoDto`.

## Preparacion para Azure

Para desplegar en Azure App Service:

1. Configurar `ConnectionStrings__DefaultConnection` o la connection string del App Service.
2. Configurar `Jwt__Key` como App Setting o, preferentemente, desde Key Vault.
3. Configurar `Jwt__Issuer`, `Jwt__Audience` y `Jwt__ExpirationMinutes` si se desea sobrescribir valores.
4. Configurar `Cors__AllowedOrigins__0` con el dominio del frontend o cliente permitido.
5. Swagger esta habilitado solo en `Development`.
