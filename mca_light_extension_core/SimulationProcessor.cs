using System;
using mca_light_extension_core.Domain;
using mca_light_extension_core.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace mca_light_extension_core
{
    public class SimulationProcessor
    {
        private static ILogger logger;

        public static void SetLogger(ILogger logHandler) => logger = logHandler;

        public static QueueObject ProcessContent(string inputContent)
        {
            QueueObject simulationQueueObject = null;

            try
            {
                simulationQueueObject = JsonConvert.DeserializeObject<QueueObject>(inputContent);
            }
            catch (Exception e)
            {
                logger?.LogError($"Failed to process input from queue - {inputContent}\r\n{e.Message}\r\n{e.StackTrace}");
                return simulationQueueObject;
            }

            //Download files from S3
            string pathToTaskFile = McaHelper.GetPathToTaskFile(simulationQueueObject?.PostProcessingGasunieAssignmentLocation);
            string pathToFlowDataFile = McaHelper.GetPathToFlowData(simulationQueueObject?.PostProcessingGasunieLocation);
            if (!AwsS3Client.DownloadFile(simulationQueueObject?.BucketName, simulationQueueObject?.PostProcessingGasunieAssignmentLocation, pathToTaskFile) ||
                !AwsS3Client.DownloadFile(simulationQueueObject?.BucketName, simulationQueueObject?.PostProcessingGasunieLocation, pathToFlowDataFile))
                return simulationQueueObject;

            //Decompress files
            CompressionHelper.DecompressFile(pathToTaskFile, out pathToTaskFile);
            CompressionHelper.DecompressFile(pathToFlowDataFile, out pathToFlowDataFile);

            //Execute MCA Light
            string outputPath = McaHelper.GetPathToOutputDirectory(simulationQueueObject?.PostProcessingGasunieAssignmentLocation);
            int exitCode = McaExecutor.RunMca(pathToTaskFile, pathToFlowDataFile, outputPath);

            //Check exit code
            if (McaHelper.ExitCodeIndicatingError(exitCode, out string exitCodeDescription))
                logger?.LogWarning($"MCA Light exited with error code [{exitCode}], indicating [{exitCodeDescription}]. Faulty ScenarioId: {simulationQueueObject?.ScenarioId}");

            return simulationQueueObject;
        }
    }
}
