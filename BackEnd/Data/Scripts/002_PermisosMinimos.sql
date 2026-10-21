-- Cuenta de aplicacion con privilegios minimos (ejecutar como administrador).
-- No almacene contrasenas en este archivo. Cree el login fuera del repositorio:
-- CREATE LOGIN cv_app WITH PASSWORD = N'<secreto-local>', CHECK_POLICY = ON;

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'cv_app')
BEGIN
    CREATE USER cv_app FOR LOGIN cv_app;
END
GO

GRANT SELECT, INSERT, UPDATE ON dbo.Vehiculos TO cv_app;
GRANT SELECT, INSERT ON dbo.VehiculoBitacora TO cv_app;
GRANT EXECUTE ON dbo.Vehiculo_ObtenerPorId TO cv_app;
GRANT EXECUTE ON dbo.Vehiculo_CambiarEstado TO cv_app;
GO
