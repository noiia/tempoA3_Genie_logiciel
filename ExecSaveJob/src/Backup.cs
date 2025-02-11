using System;
using System.IO;
using Config;
using Logger;
using Services;

namespace Services;

public abstract class Backup
{
    public string RootDir { get; set; }
    public string SavesDir { get; protected set; }
    
    public SaveJob SaveJob { get; set; }

    public void setSavesDir(string savesDir)
    {
        this.SavesDir = savesDir;
        this.setSaveDir();
        this.Refractor();
    }
    public string SaveDir { get; protected set; }
    
    protected void Refractor()
    {
        if (RootDir[^1] != '\\') RootDir += '\\';
        if (SavesDir[^1] != '\\') SavesDir += '\\';
        if (SaveDir[^1] != '\\') SaveDir += '\\';
    }
    
    protected void setSaveDir()
    {
        this.SaveDir = $"{this.SavesDir}\\{this.getLastBackupNumber(this.SavesDir) + 1}";
        
        
        // DirectoryInfo directory = new DirectoryInfo(savesDir);
        // System.IO.Directory.CreateDirectory($"{savesDir}\\{this.getLastBackupNumber(savesDir) + 1}");
    }
    
    protected Backup BackupInstance { get; set; }

    public static void GetInstance(SaveJob saveJob)
    {
        throw new NotImplementedException();
    }

    protected Backup(SaveJob saveJob)
    {
        if (Directory.Exists(saveJob.Source))
        {
            this.SaveJob = saveJob;
            this.RootDir = saveJob.Source;
            this.SavesDir = saveJob.Destination;
            this.setSaveDir();
        }
        else
        {
            Console.WriteLine($"{saveJob.Source} doesn't exist");
        }

    }
    
    protected int getLastBackupNumber(string savesDir)
    {
        if (!Directory.Exists(savesDir))
        {
            return 0;
        }
        DirectoryInfo directory = new DirectoryInfo(savesDir);
        int lastBackupNumber = 0;

        foreach (DirectoryInfo dir in directory.GetDirectories())
        {
            if (int.TryParse(dir.Name, out int num))
            {
                if (num > lastBackupNumber)
                {
                    lastBackupNumber = num;
                }
            }
        }
        return lastBackupNumber;
    }


    
    protected void CopyPasteFile(string RootFile, string ToFile, int Err = 0)
        {
            void HandleErrorLoop(int err, int Err1)
            {
                if (err == Err1)
                {
                    Console.WriteLine($"Error {Err} happened twice in a row: exit the program");
                    Environment.Exit(0);
                }
            }
            try
            {
                File.Copy(RootFile, ToFile, true);
            }
            catch (Exception E)
            {
                // Console.WriteLine(E.Message);
                // Console.WriteLine(E.GetType());

                switch (E.GetType().Name)
                {
                    case nameof(UnauthorizedAccessException):
                        // Console.WriteLine($"Access denied: From {RootFile} To {ToFile}");
                        break;
                    case nameof(DirectoryNotFoundException):
                        // Console.WriteLine($"Directory not found: From {RootFile} To {ToFile}");
                        HandleErrorLoop(Err, 2);
                        string To = Directory.GetParent(ToFile).FullName;
                        if (!Directory.Exists(To))
                        {
                            // Console.WriteLine("Create directory: " + To);
                            Directory.CreateDirectory(To);
                            CopyPasteFile(RootFile, ToFile, 2);
                        }
                        break;
                    case nameof(FileNotFoundException):
                        // Console.WriteLine($"File not found: From {RootFile} To {ToFile}");
                        break;
                    case nameof(PathTooLongException):
                        // Console.WriteLine($"Path too long: From {RootFile} To {ToFile}");
                        break;
                    case nameof(IOException):
                        // Console.WriteLine($"Already exists: From {RootFile} To {ToFile}");
                        // if (!AreFilesEqual(RootFile, ToFile))
                        // {
                        //     Console.WriteLine("Files are different");
                        //     File.Delete(ToFile);
                        //     CopyPasteFiles(RootFile, ToFile, 5);
                        // }
                        // else
                        // {
                        //     Console.WriteLine("Files are the same");
                        // }
                        break;
                    default:
                        break;
                }
            }
        }
    
    protected virtual List<string> GetFiles(string rootDir, List<string> files)
    {
        string stateFileName = "statefile.log";
        // Counters counters = new Counters(); #TODO decommenter cette ligne
        
        DirectoryInfo directoryInfo = new DirectoryInfo(rootDir);
        
        // counters.DataCount = directoryInfo.GetFiles().Length;#TODO decommenter cette ligne
        // counters.FileCount = directoryInfo.GetFiles().Count()#TODO decommenter cette ligne;
        // counters.IsActive = true;#TODO decommenter cette ligne
        
        foreach (string file in Directory.GetFiles(rootDir))
        {
            var fileInfo = new FileInfo(file);
            // RealTimeState.WriteState(this.SaveJob.Name, counters.IsActive , counters, fileInfo, SavesDir, stateFileName, ""); #TODO decommenter cette ligne
            files.Add(file);
        }

        foreach (string dir in Directory.GetDirectories(rootDir))
        {
            GetFiles(dir, files);
        }

        // counters.IsActive = false;#TODO decommenter cette ligne
        return files;
    }
    
    
    protected void turnArchiveBitTrue (string filePath)
    {
        // Console.WriteLine($"Archiving {filePath}");
        FileAttributes attributes = File.GetAttributes(filePath);

        // Activer le bit d'archive
        attributes |= FileAttributes.Archive;

        // Définir les nouveaux attributs du fichier
        File.SetAttributes(filePath, attributes);
    }
    

    protected void CopyDir()
    {
        List<string> Files = new List<string>();
        Files = GetFiles(RootDir, Files);
        foreach (string File in Files)
        {
            turnArchiveBitTrue(File);
            CopyPasteFile(File, File.Replace(RootDir, SaveDir));
        }
    }

    public void Save()
    {
        CopyDir();
    }
    
    
}