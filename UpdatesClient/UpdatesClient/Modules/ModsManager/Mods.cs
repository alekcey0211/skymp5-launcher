using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UpdatesClient.Core;
using UpdatesClient.Core.Helpers;
using UpdatesClient.Modules.Configs;
using UpdatesClient.Modules.Debugger;
using UpdatesClient.Modules.GameManager;
using UpdatesClient.Modules.GameManager.Models.ServerManifest;
using UpdatesClient.Modules.ModsManager.Models;
using UpdatesClient.Modules.SelfUpdater;

namespace UpdatesClient.Modules.ModsManager
{
    public static class Mods
    {
        private static string Cache { get => Settings.PathToSkyrimMods + "Cache\\"; }
        private static string Tmp { get => Settings.PathToSkyrimMods + "Tmp\\"; }
        private static string List { get => Settings.PathToSkyrimMods + "mods.json"; }

        private static ModsModel mods = new ModsModel();

        public static List<string> WhiteListFiles { get; } = new List<string>
        {
            "Skyrim.esm",
            "Update.esm",
            "Dawnguard.esm",
            "HearthFires.esm",
            "Dragonborn.esm"
        };
        public static List<string> WhiteListMods { get; } = new List<string>
        {
            "Skyrim",
            "Update",
            "Dawnguard",
            "HearthFires",
            "Dragonborn"
        };


        public static async void Init()
        {
            if (!string.IsNullOrWhiteSpace(Settings.PathToSkyrimMods))
            {
                IO.CreateDirectory(Settings.PathToSkyrimMods);
            }

            mods = mods.Load<ModsModel>(List);

            string[] arMods = mods.Mods.ToArray();

            foreach (var modName in arMods)
            {
                if (!Directory.Exists(Settings.PathToSkyrimMods + modName))
                {
                    if (mods.EnabledMods.Contains(modName)) mods.EnabledMods.Remove(modName);
                    mods.Mods.Remove(modName);
                    mods.Save(List);
                }
            }

            arMods = mods.Mods.ToArray();
            foreach (var modName in arMods)
            {
                await PreLoadMod(modName);
            }
        }
        //TODO: валидация файлов мода перед запуском
        private static async Task PreLoadMod(string modName)
        {
            if (ExistMod(modName))
            {
                string pathToMod = Settings.PathToSkyrimMods + modName + "\\";
                ModModel mod = new ModModel();
                try
                {
                    mod = mod.Load<ModModel>(pathToMod + "mod.json");
                }
                catch
                {
                    await RemoveMod(modName);
                }
            }
        }
        public static bool CheckModFiles(string modName)
        {
            if (!ExistMod(modName)) return false;
            string pathToMod = Settings.PathToSkyrimMods + modName + "\\";
            ModModel mod = new ModModel();
            mod = mod.Load<ModModel>(pathToMod + "mod.json");

            bool valid = true;

            Dictionary<string, uint> files = new Dictionary<string, uint>();
            IO.RecursiveHandleFile(pathToMod, (file) =>
            {
                files.Add(file.Replace(pathToMod, ""), 0);
            });

            foreach (var file in mod.Files)
            {
                if (!files.ContainsKey(file.Key)) 
                    valid = false;
            }

            return valid;
        }

        #region Old
        public static async Task OldModeEnable()
        {
            await DisableMod("SkyMPCore");
            await EnableMod("SkyMPCore");
        }
        #endregion 

