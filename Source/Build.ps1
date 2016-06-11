if (-not (Test-Path "Build"))
{
    New-Item "Build" -ItemType "Directory"
}

if (-not (Test-Path "Build\NuGet.exe"))
{
    Invoke-WebRequest "https://nuget.org/nuget.exe" -OutFile "Build\NuGet.exe"
}

&Build\NuGet.exe pack "OctoPack.Precompile.nuspec" -OutputDirectory "Build"
