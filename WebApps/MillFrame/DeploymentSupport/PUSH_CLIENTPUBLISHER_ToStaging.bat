ROBOCOPY PRMClientPublisher ../PRM_Staging/PRMClientPublisher /MIR 
powershell -Command "(gc ../PRM_Staging/PRMClientPublisher/web.config) -replace 'appsettings_PreStag', 'appsettings_Stag' | Out-File ../PRM_Staging/PRMClientPublisher/web.config"
powershell -Command "(gc ../PRM_Staging/PRMClientPublisher/web.config) -replace 'connectionStrings_PreStag', 'connectionStrings_Stag' | Out-File ../PRM_Staging/PRMClientPublisher/web.config"



