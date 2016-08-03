/***************************************************************************************************************
	Create Database Script 
	Owner: Afsheen Khan
	Date Created/Modified : 02/09/2015
	Desc: This script will generate db
****************************************************************************************************************/

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
  username character varying(100) NOT NULL,
  adddate timestamp with time zone,
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
  groupname character varying(100) NOT NULL,
  groupdescription character varying(100),
  adddate timestamp with time zone,
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


-- Table: public.reporttype

-- DROP TABLE public.reporttype;

CREATE TABLE reporttype
(
  id serial NOT NULL,
  type character varying(100) NOT NULL,
  keywords character varying(100) NOT NULL,
  CONSTRAINT pk_reporttype_id PRIMARY KEY (id),
  CONSTRAINT uq_reporttype_type UNIQUE (type)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.reporttype
  OWNER TO postgres;



-- Table: report

-- DROP TABLE report;

CREATE TABLE report
(
  id serial NOT NULL,
  reportname character varying(100) NOT NULL,
  reportdescription character varying(100),
  adddate timestamp with time zone,
  fk_report_type_id integer,
  CONSTRAINT pk_report_id PRIMARY KEY (id),
  CONSTRAINT uq_report_reportname UNIQUE (reportname),
  CONSTRAINT fk_tbl_reporttype_id FOREIGN KEY (fk_report_type_id)
      REFERENCES "reporttype" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE report
  OWNER TO postgres;
GRANT ALL ON TABLE report TO postgres WITH GRANT OPTION;
GRANT ALL ON TABLE report TO public;





/*****************************************
			Reporting Logs Tables
*****************************************/

-- Table: iislog

-- DROP TABLE iislog;

CREATE TABLE iislog
(
  id serial NOT NULL,
  useraccessdatetime timestamp with time zone,
  clientipaddress character varying(15),
  serveripaddress character varying(15),
  portnumber integer,
  commandsentmethod character varying(10),
  stepuri character varying(50),
  queryuri text,
  statuscode integer,
  substatuscode integer,
  win32statuscode integer,
  responsetime integer,
  useragent text,
  clientreferer text,
  browser character varying(25),
  eventtype character varying(25),
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




-- Table: qvauditlog

-- DROP TABLE qvauditlog;

CREATE TABLE qvauditlog
(
  id serial NOT NULL,
  useraccessdatetime timestamp with time zone,
  document text,
  eventtype character varying(25),
  message text,
  isreduced boolean,
  fk_user_id integer,
  fk_group_id integer,
  fk_report_id integer,
  adddate timestamp with time zone,
  CONSTRAINT pk_qvauditlog_id PRIMARY KEY (id),
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
ALTER TABLE qvauditlog
  OWNER TO postgres;
GRANT ALL ON TABLE qvauditlog TO postgres WITH GRANT OPTION;
GRANT ALL ON TABLE qvauditlog TO public;





-- Table: qvsessionlog

-- DROP TABLE qvsessionlog;

CREATE TABLE qvsessionlog
(
  id serial NOT NULL,
  useraccessdatetime timestamp with time zone,
  document text,
  exitreason character varying(75),
  sessionstarttime timestamp with time zone,
  sessionduration character varying(10),
  sessionendreason character varying(25),
  cpuspents double precision,
  clienttype character varying(75),
  clientaddress character varying(25),
  caltype character varying(20),
  calusagecount integer,
  browser character varying(25),
  isreduced boolean,
  fk_user_id integer,
  fk_group_id integer,
  fk_report_id integer,
  adddate timestamp with time zone,
  CONSTRAINT pk_qvsessionlog_id PRIMARY KEY (id),
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
ALTER TABLE qvsessionlog
  OWNER TO postgres;
GRANT ALL ON TABLE qvsessionlog TO postgres WITH GRANT OPTION;
GRANT ALL ON TABLE qvsessionlog TO public;




