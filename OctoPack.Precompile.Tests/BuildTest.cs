using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace OctoPack.Precompile.Tests
{
    public class BuildTest
    {
        private const string MSBuildPath = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Professional\\MSBuild\\15.0\\Bin\\MSBuild.exe";
        private static readonly string SolutionDirectoryPath = Path.GetFullPath(Path.Join(Environment.CurrentDirectory, "..\\..\\..\\..\\Source\\Test\\WebApplication1"));
        private static readonly string SolutionFilePath = Path.Join(SolutionDirectoryPath, "WebApplication1.sln");
        private static readonly string NuGetPackagePath = Path.Join(SolutionDirectoryPath, "WebApplication1\\bin\\WebApplication1.1.0.0.nupkg");

        private readonly ITestOutputHelper output;

        public BuildTest(ITestOutputHelper output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        [Fact]
        public void EnablingOctoPackEnablesOctoPackPrecompileAutomatically()
        {
            this.Build("/p:RunOctoPack=true");
            using (ZipArchive archive = ZipFile.OpenRead(NuGetPackagePath))
            {
                Assert.True(archive.Entries.Any(x => x.Name.EndsWith(".compiled")), "Couldn't find compiled files in the output NuGet package.");
            }
        }

        [Fact]
        public void RunOctoPackPrecompile_CanBeUsedToDisableThePrecompilation()
        {
            this.Build("/p:RunOctoPack=true /p:RunOctoPackPrecompile=false");
            using (ZipArchive archive = ZipFile.OpenRead(NuGetPackagePath))
            {
                Assert.False(archive.Entries.Any(x => x.Name.EndsWith(".compiled")), "Found compiled files in the output NuGet package.");
            }
        }

        private void Build(string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo(MSBuildPath, $"{SolutionFilePath} /target:Rebuild {arguments}")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (Process process = Process.Start(psi))
            {
                string log = process.StandardOutput.ReadToEnd();
                if (process.ExitCode != 0)
                {
                    this.output.WriteLine(log);
                    Assert.True(false, "MSBuild process failed. See test output.");
                }
            }
        }
    }
}
