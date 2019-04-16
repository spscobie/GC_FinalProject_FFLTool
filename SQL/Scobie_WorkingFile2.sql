/* Dummy fail safe if you F5 from the top of the file */
USE JunkDBJunkDB

/**************************/
/* AZURE FFLTool DB!!!!!! */
/**************************/

DROP TABLE tblUsers;

USE FFLTool;
CREATE TABLE tblUsers (
	UserId INT IDENTITY(100000, 1) NOT NULL,
	FirstName VARCHAR(100) NOT NULL,
	LastName VARCHAR(100) NOT NULL,
	EmailAddress VARCHAR (100) NOT NULL,
	Pw VARCHAR(100) NOT NULL,
	Administrator BIT NOT NULL,
	CONSTRAINT PK_UserId PRIMARY KEY (UserId));

INSERT INTO tblUsers
VALUES ('Jacob', 'Snover', 'mruser1@aol.com', 'admin1234', 1),
	   ('Jason', 'Kubo', 'mruser2@aol.com', 'admin1234', 1),
	   ('Beni', 'Rajanish', 'mrsuser1@aol.com', 'admin1234', 1),
	   ('Stephen', 'Scobie', 'mruser3@aol.com', 'admin1234', 1)

SELECT *
FROM tblUsers;

DROP tblTeams;

INSERT INTO tblTeams
VALUES (RAND() * 10000000000, 100000, 'Team 1', 5000),
	   (RAND() * 10000000000, 100000, 'Team 2', 5001),
	   (RAND() * 10000000000, 100000, 'Team 3', 5002),
	   (RAND() * 10000000000, 100001, 'Team A', 6000),
	   (RAND() * 10000000000, 100001, 'Team B', 7000),
	   (RAND() * 10000000000, 100002, 'Team Hers', 15000),
	   (RAND() * 10000000000, 100002, 'Team His', 17000),
	   (RAND() * 10000000000, 100003, 'Team Alpha', 5000),
	   (RAND() * 10000000000, 100003, 'Team Bravo', 5001),
	   (RAND() * 10000000000, 100003, 'Team Charlie', 5002),
	   (RAND() * 10000000000, 100003, 'Team Delta', 5000);

SELECT *
FROM tblTeams
ORDER BY UserId, TeamName

DROP TABLE tblUserWatchlists;
DELETE FROM tblUserWatchlists;
DROP TABLE tblWatchlists;

SELECT *
FROM tblUsers;

SELECT *
FROM tblTeams;

CREATE TABLE tblUserWatchlists (
	WatchlistId BIGINT IDENTITY(1000000, 1) NOT NULL,
	UserId NVARCHAR(128) NOT NULL,
	CONSTRAINT PK_UserIdWatchlistId PRIMARY KEY (UserId, WatchlistId),
	FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT UK_WatchlistId UNIQUE (WatchlistId));

CREATE TABLE tblWatchlists (
	WatchlistId BIGINT NOT NULL,
	WatchlistName VARCHAR(100) NOT NULL,
	PlayerId INT NOT NULL,
	FOREIGN KEY (WatchlistId) REFERENCES tblUserWatchlists (WatchlistId),
	CONSTRAINT PK_WatchlistPLayerID PRIMARY KEY (WatchlistId, PlayerId));

SELECT *
FROM tblUserWatchlists;

SELECT *
FROM tblWatchlists;

SELECT *
FROM AspNetUsers;

UPDATE a
SET a.WatchlistName = 'QB Test List'
FROM tblWatchlists a
WHERE a.WatchlistId = 1000047;

UPDATE a
SET a.WatchlistName = 'The Winningest List'
FROM tblWatchlists a
WHERE a.WatchlistId = 1000051;

DELETE FROM tblWatchlists
WHERE WatchlistId = 1000047 AND PlayerId NOT IN (5933, 7274);

INSERT INTO tblWatchlists
VALUES (1000047, 'QB Test List', 6825),
	   (1000047, 'QB Test List', 7274)

INSERT INTO tblWatchlists
VALUES (1000047, 'QB Test List', 9712)

SELECT *
FROM AspNetUsers
WHERE Id = 'f99dc422-0aca-44a0-8a8d-ff42a2fd3834'

SELECT *
FROM AspNetUsers
WHERE Email = 'stephen.scobie@gmail.com'

SELECT *
FROM tblUserWatchlists
WHERE UserId = 'bee9ae91-357a-4e7d-bbb6-66a8f9d3edbd'

UPDATE a
SET a.WatchlistName = 'Scobie QB List1'
FROM tblWatchlists a
WHERE a.WatchlistId = 1000351

SELECT *
FROM tblWatchlists
WHERE WatchlistId IN (SELECT DISTINCT WatchlistId
					  FROM tblUserWatchlists
					  WHERE UserId = 'bee9ae91-357a-4e7d-bbb6-66a8f9d3edbd')

UPDATE a
SET a.WatchlistName = 'Scobie WR List1'
FROM tblWatchlists a
WHERE a.WatchlistId = 1000353

-- 6/7/2018
CREATE TABLE tblJsonDump (
	ImportId INT IDENTITY(1, 1) NOT NULL,
	MySportsFeedsData VARCHAR(max),
	ImportDate TIMESTAMP
)

-- 6/15/2018
DROP TABLE [tblJsonDump]

