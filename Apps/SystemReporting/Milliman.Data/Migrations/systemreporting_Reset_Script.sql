/***************************************************************************************************************
	Truncate Cascade reset identity 
	Owner: Afsheen Khan
	Date Created/Modified : 02/09/2015
	Desc: This script will clear data and reset the identity seed to 1
****************************************************************************************************************/

/***************************************
	Truncate Cascade reset identity
****************************************/
TRUNCATE TABLE "user" restart identity CASCADE;
TRUNCATE TABLE "group" restart identity CASCADE;
TRUNCATE TABLE "report" restart identity CASCADE;


TRUNCATE TABLE "iislog" restart identity CASCADE;
TRUNCATE TABLE "qvauditlog" restart identity CASCADE;
TRUNCATE TABLE "qvsessionlog" restart identity CASCADE;


/***************************************
	Reset Sequence if it does not reset
****************************************/

ALTER SEQUENCE group_id_seq RESTART WITH 1;
ALTER SEQUENCE iislog_id_seq RESTART WITH 1;
ALTER SEQUENCE qvauditlog_id_seq RESTART WITH 1;
ALTER SEQUENCE qvsessionlog_id_seq RESTART WITH 1;
ALTER SEQUENCE report_id_seq RESTART WITH 1;
ALTER SEQUENCE user_id_seq RESTART WITH 1;



