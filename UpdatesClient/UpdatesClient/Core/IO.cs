using System;
using System.Diagnostics;
using System.IO;

namespace UpdatesClient.Core
{
    public static class IO
    {
        public static string GetFileVersion(string path)
        {
            return FileVersionInfo.GetVersionInfo(path)?.FileVersion;
        }

        public static void FileSetNormalAttribute(string path)
        {
            if (File.Exists(path) && File.GetAttributes(path) != FileAttributes.Normal) File.SetAttributes(path, FileAttributes.Normal);
        }

        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                FileSetNormalAttribute(file);
                File.Delete(file);
            }
        }

        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (DirectoryNotFoundException)
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    do
                    {
                        if (dir.Attributes.HasFlag(FileAttributes.ReparsePoint) || (int)dir.Attributes != -1) dir = dir.Parent;
                    } while (!dir.Exists);
                    Directory.Delete(dir.FullName);
                    CreateDirectory(dir.FullName);
                }
            }
        }

        public static void RemoveDirectory(string path)
        {
            if (Directory.Exists(path)) Directory.Delete(path, true);
        }

        public static void RecursiveCopy(string pathFrom, string pathTo)
        {
            foreach (DirectoryInfo dir in new DirectoryInfo(pathFrom).GetDirectories())
            {
                CreateDirectory($"{pathTo}\\{dir.Name}");
                RecursiveCopy(dir.FullName, $"{pathTo}\\{dir.Name}");
            }

            foreach (string file in Directory.GetFiles(pathFrom))
            {
                string NameFile = file.Substring(file.LastIndexOf('\\'), file.Length - file.LastIndexOf('\\'));
                string pathToDestFile = $"{pathTo}\\{NameFile}";

                FileSetNormalAttribute(pathToDestFile);

                File.Copy(file, pathToDestFile, true);
            }
        }

        public static void RecursiveHandleFile(string directory, Action<string> action)
        {
            foreach (DirectoryInfo dir in new DirectoryInfo(directory).GetDirectories())
            {
                if (!dir.Attributes.HasFlag(FileAttributes.ReparsePoint) && (int)dir.Attributes != -1)
                    RecursiveHandleFile(dir.FullName, action);
            }

            foreach (string file in Directory.GetFiles(directory))
                action.Invoke(file);
        }
    }
}
