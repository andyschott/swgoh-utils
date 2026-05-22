START TRANSACTION;
ALTER TABLE "Users" ADD "IsAdmin" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260522123804_AdminUsers', '10.0.8');

COMMIT;

