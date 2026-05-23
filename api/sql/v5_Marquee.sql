START TRANSACTION;
CREATE TABLE "Marquees" (
    "Id" text NOT NULL,
    "CharacterId" text,
    "ShipId" text,
    "IntroductionDate" date NOT NULL,
    "MarqueeEventDate" date NOT NULL,
    "ShipmentDate" date NOT NULL,
    "FarmDate" date NOT NULL,
    "AccelerationDate" date,
    CONSTRAINT "PK_Marquees" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_Marquee_Acceleration" CHECK ((
    "ShipId" IS NULL
)
OR
(
    "AccelerationDate" IS NULL
)),
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
    CONSTRAINT "FK_Marquees_Characters_CharacterId" FOREIGN KEY ("CharacterId") REFERENCES "Characters" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Marquees_Ships_ShipId" FOREIGN KEY ("ShipId") REFERENCES "Ships" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_Marquees_CharacterId" ON "Marquees" ("CharacterId");

CREATE UNIQUE INDEX "IX_Marquees_ShipId" ON "Marquees" ("ShipId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260523200427_Marquee', '10.0.8');

COMMIT;

