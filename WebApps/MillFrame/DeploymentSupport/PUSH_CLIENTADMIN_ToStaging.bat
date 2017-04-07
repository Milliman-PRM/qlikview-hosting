ROBOCOPY PRMClientAdmin ../PRM_Staging/PRMClientAdmin /MIR
powershell -Command "(gc ../PRM_Staging/PRMClientAdmin/web.config) -replace 'appsettings_PreStag', 'appsettings_Stag'  | Out-File ../PRM_Staging/PRMClientAdmin/web.config"
powershell -Command "(gc ../PRM_Staging/PRMClientAdmin/web.config) -replace 'connectionStrings_PreStag', 'connectionStrings_Stag' | Out-File ../PRM_Staging/PRMClientAdmin/web.config"


