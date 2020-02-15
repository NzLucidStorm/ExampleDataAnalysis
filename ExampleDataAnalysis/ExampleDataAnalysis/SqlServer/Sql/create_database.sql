--
-- DATABASE
--
IF DB_ID('$(dbname)') IS NULL
BEGIN
    CREATE DATABASE $(dbname)
END
GO

use $(dbname)
GO 

-- 
-- SCHEMAS
--
IF NOT EXISTS (SELECT name from sys.schemas WHERE name = 'sample')
BEGIN

	EXEC('CREATE SCHEMA sample')
    
END
GO

--
-- TABLES
--
IF  NOT EXISTS 
	(SELECT * FROM sys.objects 
	 WHERE object_id = OBJECT_ID(N'[sample].[Observation]') AND type in (N'U'))
	 
BEGIN

	CREATE TABLE [sample].[Observation](
        [ObservationID] INT PRIMARY KEY IDENTITY,
        [Province] [NVARCHAR](100),
        [Country] [NVARCHAR](100) NOT NULL,
        [Timestamp] [DATETIME2],
        [Confirmed] INT,
        [Deaths] INT,
        [Recovered] INT,
        [Lat] DOUBLE PRECISION,
        [Lon] DOUBLE PRECISION
    );

END
GO

--
-- INDEXES
--
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'UX_Observation')
BEGIN
    DROP INDEX [UX_Observation] on [sample].[Observation];
END
GO

CREATE UNIQUE INDEX UX_Observation ON [sample].[Observation](Province, Country, Timestamp);   
GO

--
-- STORED PROCEDURES
--
IF OBJECT_ID(N'[sample].[InsertOrUpdateObservation]', N'P') IS NOT NULL
BEGIN
    DROP PROCEDURE [sample].[InsertOrUpdateObservation];
END
GO

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ObservationType')
BEGIN
    DROP TYPE [sample].[ObservationType];
END
GO

CREATE TYPE [sample].[ObservationType] AS TABLE (
    [Province] [NVARCHAR](255),
    [Country] [NVARCHAR](255),
    [Timestamp] [DATETIME2],
    [Confirmed] INT,
    [Deaths] INT,
    [Recovered] INT,
    [Lat] DOUBLE PRECISION,
    [Lon] DOUBLE PRECISION
);
GO

CREATE PROCEDURE [sample].[InsertOrUpdateObservation]
  @Entities [sample].[ObservationType] ReadOnly
AS
BEGIN
    
    SET NOCOUNT ON;
 
    MERGE [sample].[Observation] AS TARGET USING @Entities AS SOURCE ON (TARGET.Province = SOURCE.Province) AND (TARGET.Country = SOURCE.Country) AND (TARGET.Timestamp = SOURCE.Timestamp)
    WHEN MATCHED THEN
        UPDATE SET TARGET.Confirmed = SOURCE.Confirmed, TARGET.Deaths = SOURCE.Deaths, TARGET.Recovered = SOURCE.Recovered, TARGET.Lat = SOURCE.Lat, TARGET.Lon = SOURCE.Lon
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Province, Country, Timestamp, Confirmed, Deaths, Recovered, Lat, Lon) 
        VALUES (SOURCE.Province, SOURCE.Country, SOURCE.Timestamp, SOURCE.Confirmed, SOURCE.Deaths, SOURCE.Recovered, SOURCE.Lat, SOURCE.Lon);

END
GO

