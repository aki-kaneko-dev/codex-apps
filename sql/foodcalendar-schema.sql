CREATE TABLE dbo.FoodEntries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserObjectId NVARCHAR(64) NOT NULL,
    UserPrincipalName NVARCHAR(256) NULL,
    EntryDate DATE NOT NULL,
    FoodName NVARCHAR(200) NOT NULL,
    Amount NVARCHAR(100) NOT NULL,
    Calories INT NOT NULL,
    TargetCalories INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE INDEX IX_FoodEntries_UserObjectId_EntryDate
ON dbo.FoodEntries(UserObjectId, EntryDate);
GO

CREATE USER [<webapp-name>] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [<webapp-name>];
ALTER ROLE db_datawriter ADD MEMBER [<webapp-name>];
GO

-- 既存テーブルを流用する場合の追加カラム例
-- ALTER TABLE dbo.FoodEntries ADD Calories INT NOT NULL CONSTRAINT DF_FoodEntries_Calories DEFAULT 0;
-- ALTER TABLE dbo.FoodEntries ADD TargetCalories INT NOT NULL CONSTRAINT DF_FoodEntries_TargetCalories DEFAULT 0;
