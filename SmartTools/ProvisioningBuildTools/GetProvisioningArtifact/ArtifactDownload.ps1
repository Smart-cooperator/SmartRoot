[CmdletBinding()]
Param (
    [Parameter(Mandatory = $True, Position=0)] 
    [string] $Artifactname,

    [Parameter(Mandatory = $True, Position=1)] 
    [string] $Path,

    [Parameter(Mandatory = $True, Position=2)] 
    [string] $Runid,

    [Parameter(Mandatory = $True, Position=3)] 
    [string] $project
)

Try {

$ExitCode = 0

$ErrorActionPreference = "Stop"

$Organization = "https://dev.azure.com/MSFTDEVICES"

$PathExist = Test-Path -Path $Path -PathType Container

if($PathExist -eq $True  )
{
  Remove-Item -Path $Path -Recurse
}

#$null = New-Item -Path $Path -ItemType Directory

$ArtifactDownloadExpression= "az pipelines runs artifact download --artifact-name $Artifactname --path $Path --run-id $Runid --project $project --org $Organization"

$ArtifactDownloadExpression

Write-Host

Invoke-Expression "$ArtifactDownloadExpression"

$PathExist = Test-Path -Path $Path -PathType Container

if($PathExist -eq $True  )
{
 Push-Location $Path

 (dir -r) | % { Unblock-File $_.fullname  }

 Pop-Location
}
else
{
 $ExitCode = -1
}


[System.Environment]::ExitCode = $ExitCode
}

Catch  {
    $Host.UI.WriteErrorLine("Error Message: $_.Exception.Message")    
    $ExitCode = $_.Exception.HResult
    [System.Environment]::ExitCode = $ExitCode
   }
[System.Environment]::ExitCode = $ExitCode
