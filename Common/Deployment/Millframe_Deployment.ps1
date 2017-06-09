<#
    ## CODE OWNERS: Ben Wyatt, Steve Gredell

    ### OBJECTIVE:
		Deploy Millframe from specified source directories to targets

    ### DEVELOPER NOTES:
    

#> 

<#
.SYNOPSIS

Deploy Millframe code to production

.PARAMETER pathMapFile

The path to a CSV file containing source and target directories, with a line for each application to be deployed.
(Source first, then target) Only existing directories can be specified. 

The destination folder will be emptied before copying files, to ensure the destination perfectly matches the source.

Fully qualified paths should be provided, including drive letter or UNC share.

The script assumes that deployment will be performed from a CI-created location to the production server.

Example file contents:
D:\InstalledApplications\PRM_CI\pre-release\Milliman,X:\InstalledApplications\PRM\Milliman
D:\InstalledApplications\PRM_CI\pre-release\ClientUserAdmin,X:\InstalledApplications\PRM\ClientUserAdmin

.EXAMPLE
.\Millframe_Deployment.ps1 -pathMapFile "D:\InstalledApplications\PRM_CI\Deploy.csv"

#>

[CmdletBinding()]
Param(
   [ValidateScript({Test-Path $_ -PathType ‘Leaf’})]
   [ValidateScript({[IO.Path]::GetExtension($_) -eq '.csv'})]
   [Parameter(Mandatory=$True)]
   [string]$pathMapFile
)

# Import $pathMapFile
$paths = Import-Csv $pathMapFile -Header @("Source","Destination")


# Pre-deployment verification
# Make sure each source and destination is a valid folder path
foreach ($line in $paths)
{
    if ((test-path $line.Source -PathType Container) -eq $false)
    {
        Write-error -Message "$($line.Source) is not a valid folder path"
        return
    }
    if ((test-path $line.Destination -PathType Container) -eq $false)
    {
        Write-error -Message "$($line.Destination) is not a valid folder path"
        return
    }
}

# Loop over CSV contents
# Empty destination directory (use get-childitem and remove-item) before copying items
# 
# Use copy-item to move items from source to destination
foreach ($line in $paths)
{
    try {
        $line.Destination | get-childitem | Remove-Item -Recurse

        $line.Source | Get-ChildItem | Copy-Item -Destination $line.Destination -Recurse
    }
    catch {
        write-output "Error occurred copying contents of $($line.source) to $($line.Destination)"
    }
}


# Loop over destination folders
# Update web.config files to point at production appsettings & connectionstrings
foreach ($line in $paths)
{
    # Manipulate web.config contents
    $webConfigFilePath = [System.IO.Path]::Combine( $line.destination, "web.config" );

    if ((Test-Path $webConfigFilePath) -eq $true)
    {
        $xml = [xml](get-content $webConfigFilePath)
        $root = $xml.get_DocumentElement();

        if ($root.connectionStrings.HasChildNodes)
        {
            # Some of the config files explicitly set the connection string, rather than pulling from template files
            # There are a couple different possible values based on current config files
            $connectionString = $root.connectionStrings.ChildNodes[0].connectionString
            $connectionString = $connectionString.Replace("PortalDB_CI","PortalDBSFv2M1")
            $connectionString = $connectionString.Replace("PortalDB_Staging","PortalDBSFv2M1")
            $root.connectionStrings.ChildNodes[0].SetAttribute("connectionString", $connectionString)
        }
        else
        {
            $root.appSettings.configSource = $root.appSettings.configSource.Replace("appsettings_CI.config", "appsettings_PROD.config")
            $root.connectionStrings.configSource = $root.connectionStrings.configSource.Replace("connectionStrings_CI.config", "connectionStrings_PROD.config")
        }
        $xml.Save($webConfigFilePath)
    }
}
