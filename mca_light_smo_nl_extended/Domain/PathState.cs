using System.IO;

namespace mca_light_smo_nl_extended.Domain
{
    public class PathState
    {
        public string Path { get; }

        public PathType? Type { get; }

        public string Description { get; }

        public bool Present { get; }

        public int? NumberOfFiles { get; }

        public PathState(string path, string description = null)
        {
            Path = path;
            Description = description;
            
            if (Directory.Exists(path))
            {
                Type = PathType.Directory;
                Present = true;
            }
            else if (File.Exists(path))
            {
                Type = PathType.File;
                Present = true;
            }
            else
            {
                Present = false;
            }

            if (Type == PathType.Directory && Present)
                NumberOfFiles = Directory.GetFiles(path).Length;
        }

        public enum PathType
        {
            File,
            Directory
        }
    }
}
