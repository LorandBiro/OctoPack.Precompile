# OctoPack Precompile

This NuGet package adds an ASP.NET precompile step to the build just before (and only if) OctoPack is called. All the files - which would be packaged by OctoPack - will be copied into an intermediate folder. The ASP.NET compiler will be executed on this intermediate folder creating the precompiled website to be packaged. If there's no nuspec file in the project the build step will generate one to tell OctoPack which files should be packaged. If you have a nuspec file you need to manually change it to include the precompiled website.

I created this to enable ASP.NET precompile in a simple TeamCity-Octopus pipeline. I didn't want to put publish profiles or any complex MSBuild 'magic' into the repository just to enable a feature that should be a simple checkbox, so I created this NuGet package to hide the complexity.

I tested it only with TeamCity Visual Studio build runner (VS2015) with Octopus plugin, but I'm sure there are some more complex scenarios where this simple build step is not enough.

## How to use
1. Install the `OctoPack.Precompile` NuGet package into the project you want to precompile on packaging.
2. If you don't have a nuspec file in your project you're done. Otherwise modify it to include the files in `obj\Release\Precompiled`. Here's a minimal example:

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
    <file src="obj\Release\Precompiled\**\*.*" target="" />
  </files>
</package>
```

> Visit http://docs.octopusdeploy.com/display/OD/Using+OctoPack or http://docs.nuget.org/create/nuspec-reference for more information.

That's it. Next time TeamCity or another build tool invokes OctoPack, precompilation will kick in and your deploy package will contain a precompiled website.
