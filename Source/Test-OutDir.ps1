cls
function Main{
cd $PSScriptRoot
	$baseDir = $PSScriptRoot
	$binariesPath = "$baseDir\Binaries"

#To delete WebApplication1 C:\GitReposD\OctoPack.Precompile\Source\Test\WebApplication1\WebApplication1\obj\Debug\Precompiled\ 

.\Build.ps1

# Restore NuGet packages before build
Push-Location ".\Test\WebApplication1"
&..\..\Build\NuGet.exe restore
DeleteFolder $binariesPath
DeleteFolder "WebApplication1\obj\Debug\Precompiled"  

# Build solution with RunOctoPack=true and specified /p:OutDir
&"c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" WebApplication1.sln  /target:Rebuild /p:RunOctoPack=true /p:OutDir="$binariesPath\"

# Assert nupkg exists
if (-not (Test-Path "$binariesPath\WebApplication1.1.0.0.nupkg"))
{
    throw "Output NuGet package doesn't exist"
}

Pop-Location
}
function DeleteFolder($dir){
    if ( Test-Path $dir ) {
               Get-ChildItem -Path  $dir -Force -Recurse | Remove-Item -force -recurse
               Remove-Item $dir -Force
    }
}
Main

if ($host.name -eq 'ConsoleHost') 
{
  Read-Host -Prompt "Press_Enter_to_continue"
}
