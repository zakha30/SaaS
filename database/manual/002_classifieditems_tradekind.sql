/*
  Manual DDL: ClassifiedItems.TradeKind (Buy | Sell).
  Run against your TransHub / SaaS database.

  Existing rows default to Sell; adjust with custom UPDATEs if needed.
*/

IF COL_LENGTH(N'dbo.ClassifiedItems', N'TradeKind') IS NULL
BEGIN
    ALTER TABLE dbo.ClassifiedItems
        ADD TradeKind NVARCHAR(16) NOT NULL
            CONSTRAINT DF_ClassifiedItems_TradeKind DEFAULT (N'Sell');
END
GO
