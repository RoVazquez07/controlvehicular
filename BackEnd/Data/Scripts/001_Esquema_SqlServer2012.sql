IF OBJECT_ID(N'dbo.VehiculoBitacora', N'U') IS NOT NULL
    DROP TABLE dbo.VehiculoBitacora;
GO

IF OBJECT_ID(N'dbo.Vehiculos', N'U') IS NOT NULL
    DROP TABLE dbo.Vehiculos;
GO

CREATE TABLE dbo.Vehiculos
(
    Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    Placa NVARCHAR(20) NOT NULL,
    Marca NVARCHAR(80) NOT NULL,
    Modelo NVARCHAR(80) NOT NULL,
    Anio INT NOT NULL,
    Estado NVARCHAR(20) NOT NULL,
    FechaRegistroUtc DATETIME2(0) NOT NULL,
    RegistradoPor NVARCHAR(64) NOT NULL,
    CONSTRAINT UQ_Vehiculos_Placa UNIQUE (Placa)
);
GO

CREATE TABLE dbo.VehiculoBitacora
(
    Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    VehiculoId INT NOT NULL,
    Accion NVARCHAR(80) NOT NULL,
    FechaUtc DATETIME2(0) NOT NULL,
    CONSTRAINT FK_VehiculoBitacora_Vehiculos FOREIGN KEY (VehiculoId) REFERENCES dbo.Vehiculos (Id)
);
GO

CREATE NONCLUSTERED INDEX IX_Vehiculos_Placa ON dbo.Vehiculos (Placa);
CREATE NONCLUSTERED INDEX IX_Vehiculos_RegistradoPor ON dbo.Vehiculos (RegistradoPor);
GO

IF OBJECT_ID(N'dbo.Vehiculo_ObtenerPorId', N'P') IS NOT NULL
    DROP PROCEDURE dbo.Vehiculo_ObtenerPorId;
GO

CREATE PROCEDURE dbo.Vehiculo_ObtenerPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Placa, Marca, Modelo, Anio, Estado, FechaRegistroUtc, RegistradoPor
    FROM dbo.Vehiculos
    WHERE Id = @Id;
END
GO

IF OBJECT_ID(N'dbo.Vehiculo_CambiarEstado', N'P') IS NOT NULL
    DROP PROCEDURE dbo.Vehiculo_CambiarEstado;
GO

CREATE PROCEDURE dbo.Vehiculo_CambiarEstado
    @Id INT,
    @Estado NVARCHAR(20),
    @FilasAfectadas INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.Vehiculos
        SET Estado = @Estado
        WHERE Id = @Id;

        SET @FilasAfectadas = @@ROWCOUNT;

        IF @FilasAfectadas = 1
        BEGIN
            INSERT INTO dbo.VehiculoBitacora (VehiculoId, Accion, FechaUtc)
            VALUES (@Id, N'CambioEstado', SYSUTCDATETIME());
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
GO
