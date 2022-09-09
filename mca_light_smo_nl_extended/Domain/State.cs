using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace mca_light_smo_nl_extended.Domain
{
    public class State
    {
        private static readonly string RootPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "C:\\" : "/";
        private readonly string mcaPath = Environment.GetEnvironmentVariable("MCA_PATH") ?? Path.Combine(RootPath, "opt");
        
        public DateTime BuildDateTime => Program.BuildDateTime;
        
        public DateTime BootDateTime => Program.BootDateTime;

        public string SimulationFlags => Environment.GetEnvironmentVariable("MCA_SIMULATION_FLAGS");

        public List<PathState> Dependencies => new List<PathState>
        {
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                new PathState(Path.Combine(mcaPath, "MCA_light_1.0.exe"), "Windows executable of Gasunie's LoadFlow tool (MCA light)") :
                new PathState(Path.Combine(mcaPath, "MCA_light_linux"), "Linux executable of Gasunie's LoadFlow tool (MCA light)"),
            new PathState(Path.Combine(mcaPath, "ETM_data")),
            new PathState(Path.Combine(mcaPath, "Grid_tasks")),
            new PathState(Path.Combine(mcaPath, "MCA_files")),
            new PathState(Path.Combine(mcaPath, "Output"))
        };
    }
}