        public static ServerModsManifest CheckCore(ServerModsManifest mods)
        {
            ServerModModel[] arrMods = mods.Mods.ToArray();

            foreach (ServerModModel mod in arrMods)
            {
                if (WhiteListFiles.Contains(mod.FileName))
                {
                    string path = $"{Settings.PathToSkyrim}\\Data\\{mod.FileName}";
                    if (File.Exists(path))
                    {
                        //TODO: -----
                        //uint lhash = Hashing.GetCRC32FromBytes(File.ReadAllBytes(path));
                        //if (mod.CRC32 == lhash)
                        mods.Mods.Remove(mod);
                    }
                }
            }
            return mods;
        }
        public static bool CheckMod(string modName, List<(string, uint)> files)
        {
            if (!ExistMod(modName)) throw new FileNotFoundException($"Mod ({modName}) not found", modName);
            string pathToMod = Settings.PathToSkyrimMods + modName + "\\";
            ModModel mod = new ModModel();
            mod = mod.Load<ModModel>(pathToMod + "mod.json");

            bool valid = true;

            if (files.Count != mod.Files.Count) return false;

            foreach (var file in files)
            {
                if (file.Item2 != mod.Files[$"Data\\{file.Item1}"]) valid = false;
            }

            return valid;
        }
        public static string GetModHash(string modName)
        {
            if (!ExistMod(modName)) return null;
            string pathToMod = Settings.PathToSkyrimMods + modName + "\\";
            ModModel mod = new ModModel();
            mod = mod.Load<ModModel>(pathToMod + "mod.json");
            return mod.Hash;

        }
        public static bool ExistMod(string modName, bool onlySkyrimMods = false)
        {
            bool m = mods.Mods.Contains(modName);
            if (m)
            {
                m = Directory.Exists(Settings.PathToSkyrimMods + modName);
                if (!m)
                {
                    if (mods.EnabledMods.Contains(modName)) mods.EnabledMods.Remove(modName);
                    mods.Mods.Remove(modName);
                    mods.Save(List);
                }
                else if (onlySkyrimMods)
                {
                    ModModel mod = new ModModel();
                    mod = mod.Load<ModModel>(Settings.PathToSkyrimMods + modName + "\\mod.json");
                    m = mod.IsSkyrimMod;
                }
            }
            return m;
        }

        public static bool IsEnableMod(string modName)
        {
            return ExistMod(modName) && mods.EnabledMods.Contains(modName);
        }

        //! Skyrim моды через жесткие ссылки без папок
        public static async Task EnableMod(string modName)
        {
            if (!ExistMod(modName)) throw new FileNotFoundException($"Mod ({modName}) not found", modName);
            if (IsEnableMod(modName)) return;

            await GameLauncher.StopGame();

            ModModel mod = new ModModel();
            mod = mod.Load<ModModel>(Settings.PathToSkyrimMods + modName + "\\mod.json");

            foreach (string file in mod.Files.Keys)
            {
                if (mod.IsSkyrimMod)
                {
                    WinFunctions.CreateHardLink($"{Settings.PathToSkyrim}\\{file}",
                           $@"{Settings.PathToSkyrimMods}{modName}\{file}", IntPtr.Zero);
                }
                else
                {
                    if (!Directory.Exists($"{Settings.PathToSkyrim}\\{Path.GetDirectoryName(file)}"))
                    {
                        string[] dirs = Path.GetDirectoryName(file).Split('\\');
                        string tpmdir = "";
                        foreach (string dir in dirs)
                        {
                            tpmdir += dir + "\\";
                            if (!Directory.Exists($"{Settings.PathToSkyrim}\\{tpmdir}"))
                            {
                                WinFunctions.CreateSymbolicLink($"{Settings.PathToSkyrim}\\{tpmdir}",
                                    $@"{Settings.PathToSkyrimMods}{modName}\{tpmdir}", Enums.SymbolicLink.Directory);
                                break;
                            }
                        }
                    }
                    else if (!File.Exists($"{Settings.PathToSkyrim}\\{file}"))
                    {
                        WinFunctions.CreateSymbolicLink($"{Settings.PathToSkyrim}\\{file}",
                            $@"{Settings.PathToSkyrimMods}{modName}\{file}", Enums.SymbolicLink.File);
                    }
                }
            }

            mods.EnabledMods.Add(modName);
            mods.Save(List);
        }

