
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
INSERT INTO reporttype (id, type, keywords) VALUES (10, 'NY_DEMO_CJR', 'CJR,DEMO');
INSERT INTO reporttype (id, type, keywords) VALUES (11, 'NY_DEMO_CJR_PREMIER', 'CJR,DEMO,PREMIER');
INSERT INTO reporttype (id, type, keywords) VALUES (12, 'VT_GAP', 'GAP');
INSERT INTO reporttype (id, type, keywords) VALUES (13, 'CAPITATION_DASHBOARD', 'CAPITATION');
INSERT INTO reporttype (id, type, keywords) VALUES (14, 'ENCOUNTER_QUALITY_DASHBOARD', 'ENCOUNTER,QUALITY');
INSERT INTO reporttype (id, type, keywords) VALUES (15, 'OHIO_FINANCIAL_DASHBOARD', 'OHIO,FINANCIAL');
INSERT INTO reporttype (id, type, keywords) VALUES (16, 'NY_DEV_BPCI_USPI', 'DEV,BPCI,USPI');
INSERT INTO reporttype (id, type, keywords) VALUES (17, 'NY_LIVE_BPCI_USPI', 'LIVE,BPCI,USPI');
INSERT INTO reporttype (id, type, keywords) VALUES (18, 'NY_DEV_BPCI_CAM', 'DEV,BPCI,CAM');
INSERT INTO reporttype (id, type, keywords) VALUES (19, 'NY_LIVE_BPCI_CAM', 'LIVE,BPCI,CAM');




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
	parsed_report_name TEXT;
	parsed_keywords TEXT;
	group_name TEXT;
	GUID_report_name TEXT;
	report_name TEXT;
BEGIN
	it := 0;			--This will keep track of how many records are updated
	
	--Goes through every report and attempts to match the reportname to the report type
	FOR r IN SELECT * from public.report ORDER BY r.id
	LOOP
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
		parsed_report_name := replace(parsed_report_name, 'REPORTING ', '');

		--Attempt to match the report name to any of the keywords in the report type table
		FOR rt IN SELECT * FROM public.reporttype
		LOOP
			parsed_keywords := '';

			parsed_keywords := replace(rt.keywords, ',', ' ');
			parsed_keywords := trim(both ' ' from parsed_keywords);		--Turn the keywords into a string that can match up with the report name

			IF parsed_report_name LIKE '%' || parsed_keywords || ' %' OR parsed_report_name = parsed_keywords THEN
				it := it + 1;
				update public.report set fk_report_type_id = rt.id where id = r.id;		--When a match is found we update the table
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


--Call the above function
SELECT * FROM public.report_type_update();
