using ManifestTools.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ManifestTools
{
    class Program
    {
        public static string pathToGame;
        public static Version gameVersion;

        static void Main(string[] args)
        {
            Console.WriteLine("Введите путь к папке с игрой");
            Console.Write(">");
            pathToGame = Console.ReadLine();

            if (File.Exists($"{pathToGame}\\SkyrimSE.exe"))
            {
                gameVersion = new Version(FileVersionInfo.GetVersionInfo($"{pathToGame}\\SkyrimSE.exe").FileVersion);

                Console.WriteLine($"Игра найдена с версией: {gameVersion}");
                Console.WriteLine($"Нажмите любую кнопку что бы продолжить");
                Console.ReadKey();

                WriteManifest();
            }
            else
            {
                Main(args);
            }
        }

        private static Dictionary<string, string> files;

        private static void WriteManifest()
        {
            files = new Dictionary<string, string>();

            ScanDir(pathToGame);

            GameManifestModel model = new GameManifestModel()
            {
                Version = gameVersion.ToString(),
                Files = files
            };

            File.WriteAllText("game.manifest.json", JsonConvert.SerializeObject(model));
        }
        

        private static void ScanDir(string directory)
        {
            foreach (DirectoryInfo dir in new DirectoryInfo(directory).GetDirectories())
            {
                ScanDir(dir.FullName);
            }

            foreach (string file in Directory.GetFiles(directory))
            {
                string path = file.Replace(pathToGame, "");
                string hash = GetMD5FromFile(File.OpenRead(file)).ToUpper();
                files.Add(path, hash);
                Console.WriteLine($"{hash} | {path}");
            }
        }

        public static string GetMD5FromFile(FileStream file)
        {
            byte[] fileData = new byte[file.Length];
            file.Read(fileData, 0, (int)file.Length);

            return GetMD5Hash(fileData);
        }
        public static string GetMD5FromText(string text)
        {
            return GetMD5Hash(Encoding.UTF8.GetBytes(text));
        }
        private static string GetMD5Hash(byte[] source)
        {
            using (MD5 algorithm = MD5.Create())
            {
                byte[] hash = algorithm.ComputeHash(source);

                return new string(hash.SelectMany(a => a.ToString("X2")).ToArray());
            }
        }
    }
}
