
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/18/2017 19:38:34
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
    [PhraseId] int IDENTITY(1,1) NOT NULL,
    [OriginalMessage] nvarchar(max)  NOT NULL,
    [CorrectedMessage] nvarchar(max)  NULL,
    [Amount] decimal(18,0)  NULL,
    [WasRecognized] bit  NOT NULL,
    [MeasureUnitFId] int  NULL,
    [ActionFId] int  NULL,
    [AdditionalTextFId] int  NULL,
    [Date] nchar(18)  NULL,
    [Time] nchar(25)  NULL
);
GO

-- Creating table 'MeasureUnits'
CREATE TABLE [dbo].[MeasureUnits] (
    [MeasureUnitId] int IDENTITY(1,1) NOT NULL,
    [Text] nchar(25)  NOT NULL
);
GO

-- Creating table 'Actions'
CREATE TABLE [dbo].[Actions] (
    [ActionId] int IDENTITY(1,1) NOT NULL,
    [Text] nchar(30)  NOT NULL
);
GO

-- Creating table 'AdditionalTexts'
CREATE TABLE [dbo].[AdditionalTexts] (
    [AdditionalTextId] int IDENTITY(1,1) NOT NULL,
    [Text] nchar(50)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [PhraseId] in table 'Phrases'
ALTER TABLE [dbo].[Phrases]
ADD CONSTRAINT [PK_Phrases]
    PRIMARY KEY CLUSTERED ([PhraseId] ASC);
GO

-- Creating primary key on [MeasureUnitId] in table 'MeasureUnits'
ALTER TABLE [dbo].[MeasureUnits]
ADD CONSTRAINT [PK_MeasureUnits]
    PRIMARY KEY CLUSTERED ([MeasureUnitId] ASC);
GO

-- Creating primary key on [ActionId] in table 'Actions'
ALTER TABLE [dbo].[Actions]
ADD CONSTRAINT [PK_Actions]
    PRIMARY KEY CLUSTERED ([ActionId] ASC);
GO

-- Creating primary key on [AdditionalTextId] in table 'AdditionalTexts'
ALTER TABLE [dbo].[AdditionalTexts]
ADD CONSTRAINT [PK_AdditionalTexts]
    PRIMARY KEY CLUSTERED ([AdditionalTextId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [MeasureUnitFId] in table 'Phrases'
ALTER TABLE [dbo].[Phrases]
ADD CONSTRAINT [FK_PhraseCanHaveMeasureUnit]
    FOREIGN KEY ([MeasureUnitFId])
    REFERENCES [dbo].[MeasureUnits]
        ([MeasureUnitId])
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
        ([ActionId])
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
        ([AdditionalTextId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PhraseCanHaveAdditionalText'
CREATE INDEX [IX_FK_PhraseCanHaveAdditionalText]
ON [dbo].[Phrases]
    ([AdditionalTextFId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------