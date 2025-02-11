using Config;

namespace Services;

public class CompleteBackup : Backup
{
    private static CompleteBackup instance;

    private string BackupId;

    private CompleteBackup(SaveJob saveJob) : base(saveJob) {}

    public static CompleteBackup GetInstance(SaveJob saveJob)
    {
        if (instance == null)
        {
            instance = new CompleteBackup(saveJob);
        }
        
        return instance;
    }
    

    
}