# Documentación de Base de Datos - CashFlow Control AI

## Diagrama ER / Entidades Principales

```text
Flow_tblEmpresas
    │
    ├──────── Flow_tblPuntosVenta
    │              │
    │              └──────── Flow_tblUsuarios
    │
    ├──────── Flow_tblConsignaciones
    │              │
    │              ├──────── Flow_tblArchivosConsignacion
    │              └──────── Flow_tblResultadosOcr
    │
    └──────── (Notificaciones/Otros en el futuro)

Catálogos:
- Flow_tblBancos
- Flow_tblRoles
- Flow_tblEstadosConsignacion
```

## Tablas

### `Flow_tblEmpresas`
- `Id` (BIGSERIAL) - PK
- `Nombre`, `Nit`, `Email`, `Telefono`, `Direccion`, `UrlLogo`, `Activo`, `FechaCreacion`

### `Flow_tblPuntosVenta` (Sucursales/Tiendas)
- `Id` (BIGSERIAL) - PK
- `EmpresaId` (BIGINT) - FK
- `Codigo`, `Nombre`, `Ciudad`, `Direccion`, `NombreGerente`, `TelefonoGerente`, `Activo`

### `Flow_tblRoles`
- `Id` (SMALLSERIAL) - PK
- `Nombre` (VARCHAR) - Ej. Administrador, Gerente Sucursal, Cajero

### `Flow_tblUsuarios`
- `Id` (BIGSERIAL) - PK
- `EmpresaId` (BIGINT) - FK
- `PuntoVentaId` (BIGINT) - FK (Nulo si es Admin de empresa)
- `RolId` (SMALLINT) - FK
- `NombreCompleto`, `Email`, `PasswordHash`, `Activo`

### `Flow_tblBancos`
- `Id` (SMALLSERIAL) - PK
- `Nombre`, `Codigo`

### `Flow_tblEstadosConsignacion`
- `Id` (SMALLSERIAL) - PK
- `Nombre` (VARCHAR) - Pendiente, Validada, Discrepancia, Error IA

### `Flow_tblConsignaciones`
- `Id` (BIGSERIAL) - PK
- `EmpresaId`, `PuntoVentaId`, `BancoId`, `EstadoId` - FKs
- `NumeroReferencia`, `MontoDeclarado`, `MontoDetectado`, `FechaConsignacion`, `HoraConsignacion`, `Observaciones`
- `CreadoPor`, `ValidadoPor` - FKs a Usuarios
- `FechaValidacion`, `FechaCreacion`

### `Flow_tblArchivosConsignacion`
- `Id` (BIGSERIAL) - PK
- `ConsignacionId` (BIGINT) - FK
- `NombreArchivo`, `UrlArchivo`, `TipoArchivo`, `TamanoArchivo`, `FechaSubida`

### `Flow_tblResultadosOcr`
- `Id` (BIGSERIAL) - PK
- `ConsignacionId` (BIGINT) - FK (Único)
- `BancoDetectado`, `ReferenciaDetectada`, `MontoDetectado`, `FechaDetectada`
- `Confianza` (NUMERIC), `TextoCrudo` (TEXT), `FechaProcesamiento`

## Mapeos de Consulta (Dapper SQL)

Utilizamos Dapper para ejecutar consultas SQL parametrizadas directas, realizando el mapeo de nombres de columna de español a inglés a través de alias SQL:

### Empresa (`Flow_tblEmpresas`)
```sql
SELECT 
    Id, 
    Nombre AS Name, 
    Nit, 
    Email, 
    Telefono AS Phone, 
    Direccion AS Address, 
    UrlLogo AS LogoUrl, 
    Activo AS IsActive, 
    FechaCreacion AS CreatedAt 
FROM Flow_tblEmpresas
```

### Punto de Venta (`Flow_tblPuntosVenta`)
```sql
SELECT 
    Id, 
    EmpresaId AS CompanyId, 
    Codigo AS Code, 
    Nombre AS Name, 
    Ciudad AS City, 
    Direccion AS Address, 
    NombreGerente AS ManagerName, 
    TelefonoGerente AS ManagerPhone, 
    Activo AS IsActive, 
    FechaCreacion AS CreatedAt 
FROM Flow_tblPuntosVenta
```

### Banco (`Flow_tblBancos`)
```sql
SELECT 
    Id, 
    Nombre AS Name, 
    Codigo AS Code 
FROM Flow_tblBancos
```

### Rol (`Flow_tblRoles`)
```sql
SELECT 
    Id, 
    Nombre AS Name 
FROM Flow_tblRoles
```

### Usuario (`Flow_tblUsuarios`)
```sql
SELECT 
    u.Id, 
    u.EmpresaId AS CompanyId, 
    e.Nombre AS CompanyName, 
    u.PuntoVentaId AS StoreId, 
    COALESCE(p.Nombre, '') AS StoreName, 
    u.RolId AS RoleId, 
    r.Nombre AS RoleName, 
    u.NombreCompleto AS FullName, 
    u.Email, 
    u.Activo AS IsActive, 
    u.FechaCreacion AS CreatedAt 
FROM Flow_tblUsuarios u
INNER JOIN Flow_tblEmpresas e ON u.EmpresaId = e.Id
LEFT JOIN Flow_tblPuntosVenta p ON u.PuntoVentaId = p.Id
INNER JOIN Flow_tblRoles r ON u.RolId = r.Id
```

### Consignación Completa (`Flow_tblConsignations`)
```sql
SELECT 
    c.Id, c.CompanyId, comp.Nombre AS CompanyName, c.StoreId, s.Nombre AS StoreName,
    c.BankId, b.Nombre AS BankName, c.StatusId,
    CASE WHEN c.StatusId = 1 THEN 'Pendiente' WHEN c.StatusId = 2 THEN 'Validada' ELSE 'Discrepancia' END AS StatusName,
    c.ReferenceNumber, c.DeclaredAmount, c.DetectedAmount, c.ConsignationDate, c.ConsignationTime,
    c.Notes, u1.NombreCompleto AS CreatedByName, u2.NombreCompleto AS ValidatedByName,
    c.ValidationDate, c.CreatedAt,
    f.FileUrl,
    o.Id, o.ConsignationId, o.DetectedBank, o.DetectedReference, o.DetectedAmount, 
    o.DetectedDate, o.Confidence, o.RawText, o.ProcessedAt
FROM Flow_tblConsignations c
INNER JOIN Flow_tblEmpresas comp ON c.CompanyId = comp.Id
INNER JOIN Flow_tblPuntosVenta s ON c.StoreId = s.Id
INNER JOIN Flow_tblBancos b ON c.BankId = b.Id
LEFT JOIN Flow_tblUsuarios u1 ON c.CreatedBy = u1.Id
LEFT JOIN Flow_tblUsuarios u2 ON c.ValidatedBy = u2.Id
LEFT JOIN Flow_tblConsignationFiles f ON c.Id = f.ConsignationId
LEFT JOIN Flow_tblOcrResults o ON c.Id = o.ConsignationId
WHERE c.StatusId = @StatusId
ORDER BY c.CreatedAt ASC
```
