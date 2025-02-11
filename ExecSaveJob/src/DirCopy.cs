namespace ExecSaveJob;
public class DirCopy
{
    private (List<string> Dirs, List<string> Files) GetFiles(string RootDir, List<string> Files)
        {
            List<string> Dirs = new List<string>();

            foreach (string File in Directory.GetFiles(RootDir))
            {
                Files.Add(File);
            }

            foreach (string Dir in Directory.GetDirectories(RootDir))
            {
                Dirs.Add(Dir);
                GetFiles(Dir, Files);
            }

            return (Dirs, Files);
        }
        
        private bool AreFilesEqual(string File1, string File2)
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
        
        private void CopyPasteFiles(string RootFile, string ToFile, int Err = 0)
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
                Console.WriteLine(E.Message);
                Console.WriteLine(E.GetType());

                switch (E.GetType().Name)
                {
                    case nameof(UnauthorizedAccessException):
                        Console.WriteLine($"Access denied: From {RootFile} To {ToFile}");
                        break;
                    case nameof(DirectoryNotFoundException):
                        Console.WriteLine($"Directory not found: From {RootFile} To {ToFile}");
                        HandleErrorLoop(Err, 2);
                        string To = Directory.GetParent(ToFile).FullName;
                        if (!Directory.Exists(To))
                        {
                            Console.WriteLine("Create directory: " + To);
                            Directory.CreateDirectory(To);
                            CopyPasteFiles(RootFile, ToFile, 2);
                        }
                        break;
                    case nameof(FileNotFoundException):
                        Console.WriteLine($"File not found: From {RootFile} To {ToFile}");
                        break;
                    case nameof(PathTooLongException):
                        Console.WriteLine($"Path too long: From {RootFile} To {ToFile}");
                        break;
                    case nameof(IOException):
                        Console.WriteLine($"Already exists: From {RootFile} To {ToFile}");
                        if (!AreFilesEqual(RootFile, ToFile))
                        {
                            Console.WriteLine("Files are different");
                            File.Delete(ToFile);
                            CopyPasteFiles(RootFile, ToFile, 5);
                        }
                        else
                        {
                            Console.WriteLine("Files are the same");
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void CopyDir(string RootDir, string ToDir)
        {
            // ServiceLogTreeStructure.WriteFile(RootDir, ToDir); #TODO decommenter cette ligne
            if (RootDir[RootDir.Length - 1] != '\\') RootDir += '\\';
            if (ToDir[ToDir.Length - 1] != '\\') ToDir += '\\';
            
            var program = new DirCopy();
            var Files = program.GetFiles(RootDir, new List<string>()).Files;
            
            foreach (string File in Files)
            {
                string a = File.Substring(RootDir.Length);
                Console.WriteLine(File);
                Console.WriteLine($"{ToDir}{a}");
                program.CopyPasteFiles(File, $"{ToDir}{a}");
            }
        }

        private (string, string) Refractor (string RootDir, string ToDir)
        {
            if (RootDir[RootDir.Length - 1] != '\\') RootDir += '\\';
            if (ToDir[ToDir.Length - 1] != '\\') ToDir += '\\';
            return (RootDir, ToDir);
        }
}