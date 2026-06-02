# Checklist tecnico para Aprobacion Directa / Promocion

Fecha de revision: 2026-06-02

Fuente revisada: `docs/Requirimientos TPI.pdf`

Resultado global:

- Aprobacion No Directa / Regularizacion: cumple.
- Aprobacion Directa / Promocion: parcial. El backend esta funcional y preparado, pero falta deploy real en Azure y configurar recursos/secretos reales en la nube.

Validacion ejecutada:

```bash
dotnet build ServiControl.slnx
```

Resultado:

```txt
Compilacion correcta.
0 advertencias
0 errores
```

## Checklist de requisitos

| Requisito del PDF / Defensa | Estado | Evidencia del proyecto | Accion pendiente |
|---|---|---|---|
| Proyectos definidos segun Clean Architecture | Cumple | `ServiControl.slnx` contiene `ServiControl.Domain`, `ServiControl.Application`, `ServiControl.Infrastructure`, `ServiControl.Presentation`. | Ninguna. |
| Domain no depende de otras capas | Cumple | `ServiControl.Domain/ServiControl.Domain.csproj` no tiene `ProjectReference`. | Ninguna. |
| Application depende de Domain | Cumple | `ServiControl.Application/ServiControl.Application.csproj` referencia `ServiControl.Domain`. | Ninguna. |
| Infrastructure depende de Application | Cumple | `ServiControl.Infrastructure/ServiControl.Infrastructure.csproj` referencia `ServiControl.Application`. No referencia directo a Presentation. | Ninguna. |
| Presentation depende de Application e Infrastructure | Cumple | `ServiControl.Presentation/ServiControl.Presentation.csproj` referencia ambas capas. | Ninguna. |
| Entidades declaradas con atributos principales | Cumple | `Domain/Entities/Usuario.cs`, `Cliente.cs`, `Trabajo.cs`, `Costo.cs`, `Metrica.cs`. | Ninguna. |
| Enums principales definidos | Cumple | `Domain/Enums/RolUsuario.cs`, `EstadoTrabajo.cs`, `CategoriaServicio.cs`. | Ninguna. |
| Reglas de dominio basicas | Cumple | `Trabajo` inicia en pendiente y valida estados; `Costo` valida no negativo; `Metrica` valida rango de fechas. | Ninguna. |
| Repositorios definidos | Cumple | Interfaces en `Application/Interfaces`: `IClienteRepository`, `ITrabajoRepository`, `ICostoRepository`, `IMetricaRepository`, `IUsuarioRepository`. | Ninguna. |
| Repositorios implementados | Cumple | Implementaciones en `Infrastructure/Persistence/Repositories`. | Ninguna. |
| Patron Generic Repository | Cumple | `Application/Interfaces/IGenericRepository.cs` y `Infrastructure/Persistence/Repositories/GenericRepository.cs`. Repositorios especificos heredan del generico. | Ninguna. |
| Repositorios especificos conservan consultas propias | Cumple | `UsuarioRepository.GetByEmailAsync`, `TrabajoRepository.GetPendientesAsync`, `CostoRepository.GetByTrabajoIdAsync`. | Ninguna. |
| Servicios definidos e inyectados | Cumple | Servicios en `Application/Services`; registro en `Presentation/Program.cs`. | Ninguna. |
| Controllers definidos con servicios inyectados | Cumple | `ClientesController`, `TrabajosController`, `CostosController`, `MetricasController`, `FeriadosController`, `AuthController`. | Ninguna. |
| Controllers sin logica de negocio | Cumple | Controllers delegan en interfaces de Application y manejan respuestas HTTP. | Mantener este criterio en futuros endpoints. |
| AuthController definido | Cumple | `Presentation/Controllers/AuthController.cs`. | Ninguna. |
| Register/Login funcionales | Cumple | `AuthService`, `UsuarioRepository`, `BCryptPasswordHasher`, `AuthController`. | Mantener pruebas manuales en Swagger. |
| Password no se guarda en texto plano | Cumple | `Infrastructure/Security/BCryptPasswordHasher.cs` usa BCrypt. | Ninguna. |
| JWT funcional | Cumple | `Infrastructure/Authentication/JwtTokenGenerator.cs`, `Presentation/Program.cs`, `IJwtTokenGenerator`. | Ninguna para entorno local/desarrollo. |
| Endpoints protegidos con JWT | Cumple | `[Authorize]` en controllers de clientes, trabajos, costos, metricas y feriados. | Ninguna. |
| Swagger con Bearer token | Cumple | `Program.cs` configura `AddSwaggerGen`, `AddSecurityDefinition` y `AddSecurityRequirement`. | Ninguna. |
| EF Core Code First | Cumple | `ApplicationDbContext.cs`, paquetes EF Core en `Infrastructure.csproj`, migraciones presentes. | Ninguna. |
| Fluent API | Cumple | `ApplicationDbContext.OnModelCreating` configura tablas, claves, relaciones, indices, precision decimal y conversion de enums. | Ninguna. |
| Migraciones y Snapshot presentes | Cumple | `Infrastructure/Persistence/Migrations/20260527181222_InitialCreate.cs` y `ApplicationDbContextModelSnapshot.cs`. | Generar nuevas migraciones si cambia el modelo. |
| SQL Server configurado | Cumple local | `appsettings.Development.json` usa LocalDB SQL Server; `UseSqlServer` en `DependencyInjection.cs`. | Para promocion, configurar SQL Server de Azure o SQL Server multiusuario equivalente. |
| Persistencia operativa en base relacional | Cumple local | `dotnet ef database update` fue usado previamente y los flujos register/login/metricas fueron probados con SQL Server LocalDB. | Validar en base de datos de Azure al desplegar. |
| Servicio externo mediante HttpClientFactory | Cumple | `IFeriadoService`, `FeriadoService`, `AddHttpClient<IFeriadoService, FeriadoService>`. | Ninguna. |
| Endpoint que consume servicio externo | Cumple | `GET /api/feriados/{anio}` en `FeriadosController`. | Ninguna. |
| GitHub Actions CI/CD | Parcial | `.github/workflows/deploy-api.yml` existe; ejecuta restore, build, publish y usa `azure/webapps-deploy@v3`. | Configurar secrets y validar una corrida real en GitHub. |
| Deploy a Azure App Service | Parcial | Workflow preparado con `AZURE_WEBAPP_NAME` y `AZURE_WEBAPP_PUBLISH_PROFILE`. | Crear App Service, cargar publish profile y ejecutar deploy real. |
| Deploy de recursos subyacentes en Azure | No cumple | El workflow despliega la API, pero no crea resource group ni base de datos. | Crear recursos en Azure manualmente o agregar IaC/pasos de provisionamiento. |
| Resource group Azure con API y base relacional multiusuario | No cumple | No hay evidencia de resource group ni SQL Server Azure en el repo. | Crear Resource Group, App Service y Azure SQL / SQL Server multiusuario. |
| Secret JWT fuera de appsettings para produccion | Cumple preparacion / Parcial operativo | `appsettings.json` no contiene `Jwt:Key`; `Program.cs` lee `Jwt:Key` desde configuracion. `appsettings.Development.json` conserva clave local de desarrollo. | Configurar `Jwt__Key` como App Setting de Azure o Key Vault. |
| Connection string fuera de appsettings para produccion | Cumple preparacion / Parcial operativo | `appsettings.json` no contiene connection string local; `DependencyInjection.cs` lee `GetConnectionString("DefaultConnection")`. | Configurar connection string en Azure App Service. |
| CORS para desarrollo y produccion | Cumple preparacion | `Program.cs` permite cualquier origen solo en Development; en produccion lee `Cors:AllowedOrigins`. | Configurar dominios reales con `Cors__AllowedOrigins__0` en Azure. |
| Swagger en Development | Cumple | `Program.cs` habilita Swagger solo cuando `app.Environment.IsDevelopment()`. | Ninguna. |
| API deployable en Azure App Service | Parcial | Proyecto Web API publicable: `ServiControl.Presentation.csproj`; workflow hace `dotnet publish`. | Completar configuracion Azure y ejecutar deploy real. |
| API accesible externamente por dominio | No cumple | No hay URL publica de Azure registrada en el proyecto. | Desplegar en Azure App Service y documentar URL. |

