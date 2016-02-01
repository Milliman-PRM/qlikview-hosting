-- Database: "systemreporting"

-- DROP DATABASE "systemreporting";

CREATE DATABASE systemreporting
  WITH OWNER = postgres
       ENCODING = 'UTF8'
       TABLESPACE = pg_default
       LC_COLLATE = 'English_United States.1252'
       LC_CTYPE = 'English_United States.1252'
       CONNECTION LIMIT = -1;
GRANT ALL ON DATABASE "systemreporting" TO public;
GRANT ALL ON DATABASE "systemreporting" TO postgres WITH GRANT OPTION;


/*********************************************************************
			Look Up Tables
**********************************************************************/
-- Table: "user"

-- DROP TABLE "user";

CREATE TABLE "user"
(
  id serial NOT NULL,
  username character(100) NOT NULL,
  CONSTRAINT pk_user_id PRIMARY KEY (id),
  CONSTRAINT uq_user_username UNIQUE (username)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "user"
  OWNER TO postgres;
GRANT ALL ON TABLE "user" TO postgres WITH GRANT OPTION;
GRANT ALL ON TABLE "user" TO public;


-- Table: "group"

-- DROP TABLE "group";

CREATE TABLE "group"
(
  id serial NOT NULL,
  groupname character(100) NOT NULL,
  groupdescription character(100),
  CONSTRAINT pk_group_id PRIMARY KEY (id),
  CONSTRAINT uq_group_groupname UNIQUE (groupname)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "group"
  OWNER TO postgres;
GRANT ALL ON TABLE "group" TO postgres WITH GRANT OPTION;
GRANT ALL ON TABLE "group" TO public;



-- Table: report

-- DROP TABLE report;

CREATE TABLE report
(
  id serial NOT NULL,
  reportname character(100) NOT NULL,
  reportdescription character(100),
  CONSTRAINT pk_report_id PRIMARY KEY (id),
  CONSTRAINT uq_report_reportname UNIQUE (reportname)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE report
  OWNER TO postgres;
GRANT ALL ON TABLE report TO postgres WITH GRANT OPTION;
GRANT ALL ON TABLE report TO public;



/*********************************************************************
			Reporting Logs Tables
**********************************************************************/

-- Table: iislog

-- DROP TABLE iislog;

CREATE TABLE iislog
(
  id serial NOT NULL,
  logcreatedate date,
  logcreatetime timestamp with time zone,
  clientipaddress character(15),
  username character(30),
  serveripaddress character(15),
  portnumber integer,
  commandsentmethod character(10),
  stepuri character(50),
  queryuri text,
  statuscode integer,
  substatuscode integer,
  win32statuscode integer,
  responsetime integer,
  useragent text,
  clientreferer text,
  browser character(25),
  eventtype character(50),
  adddate timestamp with time zone,
  fk_user_id integer,
  fk_group_id integer,
  CONSTRAINT pk_iislog_id PRIMARY KEY (id),
  CONSTRAINT fk_tbl_group_id FOREIGN KEY (fk_group_id)
      REFERENCES "group" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT fk_tbl_user_id FOREIGN KEY (fk_user_id)
      REFERENCES "user" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE iislog
  OWNER TO postgres;
GRANT ALL ON TABLE iislog TO postgres WITH GRANT OPTION;
GRANT ALL ON TABLE iislog TO public;



-- Table: qlickviewauditlog

-- DROP TABLE qlickviewauditlog;

CREATE TABLE qlickviewauditlog
(
  id serial NOT NULL,
  serverstarted timestamp(0) with time zone,
  "timestamp" timestamp with time zone,
  document text,
  eventtype character(50),
  username character(30),
  message text,
  isreduced boolean,
  fk_user_id integer,
  fk_group_id integer,
  fk_report_id integer,
  adddate timestamp with time zone,
  CONSTRAINT pk_qlickviewauditlog_id PRIMARY KEY (id),
  CONSTRAINT fk_tbl_group_id FOREIGN KEY (fk_group_id)
      REFERENCES "group" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT fk_tbl_report_id FOREIGN KEY (fk_report_id)
      REFERENCES report (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT fk_tbl_user_id FOREIGN KEY (fk_user_id)
      REFERENCES "user" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE qlickviewauditlog
  OWNER TO postgres;
GRANT ALL ON TABLE qlickviewauditlog TO postgres WITH GRANT OPTION;
GRANT ALL ON TABLE qlickviewauditlog TO public;



-- Table: qlickviewsessionlog

-- DROP TABLE qlickviewsessionlog;

CREATE TABLE qlickviewsessionlog
(
  id serial NOT NULL,
  document text,
  exitreason character(50),
  sessionstarttime timestamp with time zone,
  sessionduration integer,
  sessionendreason character(25),
  cpuspents double precision,
  identifyinguser character(30),
  clienttype character(50),
  clientaddress character(25),
  caltype character(20),
  calusagecount integer,
  browser character(25),
  isreduced boolean,
  fk_user_id integer,
  fk_group_id integer,
  fk_report_id integer,
  adddate timestamp with time zone,
  CONSTRAINT pk_qlickviewsessionlog_id PRIMARY KEY (id),
  CONSTRAINT fk_tbl_group_id FOREIGN KEY (fk_group_id)
      REFERENCES "group" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT fk_tbl_report_id FOREIGN KEY (fk_report_id)
      REFERENCES report (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT fk_tbl_user_id FOREIGN KEY (fk_user_id)
      REFERENCES "user" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE qlickviewsessionlog
  OWNER TO postgres;
GRANT ALL ON TABLE qlickviewsessionlog TO postgres WITH GRANT OPTION;
GRANT ALL ON TABLE qlickviewsessionlog TO public;
