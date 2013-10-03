using System;
using System.IO;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web;

namespace Inedo.BuildMasterExtensions.VB6
{
    /// <summary>
    /// Represents
    /// </summary>
    [ActionProperties("Make VB6 Project", "Makes a VB6 project.", "VB6")]
    [CustomEditor(typeof(MakeVB6ActionEditor))]
    public sealed class MakeVB6Action : RemoteActionBase
    {
        /// <summary>
        /// Gets or sets the project name to build
        /// </summary>
        [Persistent]
        public string ProjectName { get; set; }
        /// <summary>
        /// Gets or sets the conditional compilation arguments, as defined by vb6.exe
        /// </summary>
        [Persistent]
        public string ConditionalCompilationArguments { get; set; }

        protected override void Execute()
        {
            this.LogDebug("Making " + this.ProjectName + "...");
            this.ExecuteRemoteCommand(null);
            this.LogDebug("Make succeeded.");
        }

        protected override string ProcessRemoteCommand(string name, string[] args)
        {
            // Find path to exe
            string exePath;
            {
                var productDir = (new VB6Helper()).GetVB6ProductDir();
                if (string.IsNullOrEmpty(productDir))
                    throw new InvalidOperationException("VB6 productDir not found.");

                exePath = Path.Combine(productDir, "vb6.exe");
                if (!File.Exists(exePath))
                    throw new FileNotFoundException("vb6.exe not found in productDir.");
            }

            // Create temp file for error output
            string tempFile = Path.GetTempFileName();

            // Run make
            int exitCode = this.ExecuteCommandLine(
                exePath,
                string.Format(
                    @"/make ""{0}"" /out ""{1}"" /outdir ""{2}"" /d ""{3}""",
                    this.ProjectName,
                    tempFile,
                    this.Context.TargetDirectory,
                    this.ConditionalCompilationArguments),
                this.Context.SourceDirectory);

            // Log output
            if (exitCode == 0)
            {
                this.LogInformation(File.ReadAllText(tempFile));
            }
            else
            {
                this.LogError(File.ReadAllText(tempFile));
                this.LogError("An error was reported during compilation. VB6.EXE exited with code " + exitCode + ".");
            }

            return exitCode.ToString();
        }

        public override string ToString()
        {
            return string.Format(
                "Compile {0} from {1} to {2}.",
                this.ProjectName,
                Util.CoalesceStr(this.OverriddenSourceDirectory, "default directory"),
                Util.CoalesceStr(this.OverriddenTargetDirectory, "default directory"));
        }
    }
}
