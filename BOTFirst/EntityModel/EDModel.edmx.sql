
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/23/2017 21:53:03
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
    [MeasureUnitFId] int  NULL,
    [ActionFId] int  NULL,
    [AdditionalTextFId] int  NULL
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
    [Text] nchar(30)  NOT NULL
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
    [PhoneNumber] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Messangers'
CREATE TABLE [dbo].[Messangers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'MessengerAndUserInfo'
CREATE TABLE [dbo].[MessengerAndUserInfo] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserIdentifier] nvarchar(max)  NOT NULL,
    [UserIdFK] int  NOT NULL,
    [Messenger_Id] int  NOT NULL,
    [User_Id] int  NOT NULL
);
GO

-- Creating table 'UserAchievements'
CREATE TABLE [dbo].[UserAchievements] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FullName] nvarchar(max)  NOT NULL,
    [ShortName] nvarchar(max)  NOT NULL,
    [DateTime] nvarchar(max)  NOT NULL,
    [PhraseId] int  NOT NULL,
    [UserIdPK] int  NOT NULL,
    [FullAchievementId] int  NOT NULL,
    [ShortAchievementId] int  NULL
);
GO

-- Creating table 'FullAchievements'
CREATE TABLE [dbo].[FullAchievements] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ShortAchievementSet'
CREATE TABLE [dbo].[ShortAchievementSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
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

-- Creating primary key on [Id] in table 'Messangers'
ALTER TABLE [dbo].[Messangers]
ADD CONSTRAINT [PK_Messangers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MessengerAndUserInfo'
ALTER TABLE [dbo].[MessengerAndUserInfo]
ADD CONSTRAINT [PK_MessengerAndUserInfo]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserAchievements'
ALTER TABLE [dbo].[UserAchievements]
ADD CONSTRAINT [PK_UserAchievements]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FullAchievements'
ALTER TABLE [dbo].[FullAchievements]
ADD CONSTRAINT [PK_FullAchievements]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ShortAchievementSet'
ALTER TABLE [dbo].[ShortAchievementSet]
ADD CONSTRAINT [PK_ShortAchievementSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [MeasureUnitFId] in table 'Phrases'
ALTER TABLE [dbo].[Phrases]
ADD CONSTRAINT [FK_PhraseCanHaveMeasureUnit]
    FOREIGN KEY ([MeasureUnitFId])
    REFERENCES [dbo].[MeasureUnits]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PhraseCanHaveMeasureUnit'
CREATE INDEX [IX_FK_PhraseCanHaveMeasureUnit]
ON [dbo].[Phrases]
    ([MeasureUnitFId]);
GO

-- Creating foreign key on [ActionFId] in table 'Phrases'
ALTER TABLE [dbo].[Phrases]
ADD CONSTRAINT [FK_PhraseCanHaveAction]
    FOREIGN KEY ([ActionFId])
    REFERENCES [dbo].[Actions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PhraseCanHaveAction'
CREATE INDEX [IX_FK_PhraseCanHaveAction]
ON [dbo].[Phrases]
    ([ActionFId]);
GO

-- Creating foreign key on [AdditionalTextFId] in table 'Phrases'
ALTER TABLE [dbo].[Phrases]
ADD CONSTRAINT [FK_PhraseCanHaveAdditionalText]
    FOREIGN KEY ([AdditionalTextFId])
    REFERENCES [dbo].[AdditionalTexts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PhraseCanHaveAdditionalText'
CREATE INDEX [IX_FK_PhraseCanHaveAdditionalText]
ON [dbo].[Phrases]
    ([AdditionalTextFId]);
GO

-- Creating foreign key on [Messenger_Id] in table 'MessengerAndUserInfo'
ALTER TABLE [dbo].[MessengerAndUserInfo]
ADD CONSTRAINT [FK_OneUserIdentifierCanCorrespondOneMessenger]
    FOREIGN KEY ([Messenger_Id])
    REFERENCES [dbo].[Messangers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OneUserIdentifierCanCorrespondOneMessenger'
CREATE INDEX [IX_FK_OneUserIdentifierCanCorrespondOneMessenger]
ON [dbo].[MessengerAndUserInfo]
    ([Messenger_Id]);
GO

-- Creating foreign key on [User_Id] in table 'MessengerAndUserInfo'
ALTER TABLE [dbo].[MessengerAndUserInfo]
ADD CONSTRAINT [FK_UserCanHaveOneOrManyMessangers]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserCanHaveOneOrManyMessangers'
CREATE INDEX [IX_FK_UserCanHaveOneOrManyMessangers]
ON [dbo].[MessengerAndUserInfo]
    ([User_Id]);
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

-- Creating foreign key on [UserIdPK] in table 'UserAchievements'
ALTER TABLE [dbo].[UserAchievements]
ADD CONSTRAINT [FK_AchievementBelongsToUser]
    FOREIGN KEY ([UserIdPK])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AchievementBelongsToUser'
CREATE INDEX [IX_FK_AchievementBelongsToUser]
ON [dbo].[UserAchievements]
    ([UserIdPK]);
GO

-- Creating foreign key on [FullAchievementId] in table 'UserAchievements'
ALTER TABLE [dbo].[UserAchievements]
ADD CONSTRAINT [FK_AchievementCanHaveFullName]
    FOREIGN KEY ([FullAchievementId])
    REFERENCES [dbo].[FullAchievements]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AchievementCanHaveFullName'
CREATE INDEX [IX_FK_AchievementCanHaveFullName]
ON [dbo].[UserAchievements]
    ([FullAchievementId]);
GO

-- Creating foreign key on [ShortAchievementId] in table 'UserAchievements'
ALTER TABLE [dbo].[UserAchievements]
ADD CONSTRAINT [FK_AchievementCanHaveShortName]
    FOREIGN KEY ([ShortAchievementId])
    REFERENCES [dbo].[ShortAchievementSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AchievementCanHaveShortName'
CREATE INDEX [IX_FK_AchievementCanHaveShortName]
ON [dbo].[UserAchievements]
    ([ShortAchievementId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------