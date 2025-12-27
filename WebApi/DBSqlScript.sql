if db_id('Diagnostician') is null
begin
  create database Diagnostician collate SQL_Latin1_General_CP1_CI_AS
end;

use Diagnostician

grant connect on database :: Diagnostician to dbo

grant view any column encryption key definition, view any column master key definition on database :: Diagnostician to [public]

if object_id('Category') is null
begin
  create table Category
  (
    Id tinyint not null constraint Category_PK primary key,
    Title nvarchar(32) not null,
    Description nvarchar(max),
    constraint Category_Title_UIndex unique (Title)
  )
  INSERT INTO dbo.Category (Id, Title)
  VALUES (1, N'ACC'), (2, N'DBO'), (3, N'FRU'), (4, N'GNR'), (5, N'GRS'), (6, N'HSP'),
         (7, N'ICA'), (8, N'INV'), (9, N'NGT'), (10, N'POL'), (11, N'SLE');
end;

if object_id('Criteria') is null
begin
  create table Criteria
  (
    Id tinyint not null constraint Criteria_PK primary key,
    Title nvarchar(32) not null,
    Description nvarchar(max),
    constraint Criteria_Title_UIndex unique (Title)
  )
  INSERT INTO Criteria (Id, Title) VALUES (1, N'Value'), (2, N'Schema');
end;

if object_id('Test') is null
begin
  create table Test
  (
    Id int constraint Test_PK primary key identity (1, 1),
    Title nvarchar(256) not null,
    Auth nvarchar(max),
    InsertedAt datetime default getdate() not null,
    constraint Test_Title_UIndex unique (Title)
  )
end;

if object_id('Scenario') is null
begin
  create table Scenario
  (
    Id int constraint Scenario_PK primary key identity (1, 1),
    CategoryId tinyint not null,
    Title nvarchar(256) not null,
    Description nvarchar(max),
    InsertedAt datetime default getdate() not null,
    CONSTRAINT [FK_Scenario_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category]([Id]),
    CONSTRAINT Scenario_Title_UIndex unique (Title)

  )
end;

if object_id('Request') is null
begin
  create table Request
  (
    Id int CONSTRAINT Request_PK primary key identity (1, 1),
    ScenarioId int not null,
    Method varchar(16) not null,
    [Target] nvarchar(512) not null,
    Header nvarchar(max),
    Payload nvarchar(max),
    InsertedAt datetime default getdate() not null,
    CONSTRAINT [FK_Request_Scenario] FOREIGN KEY ([ScenarioId]) REFERENCES [dbo].[Scenario]([Id]),
    CONSTRAINT Request_ScenarioId_Method_Target_UIndex unique (ScenarioId, Method, [Target])
  )
end;

if object_id('Expectation') is null
begin
  create table Expectation
  (
    Id int constraint Expectation_PK primary key identity (1, 1),
    CriteriaId tinyint not null,
    RequestId int not null,
    Status integer not null,
    Response nvarchar(max),
    InsertedAt datetime default getdate() not null,
    CONSTRAINT [FK_Expectation_Criteria] FOREIGN KEY ([CriteriaId]) REFERENCES [dbo].[Criteria]([Id]),
    CONSTRAINT [FK_Expectation_Request] FOREIGN KEY ([RequestId]) REFERENCES [dbo].[Request]([Id]),
    CONSTRAINT Expectation_CriteriaId_RequestId_Status_UIndex unique (CriteriaId, RequestId, [Status])
  )
end;

if object_id('Run') is null
begin
  create table Run
  (
    Id int constraint Run_PK primary key identity (1,1),
    ExpectationId int not null,
    ReceivedStatus integer not null,
    ReceivedResponse nvarchar(max),
    Duration bigint not null,
    Accepted bit not null,
    InsertedAt datetime default getdate() not null,
    CONSTRAINT [FK_Run_Expectation] FOREIGN KEY ([ExpectationId]) REFERENCES [dbo].[Expectation]([Id])
  )
end;

if object_id('TestXExpectation') is null
begin
  create table TestXExpectation
  (
    TestId int not null,
    ExpectationId int not null,
    constraint TestXExpectation_PK primary key (TestId, ExpectationId),
    CONSTRAINT [FK_TestXExpectation_Test] FOREIGN KEY ([TestId]) REFERENCES [dbo].[Test]([Id]),
    CONSTRAINT [FK_TestXExpectation_Expectation] FOREIGN KEY ([ExpectationId]) REFERENCES [dbo].[Expectation]([Id])
  )
end;
