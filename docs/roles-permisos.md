# Roles y permisos

ServiControl utiliza el claim `ClaimTypes.Role` del JWT para aplicar autorizacion RBAC.

Los nombres almacenados en el token son:

```txt
Admin
Technician
Assistant
```

La clase `ServiControl.Application/Authorization/Roles.cs` centraliza estos valores para evitar strings repetidos en los controllers.

## Matriz implementada

| Endpoint | Admin | Tecnico | Asistente |
|---|:---:|:---:|:---:|
| `POST /api/auth/login` | Publico | Publico | Publico |
| `POST /api/auth/register` | Si | No | No |
| `GET /api/usuarios` | Si | No | No |
| `GET /api/usuarios/{id}` | Si | No | No |
| `PUT /api/usuarios/{id}` | Si | No | No |
| `PUT /api/usuarios/{id}/rol` | Si | No | No |
| `DELETE /api/usuarios/{id}` | Si | No | No |
| `POST /api/clientes` | Si | Si | Si |
| `GET /api/clientes` | Si | Si | Si |
| `GET /api/clientes/{id}` | Si | Si | Si |
| `PUT /api/clientes/{id}` | Si | Si | Si |
| `POST /api/trabajos` | Si | Si | Si |
| `GET /api/trabajos` | Si | Si | Si |
| `GET /api/trabajos/{id}` | Si | Si | Si |
| `GET /api/trabajos/pendientes` | Si | Si | Si |
| `PUT /api/trabajos/{id}/estado` | Si | Si | No |
| `PUT /api/trabajos/{id}/finalizar` | Si | Si | No |
| `PUT /api/trabajos/{id}/cancelar` | Si | Si | No |
| `POST /api/costos` | Si | Si | Si |
| `PUT /api/costos/{id}/estimado` | Si | Si | Si |
| `PUT /api/costos/{id}/final` | Si | Si | No |
| `GET /api/metricas/mis-metricas` | Si | Si | Si |
| `GET /api/metricas/usuario/{usuarioId}` | Si | No | No |
| `GET /api/feriados/{anio}` | Si | Si | Si |

## Creacion de administradores

No existe un endpoint publico para crear administradores. Se pueden crear uno o varios administradores de forma controlada desde una terminal con:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project .\ServiControl.Presentation\ServiControl.Presentation.csproj -- create-admin
```

El comando solicita nombre, email y password. La password se muestra oculta, se transforma a BCrypt mediante `IPasswordHasher` y nunca se guarda en texto plano. El comando finaliza sin levantar la API y puede ejecutarse nuevamente para crear otros administradores. El email debe ser unico.

El endpoint `POST /api/auth/register` requiere un JWT con rol `Admin`. Un administrador puede usarlo para crear otros usuarios y asignarles un rol. Los endpoints de `api/usuarios` permiten consultar, actualizar, cambiar roles y eliminar usuarios.

Todos los administradores actuales tienen alcance global. ServiControl todavia no separa informacion por empresa; para eso se requiere incorporar una entidad Empresa y asociar usuarios y datos operativos a ella.

## Relacion Tecnico-Asistente

Para esta etapa del TIF no se agrego una entidad Empresa, Organizacion ni Equipo. La relacion de trabajo entre un Tecnico y sus Asistentes se resuelve con el campo nullable `IdUsuarioResponsable` en la entidad `Usuario`.

Reglas implementadas:

- `Admin`: `IdUsuarioResponsable` queda en `null`.
- `Technician`: `IdUsuarioResponsable` queda en `null`.
- `Assistant`: debe tener `IdUsuarioResponsable` con el id de un usuario existente con rol `Technician`.

El frontend no decide sobre que trabajos opera un usuario. El backend resuelve el usuario operativo desde el JWT y la base de datos:

```txt
Usuario autenticado Technician -> usuario operativo = su propio Id
Usuario autenticado Assistant  -> usuario operativo = IdUsuarioResponsable
Usuario autenticado Admin      -> acceso global segun permisos
```

Esto significa que un Asistente ve y opera sobre los trabajos de su Tecnico responsable, pero no puede acceder a trabajos de otro Tecnico. Si un Asistente crea un trabajo, el registro queda guardado con `UsuarioId` del Tecnico responsable.

La resolucion vive en `OperationalUserResolver`, dentro de Application. Los servicios de Trabajos, Costos y Metricas usan ese resolver para no confiar en un `UsuarioId` enviado desde el cliente.

## Respuestas HTTP

- Sin JWT o con token invalido: `401 Unauthorized`.
- Con JWT valido pero sin el rol requerido: `403 Forbidden`.

Swagger mantiene el boton **Authorize**. El token debe enviarse con el formato `Bearer {token}`.

Cuando un administrador cambia el rol de un usuario, sus JWT anteriores son rechazados. El usuario debe iniciar sesion nuevamente para recibir un token con el rol actualizado.

La eliminacion se rechaza con `409 Conflict` cuando el usuario tiene trabajos o metricas relacionados, para preservar la integridad referencial.
