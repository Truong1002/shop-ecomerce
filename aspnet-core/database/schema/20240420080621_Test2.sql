BEGIN TRANSACTION;
GO

ALTER TABLE [AppProducts] ADD [StockQuantity] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240420080621_Test2', N'8.0.0');
GO

COMMIT;
GO

