using Config;
using Logger;

namespace AddSaveJobs;

public class ServiceAddSaveJob
{
    public const int OK = 1;
    public const int BAD_ARGS = 2;
    public const int NAME_ALREADY_USE = 3;
    public const int SOURCE_DOES_NOT_EXIST = 4;
    public const int DESTINANTION_DOES_NOT_EXIST = 5;
    public const int TYPE_DOES_NOT_EXIST = 6;
    public const int MORE_THAN_5_SAVEJOB = 7;
    public const string TYPE_FULL = "full";
    public const string TYPE_DIFF = "diff";

    public static int Run(string[] args, Configuration configuration)
    {
        if (args.Length == 4)
        {
            SaveJob saveJob = configuration.GetSaveJob(args[0]);
            if (saveJob != null)
            {
                LoggerUtility.WriteLog(LoggerUtility.Warning, "SaveJob name is already use (" + args[0] + ")");
                return NAME_ALREADY_USE;
            }

            if (!Directory.Exists(args[1]))
            {
                LoggerUtility.WriteLog(LoggerUtility.Warning, "Source folder does not exist (" + args[1] + ")");
                return SOURCE_DOES_NOT_EXIST;
            }

            if (!Directory.Exists(args[2]))
            {
                LoggerUtility.WriteLog(LoggerUtility.Warning, "Destiantion folder does not exist (" + args[2] + ")");
                return DESTINANTION_DOES_NOT_EXIST;
            }

            if (!(args[3].ToLower() == TYPE_DIFF || args[3].ToLower() == TYPE_FULL))
            {
                LoggerUtility.WriteLog(LoggerUtility.Warning, "Type args inst correct (" + args[3] + ")");
                return TYPE_DOES_NOT_EXIST;
            }

            int nextId = configuration.FindFirstFreeId();
            if (nextId == -1)
            {
                LoggerUtility.WriteLog(LoggerUtility.Info, "Cant add more than 5 SaveJob");
                return MORE_THAN_5_SAVEJOB;
            }
            else
            {
                configuration.AddSaveJob(nextId, args[0], args[1], args[2], DateTime.Now, DateTime.Now, args[3]);
                LoggerUtility.WriteLog(LoggerUtility.Info,
                    "SaveJob is created : {" + "id: " + nextId.ToString() + ", source: " + args[0] + ", destination: " +
                    args[1] + ", type: " + args[3] + "}");
                return OK;
            }
        }
        else
        {
            LoggerUtility.WriteLog(LoggerUtility.Warning, "Some args are missing or incorect");
            return BAD_ARGS;
        }
    }
}