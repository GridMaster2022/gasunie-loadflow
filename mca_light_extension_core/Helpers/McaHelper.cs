using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using mca_light_extension_core.Domain;

namespace mca_light_extension_core.Helpers
{
    public static class McaHelper
    {
        public static string GetPathToExecutable() => Path.Combine(GetMcaExecutionPath(), RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "MCA_light_2.0.exe" : "MCA_light_linux_2.0");
        public static string GetPathToConfigFile() => Path.Combine(GetMcaExecutionPath(), "MCA_files", "GridmasterNW_1.mca");
        public static string GetPathToFlowData() => Path.Combine(GetMcaExecutionPath(), "ETM_data");
        public static string GetPathToFlowData(string fileName) => Path.Combine(GetMcaExecutionPath(), "ETM_data", fileName);
        public static string GetPathToTaskDirectory() => Path.Combine(GetMcaExecutionPath(), "Grid_tasks");
        public static string GetPathToOutputDirectory() => Path.Combine(GetMcaExecutionPath(), "Output");
        public static string GetPathToOutputDirectory(string path)
        {
            string result = GetPathToOutputDirectory();

            string[] combineParams = path.Split(new[] { '\\', '/', ':' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < combineParams.Length - 1; i++) //Skip last item as it is a file
            {
                result = Path.Combine(result, combineParams[i]);
            }

            return result;
        }

        public static string GetPathToTaskFile(string path)
        {
            string result = GetPathToTaskDirectory();
            
            string[] combineParams = path.Split(new[] { '\\', '/', ':' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string param in combineParams)
            {
                result = Path.Combine(result, param);
            }

            return result;
        }

        public static string GetMcaExecutionPath()
        {
            string path = Environment.GetEnvironmentVariable("MCA_PATH");
            if (string.IsNullOrEmpty(path))
                throw new Exception("Environment variable [MCA_PATH] is not specified");

            if (!Directory.Exists(path))
                throw new Exception($"Path [{path}] specified in environment variable [MCA_PATH] does not exist");

            return path;
        }
        
        public static void SetSimulationFlags(string flagsValue)
        {
            if (string.IsNullOrEmpty(flagsValue)) return;

            string pathToConfigFile = GetPathToConfigFile();

            if (!File.Exists(pathToConfigFile)) return;

            //Read file content
            string[] fileContent = File.ReadAllLines(pathToConfigFile);
            bool fileAltered = false;
            //Look for line that contains flags
            for (int i = 0; i < fileContent.Length; i++)
            {
                string contentSpecifier = "test = ";
                if (!fileContent[i].Contains(contentSpecifier)) continue;

                int indexStart = fileContent[i].IndexOf(contentSpecifier, StringComparison.Ordinal);
                fileContent[i] = $"{fileContent[i].Substring(0, indexStart)}{contentSpecifier}{flagsValue}";
                fileAltered = true;
                break;
            }

            if (fileAltered)
                File.WriteAllLines(pathToConfigFile, fileContent);
        }

        public static string[] GetOutputFileExtensions()
        {
            string extensions = Environment.GetEnvironmentVariable("MCA_OUTPUT_EXTENSIONS");
            if (string.IsNullOrEmpty(extensions)) return Array.Empty<string>();

            return extensions.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool ExitCodeIndicatingError(int exitCode, out string message)
        {
            if (!Enum.IsDefined(typeof(McaExitCode), exitCode))
            {
                message = "Undefined exit code!";
                return true;
            }
            
            //Parse exit code and description
            McaExitCode mcaExitCode = (McaExitCode)exitCode;
            message = mcaExitCode.GetExitCodeDescription();

            //Check exit code
            return mcaExitCode != McaExitCode.RC_No_Error;
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private static string GetExitCodeDescription(this McaExitCode exitCode)
        {
            switch (exitCode)
            {
                case McaExitCode.RC_No_Error:
                    return string.Empty;
                case McaExitCode.RC_OverDate:
                    return "Programma is over de houdbaarheidsdatum";
                case McaExitCode.RC_No_RunArgumenten:
                    return "Er zijn geen argumenten opgegeven voor het programma";
                case McaExitCode.RC_No_RootDirectory:
                    return "Root voor een output-directory bestaat niet";
                case McaExitCode.RC_No_OUTPUT_RootDirectory:
                    return "Root output-directory bestaat niet";
                case McaExitCode.RC_No_INPUT_RootDirectory:
                    return "Root MCA-file bestaat niet";
                case McaExitCode.RC_No_MCAbestand:
                    return "MCA-input-bestand bestaat niet";
                case McaExitCode.RC_Overflow_name_table:
                    return "Teveel items; MCA-input-bestand is te groot";
                case McaExitCode.RC_DataFout_MCAbestand:
                    return "MCA-input-bestand heeft parser fouten";
                case McaExitCode.RC_NO_GridOpdrachtBestand:
                    return "GRID-opdracht-bestand bestaat niet";
                case McaExitCode.RC_DataFout_GRIDopdracht:
                    return "GRID-opdracht-bestand heeft parser fouten";
                case McaExitCode.RC_NO_ETMbestand:
                    return "ETM-bestand bestaat niet";
                case McaExitCode.RC_DataFout_ETMbestand:
                    return "ETM-bestand heeft parser fouten";
                case McaExitCode.AWS_Wrapper_Error:
                    return "Error in application starting MCA light";
                default:
                    return string.Empty;
            }
        }

        public static void CleanUpFiles(QueueObject queueObject)
        {
            if (queueObject?.ScenarioUuid == null) return;

            DeleteDirectory(Path.Combine(GetPathToFlowData(), queueObject.ScenarioUuid));
            DeleteDirectory(Path.Combine(GetPathToTaskDirectory(), queueObject.ScenarioUuid));
            DeleteDirectory(Path.Combine(GetPathToOutputDirectory(), queueObject.ScenarioUuid));
        }

        private static void DeleteDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return;
                Directory.Delete(path, true);
            }
            catch
            {
                //
            }
        }
    }
}
