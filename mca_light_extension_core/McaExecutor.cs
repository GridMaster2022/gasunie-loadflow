using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using mca_light_extension_core.Domain;
using mca_light_extension_core.Helpers;

namespace mca_light_extension_core
{
    public static class McaExecutor
    {
        public static int RunMca(string pathToCommandFile, string pathToFlowDataFile, string pathToOutput) => RunMca(McaHelper.GetPathToConfigFile(), pathToCommandFile, new FileInfo(pathToFlowDataFile).DirectoryName, pathToOutput);
        
        public static int RunMca(string pathToMcaFile, string pathToCommandFile, string pathToFlowData, string pathToOutput)
        {
            //Validate input
            if (!File.Exists(pathToMcaFile)) throw new Exception($"MCA config file {pathToMcaFile} does not exist");
            if (!File.Exists(pathToCommandFile)) throw new Exception($"Command file {pathToCommandFile} does not exist");
            if (!Directory.Exists(pathToFlowData) || Directory.GetFiles(pathToFlowData).Length == 0) throw new Exception($"No flow data is present in {pathToFlowData}");
            if (!Directory.Exists(pathToOutput)) Directory.CreateDirectory(pathToOutput);

            string pathToExecutable = McaHelper.GetPathToExecutable();

            Process mcaLight = Process.Start(pathToExecutable, $"\"{pathToMcaFile}\" \"{pathToCommandFile}\" \"{pathToFlowData}\" \"{pathToOutput}\"");
            if (mcaLight == null) return (int)McaExitCode.AWS_Wrapper_Error;
            while (!mcaLight.HasExited)
            {
                Thread.Sleep(100);
            }

            return mcaLight.ExitCode;
        }
    }
}
