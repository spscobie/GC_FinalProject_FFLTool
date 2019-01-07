USE [msdb]
GO

/****** Object:  Job [MySportsFeedsAPI_WeeklyUpdate]    Script Date: 1/7/2019 5:48:00 AM ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]    Script Date: 1/7/2019 5:48:00 AM ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'MySportsFeedsAPI_WeeklyUpdate', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'No description available.', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'DESKTOP-AVE919U\sscobie', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Call the API - Cumulative Stats]    Script Date: 1/7/2019 5:48:01 AM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Call the API - Cumulative Stats', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=1, 
		@retry_interval=1, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE FFLTool

--2018 season
DECLARE @season NVARCHAR(100)
SET @season = ''2018-regular''
EXEC CallAPI_CumulativeStats @season
GO

--2017 season
DECLARE @season NVARCHAR(100)
SET @season = ''2017-regular''
EXEC CallAPI_CumulativeStats @season
GO

--2016 season
DECLARE @season NVARCHAR(100)
SET @season = ''2016-2017-regular''
EXEC CallAPI_CumulativeStats @season
GO

-- 2015 season
DECLARE @season NVARCHAR(100)
SET @season = ''2015-2016-regular''
EXEC CallAPI_CumulativeStats @season
GO

-- 2014 season
DECLARE @season NVARCHAR(100)
SET @season = ''2014-2015-regular''
EXEC CallAPI_CumulativeStats @season
GO
', 
		@database_name=N'master', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [INSERT INTO tblJsonDump - Cumulative Stats]    Script Date: 1/7/2019 5:48:01 AM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'INSERT INTO tblJsonDump - Cumulative Stats', 
		@step_id=2, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE FFLTool;

INSERT INTO tblJsonDump (MySportsFeedsData2018, ImportDate, ImportMethod)
SELECT BulkColumn, CURRENT_TIMESTAMP, ''SQL Server Agent Job''
FROM OPENROWSET (BULK ''C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\cumulativestats_nfl_2018-regular.txt'', SINGLE_CLOB) as j;
GO

--2017
DECLARE @importid INT

SELECT TOP 1 @importid= ImportId
FROM tblJsonDump
ORDER BY ImportId DESC

UPDATE t
SET MySportsFeedsData2017 = json.BulkColumn
FROM tblJsonDump t
INNER JOIN (
	SELECT BulkColumn, @importid AS ImportId
	FROM OPENROWSET (BULK ''C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\cumulativestats_nfl_2017-regular.txt'', SINGLE_CLOB) j
) json ON t.ImportId = json.ImportId
GO


--2016
DECLARE @importid INT

SELECT TOP 1 @importid= ImportId
FROM tblJsonDump
ORDER BY ImportId DESC

UPDATE t
SET MySportsFeedsData2016 = json.BulkColumn
FROM tblJsonDump t
INNER JOIN (
	SELECT BulkColumn, @importid AS ImportId
	FROM OPENROWSET (BULK ''C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\cumulativestats_nfl_2016-2017-regular.txt'', SINGLE_CLOB) j
) json ON t.ImportId = json.ImportId
GO


--2015
DECLARE @importid INT

SELECT TOP 1 @importid= ImportId
FROM tblJsonDump
ORDER BY ImportId DESC

UPDATE t
SET MySportsFeedsData2015 = json.BulkColumn
FROM tblJsonDump t
INNER JOIN (
	SELECT BulkColumn, @importid AS ImportId
	FROM OPENROWSET (BULK ''C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\cumulativestats_nfl_2015-2016-regular.txt'', SINGLE_CLOB) j
) json ON t.ImportId = json.ImportId
GO


--2014
DECLARE @importid INT

SELECT TOP 1 @importid= ImportId
FROM tblJsonDump
ORDER BY ImportId DESC

UPDATE t
SET MySportsFeedsData2014 = json.BulkColumn
FROM tblJsonDump t
INNER JOIN (
	SELECT BulkColumn, @importid AS ImportId
	FROM OPENROWSET (BULK ''C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\cumulativestats_nfl_2014-2015-regular.txt'', SINGLE_CLOB) j
) json ON t.ImportId = json.ImportId
GO', 
		@database_name=N'master', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Call the API - PlayerLogs]    Script Date: 1/7/2019 5:48:01 AM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Call the API - PlayerLogs', 
		@step_id=3, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=2, 
		@retry_interval=5, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE FFLTool;

DECLARE @teamAbbr NVARCHAR(3)
DECLARE @season NVARCHAR(100)
DECLARE TeamCursor CURSOR FOR
	SELECT DISTINCT TeamAbbr
	FROM tblTeams

OPEN TeamCursor

FETCH NEXT FROM TeamCursor
INTO @teamAbbr

WHILE @@FETCH_STATUS = 0
BEGIN
	
	SET @season = N''2018-regular''
                WAITFOR DELAY ''00:00:05''
	EXEC CallAPI_PlayerLogs @season, @teamAbbr

	FETCH NEXT FROM TeamCursor
	INTO @teamAbbr
END

CLOSE TeamCursor
DEALLOCATE TeamCursor
GO


DECLARE @teamAbbr NVARCHAR(3)
DECLARE @season NVARCHAR(100)
DECLARE TeamCursor CURSOR FOR
	SELECT DISTINCT TeamAbbr
	FROM tblTeams

OPEN TeamCursor

FETCH NEXT FROM TeamCursor
INTO @teamAbbr

WHILE @@FETCH_STATUS = 0
BEGIN
	
	SET @season = N''2017-regular''
                WAITFOR DELAY ''00:00:05''
	EXEC CallAPI_PlayerLogs @season, @teamAbbr

	FETCH NEXT FROM TeamCursor
	INTO @teamAbbr
END

CLOSE TeamCursor
DEALLOCATE TeamCursor
GO


DECLARE @teamAbbr NVARCHAR(3)
DECLARE @season NVARCHAR(100)
DECLARE TeamCursor CURSOR FOR
	SELECT DISTINCT TeamAbbr
	FROM tblTeams

OPEN TeamCursor

FETCH NEXT FROM TeamCursor
INTO @teamAbbr

WHILE @@FETCH_STATUS = 0
BEGIN
	
	SET @season = N''2016-2017-regular''
                WAITFOR DELAY ''00:00:05''
	EXEC CallAPI_PlayerLogs @season, @teamAbbr

	FETCH NEXT FROM TeamCursor
	INTO @teamAbbr
END

CLOSE TeamCursor
DEALLOCATE TeamCursor
GO

DECLARE @teamAbbr NVARCHAR(3)
DECLARE @season NVARCHAR(100)
DECLARE TeamCursor CURSOR FOR
	SELECT DISTINCT TeamAbbr
	FROM tblTeams

OPEN TeamCursor

FETCH NEXT FROM TeamCursor
INTO @teamAbbr

WHILE @@FETCH_STATUS = 0
BEGIN
	
	SET @season = N''2015-2016-regular''
                WAITFOR DELAY ''00:00:05''
	EXEC CallAPI_PlayerLogs @season, @teamAbbr

	FETCH NEXT FROM TeamCursor
	INTO @teamAbbr
END

CLOSE TeamCursor
DEALLOCATE TeamCursor
GO

DECLARE @teamAbbr NVARCHAR(3)
DECLARE @season NVARCHAR(100)
DECLARE TeamCursor CURSOR FOR
	SELECT DISTINCT TeamAbbr
	FROM tblTeams

OPEN TeamCursor

FETCH NEXT FROM TeamCursor
INTO @teamAbbr

WHILE @@FETCH_STATUS = 0
BEGIN
	
	SET @season = N''2014-2015-regular''
                WAITFOR DELAY ''00:00:05''
	EXEC CallAPI_PlayerLogs @season, @teamAbbr

	FETCH NEXT FROM TeamCursor
	INTO @teamAbbr
END

CLOSE TeamCursor
DEALLOCATE TeamCursor
GO
', 
		@database_name=N'FFLTool', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Combine PlayerLogs]    Script Date: 1/7/2019 5:48:01 AM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Combine PlayerLogs', 
		@step_id=4, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE FFLTool;

DECLARE @teams NVARCHAR(1000)
DECLARE @season NVARCHAR(100)

SET @season = ''2018''
SELECT @teams = LEFT(a.Teams, LEN(a.Teams) - 1)
FROM (
		SELECT MIN(TeamId) AS Id,
			   (SELECT TeamAbbr + '','' FROM tblTeams FOR XML PATH ('''')) AS Teams
		FROM tblTeams
	  ) AS a

EXEC dbo.CombinePlayerLogs @teams, @season


SET @season = ''2017''
SELECT @teams = LEFT(a.Teams, LEN(a.Teams) - 1)
FROM (
		SELECT MIN(TeamId) AS Id,
			   (SELECT TeamAbbr + '','' FROM tblTeams FOR XML PATH ('''')) AS Teams
		FROM tblTeams
	  ) AS a

EXEC dbo.CombinePlayerLogs @teams, @season


SET @season = ''2016-2017''
SELECT @teams = LEFT(a.Teams, LEN(a.Teams) - 1)
FROM (
		SELECT MIN(TeamId) AS Id,
			   (SELECT TeamAbbr + '','' FROM tblTeams FOR XML PATH ('''')) AS Teams
		FROM tblTeams
	  ) AS a

EXEC dbo.CombinePlayerLogs @teams, @season


SET @season = ''2015-2016''
SELECT @teams = LEFT(a.Teams, LEN(a.Teams) - 1)
FROM (
		SELECT MIN(TeamId) AS Id,
			   (SELECT TeamAbbr + '','' FROM tblTeams FOR XML PATH ('''')) AS Teams
		FROM tblTeams
	  ) AS a

EXEC dbo.CombinePlayerLogs @teams, @season


SET @season = ''2014-2015''
SELECT @teams = LEFT(a.Teams, LEN(a.Teams) - 1)
FROM (
		SELECT MIN(TeamId) AS Id,
			   (SELECT TeamAbbr + '','' FROM tblTeams FOR XML PATH ('''')) AS Teams
		FROM tblTeams
	  ) AS a

EXEC dbo.CombinePlayerLogs @teams, @season', 
		@database_name=N'FFLTool', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [INSERT INTO tblJsonDump - Player Logs]    Script Date: 1/7/2019 5:48:01 AM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'INSERT INTO tblJsonDump - Player Logs', 
		@step_id=5, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE FFLTool;

--2018
DECLARE @importid INT

SELECT TOP 1 @importid= ImportId
FROM tblJsonDump
ORDER BY ImportId DESC

UPDATE t
SET MySportsFeedsDataPlayerLogs2018= json.BulkColumn
FROM tblJsonDump t
INNER JOIN (
	SELECT BulkColumn, @importid AS ImportId
	FROM OPENROWSET (BULK ''C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\combined_nfl_2018-regular_playerlogs.txt'', SINGLE_CLOB) j
) json ON t.ImportId = json.ImportId
GO


--2017
DECLARE @importid INT

SELECT TOP 1 @importid= ImportId
FROM tblJsonDump
ORDER BY ImportId DESC

UPDATE t
SET MySportsFeedsDataPlayerLogs2017= json.BulkColumn
FROM tblJsonDump t
INNER JOIN (
	SELECT BulkColumn, @importid AS ImportId
	FROM OPENROWSET (BULK ''C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\combined_nfl_2017-regular_playerlogs.txt'', SINGLE_CLOB) j
) json ON t.ImportId = json.ImportId
GO


--2016
DECLARE @importid INT

SELECT TOP 1 @importid= ImportId
FROM tblJsonDump
ORDER BY ImportId DESC

UPDATE t
SET MySportsFeedsDataPlayerLogs2016= json.BulkColumn
FROM tblJsonDump t
INNER JOIN (
	SELECT BulkColumn, @importid AS ImportId
	FROM OPENROWSET (BULK ''C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\combined_nfl_2016-2017-regular_playerlogs.txt'', SINGLE_CLOB) j
) json ON t.ImportId = json.ImportId
GO


--2015
DECLARE @importid INT

SELECT TOP 1 @importid= ImportId
FROM tblJsonDump
ORDER BY ImportId DESC

UPDATE t
SET MySportsFeedsDataPlayerLogs2015= json.BulkColumn
FROM tblJsonDump t
INNER JOIN (
	SELECT BulkColumn, @importid AS ImportId
	FROM OPENROWSET (BULK ''C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\combined_nfl_2015-2016-regular_playerlogs.txt'', SINGLE_CLOB) j
) json ON t.ImportId = json.ImportId
GO


--2014
DECLARE @importid INT

SELECT TOP 1 @importid= ImportId
FROM tblJsonDump
ORDER BY ImportId DESC

UPDATE t
SET MySportsFeedsDataPlayerLogs2014= json.BulkColumn
FROM tblJsonDump t
INNER JOIN (
	SELECT BulkColumn, @importid AS ImportId
	FROM OPENROWSET (BULK ''C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\combined_nfl_2014-2015-regular_playerlogs.txt'', SINGLE_CLOB) j
) json ON t.ImportId = json.ImportId
GO', 
		@database_name=N'master', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Call API - Get current week's schedule]    Script Date: 1/7/2019 5:48:01 AM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Call API - Get current week''s schedule', 
		@step_id=6, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE FFLTool

DECLARE @season nvarchar(100)
DECLARE @week nvarchar(2)
DECLARE @now date

SET @now = GETDATE()

SELECT
	@season = 
		CASE
			WHEN @now BETWEEN WeekStart AND WeekEnd THEN Season + WeekSuffix
			WHEN @now < (SELECT MIN(WeekStart)
						 FROM tblWeeks
						 WHERE Season = CAST((SELECT MAX(CAST(Season AS INT)) FROM tblWeeks) AS VARCHAR(4))) THEN CAST((SELECT MAX(CAST(Season AS INT) - 1) FROM tblWeeks) AS VARCHAR(4)) + WeekSuffix
			ELSE CAST((SELECT MAX(CAST(Season AS INT)) FROM tblWeeks) AS VARCHAR(4)) + WeekSuffix
		END,
	@week =
		CASE
			WHEN @now BETWEEN WeekStart AND WeekEnd THEN WeekNumber
			ELSE ''17''
		END     		
FROM tblWeeks

EXEC MySF_ApiRequest_Schedules @season, @week
GO', 
		@database_name=N'FFLTool', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [INSERT INTO tblJsonDump - Schedules]    Script Date: 1/7/2019 5:48:01 AM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'INSERT INTO tblJsonDump - Schedules', 
		@step_id=7, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE FFLTool;

DECLARE @importid INT

SELECT TOP 1 @importid= ImportId
FROM tblJsonDump
ORDER BY ImportId DESC

UPDATE t
SET MySportsFeedsDataSchedules= json.BulkColumn
FROM tblJsonDump t
INNER JOIN (
	SELECT BulkColumn, @importid AS ImportId
	FROM OPENROWSET (BULK ''C:\Users\sscobie\Documents\Visual Studio 2017\Projects\GC_FinalProject_FFLTool\SQL\response_nfl_2018-regular_schedule.txt'', SINGLE_CLOB) j
) json ON t.ImportId = json.ImportId
GO', 
		@database_name=N'FFLTool', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Push to Azure via Linked Server]    Script Date: 1/7/2019 5:48:01 AM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Push to Azure via Linked Server', 
		@step_id=8, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE FFLTool

-- Maintain only one record at a time. No archiving on web app database
EXEC [FFLTOOL_AZUREDB].[FFLTool].sys.sp_executesql N''TRUNCATE TABLE dbo.tblJsonDump''
GO

INSERT INTO [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump (MySportsFeedsData2018, MySportsFeedsData2017, MySportsFeedsData2016, MySportsFeedsData2015, MySportsFeedsData2014, MySportsFeedsDataSchedules, CreationDate, ImportDate, ImportMethod)
SELECT TOP 1
	MySportsFeedsData2018,
	MySportsFeedsData2017,
	MySportsFeedsData2016,
	MySportsFeedsData2015,
	MySportsFeedsData2014,
	MySportsFeedsDataSchedules,
	ImportDate,
	CURRENT_TIMESTAMP,
	ImportMethod
FROM tblJsonDump
ORDER BY ImportDate DESC
GO

UPDATE t
SET t.MySportsFeedsDataPlayerLogs2018 = s.MySportsFeedsDataPlayerLogs2018
FROM [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump t
CROSS JOIN
(
	SELECT TOP 1
		ImportId,
		MySportsFeedsDataPlayerLogs2018
	FROM tblJsonDump
	ORDER BY ImportDate DESC
) s
WHERE t.ImportId = (SELECT MAX(ImportId) FROM [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump)
GO

UPDATE t
SET t.MySportsFeedsDataPlayerLogs2017 = s.MySportsFeedsDataPlayerLogs2017
FROM [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump t
CROSS JOIN
(
	SELECT TOP 1
		ImportId,
		MySportsFeedsDataPlayerLogs2017
	FROM tblJsonDump
	ORDER BY ImportDate DESC
) s
WHERE t.ImportId = (SELECT MAX(ImportId) FROM [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump)
GO

UPDATE t
SET t.MySportsFeedsDataPlayerLogs2016 = s.MySportsFeedsDataPlayerLogs2016
FROM [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump t
CROSS JOIN
(
	SELECT TOP 1
		ImportId,
		MySportsFeedsDataPlayerLogs2016
	FROM tblJsonDump
	ORDER BY ImportDate DESC
) s
WHERE t.ImportId = (SELECT MAX(ImportId) FROM [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump)
GO

UPDATE t
SET t.MySportsFeedsDataPlayerLogs2015 = s.MySportsFeedsDataPlayerLogs2015
FROM [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump t
CROSS JOIN
(
	SELECT TOP 1
		ImportId,
		MySportsFeedsDataPlayerLogs2015
	FROM tblJsonDump
	ORDER BY ImportDate DESC
) s
WHERE t.ImportId = (SELECT MAX(ImportId) FROM [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump)
GO

UPDATE t
SET t.MySportsFeedsDataPlayerLogs2014 = s.MySportsFeedsDataPlayerLogs2014
FROM [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump t
CROSS JOIN
(
	SELECT TOP 1
		ImportId,
		MySportsFeedsDataPlayerLogs2014
	FROM tblJsonDump
	ORDER BY ImportDate DESC
) s
WHERE t.ImportId = (SELECT MAX(ImportId) FROM [FFLTOOL_AZUREDB].FFLTool.dbo.tblJsonDump)
GO


-- Clean uo local database, maintaining data only from  the last five job runs
DELETE FROM tblJsonDump
WHERE ImportId IN 
(
	SELECT a.ImportId
	FROM (
		SELECT ImportId, ROW_NUMBER() OVER(ORDER BY ImportId DESC) AS ''N_Rank''
		FROM tblJsonDump
	) a
	WHERE a.N_Rank > 5
)', 
		@database_name=N'master', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'MySportsFeedAPI_Weekday', 
		@enabled=1, 
		@freq_type=8, 
		@freq_interval=38, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=1, 
		@active_start_date=20180409, 
		@active_end_date=99991231, 
		@active_start_time=20000, 
		@active_end_time=235959, 
		@schedule_uid=N'4d0d2621-4653-40c0-808a-e50fc471418b'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'MySportsFeedsAPI_Sunday', 
		@enabled=1, 
		@freq_type=8, 
		@freq_interval=1, 
		@freq_subday_type=8, 
		@freq_subday_interval=2, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=1, 
		@active_start_date=20180412, 
		@active_end_date=99991231, 
		@active_start_time=163000, 
		@active_end_time=235959, 
		@schedule_uid=N'3fcb32db-3097-4d50-86ae-d0eba13a5789'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:

GO


