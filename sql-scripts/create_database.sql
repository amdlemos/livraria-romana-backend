IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Books] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NULL,
    [OriginalTitle] nvarchar(max) NULL,
    [Author] nvarchar(max) NULL,
    [PublishingCompany] nvarchar(max) NULL,
    [ISBN] nvarchar(max) NULL,
    [PublicationYear] nvarchar(max) NULL,
    [Amount] int NOT NULL,
    CONSTRAINT [PK_Books] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NULL,
    [Password] nvarchar(max) NULL,
    [Hash] nvarchar(max) NULL,
    [Salt] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [Role] nvarchar(max) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200126171823_create_database', N'2.2.0-rtm-35687');

GO

