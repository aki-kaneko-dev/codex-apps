# FoodCalendar

Azure App Service (Windows) 向けの検証用 ASP.NET Core MVC アプリです。Entra ID でサインインしたユーザーが、その日食べたものと量を Azure SQL Database に登録し、当週分のデータを確認できます。

## 主な機能

- `Microsoft.Identity.Web` による Entra ID サインイン
- `DefaultAzureCredential` + マネージド ID による Azure SQL Database 接続
- 食事登録フォームと当週一覧を同一画面に表示
- App Service カスタムドメイン `hr.dev1contoso.akkaneko.net` を想定した設定例

## セットアップ

1. `appsettings.json` または App Service のアプリ設定に以下を設定します。
   - `AzureAd__TenantId`
   - `AzureAd__ClientId`
   - `ConnectionStrings__SqlServer`
2. App Service のシステム割り当てマネージド ID を有効化します。
3. Azure SQL Database 側で `dbo.FoodEntries` テーブルを作成し、Web App の MI に `db_datareader` / `db_datawriter` を付与します。
4. Redirect URI に `https://hr.dev1contoso.akkaneko.net/signin-oidc` を追加します。

## SQL スキーマ

```sql
CREATE TABLE dbo.FoodEntries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserObjectId NVARCHAR(64) NOT NULL,
    UserPrincipalName NVARCHAR(256) NULL,
    EntryDate DATE NOT NULL,
    FoodName NVARCHAR(200) NOT NULL,
    Amount NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE INDEX IX_FoodEntries_UserObjectId_EntryDate
ON dbo.FoodEntries(UserObjectId, EntryDate);
```
