SET SQL DIALECT 3; 

CREATE TABLE ASPNETROLE (ID VARCHAR(36) NOT NULL,
        NAME VARCHAR(64) NOT NULL,
        NORMALIZEDNAME VARCHAR(64) NOT NULL,
        CONCURRENCYSTAMP VARCHAR(64),
PRIMARY KEY (ID),
UNIQUE (NAME),
UNIQUE (NORMALIZEDNAME));

CREATE TABLE ASPNETUSER (ID VARCHAR(36) NOT NULL,
        USERNAME VARCHAR(64) NOT NULL,
        EMAIL VARCHAR(64),
        ACCESSFAILEDCOUNT INTEGER,
        CONCURRENCYSTAMP VARCHAR(64),
        EMAILCONFIRMED CHAR(1),
        LOCKOUTENABLED CHAR(1),
        LOCKOUTEND TIMESTAMP,
        NORMALIZEDEMAIL VARCHAR(64),
        NORMALIZEDUSERNAME VARCHAR(64),
        PASSWORDHASH VARCHAR(128),
        PHONENUMBER VARCHAR(24),
        PHONENUMBERCONFIRMED CHAR(1),
        SECURITYSTAMP VARCHAR(64),
        TWOFACTORENABLED INTEGER,
PRIMARY KEY (ID),
UNIQUE (USERNAME));

CREATE TABLE ASPNETUSERINROLE (USERID VARCHAR(36),
        ROLEID VARCHAR(36),
PRIMARY KEY (USERID,ROLEID));

ALTER TABLE ASPNETUSERINROLE ADD FOREIGN KEY (USERID) REFERENCES ASPNETUSER (ID) ON DELETE CASCADE;
ALTER TABLE ASPNETUSERINROLE ADD FOREIGN KEY (ROLEID) REFERENCES ASPNETROLE (ID) ON DELETE CASCADE;

CREATE TABLE WEATHERSTATION (ID VARCHAR(36) NOT NULL,
        NAME VARCHAR(64) NOT NULL,
        LOCATION VARCHAR(64),
        LATITUDE FLOAT,
        LONGITUDE FLOAT,
        ISPUBLIC CHAR(1),
        SECRET VARCHAR(32),
        WEBCAMURL VARCHAR(256),
        HASTEMPERATURE CHAR(1),
        HASPRESSURE CHAR(1),
        HASHUMIDITY CHAR(1),
        HASUV CHAR(1),
PRIMARY KEY (ID),
UNIQUE (NAME));

CREATE TABLE LOGENTRY (ID INTEGER NOT NULL,
	STATIONID VARCHAR(36) NOT NULL,
	LOGDATE TIMESTAMP NOT NULL,
	TEMPERATURE FLOAT,
	HUMIDITY SMALLINT,
	PRESSURE SMALLINT,
	UV FLOAT,
PRIMARY KEY (ID));

ALTER TABLE LOGENTRY ADD FOREIGN KEY (STATIONID) REFERENCES WEATHERSTATION (ID) ON DELETE CASCADE;

CREATE GENERATOR GEN_LOGENTRY_ID;
SET GENERATOR GEN_LOGENTRY_ID TO 0;

set term !! ;
CREATE TRIGGER TR_LOGENTRY_ID FOR LOGENTRY
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
if (NEW.ID is NULL) then NEW.ID = GEN_ID(GEN_LOGENTRY_ID, 1);
END!!
set term ; !!

CREATE TABLE DASHBOARDITEM (ID INTEGER NOT NULL,
        USERID VARCHAR(36) NOT NULL,
        WIDGETID VARCHAR(36) NOT NULL,
        X INT,
        Y INT,
PRIMARY KEY (ID));

ALTER TABLE DASHBOARDITEM ADD FOREIGN KEY (USERID) REFERENCES ASPNETUSER (ID) ON DELETE CASCADE;

CREATE GENERATOR GEN_DASHBOARD_ITEM_ID;
SET GENERATOR GEN_DASHBOARD_ITEM_ID TO 0;

set term !! ;
CREATE TRIGGER TR_DASHBOARD_ID FOR DASHBOARDITEM
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
if (NEW.ID is NULL) then NEW.ID = GEN_ID(GEN_DASHBOARD_ITEM_ID, 1);
END!!
set term ; !!

CREATE TABLE DASHBOARDFILTER (DASHBOARDITEMID INT NOT NULL,
        WEATHERSTATIONID VARCHAR(36) NOT NULL,
PRIMARY KEY (DASHBOARDITEMID, WEATHERSTATIONID));

ALTER TABLE DASHBOARDFILTER ADD FOREIGN KEY (DASHBOARDITEMID) REFERENCES DASHBOARDITEM (ID) ON DELETE CASCADE;
ALTER TABLE DASHBOARDFILTER ADD FOREIGN KEY (WEATHERSTATIONID) REFERENCES WEATHERSTATION (ID) ON DELETE CASCADE;

CREATE TABLE METAINFO (DBREVISION INTEGER NOT NULL);
