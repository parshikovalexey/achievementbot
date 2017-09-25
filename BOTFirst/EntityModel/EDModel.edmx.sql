
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/26/2017 04:35:50
-- Generated from EDMX file: C:\!Files\!APF\!ProgrammingAndDevelopment\1Студенческий проект achievementbot\achievementbot\BOTFirst\EntityModel\EDModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [achievementbotdb];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_PhraseCanHaveMeasureUnit]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Phrases] DROP CONSTRAINT [FK_PhraseCanHaveMeasureUnit];
GO
IF OBJECT_ID(N'[dbo].[FK_PhraseCanHaveAction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Phrases] DROP CONSTRAINT [FK_PhraseCanHaveAction];
GO
IF OBJECT_ID(N'[dbo].[FK_PhraseCanHaveAdditionalText]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Phrases] DROP CONSTRAINT [FK_PhraseCanHaveAdditionalText];
GO
IF OBJECT_ID(N'[dbo].[FK_AchievementContainsOnePhrase]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserAchievements] DROP CONSTRAINT [FK_AchievementContainsOnePhrase];
GO
IF OBJECT_ID(N'[dbo].[FK_AchievementBelongsToUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserAchievements] DROP CONSTRAINT [FK_AchievementBelongsToUser];
GO
IF OBJECT_ID(N'[dbo].[FK_UserCanHaveOneOrManyMessangers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserMessengers] DROP CONSTRAINT [FK_UserCanHaveOneOrManyMessangers];
GO
IF OBJECT_ID(N'[dbo].[FK_UserAchievementContainsAchievement]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserAchievements] DROP CONSTRAINT [FK_UserAchievementContainsAchievement];
GO
IF OBJECT_ID(N'[dbo].[FK_OneMessengerCanMatchManyUserMessengers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserMessengers] DROP CONSTRAINT [FK_OneMessengerCanMatchManyUserMessengers];
GO
IF OBJECT_ID(N'[dbo].[FK_ActionCanBeAchievementForm]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Actions] DROP CONSTRAINT [FK_ActionCanBeAchievementForm];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Phrases]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Phrases];
GO
IF OBJECT_ID(N'[dbo].[MeasureUnits]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MeasureUnits];
GO
IF OBJECT_ID(N'[dbo].[Actions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Actions];
GO
IF OBJECT_ID(N'[dbo].[AdditionalTexts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdditionalTexts];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Messengers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Messengers];
GO
IF OBJECT_ID(N'[dbo].[UserMessengers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserMessengers];
GO
IF OBJECT_ID(N'[dbo].[UserAchievements]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserAchievements];
GO
IF OBJECT_ID(N'[dbo].[Achievements]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Achievements];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Phrases'
CREATE TABLE [dbo].[Phrases] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [OriginalMessage] nvarchar(max)  NOT NULL,
    [CorrectedMessage] nvarchar(max)  NULL,
    [Amount] decimal(18,0)  NULL,
    [WasRecognized] bit  NOT NULL,
    [Date] nchar(18)  NULL,
    [Time] nchar(25)  NULL,
    [MeasureUnitId] int  NULL,
    [ActionId] int  NULL,
    [AdditionalTextId] int  NULL
);
GO

-- Creating table 'MeasureUnits'
CREATE TABLE [dbo].[MeasureUnits] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Text] nchar(25)  NOT NULL
);
GO

-- Creating table 'Actions'
CREATE TABLE [dbo].[Actions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Text] nchar(30)  NOT NULL,
    [AchievementId] int  NULL
);
GO

-- Creating table 'AdditionalTexts'
CREATE TABLE [dbo].[AdditionalTexts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Text] nchar(50)  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PhoneNumber] nchar(11)  NULL,
    [Name] nchar(40)  NOT NULL
);
GO

-- Creating table 'Messengers'
CREATE TABLE [dbo].[Messengers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nchar(15)  NOT NULL
);
GO

-- Creating table 'UserMessengers'
CREATE TABLE [dbo].[UserMessengers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MessengerUserIdentifier] nvarchar(max)  NOT NULL,
    [UserId] int  NOT NULL,
    [MessengerId] int  NOT NULL
);
GO

-- Creating table 'UserAchievements'
CREATE TABLE [dbo].[UserAchievements] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DateAndTime] datetime  NOT NULL,
    [PhraseId] int  NOT NULL,
    [UserId] int  NOT NULL,
    [AchievementId] int  NOT NULL
);
GO

-- Creating table 'Achievements'
CREATE TABLE [dbo].[Achievements] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nchar(120)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Phrases'
ALTER TABLE [dbo].[Phrases]
ADD CONSTRAINT [PK_Phrases]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MeasureUnits'
ALTER TABLE [dbo].[MeasureUnits]
ADD CONSTRAINT [PK_MeasureUnits]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Actions'
ALTER TABLE [dbo].[Actions]
ADD CONSTRAINT [PK_Actions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AdditionalTexts'
ALTER TABLE [dbo].[AdditionalTexts]
ADD CONSTRAINT [PK_AdditionalTexts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Messengers'
ALTER TABLE [dbo].[Messengers]
ADD CONSTRAINT [PK_Messengers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserMessengers'
ALTER TABLE [dbo].[UserMessengers]
ADD CONSTRAINT [PK_UserMessengers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserAchievements'
ALTER TABLE [dbo].[UserAchievements]
ADD CONSTRAINT [PK_UserAchievements]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Achievements'
ALTER TABLE [dbo].[Achievements]
ADD CONSTRAINT [PK_Achievements]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [MeasureUnitId] in table 'Phrases'
ALTER TABLE [dbo].[Phrases]
ADD CONSTRAINT [FK_PhraseCanHaveMeasureUnit]
    FOREIGN KEY ([MeasureUnitId])
    REFERENCES [dbo].[MeasureUnits]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PhraseCanHaveMeasureUnit'
CREATE INDEX [IX_FK_PhraseCanHaveMeasureUnit]
ON [dbo].[Phrases]
    ([MeasureUnitId]);
GO

-- Creating foreign key on [ActionId] in table 'Phrases'
ALTER TABLE [dbo].[Phrases]
ADD CONSTRAINT [FK_PhraseCanHaveAction]
    FOREIGN KEY ([ActionId])
    REFERENCES [dbo].[Actions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PhraseCanHaveAction'
CREATE INDEX [IX_FK_PhraseCanHaveAction]
ON [dbo].[Phrases]
    ([ActionId]);
GO

-- Creating foreign key on [AdditionalTextId] in table 'Phrases'
ALTER TABLE [dbo].[Phrases]
ADD CONSTRAINT [FK_PhraseCanHaveAdditionalText]
    FOREIGN KEY ([AdditionalTextId])
    REFERENCES [dbo].[AdditionalTexts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PhraseCanHaveAdditionalText'
CREATE INDEX [IX_FK_PhraseCanHaveAdditionalText]
ON [dbo].[Phrases]
    ([AdditionalTextId]);
GO

-- Creating foreign key on [PhraseId] in table 'UserAchievements'
ALTER TABLE [dbo].[UserAchievements]
ADD CONSTRAINT [FK_AchievementContainsOnePhrase]
    FOREIGN KEY ([PhraseId])
    REFERENCES [dbo].[Phrases]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AchievementContainsOnePhrase'
CREATE INDEX [IX_FK_AchievementContainsOnePhrase]
ON [dbo].[UserAchievements]
    ([PhraseId]);
GO

-- Creating foreign key on [UserId] in table 'UserAchievements'
ALTER TABLE [dbo].[UserAchievements]
ADD CONSTRAINT [FK_AchievementBelongsToUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AchievementBelongsToUser'
CREATE INDEX [IX_FK_AchievementBelongsToUser]
ON [dbo].[UserAchievements]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'UserMessengers'
ALTER TABLE [dbo].[UserMessengers]
ADD CONSTRAINT [FK_UserCanHaveOneOrManyMessangers]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserCanHaveOneOrManyMessangers'
CREATE INDEX [IX_FK_UserCanHaveOneOrManyMessangers]
ON [dbo].[UserMessengers]
    ([UserId]);
GO

-- Creating foreign key on [AchievementId] in table 'UserAchievements'
ALTER TABLE [dbo].[UserAchievements]
ADD CONSTRAINT [FK_UserAchievementContainsAchievement]
    FOREIGN KEY ([AchievementId])
    REFERENCES [dbo].[Achievements]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserAchievementContainsAchievement'
CREATE INDEX [IX_FK_UserAchievementContainsAchievement]
ON [dbo].[UserAchievements]
    ([AchievementId]);
GO

-- Creating foreign key on [MessengerId] in table 'UserMessengers'
ALTER TABLE [dbo].[UserMessengers]
ADD CONSTRAINT [FK_OneMessengerCanMatchManyUserMessengers]
    FOREIGN KEY ([MessengerId])
    REFERENCES [dbo].[Messengers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OneMessengerCanMatchManyUserMessengers'
CREATE INDEX [IX_FK_OneMessengerCanMatchManyUserMessengers]
ON [dbo].[UserMessengers]
    ([MessengerId]);
GO

-- Creating foreign key on [AchievementId] in table 'Actions'
ALTER TABLE [dbo].[Actions]
ADD CONSTRAINT [FK_AchievementAction]
    FOREIGN KEY ([AchievementId])
    REFERENCES [dbo].[Achievements]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AchievementAction'
CREATE INDEX [IX_FK_AchievementAction]
ON [dbo].[Actions]
    ([AchievementId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------