# Code Owners: Ben Wyatt, Steve Gredell

### OBJECTIVE:
#  Run deployment steps on "testing server" to deploy MillFrame and components

### DEVELOPER NOTES:
#  Is pushed out to the "testing server" where it is picked up by a scheduled task and run

$zipPath = "D:\installedapplications\prm_ci\<<branch_name>>\publish.zip"
$unzipPath = "D:\installedapplications\prm_ci\<<branch_name>>\"
$outputPath = "D:\installedapplications\prm_ci\<<branch_name>>\error.log"
$urlFilePath = "D:\installedapplications\prm_ci\<<branch_name>>\urls.log"
$urlBase = "https://prm2.milliman.com"
$errorCode = 0

try
{
    $unzipCommand = "D:\InstalledApplications\7zip\7za.exe e $zipPath -o $unzipPath"
    Invoke-Command $unzipCommand
}
catch
{
    $errorCode = 3
}

# Clear URL file, if it exists
set-content -LiteralPath $urlFilePath "Published URLs:"

# Remove error.log file, if it exists
if (test-path $outputPath)
{
    remove-item $outputPath
}

# (Re-)create applications and log deployed URLs to text file
try 
{
    $apps = @{
                "/prm_ci_<<branch_name>>_ClientPublisher" = "D:\installedapplications\prm_ci\<<branch_name>>\ClientPublisher";
                "/prm_ci_<<branch_name>>_ClientAdmin" = "D:\installedapplications\prm_ci\<<branch_name>>\ClientUserAdmin";
                "/prm_ci_<<branch_name>>" = "D:\installedapplications\prm_ci\<<branch_name>>\Milliman";
                "/prm_ci_<<branch_name>>_MillimanServices" = "D:\installedapplications\prm_ci\<<branch_name>>\MillimanServices";
                "/prm_ci_<<branch_name>>_ProjectManagementConsoleServices" = "D:\installedapplications\prm_ci\<<branch_name>>\ProjectManagementConsoleServices";
                "/prm_ci_<<branch_name>>_ProjectManConsole" = "D:\installedapplications\prm_ci\<<branch_name>>\ProjectManConsole";
                "/prm_ci_<<branch_name>>_UserAdmin" = "D:\installedapplications\prm_ci\<<branch_name>>\UserAdmin"
                }

    $name = "CI_<<branch_name>>"
    $appPool = Get-ChildItem –Path IIS:\AppPools | where {$_.name -eq $name}

    # Create branch-specific app pool if it doesn't already exist
    if (-not $appPool)
    {
        $command = "C:\windows\system32\inetsrv\appcmd.exe add apppool /name:$name /managedRuntimeVersion:v4.0"
        invoke-expression $command 

        # Configuring credentials must be done separately from creating the application pool
        $command = "C:\windows\system32\inetsrv\appcmd.exe set config /section:applicationPools `"/[name='$name'].processModel.identityType:SpecificUser`" `"/[name='$name'].processModel.userName:indy-prm-2\MillimanAdmin`" `"/[name='$name'].processModel.password:<<pool_password>>`""
        invoke-expression $command
    }


    foreach ($app in $apps.GetEnumerator())
    {
        # Manipulate web.config contents
        $webConfigFilePath = [System.IO.Path]::Combine( $app.Value, "web.config" );
        $appConfigFilePath = [System.IO.Path]::Combine( $app.Value, "bin\Configs\appsettings_CI.config" );

        if ((test-path $appConfigFilePath) -eq $false)
        {
            $appConfigFilePath = [System.IO.Path]::Combine( $app.Value, "Configs\appsettings_CI.config" );
        }

        if ((Test-Path $webConfigFilePath) -eq $true)
        {
            $xml = [xml](get-content $webConfigFilePath)
            $root = $xml.get_DocumentElement();

            if ($root.connectionStrings.HasChildNodes)
            {
                # Some of the config files explicitly set the connection string, rather than pulling from template files
                # There are a couple different possible values based on current config files
                # Replace both with the _CI database
                $connectionString = $root.connectionStrings.ChildNodes.connectionString.Replace("PortalDB_Staging","PortalDB_CI")
                $connectionString = $root.connectionStrings.ChildNodes.connectionString.Replace("PortalDBSFv2M1","PortalDB_CI")
                $root.connectionStrings.ChildNodes[0].SetAttribute("connectionString", $connectionString)

            }
            else
            {
                # Web.config files without nested connectionStrings have a consistent format, but may be in different locations
                if ($app.Key -eq "/prm_ci_<<branch_name>>_UserAdmin")
                {
                     # User Admin puts its config files in a different location, so we need to handle that
                    $root.appSettings.configSource = "Configs\appsettings_CI.config"
                    $root.connectionStrings.configSource = "Configs\connectionStrings_CI.config"
                }
                else
                {
                    # Handle the standard configuration files
                    $root.appSettings.configSource = "bin\Configs\appsettings_CI.config"
                    $root.connectionStrings.configSource = "bin\Configs\connectionStrings_CI.config"
                }

            }

            # Replace ((branch_name)) with <<branch_name>> in appsettings_CI.config
            if (Test-Path $appConfigFilePath)
            {
                $appSettings = (get-content $appConfigFilePath).Replace("((branch_name))", "<<branch_name>>")
                Set-Content -LiteralPath $appConfigFilePath $appSettings
            }
            $xml.Save($webConfigFilePath)
        }

        # Replace QVDocuments path in PMC MappingFile.mapping: https://indy-github.milliman.com/PRM/qlikview-hosting/issues/142
        $mappingFile = "D:\InstalledApplications\PRM_CI\<<branch_name>>\ProjectManConsole\AppCode\MappingFile.mapping"
        Set-Content -LiteralPath $mappingFile (get-content $mappingFile).Replace("PRM_Staging_PreStaging\PMC_Directories", "PRM_CI_Support\Runtime")

        # If the web application already exists, remove it
        if ((Get-WebApplication $app.Key).Count -gt 0) { Remove-WebApplication -Name $app.Key -Site "Default Web Site" }

        # Create web application
        New-WebApplication -Name $app.Key -PhysicalPath $app.Value -Site "Default Web Site" -ApplicationPool "CI_<<branch_name>>"
        Add-Content -LiteralPath $urlFilePath ($urlBase + $app.Name + "/")
    }
}
catch 
{
    $errorCode = 1
}

# Set the Forms Authentication loginURL property for Milliman application
if ($errorCode -eq 0)
{
    try
    {
        set-webconfigurationproperty -PSPath "MACHINE/WEBROOT/APPHOST/Default Web Site/prm_ci_<<branch_name>>" -Filter "system.web/authentication/forms" -name loginURL -value "UserLogin.aspx"
    }
    catch
    {
        $errorCode = 2
    }
}

# Write out the error code
Set-Content -LiteralPath $outputPath $errorCode
