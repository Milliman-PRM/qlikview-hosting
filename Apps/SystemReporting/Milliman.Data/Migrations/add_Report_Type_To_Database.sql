
--Creates the reporttype table 
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

--Populate the report type table
INSERT INTO reporttype (id, type, keywords) VALUES (1, 'CARE_COORDINATOR_REPORT', 'CARE,COORDINATOR');
INSERT INTO reporttype (id, type, keywords) VALUES (2, 'COST_MODEL_DASHBOARD', 'COST,MODEL');
INSERT INTO reporttype (id, type, keywords) VALUES (3, 'PRCA', 'PRCA');
INSERT INTO reporttype (id, type, keywords) VALUES (4, 'NY_DEV_BPCI_PREMIER', 'DEV,BPCI,PREMIER');
INSERT INTO reporttype (id, type, keywords) VALUES (5, 'NY_LIVE_BPCI_PREMIER', 'LIVE,BPCI,PREMIER');
INSERT INTO reporttype (id, type, keywords) VALUES (6, 'NY_DEMO_BPCI_PREMIER', 'BPCI,DEMO,PREMIER');
INSERT INTO reporttype (id, type, keywords) VALUES (7, 'NY_DEMO_BPCI_MILLIMAN', 'BPCI,DEMO,MILLIMAN');
INSERT INTO reporttype (id, type, keywords) VALUES (8, 'NY_DEV_CJR', 'DEV,BPCI,CJR');
INSERT INTO reporttype (id, type, keywords) VALUES (9, 'NY_LIVE_CJR', 'LIVE,BPCI,CJR');
INSERT INTO reporttype (id, type, keywords) VALUES (10, 'NY_DEMO_CJR', 'CJR,REPORTING,DEMO');
INSERT INTO reporttype (id, type, keywords) VALUES (11, 'NY_DEMO_CJR_PREMIER', 'CJR,DEMO,PREMIER');
INSERT INTO reporttype (id, type, keywords) VALUES (12, 'VT_GAP', 'GAP');
INSERT INTO reporttype (id, type, keywords) VALUES (13, 'CAPITATION_DASHBOARD', 'CAPITATION');
INSERT INTO reporttype (id, type, keywords) VALUES (14, 'ENCOUNTER_QUALITY_DASHBOARD', 'ENCOUNTER,QUALITY');
INSERT INTO reporttype (id, type, keywords) VALUES (15, 'OHIO_FINANCIAL_DASHBOARD', 'OHIO,FINANCIAL');
INSERT INTO reporttype (id, type, keywords) VALUES (16, 'NY_DEV_BPCI_USPI', 'DEV,BPCI,USPI');
INSERT INTO reporttype (id, type, keywords) VALUES (17, 'NY_LIVE_BPCI_USPI', 'LIVE,BPCI,USPI');
INSERT INTO reporttype (id, type, keywords) VALUES (18, 'NY_DEV_BPCI_CAM', 'DEV,BPCI,CAM');
INSERT INTO reporttype (id, type, keywords) VALUES (19, 'NY_LIVE_BPCI_CAM', 'LIVE,BPCI,CAM');


