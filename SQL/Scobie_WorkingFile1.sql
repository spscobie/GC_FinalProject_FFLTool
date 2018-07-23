-- Dummy check in case you execute the entire file!

/**************************/
/* LOCAL FFLTool DB!!!!!! */
/**************************/

USE JunkDBJunkDBJunkDB;

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

CREATE TABLE tblTeams (
	TeamId BIGINT NOT NULL,
	UserId INT NOT NULL,
	TeamName VARCHAR(256) NOT NULL,
	PlayerId INT NOT NULL,
	CONSTRAINT PK_TeamId PRIMARY KEY (TeamId));

DECLARE @randNum bigint;
SET @randNum = RAND() * 10000000000;
SELECT @randNum;
SELECT HASHBYTES('MD5', CAST(@randNum AS nvarchar(10)));

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

/* 1/10/18 */
EXEC sp_configure 'show advanced options', 1;
GO
RECONFIGURE;
GO
EXEC sp_configure 'clr enabled', 1;
GO
RECONFIGURE;
GO

ALTER DATABASE FFLTool
SET Trustworthy ON;
GO

CREATE PROCEDURE dbo.CallAPI WITH EXECUTE AS CALLER 
AS
EXTERNAL NAME MySportsFeedsAPI_SPs.StoredProcedures.MySF_ApiRequest_Position_QB_RB_WR_TE_K
GO

USE FFLTool

EXEC dbo.CallAPI;

-- 6/13/2018
DROP TABLE [tblJsonDump]

