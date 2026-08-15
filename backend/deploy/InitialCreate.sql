IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    CREATE TABLE [Hotels] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [Tier] int NOT NULL,
        [ContactEmail] nvarchar(max) NULL,
        [ContactPhone] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_Hotels] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    CREATE TABLE [Leads] (
        [Id] uniqueidentifier NOT NULL,
        [HotelName] nvarchar(200) NOT NULL,
        [ContactName] nvarchar(200) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [Phone] nvarchar(max) NULL,
        [Message] nvarchar(max) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_Leads] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    CREATE TABLE [Activities] (
        [Id] uniqueidentifier NOT NULL,
        [HotelId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Category] nvarchar(100) NOT NULL,
        [Price] decimal(10,2) NOT NULL,
        [DurationMinutes] int NOT NULL,
        [ImageUrl] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_Activities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Activities_Hotels_HotelId] FOREIGN KEY ([HotelId]) REFERENCES [Hotels] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    CREATE TABLE [HotelStaffUsers] (
        [Id] uniqueidentifier NOT NULL,
        [HotelId] uniqueidentifier NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [Role] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_HotelStaffUsers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HotelStaffUsers_Hotels_HotelId] FOREIGN KEY ([HotelId]) REFERENCES [Hotels] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    CREATE TABLE [ActivitySlots] (
        [Id] uniqueidentifier NOT NULL,
        [ActivityId] uniqueidentifier NOT NULL,
        [StartTime] datetimeoffset NOT NULL,
        [Capacity] int NOT NULL,
        [BookedCount] int NOT NULL,
        CONSTRAINT [PK_ActivitySlots] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ActivitySlots_Activities_ActivityId] FOREIGN KEY ([ActivityId]) REFERENCES [Activities] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    CREATE TABLE [Reservations] (
        [Id] uniqueidentifier NOT NULL,
        [SlotId] uniqueidentifier NOT NULL,
        [GuestName] nvarchar(200) NOT NULL,
        [RoomNumber] nvarchar(20) NOT NULL,
        [ChargeToRoom] bit NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [CancelledAt] datetimeoffset NULL,
        CONSTRAINT [PK_Reservations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reservations_ActivitySlots_SlotId] FOREIGN KEY ([SlotId]) REFERENCES [ActivitySlots] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Activities_HotelId] ON [Activities] ([HotelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ActivitySlots_ActivityId] ON [ActivitySlots] ([ActivityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Hotels_Code] ON [Hotels] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HotelStaffUsers_Email] ON [HotelStaffUsers] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_HotelStaffUsers_HotelId] ON [HotelStaffUsers] ([HotelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Reservations_SlotId] ON [Reservations] ([SlotId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260815181448_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260815181448_InitialCreate', N'8.0.30');
END;
GO

COMMIT;
GO

