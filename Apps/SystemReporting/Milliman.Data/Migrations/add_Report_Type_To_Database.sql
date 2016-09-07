
 --/*********************************************************************************************************************/--
  --/*** This code has been tested and verified and must be used						AK 08/15/2016	*****************/--
   --/*********************************************************************************************************************/--

--1 Creates the reporttype table 
CREATE TABLE reporttype
(
  id serial NOT NULL,
  type character varying(100) NULL,
  keywords character varying(100) NULL,
  CONSTRAINT pk_reporttype_id PRIMARY KEY (id),
  CONSTRAINT uq_reporttype_type UNIQUE (type)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.reporttype
  OWNER TO postgres;

 --/*********************************************************************************************************************/--

--2. Populate the report type table
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


 --/*********************************************************************************************************************/--

--3. Alter the report table to accomodate the report type field and create foreign key reference
  ALTER TABLE public.report
    DROP COLUMN reporttype,
	ADD fk_report_type_id INTEGER,
		ADD CONSTRAINT fk_report_type_id FOREIGN KEY(fk_report_type_id) REFERENCES public.reporttype (id)
               ON UPDATE NO ACTION ON DELETE NO ACTION;



--4. Create Temp Table to Update Report Data
CREATE TABLE public.report_temp (LIKE public.report INCLUDING ALL);
ALTER TABLE public.report_temp ALTER id DROP DEFAULT;
CREATE SEQUENCE report_temp_id_seq;
INSERT INTO public.report_temp SELECT * FROM public.report order by id;
SELECT setval('report_temp_id_seq', (SELECT max(id) FROM public.report), true);
ALTER TABLE public.report ALTER id SET DEFAULT nextval('report_id_seq');

--5. Create Temp Table to qvauditlog
CREATE TABLE public.qvauditlog_temp (LIKE public.qvauditlog INCLUDING ALL);
ALTER TABLE public.qvauditlog_temp ALTER id DROP DEFAULT;
CREATE SEQUENCE qvauditlog_temp_id_seq;
INSERT INTO public.qvauditlog_temp SELECT * FROM public.qvauditlog order by id;
SELECT setval('qvauditlog_temp_id_seq', (SELECT max(id) FROM public.qvauditlog), true);
ALTER TABLE public.qvauditlog ALTER id SET DEFAULT nextval('qvauditlog_id_seq');

 --/*********************************************************************************************************************/--

--6. Create the function to update all of the pre exsisting reports
-- Function: public.report_type_update()

-- DROP FUNCTION public.report_type_update();

CREATE OR REPLACE FUNCTION public.report_type_update()
  RETURNS integer AS
$BODY$

DECLARE
	r report_temp;
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
	FOR r IN SELECT * from public.report_temp ORDER BY id
	LOOP
		holding_id := NULL;
		report_name := r.reportname;	
		IF report_name NOT LIKE '% %' AND LENGTH(report_name) = 32 THEN
			
			--If a GUID then go through the qvauditlog to match the GUID to a report
			FOR a IN SELECT * FROM public.qvauditlog_temp WHERE document LIKE '%' || report_name || '%' LIMIT 1
			LOOP
				group_name := substring(a.document from 0 for position('REDUCEDCACHEDQVWS' in a.document));
				group_name := replace (group_name, '\', '\\');		--Deal with those pesky escape charachters
			END LOOP;
			
			--After we get the group directory we find the root report name of that directory. This is the name we will match to a type
			FOR l IN SELECT * FROM public.qvauditlog_temp WHERE document LIKE '%' || group_name || '%' AND document NOT LIKE '%REDUCEDCACHEDQVWS%' LIMIT 1
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
		parsed_report_name := replace(parsed_report_name, 'REPORTING ', ''); --it was in commit# c37a55c9
		
		--Attempt to match the report name to any of the keywords in the report type table
		FOR rt IN SELECT * FROM public.reporttype
		LOOP
			parsed_keywords := '';
			parsed_keywords := replace(rt.keywords, ',', ' ');
			parsed_keywords := trim(both ' ' from parsed_keywords);		--Turn the keywords into a string that can match up with the report name

			IF parsed_report_name LIKE '%' || parsed_keywords || ' %' OR parsed_report_name = parsed_keywords THEN
				it := it + 1;
				UPDATE public.report_temp set fk_report_type_id = rt.id where id = r.id;		--When a match is found we update the table
			END IF;

		END LOOP;
	END LOOP;	
RETURN it;			--Returns the amount of fields with new report types

END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION public.report_type_update()
  OWNER TO postgres;


--/*********************************************************************************************************************/--
--7. Call/Execute the above function
SELECT * FROM public.report_type_update();
--/*********************************************************************************************************************/--
--/*********************************************************************************************************************/--
--8. Drop the FK Contraint from qvauditlog & qvsessionlog
ALTER TABLE public.qvauditlog DROP CONSTRAINT fk_tbl_report_id;
ALTER TABLE public.qvsessionlog DROP CONSTRAINT fk_tbl_report_id;

--9. Truncate the report table
TRUNCATE TABLE "report";
--10. reset the sequence
ALTER SEQUENCE report_id_seq RESTART WITH 1;

--11. Insert into report data from temp
INSERT INTO public.report(id, reportname, reportdescription, adddate, fk_report_type_id)
	SELECT id, reportname, reportdescription, adddate, fk_report_type_id
	FROM report_temp Order by id;
	
--12. Re-establish the qvauditlog & qvsessionlog constraint
ALTER TABLE public.qvauditlog ADD CONSTRAINT fk_tbl_report_id FOREIGN KEY(fk_report_id) REFERENCES public.report (id);
ALTER TABLE public.qvsessionlog ADD CONSTRAINT fk_tbl_report_id FOREIGN KEY(fk_report_id) REFERENCES public.report (id);
--/*********************************************************************************************************************/--
--RESEQUENCE THE REPORT

-- Login to psql and run the following
-- What is the result?
SELECT MAX(id) FROM public.report;

-- Then run...
-- This should be higher than the last result.
SELECT nextval('report_id_seq');

-- If it's not higher... run this set the sequence last to your highest pid it. 
-- (wise to run a quick pg_dump first...)
SELECT setval('report_id_seq', (SELECT MAX(id) FROM public.report));
-- if your tables might have no rows
-- false means the set value will be returned by the next nextval() call    
SELECT setval('report_id_seq', COALESCE((SELECT MAX(id)+1 FROM public.report), 1), false);

--/*********************************************************************************************************************/--
--13. Drop temp table
Drop Table report_temp
Drop Table qvauditlog_temp
Drop Table qvsessionlog_temp