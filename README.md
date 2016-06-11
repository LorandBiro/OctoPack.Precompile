# OctoPack Precompile

This NuGet package adds an ASP.NET precompile step to the build just before (and only if) OctoPack is called. All the files - which would be packaged by OctoPack - will be copied into an intermediate folder. The ASP.NET compiler will be executed on this intermediate folder creating the precompiled website to be packaged. This target doesn't tell OctoPack where are these files so a nuspec file is needed to specify the location of the precompiled website.

I created this to enable ASP.NET precompile in a simple TeamCity-Octopus pipeline. I didn't want to put publish profiles or any complex MSBuild 'magic' into the repository just to enable a feature that should be a simple checkbox, so I created this NuGet package to hide the complexity.

I tested it only with TeamCity Visual Studio build runner (VS2015) with Octopus plugin, but I'm sure there are some more complex scenarios where this simple build step is not enough.

## How to use
1. Install the `OctoPack.Precompile` NuGet package into the project you want to precompile on packaging.
2. Create a nuspec file for OctoPack or modify it if you already have to include the files in `obj/Precompiled`. The nuspec file should be in the same directory as your project file matching its name. For example if your project file is `YourProject.csproj`, the nuspec file should be `YourProject.nuspec`. Here's a minimal example:

```XML
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
  <metadata>
    <id>YourProject</id>
    <authors>YourProject</authors>
    <description>YourProject</description>
    <version>1.0.0</version>
  </metadata>
  <files>
    <file src="obj\Precompiled\**\*.*" target="" />
  </files>
</package>
```

> Visit http://docs.octopusdeploy.com/display/OD/Using+OctoPack or http://docs.nuget.org/create/nuspec-reference for more information.

That's it. Next time TeamCity or another build tool invokes OctoPack, precompilation will kick in and your deploy package will contain a precompiled website.