## Observaciones tecnicas

1. El backend cumple ampliamente los requisitos de regularizacion.
2. Para promocion, el codigo ya cubre autenticacion JWT, EF Core, repositorio generico, HttpClientFactory y workflow preparado.
3. Lo pendiente de promocion esta concentrado en infraestructura real de Azure:
   - App Service.
   - Base de datos relacional multiusuario.
   - Variables de entorno / App Settings.
   - Secrets de GitHub Actions.
   - Ejecucion real del workflow.
4. No se detectan secretos reales en `appsettings.json`. La clave JWT local queda solo en `appsettings.Development.json`.
5. Existe documentacion complementaria del flujo en `docs/flujo-backend.md`.

## Comandos utiles para defensa

Compilar:

```bash
dotnet build ServiControl.slnx
```

Ejecutar API:

```bash
dotnet run --project ServiControl.Presentation/ServiControl.Presentation.csproj
```

Swagger local:

```txt
http://localhost:5141/swagger
```

Crear migracion si cambia el modelo:

```bash
dotnet ef migrations add NombreMigracion --project ServiControl.Infrastructure/ServiControl.Infrastructure.csproj --startup-project ServiControl.Presentation/ServiControl.Presentation.csproj --output-dir Persistence/Migrations
```

Aplicar migraciones:

```bash
dotnet ef database update --project ServiControl.Infrastructure/ServiControl.Infrastructure.csproj --startup-project ServiControl.Presentation/ServiControl.Presentation.csproj
```
