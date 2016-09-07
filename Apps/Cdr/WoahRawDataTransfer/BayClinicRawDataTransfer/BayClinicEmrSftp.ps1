# Requires first enabling script execution by running the following as Administrator:
# Set-ExecutionPolicy RemoteSigned
try
{
    Write-Host ("Bay Clinic Emr feed retrieval script launched at {0}" -f (Get-Date))

	$ftpUserName = (Get-Item env:bayclinicemr_ftpusername).Value
    $ftpPassword = (Get-Item env:bayclinicemr_ftppassword).Value
    $remotePath = (Get-Item env:bayclinicemr_ftpremotepath).Value
    $localPath = (Get-Item env:bayclinicemr_ftplocalpath).Value

    # Load WinSCP .NET assembly
    Add-Type -Path "WinSCPnet.dll"
 
    # Setup session options
    $sessionOptions = New-Object WinSCP.SessionOptions -Property @{
        Protocol = [WinSCP.Protocol]::Sftp
        HostName = "159.140.212.117"
        UserName = $ftpUserName
        Password = $ftpPassword
        SshHostKeyFingerprint = "ssh-dss 1024 4c:19:06:f3:43:66:db:25:6a:08:a7:b1:4e:0d:02:97"
    }
 
    $session = New-Object WinSCP.Session
 
    try
    {
        # ensure that the remote and local paths end with a path separator character
        if (-Not $remotePath.EndsWith('/')) 
        {
            $remotePath = "$($remotePath)/"
        }
        if (-Not $localPath.EndsWith('\')) 
        {
            $localPath = "$($localPath)\"
        }

        # Connect to remote server, will puke if it fails
        $session.Open($sessionOptions)

        # Set up transfer options 
        $transferOptions = New-Object WinSCP.TransferOptions
        $transferOptions.TransferMode = [WinSCP.TransferMode]::Binary

        # get list of remote files in the desired remote path 
        $directory = $session.ListDirectory($remotePath)

        foreach ($fileInfo in $directory.Files)
        {
            Write-Host ("Found directory entry: {0}, size {1}, permissions {2}, last modified {3}" -f $fileInfo.Name, $fileInfo.Length, $fileInfo.FilePermissions, $fileInfo.LastWriteTime)

            # perform processing on .zip files only
            if ($fileInfo.Name -like '*.zip') 
            {
                $remoteFileFullPath = "$($remotePath)$($fileInfo.Name)"

                # Try to get the file
                $transferResult = $session.GetFiles($remoteFileFullPath, $localPath, $False, $transferOptions)

                # Check the success of the transfer
                if ($transferResult.IsSuccess)
                {
                    Write-Host ("Download of {0} succeeded" -f $remoteFileFullPath)

                    # Remove the file from the server
                    $removeResult = $session.RemoveFiles($remoteFileFullPath)
                    if ($removeResult.IsSuccess)
                    {
                        Write-Host ("File {0} successfully removed from remote system" -f $remoteFileFullPath)
                    }
                    else
                    {
                        Write-Host ("File {0} failed to be removed from remote system" -f $remoteFileFullPath)
                        $removeResult.Check()
                    }
                }
                else
                {
                    foreach ($transfer in $transferResult.Transfers)
                    {
                        Write-Host ("Download of {0} failed" -f $remoteFileFullPath)
                        # Throw on any transfer error
                        $transferResult.Check()
                    }
                }
            }
        }
    }
    catch
    {
        $ErrorMessage = $_.Exception.Message
        $FailedItem = $_.Exception.ItemName
        Write-Host ("Exception with message: {0} from item {1}" -f $ErrorMessage, $FailedItem)
		throw $_.Exception
    }
    finally
    {
        # Disconnect, clean up
        $session.Dispose()
    }
 
    exit 0
}
catch [Exception]
{
    Write-Host ("Error: {0}" -f $_.Exception.Message)
    exit 1
}
