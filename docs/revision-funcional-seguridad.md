# Revision funcional y de seguridad

## Ownership

El claim `IdUsuario` del JWT identifica al usuario autenticado. Application accede a ese valor mediante `ICurrentUserContext`, sin aceptar `UsuarioId` desde el frontend para operaciones propias.

- Admin conserva acceso global.
- Tecnico consulta y modifica trabajos asociados a su propio `UsuarioId`.
- Asistente consulta y modifica trabajos asociados al `UsuarioId` de su Tecnico responsable.
- Los costos se autorizan a traves del propietario del trabajo asociado.
- Las metricas propias usan el usuario operativo resuelto en Application.

## Usuario operativo

Para resolver la colaboracion entre Tecnico y Asistente sin agregar Empresa, se agrego `IdUsuarioResponsable` en `Usuario`.

Reglas:

- Si el usuario es `Admin`, `IdUsuarioResponsable` debe quedar `null`.
- Si el usuario es `Technician`, `IdUsuarioResponsable` debe quedar `null`.
- Si el usuario es `Assistant`, `IdUsuarioResponsable` es obligatorio y debe apuntar a un usuario existente con rol `Technician`.

El servicio `OperationalUserResolver` calcula el usuario operativo:

```txt
Technician -> usa su propio Id
Assistant  -> usa el Id del Technician responsable
Admin      -> conserva acceso global
```

Los servicios de `Trabajos`, `Costos` y `Metricas` usan ese usuario operativo para filtrar y crear datos. Por eso un Asistente puede trabajar sobre los mismos trabajos que su Tecnico responsable, pero no sobre trabajos de otro Tecnico. Si un Asistente crea un trabajo, se guarda con el `UsuarioId` del Tecnico responsable.

La entidad `Cliente` no tiene `UsuarioId`. Sin modificar el DER no es posible asignar ownership fiable a clientes, por lo que conservan el acceso definido por roles. `ClienteId` en un trabajo solo se valida por existencia.

## Estados de trabajo

`TrabajoService` centraliza las transiciones:

```txt
Pendiente -> EnProceso, Finalizado o Cancelado
EnProceso -> Finalizado o Cancelado
Finalizado -> ninguna transicion
Cancelado -> ninguna transicion
```

No se permite regresar a Pendiente ni repetir estados terminales.

## Endpoints afectados

### Trabajos

- `POST /api/trabajos`: ya no recibe `UsuarioId`; usa el usuario operativo.
- `GET /api/trabajos`: Admin ve todos; Tecnico ve los suyos; Asistente ve los de su Tecnico responsable.
- `GET /api/trabajos/{id}`: usuarios comunes solo acceden a trabajos de su usuario operativo.
- `GET /api/trabajos/pendientes`: Admin ve todos; usuarios comunes solo ven pendientes de su usuario operativo.
- Endpoints de estado: validan ownership y transiciones.

No existen actualmente endpoints para obtener trabajos por estado o por cliente.

### Costos

- `POST /api/costos`: valida ownership del trabajo antes de crear o actualizar el estimado.
- `PUT /api/costos/{id}/estimado`: el `id` representa `TrabajoId` y valida ownership.
- `PUT /api/costos/{id}/final`: valida ownership; Asistente sigue bloqueado por rol.

Un trabajo puede no tener costo. Si no existe, el servicio crea el registro; si existe, actualiza la misma fila.

### Metricas

- `GET /api/metricas/mis-metricas`: Admin, Tecnico y Asistente; usa el usuario operativo.
- `GET /api/metricas/usuario/{usuarioId}`: solo Admin.

Los parametros son fechas simples:

```txt
periodoInicio=2026-06-01
periodoFin=2026-06-30
```

Application convierte inicio a `00:00:00` y fin a `23:59:59.9999999`. Solo trabajos finalizados aportan ingresos; cancelados no aportan. Se calculan total, finalizados, pendientes y promedio. `GenericRepository.AddAsync` ejecuta `SaveChangesAsync`, por lo que la metrica se persiste.

## Email

Los DTOs de registro, login, creacion administrativa y actualizacion de usuarios usan `[Required]` y `[EmailAddress]`. Los emails opcionales de Cliente usan `[EmailAddress]`. La unicidad de email sigue validandose en Application y SQL Server.

## Cambio de rol

`OnTokenValidated` compara en cada request el rol del JWT con el rol vigente en base de datos. Si el usuario fue eliminado o el rol cambio, el token se rechaza con `401` y debe realizarse un nuevo login. No se utilizan refresh tokens.

## Pruebas en Swagger

1. Reiniciar la API para cargar los cambios y hacer login con cada rol.
2. Crear un trabajo como Tecnico y verificar que Swagger ya no solicite `UsuarioId`.
3. Con otro usuario, intentar consultar o modificar ese trabajo; no debe obtener acceso.
4. Probar `Pendiente -> Cancelado` y luego intentar finalizar o iniciar; debe responder conflicto.
5. Registrar costo estimado propio como Asistente; debe funcionar.
6. Registrar costo final como Asistente; debe responder `403`.
7. Probar `mis-metricas` con fechas `yyyy-MM-dd` y verificar que usa el usuario operativo.
8. Probar la ruta administrativa de metricas con Admin y luego con Tecnico; Tecnico debe recibir `403`.
9. Cambiar el rol de un usuario y reutilizar su token anterior; debe recibir `401`. Luego hacer login nuevamente.
10. Crear un Asistente con `IdUsuarioResponsable` apuntando a un Tecnico y verificar que ve los trabajos de ese Tecnico.
11. Crear un trabajo logueado como Asistente y verificar en SQL Server que `Trabajos.UsuarioId` corresponde al Tecnico responsable.

## Migraciones

Se agrego la migracion `AddUsuarioResponsable`, que incorpora la columna nullable `IdUsuarioResponsable` en `Usuarios`, un indice y una FK contra la misma tabla `Usuarios`. Esta migracion debe aplicarse con `dotnet ef database update` para que SQL Server tenga la nueva relacion.
