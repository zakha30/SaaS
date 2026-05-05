# Manual SQL scripts

Apply these **in order** on your SQL Server database **before** deploying the API changes that read/write `PostKind` and `TradeKind`.

No EF migrations are included here — you control execution timing and can adjust statements per environment.

1. `001_joblistings_postkind.sql` — jobs seek vs offer column + optional backfill from legacy `EmploymentType`.
2. `002_classifieditems_tradekind.sql` — classified buy vs sell column.

After running scripts, restart the API.