/****** Object:  Table [dbo].[tblJsonDump]    Script Date: 6/13/2018 6:28:55 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblJsonDump](
	[ImportId] [int] IDENTITY(1,1) NOT NULL,
	[MySportsFeedsData] [varchar](max) NULL,
	[CreationDate] [datetime2] NOT NULL,
	[ImportDate] [datetime2] NOT NULL,
	ImportMethod varchar(100) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 6/17/2018
SELECT *
FROM tblJsonDump

ALTER TABLE tblJsonDump
ADD ImportMethod varchar(100)

UPDATE tblJsonDump
SET ImportMethod = 'Manual'
WHERE ImportId = 1

UPDATE tblJsonDump
SET ImportMethod = 'SQL Server Agent Job'
WHERE ImportId = 2

DELETE FROM tblJsonDump
WHERE ImportId = 3

ALTER TABLE tblJsonDump
ADD MySportsFeedsData2017 [varchar](max) NULL,
	MySportsFeedsData2016 [varchar](max) NULL,
	MySportsFeedsData2015 [varchar](max) NULL,
	MySportsFeedsData2014 [varchar](max) NULL,
	MySportsFeedsDataPlayerLogs [varchar](max) NULL,
	MySportsFeedsDataSchedules [varchar](max) NULL

ALTER TABLE tblJsonDump
ADD MySportsFeedsDataPlayerLogs2017 [varchar](max) NULL,
	MySportsFeedsDataPlayerLogs2016 [varchar](max) NULL,
	MySportsFeedsDataPlayerLogs2015 [varchar](max) NULL,
	MySportsFeedsDataPlayerLogs2014 [varchar](max) NULL

USE FFLTool

DELETE FROM tblJsonDump

DROP TABLE tblJsonDump

SELECT *
FROM tblJsonDump

CREATE TABLE tblJsonDump (
	ImportId int IDENTITY(1,1) NOT NULL,
	MySportsFeedsData2018 varchar(max) NULL,
	MySportsFeedsData2017 varchar(max) NULL,
	MySportsFeedsData2016 varchar(max) NULL,
	MySportsFeedsData2015 varchar(max) NULL,
	MySportsFeedsData2014 varchar(max) NULL,
	MySportsFeedsDataPlayerLogs2018 varchar(max) NULL,
	MySportsFeedsDataPlayerLogs2017 varchar(max) NULL,
	MySportsFeedsDataPlayerLogs2016 varchar(max) NULL,
	MySportsFeedsDataPlayerLogs2015 varchar(max) NULL,
	MySportsFeedsDataPlayerLogs2014 varchar(max) NULL,
	MySportsFeedsDataSchedules varchar(max) NULL,
	CreationDate datetime2 NOT NULL,
	ImportDate datetime2 NOT NULL,
	ImportMethod varchar(100),
	CONSTRAINT PK_ImportId PRIMARY KEY (ImportId)
)

SELECT [ImportId]
      ,[MySportsFeedsData2018]
      ,[MySportsFeedsData2017]
      ,[MySportsFeedsData2016]
      ,[MySportsFeedsData2015]
      ,[MySportsFeedsData2014]
      ,[MySportsFeedsDataPlayerLogs2018]
      ,[MySportsFeedsDataPlayerLogs2017]
      ,[MySportsFeedsDataPlayerLogs2016]
      ,[MySportsFeedsDataPlayerLogs2015]
      ,[MySportsFeedsDataPlayerLogs2014]
      ,[MySportsFeedsDataSchedules]
      ,[CreationDate]
      ,[ImportDate]
      ,[ImportMethod]
  FROM [FFLTOOL_AZUREDB].[FFLTool].[dbo].[tblJsonDump]
GO


USE FFLTool

CREATE VIEW vwJson_MySportsFeedsData2018 AS 
SELECT 
	ImportId,
	MySportsFeedsData2018
FROM tblJsonDump;

SELECT *
FROM vwJson_MySportsFeedsData2018

CREATE VIEW vwMySportsFeedsData2018 AS 
SELECT 
	ImportId,
	MySportsFeedsData2018
FROM tblJsonDump
GO

CREATE VIEW vwMySportsFeedsData2017 AS 
SELECT 
	ImportId,
	MySportsFeedsData2017
FROM tblJsonDump
GO

CREATE VIEW vwMySportsFeedsData2016 AS 
SELECT 
	ImportId,
	MySportsFeedsData2016
FROM tblJsonDump
GO

CREATE VIEW vwMySportsFeedsData2015 AS 
SELECT 
	ImportId,
	MySportsFeedsData2015
FROM tblJsonDump
GO

CREATE VIEW vwMySportsFeedsData2014 AS 
SELECT 
	ImportId,
	MySportsFeedsData2014
FROM tblJsonDump
GO

CREATE VIEW vwMySportsFeedsDataPlayerLogs2018 AS 
SELECT 
	ImportId,
	MySportsFeedsDataPlayerLogs2018
FROM tblJsonDump
GO

CREATE VIEW vwMySportsFeedsDataPlayerLogs2017 AS 
SELECT 
	ImportId,
	MySportsFeedsDataPlayerLogs2017
FROM tblJsonDump
GO

CREATE VIEW vwMySportsFeedsDataPlayerLogs2016 AS 
SELECT 
	ImportId,
	MySportsFeedsDataPlayerLogs2016
FROM tblJsonDump
GO

CREATE VIEW vwMySportsFeedsDataPlayerLogs2015 AS 
SELECT 
	ImportId,
	MySportsFeedsDataPlayerLogs2015
FROM tblJsonDump
GO

CREATE VIEW vwMySportsFeedsDataPlayerLogs2014 AS 
SELECT 
	ImportId,
	MySportsFeedsDataPlayerLogs2014
FROM tblJsonDump
GO

SELECT *
FROM vwMySportsFeedsDataPlayerLogs2014