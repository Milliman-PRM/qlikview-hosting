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
        New-WebApplication -Force -Name $app.Name -PhysicalPath $app.Value -Site "Default Web Site" -ApplicationPool "Qlikview IIS"
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
