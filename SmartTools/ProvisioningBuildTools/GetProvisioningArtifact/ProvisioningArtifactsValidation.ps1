#-------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation.  All rights reserved.
#------------------------------------------------------------------------------- 
[CmdletBinding()]
Param (
    [Parameter(Mandatory = $True, Position=0)] 
    [string] $ExpectedVersion,
	
	[Parameter(Mandatory = $True, Position=2)] 
    [string] $Path 
)

$global:ErrorActionPreference = 'stop'

Push-Location $Path

Write-Host "Version Info"
Write-Host "--------------------------------"

Write-Host "ExpectedVersion: $ExpectedVersion"
Write-Host

$ExistingVersion = Get-Content version.txt

Write-Host "ExistingVersion: $ExistingVersion" 
Write-Host

if ($ExistingVersion -eq $ExpectedVersion) {
    Write-Host -fore green "Version MATCH"
	Write-Host
} else {
    $Host.UI.WriteErrorLine("Version MISMATCH")
    exit 1
}

Write-Host "Hash Info"
Write-Host "--------------------------------"
Write-Host " - Reading hash.txt"
$HashInfo = Get-Content hash.txt

Write-Host " - Calculating Hashes"    
$HashOutput = Get-ChildItem * -Recurse | Get-FileHash | ForEach-Object {  
      #$MyPath = $(Resolve-Path -relative $_.Path)
      #if ($MyPath -ne ".\hash.txt") {
        "$($_.Algorithm) $($_.Hash) $(Resolve-Path -relative $_.Path)"
      #}
   }

Write-Host " - Comparing"    
$HashInfo
Write-Host
$HashOutput
Write-Host

$DifferentValues = @()
foreach ($line in $HashInfo) {
    if ($HashOutput -notcontains $line) {
        $DifferentValues += $line
    }
}

#foreach ($line in $HashOutput) {
#    if ($HashInfo -notcontains $line) {
#        $DifferentValues += $line
#    }
#}

Pop-Location

if ($DifferentValues.Length -eq 0) {
    Write-Host
    Write-Host -fore green "--------------------------------"
    Write-Host -fore green "             MATCH              "
    Write-Host -fore green "--------------------------------"
    exit 0
} else {
    $Host.UI.WriteErrorLine("--------------------------------")
    $Host.UI.WriteErrorLine("           MISMATCH             ")
    $Host.UI.WriteErrorLine("--------------------------------")

    $DifferentValues | ForEach-Object {Write-Host $_}
    
    $Host.UI.WriteErrorLine("--------------------------------")
    $Host.UI.WriteErrorLine("             FAIL               ")
    $Host.UI.WriteErrorLine("--------------------------------")
    exit 1
}
