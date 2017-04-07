ROBOCOPY PRM_ProjectManagementConsole ../PRM_Staging/PRM_ProjectManagementConsole /MIR
ROBOCOPY PRM_ProjectManagementServices ../PRM_Staging/PRM_ProjectManagementServices /MIR

REM alter references in PMC web.config from prestaging to staging
powershell -Command "(gc ../PRM_Staging/PRM_ProjectManagementConsole/web.config) -replace 'appsettings_PreStag_PMC.config', 'appsettings_Stag_PMC.config' | Out-File ../PRM_Staging/PRM_ProjectManagementConsole/web.config"
powershell -Command "(gc ../PRM_Staging/PRM_ProjectManagementConsole/web.config) -replace 'connectionStrings_PreStag.config', 'connectionStrings_Stag.config' | Out-File ../PRM_Staging/PRM_ProjectManagementConsole/web.config"
powershell -Command "(gc ../PRM_Staging/PRM_ProjectManagementConsole/web.config) -replace 'prm_projectmanagementservices_prestaging', 'prm_projectmanagementservices_staging' | Out-File ../PRM_Staging/PRM_ProjectManagementConsole/web.config"

REM alter references in PMC mappingfile.mapping from prestaging to staging
powershell -Command "(gc ../PRM_Staging/PRM_ProjectManagementConsole/AppCode/MappingFile.mapping) -replace 'PRM_Staging_PreStaging', 'PRM_Staging' | Out-File ../PRM_Staging/PRM_ProjectManagementConsole/AppCode/MappingFile.mapping"

REM alter references in ProjectManagementServices web.config from prestaging to staging
powershell -Command "(gc ../PRM_Staging/PRM_ProjectManagementServices/web.config) -replace 'PortalDB_PreStaging', 'PortalDB_Staging' | Out-File ../PRM_Staging/PRM_ProjectManagementServices/web.config"
powershell -Command "(gc ../PRM_Staging/PRM_ProjectManagementServices/web.config) -replace 'PRM_PreStaging', 'PRM_Staging' | Out-File ../PRM_Staging/PRM_ProjectManagementServices/web.config"
powershell -Command "(gc ../PRM_Staging/PRM_ProjectManagementServices/web.config) -replace 'PRM_Staging_PreStaging', 'PRM_Staging' | Out-File ../PRM_Staging/PRM_ProjectManagementServices/web.config"
