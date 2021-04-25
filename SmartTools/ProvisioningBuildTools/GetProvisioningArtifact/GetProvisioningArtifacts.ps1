[CmdletBinding()]
Param (
    [Parameter(Mandatory = $True, Position=0)] 
    [string] $Branch,
    
    [Parameter(Mandatory = $True, Position=1)] 
    [string] $Tag,
    
    [Parameter(Mandatory = $True, Position=2)] 
    [string] $Path 
)

Try {

$Exitcode = 0

$ErrorActionPreference = "Stop"

$Organization = "https://dev.azure.com/MSFTDEVICES"

#$DefinitionName = "CI_DeviceProvisioning"
$DefinitionID = "8074" 

$Project = "Vulcan"

$Status = "completed"

#$Result = "succeeded"

$Tags = "Official"

#$BuildListExpression = "az pipelines build list --organization $Organization --project $Project --definition-ids $DefinitionID --tags $Tags --branch $Branch --status $Status --result $Result --top 1 -o Table"

$BuildListExpression = "az pipelines build list --organization $Organization --project $Project --definition-ids $DefinitionID --tags $Tags --branch $Branch --status $Status --top 1 -o Table"

$BuildListExpression

Write-Host

$BuildListInfo = Invoke-Expression "$BuildListExpression"

$BuildListInfo

Write-Host

If ($LASTEXITCODE -eq 1 -or $BuildListInfo -eq $null )
{
    $AZLoginExpression = "az login"
    
    $AZLoginExpression

    Invoke-Expression "$AZLoginExpression"

    "$BuildListExpression"

    Write-Host

    $BuildListInfo = Invoke-Expression "$BuildListExpression"

    $BuildListInfo

    Write-Host
}

If($BuildListInfo -and $BuildListInfo.Length -gt 2)
{ 
   $LastIsSpace = $false
   $Key = [System.String]::Empty
   $value = [System.String]::Empty
   $IgnoreCase = [System.StringComparer]::InvariantCultureIgnoreCase    

   $BuildDictInfo = New-Object -TypeName 'System.Collections.Generic.Dictionary[String,String]' -ArgumentList  $IgnoreCase


   for($i=0;$i -lt $BuildListInfo[1].Length ;$i++)
    {
      if(($BuildListInfo[1][$i] -ne " "))
      {
         if ($LastIsSpace -eq $True)
         {
          $LastIsSpace = $false
          $Key = [System.String]::Empty
          $value = [System.String]::Empty
         }

         $Key += $BuildListInfo[0][$i]
         $value += $BuildListInfo[2][$i]
      }
      else
      {
        if ($LastIsSpace -eq $false)
        {
           $LastIsSpace = $True
           $Key = $Key.Trim()
           $value = $value.Trim()
           $BuildDictInfo[$Key] = $value;
           #"$Key : $value" | Write-Host
        }        
      }
     }

     $Key = $Key.Trim()
     $value = $value.Trim()
     $BuildDictInfo[$Key] = $value;
     #"$Key : $value" | Write-Host


     if($BuildDictInfo.ContainsKey("ID") -and $BuildDictInfo.ContainsKey("Number") -and $BuildDictInfo.ContainsKey("Result") -and $BuildDictInfo["Number"].StartsWith($Tag))
     {
           if($BuildDictInfo["Result"].ToUpper().Contains("succeeded".ToUpper()))
           {
               $ID = $BuildDictInfo["ID"]
               $ProvisioningPackageArtifactName = $BuildDictInfo["Number"] + "_provisioningpackage"

               $Path = [System.IO.Path]::Combine($Path, $ProvisioningPackageArtifactName)

               $PathExist = Test-Path -Path $Path -PathType Container

               if($PathExist -eq $True  )
               {
                   Remove-Item -Path $Path -Recurse
               }

               $ZipFilePath = "$Path.zip"

               $PathExist = Test-Path -Path $ZipFilePath -PathType Leaf

               if($PathExist -eq $True  )
               {
                   Remove-Item -Path $ZipFilePath -Force
               }

               $ArtifactListExpression = "az pipelines runs artifact list --organization $Organization --project $Project --run-id $ID -o table"
               $ArtifactListExpression

               Write-Host

               $ArtifactListInfo = Invoke-Expression "$ArtifactListExpression"

               $ArtifactListInfo

               Write-Host

               $LastIsSpace = $false
               $Key = [System.String]::Empty
               $value = "0"
               $IgnoreCase = [System.StringComparer]::InvariantCultureIgnoreCase    

               $ArtifactDictInfo = New-Object -TypeName 'System.Collections.Generic.Dictionary[String,String]' -ArgumentList  $IgnoreCase

               If($ArtifactListInfo -and $ArtifactListInfo.Length -gt 2)
               {
                   for($i=0;$i -lt $ArtifactListInfo[1].Length ;$i++)
                   {   
                       if(($ArtifactListInfo[1][$i] -ne " "))
                       {
                           if($LastIsSpace -eq $True)
                           {
                               $LastIsSpace = $false
                               $Key = [System.String]::Empty
                               $value = $i.ToString()
                           }

                           $Key += $ArtifactListInfo[0][$i]
                       }
                       else
                       {
                           if($LastIsSpace -eq $False)
                           {
                               $LastIsSpace = $True
                               $Key = $Key.Trim()             
                               $value = $value + "," +($i - [convert]::ToInt32($value).ToString())
                               $ArtifactDictInfo[$Key] = $value
                               #"$Key : $value" | Write-Host
                           }
                       }
                   }

                   $Key = $Key.Trim()
                   $value = $value + "," + ($ArtifactListInfo[1].Length - [convert]::ToInt32($value)).ToString();
                   $ArtifactDictInfo[$Key] = $value;
                   #"$Key : $value" | Write-Host

                   $Artifacts = New-Object -TypeName 'System.Collections.Generic.List[String]'

                   for($i=2;$i -lt $ArtifactListInfo.Length ;$i++)
                   {
                       $StartIndex = [convert]::ToInt32($ArtifactDictInfo["Name"].Split(',')[0])
                       $Length = [convert]::ToInt32($ArtifactDictInfo["Name"].Split(',')[1])                          
                       $ArtifactName = $ArtifactListInfo[$i].Substring($StartIndex,$Length).ToUpper().Trim()
                       $Artifacts.Add($ArtifactName)
                       #$ArtifactName
                   }
           
                   if($Artifacts.Contains($ProvisioningPackageArtifactName.ToUpper().Trim()))
                   {
                    .\ArtifactDownload $ProvisioningPackageArtifactName $Path $ID $Project

                    $PathExist = Test-Path -Path $Path -PathType Container

                    if($PathExist -eq $True  )
                    {
					  .\ProvisioningArtifactsValidation $BuildDictInfo["Number"] $Path	
					  
                      if($LASTEXITCODE -eq 0)
                      {					  
						  Add-Type -assembly "system.io.compression.filesystem" | Out-Null

						  [io.compression.zipfile]::CreateFromDirectory($Path, $ZipFilePath)

						  "Please share: $ZipFilePath" | Write-Host
					  }
					  else
					  {
						$Exitcode = -1
                        $Host.UI.WriteErrorLine("Artifact: $ProvisioningPackageArtifactName Download failed with hash validation mismatch!!!") 
					  }
                    }
                    else
                    {
                        $Exitcode = -1
                        $Host.UI.WriteErrorLine("Artifact: $ProvisioningPackageArtifactName Download failed!!!") 
                    }
                   }
                   else
                   {
                    $Exitcode = -1
                    $Host.UI.WriteErrorLine("Artifact: $ProvisioningPackageArtifactName not found!!!") 
                   }   
               }
               else
               {
                $Exitcode = -1
                $Host.UI.WriteErrorLine("Artifact: $ProvisioningPackageArtifactName not found!!!") 
               }
           }
           else
           {
             $Exitcode = -1
             $Host.UI.WriteErrorLine("Server build not succeeded!!!") 
           }                 
     }
     else
     {
       $Exitcode = -1
       $Host.UI.WriteErrorLine("Server build not found!!!") 
     }
}
Else
{
   $Exitcode = -1
   $Host.UI.WriteErrorLine("Server build not found!!!") 
}
}

Catch  {
    $Host.UI.WriteErrorLine("Error Message: $_.Exception.Message")
    
    if($_.Exception.Message.ToUpper().Contains("'az' is not recognized as".ToUpper()))
    {
      Write-Host

      $Host.UI.WriteErrorLine("Please install AZURE CLI from https://docs.microsoft.com/en-us/cli/azure/ before get provisioning artifact!!!")
      $Host.UI.WriteErrorLine("Please close and reopen invoker after install AZURE CLI!!!")
    }
       
    $Exitcode = $_.Exception.HResult
    [System.Environment]::ExitCode = $ExitCode
    [System.Environment]::Exit([System.Environment]::ExitCode)
   }
[System.Environment]::ExitCode = $ExitCode
[System.Environment]::Exit([System.Environment]::ExitCode)
