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
| `GET /api/metricas` | Si | Si | No |
| `GET /api/feriados/{anio}` | Si | Si | Si |

## Primer administrador

No existe un endpoint publico para crear administradores. El primer administrador debe crearse de forma controlada en desarrollo o directamente en la base de datos, guardando siempre un hash BCrypt valido para la password.

El endpoint `POST /api/auth/register` requiere un JWT con rol `Admin`. Un administrador puede usarlo para crear otros usuarios y asignarles un rol. Los endpoints de `api/usuarios` permiten consultar, actualizar, cambiar roles y eliminar usuarios.

## Respuestas HTTP

- Sin JWT o con token invalido: `401 Unauthorized`.
- Con JWT valido pero sin el rol requerido: `403 Forbidden`.

Swagger mantiene el boton **Authorize**. El token debe enviarse con el formato `Bearer {token}`.

La eliminacion se rechaza con `409 Conflict` cuando el usuario tiene trabajos o metricas relacionados, para preservar la integridad referencial.
