using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace OctoPack.Precompile.Tests
{
    public class BuildTest
    {
        private const string MSBuildPath = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Professional\\MSBuild\\15.0\\Bin\\MSBuild.exe";
        private static readonly string SolutionDirectoryPath = Path.GetFullPath(Path.Join(Environment.CurrentDirectory, "..\\..\\..\\..\\TestWebApplication"));
        private static readonly string SolutionFilePath = Path.Join(SolutionDirectoryPath, "TestWebApplication.sln");
        private static readonly string NuGetPackagePath = Path.Join(SolutionDirectoryPath, "bin\\TestWebApplication.0.0.0.0.nupkg");

        private readonly ITestOutputHelper output;

        public BuildTest(ITestOutputHelper output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        [Fact]
        public async Task EnablingOctoPackEnablesOctoPackPrecompileAutomatically()
        {
            await this.BuildAsync("/p:RunOctoPack=true");
            using (ZipArchive archive = ZipFile.OpenRead(NuGetPackagePath))
            {
                Assert.True(archive.Entries.Any(x => x.Name.EndsWith(".compiled")), "Couldn't find compiled files in the output NuGet package.");
            }
        }

        [Fact]
        public async Task RunOctoPackPrecompile_CanBeUsedToDisableThePrecompilation()
        {
            await this.BuildAsync("/p:RunOctoPack=true /p:RunOctoPackPrecompile=false");
            using (ZipArchive archive = ZipFile.OpenRead(NuGetPackagePath))
            {
                Assert.False(archive.Entries.Any(x => x.Name.EndsWith(".compiled")), "Found compiled files in the output NuGet package.");
            }
        }

        private async Task BuildAsync(string arguments)
        {
            await this.RestoreNuGetPackagesAsync();
            await this.ExecuteProcessAsync(MSBuildPath, $"{SolutionFilePath} /target:Rebuild {arguments}");
        }

        private async Task RestoreNuGetPackagesAsync()
        {
            await this.EnsureNuGetIsDownloadedAsync();
            await this.ExecuteProcessAsync("nuget.exe", $"restore {SolutionFilePath}");
        }

        private async Task EnsureNuGetIsDownloadedAsync()
        {
            if (File.Exists("nuget.exe"))
            {
                return;
            }

            WebClient wc = new WebClient();
            await wc.DownloadFileTaskAsync(new Uri("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"), "nuget.exe");
        }

        private async Task ExecuteProcessAsync(string filePath, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo(filePath, arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (Process process = Process.Start(psi))
            {
                string log = await process.StandardOutput.ReadToEndAsync();
                this.output.WriteLine(log);
                if (process.ExitCode != 0)
                {
                    Assert.True(false, $"{Path.GetFileName(filePath)} failed with exit code {process.ExitCode}. See test output.");
                }
            }
        }
    }
}
