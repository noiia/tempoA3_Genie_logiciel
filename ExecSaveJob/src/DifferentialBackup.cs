using System.Dynamic;
using Config;

namespace Services;

public class DifferentialBackup : Backup
{
    private static DifferentialBackup instance;

    private DifferentialBackup(SaveJob saveJob) : base(saveJob) { }

    public static DifferentialBackup GetInstance(SaveJob saveJob)
    {
        if (instance == null)
        {
            instance = new DifferentialBackup(saveJob);
        }
        
        return instance;
    }

    protected override List<string> GetFiles(string RootDir, List<string> Files)
    {
        foreach (string File in Directory.GetFiles(RootDir))
        {
            if (!isArchived(File))
            {
                Files.Add(File);
            }
        }

        foreach (string Dir in Directory.GetDirectories(RootDir))
        {
            GetFiles(Dir, Files);
        }

        return Files;
    }
    
    
    
    protected bool isArchived(string path)
    {
        bool AreFilesEqual(string File1, string File2)
        {
            if (File1 == File2)
            {
                return true;
            }

            if (!File.Exists(File1) || !File.Exists(File2))
            {
                return false;
            }

            FileInfo FileInfo1 = new FileInfo(File1);
            FileInfo FileInfo2 = new FileInfo(File2);

            if (FileInfo1.Length != FileInfo2.Length)
            {
                return false;
            }

            using (FileStream fs1 = File.OpenRead(File1))
            using (FileStream fs2 = File.OpenRead(File2))
            {
                int File1Byte;
                int File2Byte;

                do
                {
                    File1Byte = fs1.ReadByte();
                    File2Byte = fs2.ReadByte();
                }
                while ((File1Byte == File2Byte) && (File1Byte != -1));

                return (File1Byte - File2Byte) == 0;
            }
        }
        int lastBackupNumber = this.getLastBackupNumber(SavesDir);
        Console.WriteLine(lastBackupNumber);
        string verify = path.Replace(RootDir, SaveDir).Replace((lastBackupNumber + 1).ToString(), (lastBackupNumber).ToString());
        Console.WriteLine(verify);

        if (AreFilesEqual(path, verify))
        {
            Console.WriteLine($"Same file {path} {verify}");
            return true;
        }
        else
        {
            Console.WriteLine($"Different file {path} {verify}");
            return false;
        }
        
        
        // FileAttributes attributes = File.GetAttributes(path);
        // Console.WriteLine($"aaa {path} {attributes} {(attributes & FileAttributes.Archive) == FileAttributes.Archive}");
        // return (attributes & FileAttributes.Archive) == FileAttributes.Archive;
    }
}