CREATE TABLE temp
(
  id integer NOT NULL DEFAULT nextval('report_id_seq'::regclass),
  reportname character varying(100) NOT NULL,
  reportdescription character varying(100),
  adddate timestamp with time zone,
  fk_report_type_id integer,
  CONSTRAINT pk_reports_id PRIMARY KEY (id),
  CONSTRAINT fk_report_typse_id FOREIGN KEY (fk_report_type_id)
      REFERENCES public.reporttype (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT uq_reports_reportname UNIQUE (reportname)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.reporttype
  OWNER TO postgres;

--Alter the report table to accomodate the report type field and create foreign key reference
  ALTER TABLE public.report
    DROP COLUMN reporttype,
    ADD fk_report_type_id INTEGER,
    ADD CONSTRAINT fk_report_type_id FOREIGN KEY(fk_report_type_id) REFERENCES public.reporttype (id)
               ON UPDATE NO ACTION ON DELETE NO ACTION;


--Create the function to update all of the pre exsisting reports
-- Function: public.report_type_update()

-- DROP FUNCTION public.report_type_update();

CREATE OR REPLACE FUNCTION public.report_type_update()
  RETURNS integer AS
$BODY$



DECLARE
	r report;
	rt reporttype;
	a qvauditlog;
	l qvauditlog;
	it integer;
	holding_id integer;
	parsed_report_name TEXT;
	parsed_keywords TEXT;
	group_name TEXT;
	GUID_report_name TEXT;
	report_name TEXT;
	Empty_String TEXT;
BEGIN
	it := 0;			--This will keep track of how many records are updated
	Empty_String = NULL;
	--Goes through every report and attempts to match the reportname to the report type
	FOR r IN SELECT * from public.report ORDER BY id
	LOOP
		holding_id := NULL;
		report_name := r.reportname;	
		IF report_name NOT LIKE '% %' AND LENGTH(report_name) = 32 THEN
			
			--If a GUID then go through the qvauditlog to match the GUID to a report
			FOR a IN SELECT * FROM public.qvauditlog WHERE document LIKE '%' || report_name || '%' LIMIT 1
			LOOP
				group_name := substring(a.document from 0 for position('REDUCEDCACHEDQVWS' in a.document));
				group_name := replace (group_name, '\', '\\');		--Deal with those pesky escape charachters
			END LOOP;
			
			--After we get the group directory we find the root report name of that directory. This is the name we will match to a type
			FOR l IN SELECT * FROM public.qvauditlog WHERE document LIKE '%' || group_name || '%' AND document NOT LIKE '%REDUCEDCACHEDQVWS%' LIMIT 1
			LOOP
				group_name := replace(group_name, '\\', '\');

				GUID_report_name := reverse(l.document);
				GUID_report_name := substring(GUID_report_name, position('.' in GUID_report_name) + 1, position('\' in GUID_report_name) - 5); --parse the directory to only get the report name
				GUID_report_name := reverse(GUID_report_name);

				report_name := GUID_report_name;
			END LOOP;

		END IF;
		
		--Replace some undesirable charachters or strings that could be in the report name
		parsed_report_name := '';
		parsed_report_name := replace(report_name, '- ', '');

		--Attempt to match the report name to any of the keywords in the report type table
		FOR rt IN SELECT * FROM public.reporttype
		LOOP
			
			parsed_keywords := '';

			parsed_keywords := replace(rt.keywords, ',', ' ');
			parsed_keywords := trim(both ' ' from parsed_keywords);		--Turn the keywords into a string that can match up with the report name

			IF parsed_report_name LIKE '%' || parsed_keywords || ' %' OR parsed_report_name = parsed_keywords THEN
				it := it + 1;
				holding_id = rt.id;
				--update public.report set fk_report_type_id = rt.id where id = r.id;		--When a match is found we update the table
			END IF;
				
		END LOOP;
		--update public.report set fk_report_type_id = temp_id where id = r.id;
		INSERT INTO public.temp (id, reportname, reportdescription, adddate, fk_report_type_id) VALUES (r.id, r.reportname, Empty_String, r.adddate, holding_id);
	END LOOP;
RETURN it;			--Returns the amount of fields with new report types



END;








$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION public.report_type_update()
  OWNER TO postgres;


--Call the above function
SELECT * FROM public.report_type_update();

  ALTER TABLE public.qvsessionlog
	DROP CONSTRAINT fk_tbl_report_id;

  ALTER TABLE public.qvauditlog
	DROP CONSTRAINT fk_tbl_report_id;

DROP VIEW public.view_activity_log;
DROP VIEW public.view_session_log;
DROP TABLE public.report;

ALTER TABLE public.temp
	RENAME TO report;

ALTER TABLE public.qvauditlog
ADD  CONSTRAINT fk_tbl_report_id FOREIGN KEY (fk_report_id)
      REFERENCES public.report (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION;

ALTER TABLE public.qvsessionlog
ADD   CONSTRAINT fk_tbl_report_id FOREIGN KEY (fk_report_id)
      REFERENCES public.report (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION;
	-- View: public.view_session_log

-- DROP VIEW public.view_session_log;

CREATE OR REPLACE VIEW public.view_session_log AS 
 WITH sessions AS (
         SELECT qvsessionlog.id,
            tstzrange(qvsessionlog.useraccessdatetime,
                CASE
                    WHEN qvsessionlog.sessionendreason::text = 'SESSIONTIMEOUT'::text THEN qvsessionlog.useraccessdatetime + qvsessionlog.sessionduration::interval - '00:30:00'::interval
                    ELSE qvsessionlog.useraccessdatetime + qvsessionlog.sessionduration::interval
                END, '[]'::text) AS sessionrange,
            qvsessionlog.fk_user_id,
            qvsessionlog.fk_group_id,
            qvsessionlog.fk_report_id
           FROM qvsessionlog
          ORDER BY qvsessionlog.fk_user_id, qvsessionlog.fk_group_id, qvsessionlog.fk_report_id, (tstzrange(qvsessionlog.useraccessdatetime,
                CASE
                    WHEN qvsessionlog.sessionendreason::text = 'SESSIONTIMEOUT'::text THEN qvsessionlog.useraccessdatetime + qvsessionlog.sessionduration::interval - '00:30:00'::interval
                    ELSE qvsessionlog.useraccessdatetime + qvsessionlog.sessionduration::interval
                END, '[]'::text))
        ), merged_sessions AS (
         SELECT sessions.fk_user_id,
            sessions.fk_group_id,
            sessions.fk_report_id,
            merge_ranges(array_agg(sessions.sessionrange)) AS mergedrange
           FROM sessions
          GROUP BY sessions.fk_user_id, sessions.fk_group_id, sessions.fk_report_id
        ), dataset AS (
         SELECT min(sessions.id) AS first_session,
            max(sessions.id) AS last_session,
            count(sessions.id) AS included_sessions,
            merged.mergedrange,
            merged.fk_user_id,
            merged.fk_group_id,
            merged.fk_report_id
           FROM merged_sessions merged
             JOIN sessions ON merged.fk_user_id = sessions.fk_user_id AND merged.fk_group_id = sessions.fk_group_id AND merged.fk_report_id = sessions.fk_report_id AND merged.mergedrange && sessions.sessionrange
          GROUP BY merged.fk_user_id, merged.fk_group_id, merged.fk_report_id, merged.mergedrange
        )
 SELECT dataset.first_session AS sessionid,
    dataset.included_sessions,
        CASE
            WHEN report.reportname::text ~~ '%CARE COORDINATOR REPORT%'::text THEN 'Care Coordinator Report'::text
            WHEN report.reportname::text ~~ '%COST MODEL DASHBOARD%'::text THEN 'Cost Model Dashboard'::text
            WHEN report.reportname::text ~~ '%PRCA%'::text THEN 'PRCA Report'::text
            WHEN report.reportname::text ~~ '%LIVE BPCI - CAM%'::text THEN 'Live BPCI - CAM'::text
            WHEN report.reportname::text ~~ '%LIVE BPCI - CJR%'::text THEN 'Live BPCI - CJR'::text
            WHEN report.reportname::text ~~ '%LIVE BPCI - PREMIER%'::text THEN 'Live BPCI - Premier'::text
            WHEN report.reportname::text ~~ '%LIVE BPCI - USPI%'::text THEN 'Live BPCI - USPI'::text
            WHEN report.reportname::text ~~ '%OHIO FINANCIAL DASHBOARD%'::text THEN 'Ohio Financial Dashboard'::text
            WHEN report.reportname::text ~~ '%CAPITATION DASHBOARD%'::text THEN 'Capitation Dashboard'::text
            WHEN report.reportname::text ~~ '%ENCOUNTER QUALITY DASHBOARD%'::text THEN 'Encounter Quality Dashboard'::text
            WHEN report.reportname::text ~~ '%PIHP SUBMITTED ENCOUNTER DASHBOARD%'::text THEN 'PIHP Submitted Encounter Dashboard'::text
            WHEN report.reportname::text ~~ '%GAP.QVW%'::text THEN 'GAP'::text
            WHEN report.reportname::text ~~ '%SOC WC%'::text THEN 'SOC WC'::text
            WHEN report.reportname::text ~~ '%LOAD TABLES DVW.QVW'::text THEN 'Load Tables DVW'::text
            WHEN report.reportname::text ~~ '%VERMONT POC.QVW'::text THEN 'Vermont PoC'::text
            ELSE 'Other/Unknown'::text
        END AS document_type,
    lower(dataset.mergedrange) AS session_start_time,
    upper(dataset.mergedrange) - lower(dataset.mergedrange) AS session_duration,
        CASE
            WHEN upper(lastsession.browser::text) ~~ '%ANDROID%'::text THEN 'Android'::text
            WHEN upper(lastsession.browser::text) ~~ '%CHROME%'::text THEN 'Chrome'::text
            WHEN upper(lastsession.browser::text) ~~ '%MSIE%'::text THEN 'MSIE'::text
            WHEN upper(lastsession.browser::text) ~~ '%FIREFOX%'::text OR upper(lastsession.browser::text) ~~ '%GECKO%'::text THEN 'Firefox'::text
            WHEN upper(lastsession.browser::text) ~~ '%SAFARI%'::text THEN 'Safari'::text
            ELSE 'Unknown'::text
        END AS browser_type,
        CASE
            WHEN upper(lastsession.browser::text) ~~ '%CHROME%'::text THEN 'Chrome'::character varying
            WHEN upper(lastsession.browser::text) ~~ '%GECKO%'::text THEN replace(lastsession.browser::text, 'GECKO'::text, 'Firefox'::text)::character varying
            WHEN lastsession.browser::text = 'msie'::text THEN 'MSIE'::character varying
            WHEN lastsession.browser::text = 'safari'::text THEN 'Safari'::character varying
            WHEN lastsession.browser::text = ANY (ARRAY[''::character varying::text, NULL::character varying::text]) THEN 'Unknown'::character varying
            ELSE lastsession.browser
        END AS browser_version,
    dataset.fk_user_id AS userid,
    dataset.fk_group_id AS groupid,
    dataset.fk_report_id AS reportid
   FROM dataset
     JOIN report ON dataset.fk_report_id = report.id
     JOIN qvsessionlog lastsession ON dataset.last_session = lastsession.id
  ORDER BY dataset.included_sessions DESC;

ALTER TABLE public.view_session_log
  OWNER TO postgres;


  -- View: public.view_activity_log

-- DROP VIEW public.view_activity_log;

CREATE OR REPLACE VIEW public.view_activity_log WITH (security_barrier=true) AS 
 WITH fetch_names AS (
         SELECT qvauditlog.id,
            selected_fields.field_name,
            char_length(selected_fields.field_name) = max(char_length(selected_fields.field_name)) OVER (PARTITION BY qvauditlog.id ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS longest_name
           FROM qvauditlog
             JOIN selected_fields ON qvauditlog.message ~~ (selected_fields.field_name || '%'::text)
          WHERE qvauditlog.eventtype::text = ANY (ARRAY['Selection'::character varying::text, 'Bookmark Selection'::character varying::text])
        ), sessions AS (
         SELECT view_session_log.sessionid,
            view_session_log.userid,
            view_session_log.groupid,
            view_session_log.reportid,
            tstzrange(view_session_log.session_start_time, view_session_log.session_start_time + view_session_log.session_duration, '[]'::text) AS session_range
           FROM view_session_log
        )
 SELECT sessions.sessionid,
    log.useraccessdatetime AS actiondatetime,
    log.eventtype AS event_type,
        CASE
            WHEN (log.eventtype::text = ANY (ARRAY['Selection'::character varying::text, 'Bookmark Selection'::character varying::text])) AND fetch_names.field_name IS NOT NULL THEN fetch_names.field_name
            WHEN (log.eventtype::text = ANY (ARRAY['Selection'::character varying::text, 'Bookmark Selection'::character varying::text])) AND log.message <> 'Clear All'::text THEN 'Unmatched Field'::text
            ELSE log.message
        END AS event_message
   FROM qvauditlog log
     LEFT JOIN fetch_names ON log.id = fetch_names.id AND fetch_names.longest_name = true
     JOIN sessions ON log.fk_user_id = sessions.userid AND log.fk_group_id = sessions.groupid AND log.fk_report_id = sessions.reportid AND log.useraccessdatetime <@ sessions.session_range;

ALTER TABLE public.view_activity_log
  OWNER TO postgres;
