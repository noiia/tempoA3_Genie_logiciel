using Config;

namespace AddSaveJobs
{
    public class Program
    {
        // AddSaveJob <name> <source> <destination> <type>
        public static void Main(string[] args)
        {
            var configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EasySave\\" + "config.json";
            Configuration config = new Configuration(configPath);
            config.initDir();
            // config.LoadConfiguration();//#TODO handle erreur pas de fichier 
            // config.GetSaveJob(args[0]);
            // ServiceAddSaveJob.Run(args, config);
        }
    }
}