using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;

namespace mca_light_smo_nl_extended
{
    public class Program
    {
        public static DateTime BuildDateTime;
        public static DateTime BootDateTime;
        
        public static void Main(string[] args)
        {
            BuildDateTime = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;
            BootDateTime = DateTime.Now;

            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
