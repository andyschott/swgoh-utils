START TRANSACTION;
CREATE TABLE "EarnableShards" (
    "Id" text NOT NULL,
    "UserId" text NOT NULL,
    "CharacterId" text,
    "ShipId" text,
    "Shards" integer NOT NULL,
    "FarmingStatus" integer NOT NULL,
    CONSTRAINT "PK_EarnableShards" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_Marquee_Entity" CHECK ((
    "CharacterId" IS NOT NULL
    AND
    "ShipId" IS NULL
)
OR
(
    "CharacterId" IS NULL
    AND
    "ShipId" IS NOT NULL
)),
    CONSTRAINT "FK_EarnableShards_Characters_CharacterId" FOREIGN KEY ("CharacterId") REFERENCES "Characters" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EarnableShards_Ships_ShipId" FOREIGN KEY ("ShipId") REFERENCES "Ships" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EarnableShards_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_EarnableShards_CharacterId" ON "EarnableShards" ("CharacterId");

CREATE UNIQUE INDEX "IX_EarnableShards_ShipId" ON "EarnableShards" ("ShipId");

CREATE INDEX "IX_EarnableShards_UserId" ON "EarnableShards" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260524224345_EarnableShards', '10.0.8');

COMMIT;

