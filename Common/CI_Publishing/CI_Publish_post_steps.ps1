$outputPath = "D:\installedapplications\prm_ci\<<branch_name>>\error.log"
$urlFilePath = "D:\installedapplications\prm_ci\<<branch_name>>\urls.log"
$urlBase = "https://indy-prm-2.milliman.com"
$errorCode = 0

# Clear URL file, if it exists
set-content -LiteralPath $urlFilePath "Published URLs:"

# Remove error.log file, if it exists
remove-item $outputPath

# (Re-)create applications and log deployed URLs to text file
try 
{
    $apps = @{
                "/prm_ci_<<branch_name>>_ClientPublisher" = "D:\installedapplications\prm_ci\<<branch_name>>\ClientPublisher";
                "/prm_ci_<<branch_name>>_ClientUserAdmin" = "D:\installedapplications\prm_ci\<<branch_name>>\ClientUserAdmin";
                "/prm_ci_<<branch_name>>_Milliman" = "D:\installedapplications\prm_ci\<<branch_name>>\Milliman";
                "/prm_ci_<<branch_name>>_MillimanServices" = "D:\installedapplications\prm_ci\<<branch_name>>\MillimanServices";
                "/prm_ci_<<branch_name>>_ProjectManagementConsoleServices" = "D:\installedapplications\prm_ci\<<branch_name>>\ProjectManagementConsoleServices";
                "/prm_ci_<<branch_name>>_ProjectManConsole" = "D:\installedapplications\prm_ci\<<branch_name>>\ProjectManConsole";
                "/prm_ci_<<branch_name>>_UserAdmin" = "D:\installedapplications\prm_ci\<<branch_name>>\UserAdmin"
				}

    foreach ($app in $apps.GetEnumerator())
    {
        # Manipulate web.config contents
        $webConfigFilePath = [System.IO.Path]::Combine( $app.Value, "web.config" );

        if ((Test-Path $webConfigFilePath) -eq $true -and $app.Key -ne "/prm_ci_<<branch_name>>_MillimanServices")
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
            elseif ($app.Key -eq "/prm_ci_<<branch_name>>_UserAdmin")
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

            $xml.Save($webConfigFilePath)
        }

        # If the web application already exists, remove it
        if ((Get-WebApplication $app.Key).Count -gt 0) { Remove-WebApplication -Name $app.Key -Site "Default Web Site" }

        # Create web application
        New-WebApplication -Name $app.Key -PhysicalPath $app.Value -Site "Default Web Site" -ApplicationPool "CI_IIS"
        Add-Content -LiteralPath $urlFilePath ($urlBase + $app.Name + "/")
    }
}
catch 
{
    $errorCode = 1
    Set-Content -LiteralPath $outputPath $errorCode
}

# Set the Forms Authentication loginURL property for Milliman application
try
{
    set-webconfigurationproperty -PSPath "MACHINE/WEBROOT/APPHOST/Default Web Site/prm_ci_<<branch_name>>_Milliman" -Filter "system.web/authentication/forms" -name loginURL -value "UserLogin.aspx"
}
catch
{
    $errorCode = 2
    Set-Content -LiteralPath $outputPath $errorCode
}

# If we've reached this point and $errorCode is still 0, everything was fine
if ($errorCode -eq 0)
{
    Set-Content -LiteralPath $outputPath $errorCode
}
