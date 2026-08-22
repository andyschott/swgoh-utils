START TRANSACTION;
CREATE TABLE "ConquestRewards" (
    "Id" text NOT NULL,
    "CharacterId" text,
    "ShipId" text,
    "RewardPhase" integer NOT NULL,
    "InitialUnlockDate" date NOT NULL,
    "FinalRewardCreateDate" date NOT NULL,
    "ProvingGroundsDate" date NOT NULL,
    CONSTRAINT "PK_ConquestRewards" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_ConquestReward_Entity" CHECK ((
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
    CONSTRAINT "FK_ConquestRewards_Characters_CharacterId" FOREIGN KEY ("CharacterId") REFERENCES "Characters" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ConquestRewards_Ships_ShipId" FOREIGN KEY ("ShipId") REFERENCES "Ships" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_ConquestRewards_CharacterId" ON "ConquestRewards" ("CharacterId");

CREATE UNIQUE INDEX "IX_ConquestRewards_ShipId" ON "ConquestRewards" ("ShipId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260815143811_ConquestRewards', '10.0.10');

COMMIT;

