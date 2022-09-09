using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using mca_light_extension_core;
using mca_light_extension_core.Domain;
using mca_light_extension_core.Helpers;

namespace mca_light_smo_nl_extended
{
    public class Startup
    {
        private static ILogger loggerModule;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
#if DEBUG
            SetDebugEnvironmentVariables();
#endif

            Configuration = configuration;
            loggerModule = logger;

            InitializeComponents();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);

            app.UseStaticFiles(); // For the wwwroot folder if you need it

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void InitializeComponents()
        {
            //Configure the MCA simulation flags as specified in the environment variables
            McaHelper.SetSimulationFlags(Environment.GetEnvironmentVariable("MCA_SIMULATION_FLAGS"));

            //Forward logger to components
            AwsSqsClient.SetLogger(loggerModule);
            AwsS3Client.SetLogger(loggerModule);
            SimulationProcessor.SetLogger(loggerModule);
            CompressionHelper.SetLogger(loggerModule);

            //Listen to SQS queue and link simulation
            AwsSqsClient.ReadMessageFromSqs(SimulationProcessor.ProcessContent, SimulationRunFinished, ApplicationFinished);
        }

        private void SimulationRunFinished(object queueObject)
        {
            if (!(queueObject is QueueObject sqsObject)) return;

            //Compress result
            string outputDirectory = McaHelper.GetPathToOutputDirectory(sqsObject.PostProcessingGasunieAssignmentLocation);
            if (!CompressionHelper.TryCompressFolder(outputDirectory, out string archivePath, McaHelper.GetOutputFileExtensions())) return;

            //Write output archive to S3
            string pathOnS3 = AwsHelper.GetStoragePathOnS3(archivePath);
            AwsS3Client.UploadFile(sqsObject.BucketName, pathOnS3, archivePath);

            //Notify SQS
            sqsObject.GasunieLoadFlowLocation = pathOnS3;
            AwsSqsClient.WriteMessageToSqs(sqsObject);

            //Clean files on disk
            McaHelper.CleanUpFiles(sqsObject);

            //Start listening to SQS queue again
            AwsSqsClient.ReadMessageFromSqs(SimulationProcessor.ProcessContent, SimulationRunFinished, ApplicationFinished);
        }

        private void ApplicationFinished()
        {
            loggerModule.LogInformation("Application finished. Terminating sub-modules");
            //Stop listening to SQS queue
            AwsSqsClient.Stop();
            //Terminate application
            Environment.Exit(0);
        }

    }
}
