using Config;

namespace AddSaveJob
{
    public class Program
    {
        // AddSaveJob <name> <source> <destination> <type>
        public static void Main(string[] args)
        {
            Configuration config = new Configuration(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EasySave\\" + "config.json");
            config.LoadConfiguration();//#TODO handle erreur pas de fichier 
            config.GetSaveJob(args[0]);
            ServiceAddSaveJob.Run(args, config);
        }
    }
}