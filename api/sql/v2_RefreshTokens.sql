START TRANSACTION;
CREATE TABLE "RefreshTokens" (
    "Id" text NOT NULL,
    "UserId" text NOT NULL,
    "TokenHash" text NOT NULL,
    "ExpiresAtUtc" timestamp with time zone NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "RevokedAtUtc" timestamp with time zone,
    "ReplacedByTokenId" text,
    "ParentTokenId" text,
    CONSTRAINT "PK_RefreshTokens" PRIMARY KEY ("Id")
);

CREATE UNIQUE INDEX "IX_RefreshTokens_TokenHash" ON "RefreshTokens" ("TokenHash");

CREATE INDEX "IX_RefreshTokens_UserId_RevokedAtUtc_ExpiresAtUtc" ON "RefreshTokens" ("UserId", "RevokedAtUtc", "ExpiresAtUtc");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260520142629_RefreshTokens', '10.0.8');

COMMIT;

