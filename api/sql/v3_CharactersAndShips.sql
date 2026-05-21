START TRANSACTION;
CREATE TABLE "Characters" (
    "Id" text NOT NULL,
    "IsAccelerated" boolean NOT NULL,
    "Name" text NOT NULL,
    "Locations" integer[] NOT NULL,
    CONSTRAINT "PK_Characters" PRIMARY KEY ("Id")
);

CREATE TABLE "Ships" (
    "Id" text NOT NULL,
    "Name" text NOT NULL,
    "Locations" integer[] NOT NULL,
    CONSTRAINT "PK_Ships" PRIMARY KEY ("Id")
);

CREATE UNIQUE INDEX "IX_Characters_Name" ON "Characters" ("Name");

CREATE UNIQUE INDEX "IX_Ships_Name" ON "Ships" ("Name");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260521142303_CharactersAndShips', '10.0.8');

COMMIT;

