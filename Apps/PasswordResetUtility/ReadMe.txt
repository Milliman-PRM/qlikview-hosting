PasswordRestUtility Application Description:

This application is designed to query the membership database and evaluate each membership user and measure the time span in days to detect when the user changed password last time by evaluating the DB filed value LastPasswordChangedDate for each user. 

Requirements: This application requires SQLExpress and Membership database.

/***********************************************************************************/
Processing Instruction. Choose following methods for the application Execution... 

1. Application and userEmail					
		Ex: PasswordResetUtilityApplication.exe  'abs@somthing.com' 
2. Application only 							
		Ex: PasswordResetUtilityApplication.exe


/***********************************************************************************/
This program depends on following appSettings for processing of log files:

	/----------------------TODO: we will change the directory C to D ----------------------/
  -- Specifying the directory location where the password reset files will be created
  <add key="UserProfileDirectory" value="C:\ResetUserInfoRoot" />

  -- For Exception logs recordings
  <add key="ExceptionLoggerDirectory" value="C:\ResetUserInfoRoot\Exceptions" />

  -- Specifying the max expiration 'days' that will be used to match against DB field LastPasswordChangedDate value to detect if the password needs to reset
  <add key="PasswordExpiresInDays" value="45"/>

/***********************************************************************************/ Application Key values:

  <add key="ProcessAllUsers" value="True"/> 
  **This key will force application to process all user in the membership database only if the value is set to "True"
  ** Case in-sensitive ** 

  <add key="ProcessSingleUser" value="test@test.com"/>
  **This key will force application to process one user in the membership database only if the value is set to "userEmailAddress"

  <add key="FileGenerateCounter" value="50"/>
  **This key will force application to generate exact number of files if the value is set to "integer_Number"
  **In order to generate the exact number of files it is required to set the "ProcessAllUsers" to "true"
  
  <add key="DirectoryCleanUp" value="TRUE"/>
  **This key will force application to perform the directory clean up and delete any file that does not have a corresponding or matching record in the membership DB
    

/***********************************************************************************/ 
	This application can be executed by setting multiple variables and conditions
 /----------------------------------------------------------------------------------------------------------------------/
  <!--senario1: Assuming you want to process the whole membership db to generate all users in the db, and you 
  want the application to perform the directory clean up-->

  <!--
	if the value for "ProcessAllUsers" is set to "True", 
			 value for "DirectoryCleanUp" is also set to "True", 
	then application will process all users and generate one file per user in db. 
	After the files are generated, application will perform a directory clean up to make sure 
	that there are no "un-wanted" files or the files that does not have a user record in db exist in the Directory 
	-->

  <add key="ProcessAllUsers" value="True"/>
  <add key="ProcessSingleUser" value=""/>
  <add key="FileGenerateCounter" value=""/>
  <add key="DirectoryCleanUp" value="True"/>

/----------------------------------------------------------------------------------------------------------------------/
 <!--senario2: Assuming you want to process the whole membership db to generate all users in the db, and you want the application to perform the directory clean up-->

  <!--
if the value for "ProcessAllUsers" is set to "True", 
			 value for "FileGenerateCounter" is set to a numeric number, 
			 value for "DirectoryCleanUp" is also set to "True", 
	then application will process all users but generate only specific number of files 
	that is defined in "FileGenerateCounter". 
	After the files are generated, application will perform a directory clean up to make sure that there are no un wanted files in the Directory 
-->

  <add key="ProcessAllUsers" value="True"/>
  <add key="ProcessSingleUser" value=""/>
  <add key="FileGenerateCounter" value="50"/>
  <add key="DirectoryCleanUp" value="TRUE"/>

/----------------------------------------------------------------------------------------------------------------------/
<!--senario3: Assuming you want to process a specific user in db, and you want the application to perform the directory clean up-->

  <!--
if the value for "ProcessAllUsers" is set to "NO" or "" [empty strings], 
			 value for "FileGenerateCounter" is set to "" [empty strings], 
			 value for "DirectoryCleanUp" is also set to "no", 
	then application will process that specific user and generate only that user file.
	After the file is generated, application will not perform a directory clean up 
-->

  <add key="ProcessAllUsers" value="No"/>
  OR
  <add key="ProcessAllUsers" value=" "/>
  <add key="ProcessSingleUser" value="someone@somthing.org"/>
  <add key="FileGenerateCounter" value=""/>
  <add key="DirectoryCleanUp" value=" NO"/>


	YOU CAN CREATE YOUR OWN SENARIOS BY USING THE FOLLOWING KEYS
/----------------------------------------------------------------------------------------------------------------------/
-- Acceptable Processing values for program to process that specific action / task or to execute that part of code
	
	/------------ ProcessAllUsers --------------/
	<add key="ProcessAllUsers" value="True"/>			[Function Will be processed]  
	OR
	<add key="ProcessAllUsers" value="TRUE"/>			[Function Will be processed]  
	OR
	<add key="ProcessAllUsers" value="NO"/>			[Function will not process]
	OR 
	<add key="ProcessAllUsers" value=""/>				[Function will not process]

	/------------ ProcessSingleUser --------------/
	<add key="ProcessSingleUser" value="someone@somthing.org"/> 	[Function Will be processed]  
	OR
	<add key="ProcessSingleUser" value=""/>			 [Function will not process]

	/------------ FileGenerateCounter --------------/
	<add key="FileGenerateCounter" value="2"/>			[Function Will be processed]  
	OR
	<add key="FileGenerateCounter" value=""/>			[Function will not process]
  
  	/------------ DirectoryCleanUp --------------/
	<add key="DirectoryCleanUp" value="True"/>			[Function Will be processed]  
	OR
	<add key="DirectoryCleanUp" value="TRUE"/>			[Function Will be processed]  
	OR
	<add key="DirectoryCleanUp" value="no"/>			[Function will not process]
	OR
	<add key="DirectoryCleanUp" value=""/>			[Function will not process]