/****** Object:  Table [dbo].[tblJsonDump]    Script Date: 6/13/2018 6:28:55 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblJsonDump](
	[ImportId] [int] IDENTITY(1,1) NOT NULL,
	[MySportsFeedsData] [varchar](max) NULL,
	[ImportDate] [datetime2] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SELECT BulkColumn, CURRENT_TIMESTAMP
FROM OPENROWSET (BULK 'C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\response_nfl_2017reg.txt', SINGLE_CLOB) as j

SELECT BulkColumn
INTO #temp 
FROM OPENROWSET (BULK 'C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\response_nfl_2017reg.txt', SINGLE_CLOB) as j

SELECT *
FROM #temp

INSERT INTO tblJsonDump (MySportsFeedsData, ImportDate, ImportMethod)
SELECT BulkColumn, CURRENT_TIMESTAMP, 'Manual'
FROM OPENROWSET (BULK 'C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\response_nfl_2017reg.txt', SINGLE_CLOB) as j

SELECT *
FROM tblJsonDump

--6/16/2018
SELECT *
FROM [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump

INSERT INTO [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump (MySportsFeedsData, CreationDate, ImportDate, ImportMethod)
SELECT
	MySportsFeedsData,
	ImportDate,
	CURRENT_TIMESTAMP,
	ImportMethod
FROM tblJsonDump

-- 6/17/2018
select [Current LSN],
       [Operation],
       [Transaction Name],
       [Transaction ID],
       [Transaction SID],
       [SPID],
       [Begin Time]
FROM   fn_dblog(null,null)
ORDER BY [Begin Time] DESC

ALTER TABLE tblJsonDump
ADD ImportMethod varchar(100)

UPDATE tblJsonDump
SET ImportMethod = 'Manual'
WHERE ImportId = 1

INSERT INTO [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump (MySportsFeedsData, CreationDate, ImportDate, ImportMethod)
SELECT TOP 1
	MySportsFeedsData,
	ImportDate,
	CURRENT_TIMESTAMP,
	ImportMethod
FROM tblJsonDump
ORDER BY ImportDate DESC

USE FFLTool

SELECT *
FROM tblJsonDump

SELECT *
FROM [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump

-- 6/24/2018

CREATE PROCEDURE dbo.CallAPI_AllPos WITH EXECUTE AS CALLER 
AS
EXTERNAL NAME MySportsFeedsAPI_SPs.StoredProcedures.MySF_ApiRequest_Position_QB_RB_WR_TE_K
GO

CREATE PROCEDURE dbo.CallAPI_Pos @pos nvarchar (2) WITH EXECUTE AS CALLER 
AS
EXTERNAL NAME MySportsFeedsAPI_SPs.StoredProcedures.MySF_ApiRequest_Position
GO

EXEC dbo.CallAPI_AllPos;

DECLARE @pos nvarchar (2)
SET @pos = 'qb'
EXEC dbo.CallAPI_Pos @pos;

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

CREATE TABLE tblTeams (
	TeamId smallint IDENTITY(1,1) NOT NULL,
	TeamAbbr varchar(3) NOT NULL,
	TeamCity varchar(50) NOT NULL,
	TeamName varchar(50) NOT NULL,
	FullName varchar(100) NOT NULL
)

SELECT *
FROM tblTeams

-- Cursor bits (Executes in approx 00h:06m:59s running all teams!)
DECLARE @teamAbbr NVARCHAR(3)
DECLARE @season NVARCHAR(100)
DECLARE TeamCursor CURSOR FOR
	SELECT DISTINCT TeamAbbr
	FROM tblTeams
	--WHERE TeamAbbr IN ('DET', 'CHI')

OPEN TeamCursor

FETCH NEXT FROM TeamCursor
INTO @teamAbbr

WHILE @@FETCH_STATUS = 0
BEGIN
	
	SET @season = N'2017-regular'
	EXEC CallAPI_PlayerLogs @season, @teamAbbr

	RAISERROR(@teamAbbr, 10, 1) WITH NOWAIT

	FETCH NEXT FROM TeamCursor
	INTO @teamAbbr
END

CLOSE TeamCursor
DEALLOCATE TeamCursor

--Web example
SET NOCOUNT ON
GO
DECLARE @iteration AS INT
SET @iteration = 1
WHILE(@iteration<=10)
BEGIN
    SELECT 'Start of Iteration ' + CAST(@iteration AS VARCHAR)
    WAITFOR DELAY '00:00:01'
    PRINT 'End Of Iteration ' + CAST(@iteration AS VARCHAR)
    SET @iteration+=1   
 
    RAISERROR('Yowza', 10, 1) WITH NOWAIT
END
GO

ALTER ASSEMBLY MySportsFeedsAPI_SPs
FROM 'C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\CLR\MySportsFeedsAPI_SPs\bin\Release\MySportsFeedsAPI_SPs.dll'
GO

CREATE PROCEDURE dbo.CallAPI_CumulativeStats @season NVARCHAR(100) WITH EXECUTE AS CALLER 
AS
EXTERNAL NAME MySportsFeedsAPI_SPs.StoredProcedures.MySF_ApiRequest_Position_QB_RB_WR_TE_K
GO

CREATE PROCEDURE dbo.CallAPI_PlayerLogs @season NVARCHAR(100), @team NVARCHAR(3) WITH EXECUTE AS CALLER 
AS
EXTERNAL NAME MySportsFeedsAPI_SPs.StoredProcedures.MySF_ApiRequest_PlayerLogs
GO

--Cumulative Stats
DECLARE @season NVARCHAR(100)
SET @season = '2017-regular'
EXEC CallAPI_CumulativeStats @season
GO

-- Player Logs
DECLARE @season NVARCHAR(100)
DECLARE @team NVARCHAR(3)

SET @season = '2017-regular'
SET @team = 'DET'
EXEC CallAPI_PlayerLogs @season, @team
GO
/*
	1) Done! Create new stored procedures to handle player log api calls and game schedule api calls
	2) Create BULK import process that grabs each team's player logs and UNIONs them together into one column. Make them JSON friendly for web app!
	3) Maybe create some new UDFs or SPs for our CURSOR and BULK import processes
	4) Modify SQLAgent job: call CallAPI with yearids, probably just call it multiple times rather than create a year table???
*/

--Didn't use this. Ended up using CLR method with the text files for player logs.
SELECT BulkColumn, CURRENT_TIMESTAMP
FROM OPENROWSET (BULK 'C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\response_nfl_2017reg_playerlogs_DET.txt', SINGLE_CLOB) as j
	UNION
SELECT BulkColumn, CURRENT_TIMESTAMP
FROM OPENROWSET (BULK 'C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\response_nfl_2017reg_playerlogs_CHI.txt', SINGLE_CLOB) as j

SELECT BulkColumn
INTO #temp 
FROM OPENROWSET (BULK 'C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\response_nfl_2017reg.txt', SINGLE_CLOB) as j

SELECT *
FROM #temp

INSERT INTO tblJsonDump (MySportsFeedsData, ImportDate, ImportMethod)
SELECT BulkColumn, CURRENT_TIMESTAMP, 'Manual'
FROM OPENROWSET (BULK 'C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\response_nfl_2017reg.txt', SINGLE_CLOB) as j

