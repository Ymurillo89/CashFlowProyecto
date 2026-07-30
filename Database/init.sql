CREATE TABLE IF NOT EXISTS Flow_tblEmpresas (
    Id              BIGSERIAL PRIMARY KEY,
    Nombre          VARCHAR(200) NOT NULL,
    Nit             VARCHAR(30),
    Email           VARCHAR(150),
    Telefono        VARCHAR(30),
    Direccion       VARCHAR(250),
    UrlLogo         TEXT,
    Activo          BOOLEAN NOT NULL DEFAULT TRUE,
    FechaCreacion   TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Flow_tblPuntosVenta (
    Id              BIGSERIAL PRIMARY KEY,
    EmpresaId       BIGINT NOT NULL,
    Codigo          VARCHAR(20) NOT NULL,
    Nombre          VARCHAR(150) NOT NULL,
    Ciudad          VARCHAR(100),
    Direccion       VARCHAR(250),
    NombreGerente   VARCHAR(150),
    TelefonoGerente VARCHAR(30),
    Activo          BOOLEAN NOT NULL DEFAULT TRUE,
    FechaCreacion   TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_punto_empresa
        FOREIGN KEY(EmpresaId)
        REFERENCES Flow_tblEmpresas(Id)
);

CREATE TABLE IF NOT EXISTS Flow_tblRoles (
    Id          SMALLSERIAL PRIMARY KEY,
    Nombre      VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Flow_tblUsuarios (
    Id                  BIGSERIAL PRIMARY KEY,
    EmpresaId           BIGINT NOT NULL,
    PuntoVentaId        BIGINT,
    RolId               SMALLINT NOT NULL,

    NombreCompleto      VARCHAR(150) NOT NULL,
    Email               VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash        TEXT NOT NULL,

    Activo              BOOLEAN NOT NULL DEFAULT TRUE,
    FechaCreacion       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_usuario_empresa
        FOREIGN KEY(EmpresaId)
        REFERENCES Flow_tblEmpresas(Id),

    CONSTRAINT fk_usuario_punto
        FOREIGN KEY(PuntoVentaId)
        REFERENCES Flow_tblPuntosVenta(Id),

    CONSTRAINT fk_usuario_rol
        FOREIGN KEY(RolId)
        REFERENCES Flow_tblRoles(Id)
);

CREATE TABLE IF NOT EXISTS Flow_tblBancos (
    Id              SMALLSERIAL PRIMARY KEY,
    Nombre          VARCHAR(100) NOT NULL,
    Codigo          VARCHAR(20)
);

CREATE TABLE IF NOT EXISTS Flow_tblEstadosConsignacion (
    Id          SMALLSERIAL PRIMARY KEY,
    Nombre      VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS Flow_tblConsignaciones (
    Id                      BIGSERIAL PRIMARY KEY,
    EmpresaId               BIGINT NOT NULL,
    PuntoVentaId            BIGINT NOT NULL,
    BancoId                 SMALLINT NOT NULL,
    EstadoId                SMALLINT NOT NULL,
    NumeroReferencia        VARCHAR(80),
    MontoDeclarado          NUMERIC(18,2) NOT NULL,
    MontoDetectado          NUMERIC(18,2),
    FechaConsignacion       DATE NOT NULL,
    HoraConsignacion        TIME,
    Observaciones           TEXT,
    CreadoPor               BIGINT NOT NULL,
    ValidadoPor             BIGINT,
    FechaValidacion         TIMESTAMP,
    FechaCreacion           TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_consig_empresa
        FOREIGN KEY(EmpresaId)
        REFERENCES Flow_tblEmpresas(Id),

    CONSTRAINT fk_consig_punto
        FOREIGN KEY(PuntoVentaId)
        REFERENCES Flow_tblPuntosVenta(Id),

    CONSTRAINT fk_consig_banco
        FOREIGN KEY(BancoId)
        REFERENCES Flow_tblBancos(Id),

    CONSTRAINT fk_consig_estado
        FOREIGN KEY(EstadoId)
        REFERENCES Flow_tblEstadosConsignacion(Id),

    CONSTRAINT fk_consig_usuario_crea
        FOREIGN KEY(CreadoPor)
        REFERENCES Flow_tblUsuarios(Id),

    CONSTRAINT fk_consig_usuario_valida
        FOREIGN KEY(ValidadoPor)
        REFERENCES Flow_tblUsuarios(Id)
);

CREATE TABLE IF NOT EXISTS Flow_tblArchivosConsignacion (
    Id                  BIGSERIAL PRIMARY KEY,
    ConsignacionId      BIGINT NOT NULL,
    NombreArchivo       VARCHAR(200),
    UrlArchivo          TEXT NOT NULL,
    TipoArchivo         VARCHAR(20),
    TamanoArchivo       BIGINT,
    FechaSubida         TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_archivo_consig
        FOREIGN KEY(ConsignacionId)
        REFERENCES Flow_tblConsignaciones(Id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Flow_tblResultadosOcr (
    Id                      BIGSERIAL PRIMARY KEY,
    ConsignacionId          BIGINT NOT NULL UNIQUE,
    BancoDetectado          VARCHAR(100),
    ReferenciaDetectada     VARCHAR(100),
    MontoDetectado          NUMERIC(18,2),
    FechaDetectada          DATE,
    Confianza               NUMERIC(5,2),
    TextoCrudo              TEXT,
    FechaProcesamiento      TIMESTAMP,

    CONSTRAINT fk_ocr_consig
        FOREIGN KEY(ConsignacionId)
        REFERENCES Flow_tblConsignaciones(Id)
        ON DELETE CASCADE
);

-- Datos iniciales básicos para catálogos
INSERT INTO Flow_tblRoles (Nombre) VALUES ('Administrador'), ('Gerente Sucursal'), ('Cajero') ON CONFLICT DO NOTHING;
INSERT INTO Flow_tblEstadosConsignacion (Nombre) VALUES ('Pendiente'), ('Validada'), ('Discrepancia'), ('Error IA') ON CONFLICT DO NOTHING;
INSERT INTO Flow_tblBancos (Nombre, Codigo) VALUES ('Bancolombia', '007'), ('Davivienda', '051'), ('Banco de Bogotá', '001') ON CONFLICT DO NOTHING;