        //! Skyrim моды через жесткие ссылки без папок
        public static async Task DisableMod(string modName)
        {
            if (!ExistMod(modName)) throw new FileNotFoundException($"Mod ({modName}) not found", modName);
            if (!IsEnableMod(modName)) return;

            await GameLauncher.StopGame();

            ModModel mod = new ModModel();
            
            string pathTmp = Settings.PathToSkyrimMods + modName + "\\";
            mod = mod.Load<ModModel>(pathTmp + "mod.json");

            IO.RecursiveHandleFile(pathTmp, (file) =>
            {
                string filePath = file.Replace(pathTmp, "");
                if (!mod.Files.ContainsKey(filePath)) mod.Files.Add(filePath, 0);
            });

            foreach (string file in mod.Files.Keys)
            {
                if (mod.IsSkyrimMod)
                {
                    if (mod.HasMainFile && Directory.Exists($"{Settings.PathToSkyrim}\\{Path.GetDirectoryName(file)}")
                        && File.Exists($"{Settings.PathToSkyrim}\\{file}"))
                    {
                        File.Delete($"{Settings.PathToSkyrim}\\{file}");
                    }
                }
                else
                {
                    if (Directory.Exists($"{Settings.PathToSkyrim}\\{Path.GetDirectoryName(file)}"))
                    {
                        string[] dirs = Path.GetDirectoryName(file).Split('\\');
                        string tpmdir = "";
                        foreach (string dir in dirs)
                        {
                            tpmdir += dir + "\\";
                            DirectoryInfo di = new DirectoryInfo($"{Settings.PathToSkyrim}\\{tpmdir}");
                            if (di.Attributes.HasFlag(FileAttributes.ReparsePoint))
                            {
                                Directory.Delete($"{Settings.PathToSkyrim}\\{tpmdir}");
                                break;
                            }
                        }
                    }

                    if (Directory.Exists($"{Settings.PathToSkyrim}\\{Path.GetDirectoryName(file)}")
                        && File.Exists($"{Settings.PathToSkyrim}\\{file}")
                        && File.GetAttributes($"{Settings.PathToSkyrim}\\{file}").HasFlag(FileAttributes.ReparsePoint))
                    {
                        File.Delete($"{Settings.PathToSkyrim}\\{file}");
                    }
                }
            }

            try
            {
                if (mod.IsSkyrimMod && mod.HasMainFile)
                {
                    string path = DefaultPaths.PathToLocalSkyrim + "Plugins.txt";
                    if (Directory.Exists(DefaultPaths.PathToLocalSkyrim) && File.Exists(path))
                    {
                        IO.FileSetNormalAttribute(path);
                        string content = File.ReadAllText(path);
                        string[] cmods = content.Split('\n');
                        List<string> nmods = new List<string>(cmods.Length - 1);
                        for (int i = 0; i < cmods.Length; i++)
                        {
                            if (cmods[i] != $"*{mod.MainFile}") nmods.Add(cmods[i]);
                        }
                        content = string.Join("\n", nmods);
                        File.WriteAllText(path, content);
                    }
                }
            }
            catch (Exception e) { Logger.Error("DisableSkyrimMod", e); }

            mods.EnabledMods.Remove(modName);
            mods.Save(List);
        }
        public static async Task DisableAll(bool ignoreWL = false)
        {
            await GameLauncher.StopGame();

            List<string> WhiteList = new List<string>(3)
            {
                "SKSE",
                "SkyMPCore",
                "RuFixConsole"
            };

            string[] enMods = mods.EnabledMods.ToArray();

            foreach (string mod in enMods)
            {
                if (!ignoreWL && WhiteList.Contains(mod)) continue;
                await DisableMod(mod);
            }
        }


        public static string GetTmpPath()
        {
            string path = Tmp + Hashing.GetMD5FromText(DateTime.UtcNow.ToString());
            IO.CreateDirectory(path);
            return path;
        }

        public static async Task AddMod(string modName, string hash, string pathTmp, bool isSkyrimMod, string mainFile = null)
        {
            await GameLauncher.StopGame();

            ModModel mod = new ModModel();

            if (ExistMod(modName))
            {
                mod = mod.Load<ModModel>($"{Settings.PathToSkyrimMods}{modName}\\mod.json");
            }
            else
            {
                mod.Name = modName;
            }
            mod.Hash = hash;
            mod.IsSkyrimMod = isSkyrimMod;

            if (mainFile != null)
            {
                mod.HasMainFile = true;
                mod.MainFile = mainFile;
            }

            IO.RecursiveHandleFile(pathTmp, (file) =>
            {
                string filePath = file.Replace(pathTmp + "\\", "");
                uint fileHash = Hashing.GetCRC32FromBytes(File.ReadAllBytes(file));
                if (mod.Files.ContainsKey(filePath))
                {
                    mod.Files[filePath] = fileHash;
                }
                else
                {
                    mod.Files.Add(filePath, fileHash);
                }
            });

            IO.CreateDirectory(Settings.PathToSkyrimMods + mod.Name);
            IO.RecursiveCopy(pathTmp, Settings.PathToSkyrimMods + mod.Name);

            Directory.Delete(pathTmp, true);

            if (!mods.Mods.Contains(mod.Name))
            {
                mods.Mods.Add(mod.Name);
            }

            mod.Save(Settings.PathToSkyrimMods + mod.Name + "\\mod.json");
            mods.Save(List);
        }
        public static async Task RemoveMod(string modName)
        {
            if (ExistMod(modName))
            {
                string pathToMod = Settings.PathToSkyrimMods + modName + "\\";
                if (IsEnableMod(modName)) await DisableMod(modName);
                IO.RemoveDirectory(pathToMod);
                mods.Mods.Remove(modName);
                if (mods.EnabledMods.Contains(modName)) mods.EnabledMods.Remove(modName);
            }
        }
    }
}
