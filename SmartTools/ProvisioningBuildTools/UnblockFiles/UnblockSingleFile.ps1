[CmdletBinding()]
Param (   
    [Parameter(Mandatory = $True, Position=0)] 
    [string] $fullname
)

Unblock-File $fullname