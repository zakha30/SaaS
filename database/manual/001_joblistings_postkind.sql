/*
  Manual DDL: JobListings.PostKind (SeekingWork | OfferingJob).
  Run against your TransHub / SaaS database.

  Backfill: copies legacy seek/offer values that were stored in EmploymentType,
  then clears EmploymentType for those rows so that column can be reused for
  optional labels like "Full-time", "Contract", etc.
*/

IF COL_LENGTH(N'dbo.JobListings', N'PostKind') IS NULL
BEGIN
    ALTER TABLE dbo.JobListings
        ADD PostKind NVARCHAR(32) NOT NULL
            CONSTRAINT DF_JobListings_PostKind DEFAULT (N'OfferingJob');

    -- Migrate rows that used EmploymentType only as seek/offer marker
    UPDATE dbo.JobListings
    SET PostKind = EmploymentType
    WHERE EmploymentType IN (N'SeekingWork', N'OfferingJob');

    UPDATE dbo.JobListings
    SET EmploymentType = N''
    WHERE EmploymentType IN (N'SeekingWork', N'OfferingJob');
END
GO
