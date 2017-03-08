#cls
cd $PSScriptRoot
	$baseDir = $PSScriptRoot
	$binariesPath = "$baseDir\Binaries"

.\Build.ps1

# Restore NuGet packages before build
Push-Location ".\Test\WebApplication1"
&..\..\Build\NuGet.exe restore

# Build solution with RunOctoPack=true
#&"c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" WebApplication1.sln /target:Rebuild /p:RunOctoPack=true
&"c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" WebApplication1.sln  /target:Rebuild /p:RunOctoPack=true /p:OutDir="$binariesPath\"

# Assert nupkg exists
if (-not (Test-Path "WebApplication1\bin\WebApplication1.1.0.0.nupkg"))
{
    throw "Output NuGet package doesn't exist"
}

Pop-Location

if ($host.name -eq 'ConsoleHost') 
{
  Read-Host -Prompt "Press_Enter_to_continue"
}