SELECT *
FROM sys.assemblies

--web examples of concatenation
/*Example 1: backwards compatibility to SQL Server 2000*/
declare @tmp varchar(250)
SET @tmp = ''
select @tmp = @tmp + TeamAbbr + ', ' from tblTeams

select SUBSTRING(@tmp, 0, LEN(@tmp))

/*Example 2: From SQL Server 2008 onward*/
Select TeamAbbr + ',' AS [text()]
From tblTeams
For XML PATH ('')

Select TeamAbbr + ','
From tblTeams
For XML PATH ('')

SELECT MIN(TeamId) AS Id,
	  (SELECT TeamAbbr + ',' FROM tblTeams FOR XML PATH ('')) AS Teams
FROM tblTeams

SELECT LEFT(a.Teams, LEN(a.Teams) - 1) AS Teams
FROM (
		SELECT MIN(TeamId) AS Id,
			   (SELECT TeamAbbr + ',' FROM tblTeams FOR XML PATH ('')) AS Teams
		FROM tblTeams
	  ) AS a

DECLARE @Teams NVARCHAR(1000)

SELECT @Teams = LEFT(a.Teams, LEN(a.Teams) - 1)
FROM (
		SELECT MIN(TeamId) AS Id,
			   (SELECT TeamAbbr + ',' FROM tblTeams FOR XML PATH ('')) AS Teams
		FROM tblTeams
	  ) AS a

PRINT @Teams

ALTER ASSEMBLY MySportsFeedsAPI_SPs
FROM 'C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\CLR\MySportsFeedsAPI_SPs\bin\Release\MySportsFeedsAPI_SPs.dll'
GO

CREATE PROCEDURE dbo.CallAPI_CumulativeStats @season NVARCHAR(100) WITH EXECUTE AS CALLER 
AS
	EXTERNAL NAME MySportsFeedsAPI_SPs.StoredProcedures.MySF_ApiRequest_Position_QB_RB_WR_TE_K
GO

CREATE PROCEDURE dbo.CallAPI_PlayerLogs @season NVARCHAR(100), @team NVARCHAR(3) WITH EXECUTE AS CALLER 
AS
	EXTERNAL NAME MySportsFeedsAPI_SPs.StoredProcedures.MySF_ApiRequest_PlayerLogs
GO

CREATE PROCEDURE dbo.CombinePlayerLogs @teams NVARCHAR(1000) WITH EXECUTE AS CALLER 
AS
	EXTERNAL NAME MySportsFeedsAPI_SPs.StoredProcedures.MySF_CombinePlayerLogs
GO

DECLARE @teams NVARCHAR(1000)

SELECT @teams = LEFT(a.Teams, LEN(a.Teams) - 1)
FROM (
		SELECT MIN(TeamId) AS Id,
			   (SELECT TeamAbbr + ',' FROM tblTeams FOR XML PATH ('')) AS Teams
		FROM tblTeams
	  ) AS a

EXEC dbo.CombinePlayerLogs @teams

SELECT BulkColumn
INTO #temp 
FROM OPENROWSET (BULK 'C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\combined_nfl_2017reg_playerlogs.txt', SINGLE_CLOB) as j

SELECT *
FROM #temp

SELECT *
FROM tblJsonDump

SELECT *
FROM tblTeams

-- from SQL Server Agent job
USE FFLTool

-- 2017 season
DECLARE @season NVARCHAR(100)
SET @season = '2017-regular'
EXEC CallAPI_CumulativeStats @season
GO


/* PREVIOUS SEASONS NOT WORKING!! API access isue???? */
-- 2016 season
DECLARE @season NVARCHAR(100)
SET @season = '2016-2017-regular'
EXEC CallAPI_CumulativeStats @season
GO

-- 2015 season
DECLARE @season NVARCHAR(100)
SET @season = '2015-2016-regular'
EXEC CallAPI_CumulativeStats @season
GO

-- 2014 season
DECLARE @season NVARCHAR(100)
SET @season = '2014-2015-regular'
EXEC CallAPI_CumulativeStats @season
GO

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
	ImportDate datetime2 NOT NULL,
	ImportMethod varchar(100),
	CONSTRAINT PK_ImportId PRIMARY KEY (ImportId)
)