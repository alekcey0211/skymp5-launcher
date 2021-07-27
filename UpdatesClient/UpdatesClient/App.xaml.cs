using SingleInstanceApp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using UpdatesClient.Core;
using UpdatesClient.Modules;
using UpdatesClient.Modules.Configs;
using UpdatesClient.Modules.Debugger;
using UpdatesClient.Modules.GameManager.Helpers;
using UpdatesClient.Modules.ModsManager;
using UpdatesClient.Modules.Recovery.UI;
using UpdatesClient.Modules.SelfUpdater;
using Res = UpdatesClient.Properties.Resources;
using SplashScreen = UpdatesClient.Modules.SelfUpdater.SplashScreen;

namespace UpdatesClient
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    internal delegate void Invoker();
    public partial class App : Application, ISingleInstance
    {
        private const string Unique = "{F627FE18-0573-4F39-A2BF-8A564F10FC30}";
        private const string BeginUpdate = "begin";
        private const string EndUpdate = "end";

        internal delegate void ApplicationInitializeDelegate(SplashScreen splashWindow);
        internal ApplicationInitializeDelegate ApplicationInitialize;

        public static new App Current { get { return Application.Current as App; } }
        public static Application AppCurrent { get; private set; }

        private readonly bool mainInstance = false;

        public App()
        {
            try
            {
                AppCurrent = Current;
            }
            catch { }
            if (LocalesManager.CheckResxLocales() && Environment.GetCommandLineArgs().Length == 1)
            {
                Process.Start(ResourceAssembly.Location);
                Application.Current.Shutdown();
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            ModulesManager.PreInitModules();

            if (!Modules.SelfUpdater.Security.CheckEnvironment()) { ExitApp(); return; }
            if (!HandleCmdArgs()) { ExitApp(); return; }

            try
            {
                if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
                {
                    mainInstance = true;
                    InitApp();
                }
                else
                {
                    ExitApp();
                }
            }
            catch (Exception e)
            {
                Logger.FatalError("App", e);
            }
        }

        //TODO: Переработать
        private bool HandleCmdArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                bool eUpdate = false;
                int trying = 0;
                try
                {
                    switch (args[1])
                    {
                        case EndUpdate:
                            eUpdate = true;
                            Thread.Sleep(500);
                            try
                            {
                                if (File.Exists($"{args[2]}.update.exe"))
                                {
                                    File.SetAttributes($"{args[2]}.update.exe", FileAttributes.Normal);
                                    File.Delete($"{args[2]}.update.exe");
                                }
                            }
                            catch (IOException io)
                            //фикс ошибки занятого файла, он должен освободится через какое то время
                            {
                                trying++;
                                if (trying < 5) goto case EndUpdate;
                                else throw new TimeoutException("Timeout \"EndUpdate\"", io);
                            }
                            break;
                        case BeginUpdate:
                            Thread.Sleep(500);
                            try
                            {
                                File.Copy(EnvParams.PathToFile, $"{args[2]}.exe", true);
                                File.SetAttributes($"{args[2]}.exe", FileAttributes.Normal);
                            }
                            catch (IOException io)
                            //фикс ошибки занятого файла, он должен освободится через какое то время
                            {
                                trying++;
                                if (trying < 5) goto case BeginUpdate;
                                else throw new TimeoutException("Timeout \"BeginUpdate\"", io);
                            }
                            Process.Start($"{args[2]}.exe", $"{EndUpdate} {args[2]}");
                            goto default;
                        case "recovery":
                            RecoveryWindow rw = new RecoveryWindow();
                            rw.ShowDialog();
                            break;
                        case "repair":
                            Settings.Reset();
                            break;
                        case "repair-client":
                            try
                            {
                                ModVersion.Reset();
                            }
                            catch (Exception e) { MessageBox.Show($"{e.Message}", $"{Res.Error}"); }
                            goto default;
                        case "clear-client":
                            try
                            {
                                Mods.Init();
                                Mods.DisableAll(true).Wait();
                                GameCleaner.Clear();
                            }
                            catch (Exception e) { MessageBox.Show($"{e.Message}", $"{Res.Error}"); }
                            goto default;
                        case "clear-full-client":
                            try
                            {
                                Mods.Init();
                                Mods.DisableAll(true).Wait();
                                GameCleaner.Clear(true);
                            }
                            catch (Exception e) { MessageBox.Show($"{e.Message}", $"{Res.Error}"); }
                            goto default;
                        default:
                            ExitApp();
                            return false;
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    if (!eUpdate)
                    {
                        MessageBox.Show($"{Res.ErrorEndSelfUpdate}\n{e.Message}", $"{Res.Error}");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("HandleCmdArgs", e);
                    return false;
                }
            }
            return true;
        }

#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        private async void ApplicationInit(SplashScreen SplashWindow)
#pragma warning restore CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        {
            try
            {
#if (DEBUG || DeR)
                Thread.Sleep(100);
                if (CanRun())
                {
                    SplashWindow.Ready();
                    if (SplashWindow.Wait()) StartLuancher();
                }
#else
                SplashWindow.SetStatus($"{Res.CheckSelfUpdate}");

                await Updater.Init(SplashWindow);

                if (Updater.UpdateAvailable())
                {
                    SplashWindow.SetStatus($"{Res.SelfUpdating}");
                    SplashWindow.SetProgressMode(false);

                    if (Updater.Update())
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = $"{EnvParams.NameOfFileWithoutExtension}.update.exe";
                        p.StartInfo.Arguments = $"{BeginUpdate} {EnvParams.NameOfFileWithoutExtension}";
                        p.Start();
                    }
                    else
                    {
                        SplashWindow.SetStatus($"{Res.ErrorSelfUpdate}");
                        Thread.Sleep(1500);
                    }
                }
                else if (CanRun())
                {
                    SplashWindow.SetStatus($"{Res.Done}");
                    SplashWindow.SetProgressMode(false);
                    SplashWindow.Ready();
                    if (SplashWindow.Wait()) StartLuancher();
                }
#endif
            }
            catch (WebException e)
            {
                MessageBox.Show($"{Res.Details}: {e.Message}", $"{Res.ConnectionError}");
            }
            catch (WebSocketException e)
            {
                MessageBox.Show($"{Res.Details}: {e.Message}", $"{Res.ConnectionError}");
            }
            catch (UnauthorizedAccessException uae)
            {
                FileAttributes attributes = FileAttributes.Normal;
                try
                {
                    attributes = File.GetAttributes(Updater.PathToFileUpdate);
                }
                catch { }
                Logger.Error($"CriticalError_{Modules.SelfUpdater.Security.UID}_{attributes}", uae);
                MessageBox.Show($"{Res.Details}: {uae.Message}\n{Res.UrId}: {Modules.SelfUpdater.Security.UID}", $"{Res.CriticalError}");
            }
            catch (Exception e)
            {
                Logger.Error($"CriticalError_{Modules.SelfUpdater.Security.UID}", e);
                MessageBox.Show($"{Res.Details}: {e.Message}\n{Res.UrId}: {Modules.SelfUpdater.Security.UID}", $"{Res.CriticalError}");
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string newName = args.Name.Replace(" ", string.Empty).Split(',')[0].Replace('.', '_');
            if (newName.EndsWith("_resources"))
            {
                AssemblyName MissingAssembly = new AssemblyName(args.Name);
                string locale = MissingAssembly.CultureInfo?.Name;
                if (LocalesManager.ExistLocale(locale))
                {
                    return Assembly.LoadFile(LocalesManager.GetPathToLocaleLib(locale));
                }
                return null;
            }
            else
            {
                try
                {
                    byte[] bytes = (byte[])Res.ResourceManager.GetObject(newName);
                    return Assembly.Load(bytes);
                }
                catch { }
            }
            return null;
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            try
            {
                if (Current?.MainWindow?.WindowState == WindowState.Minimized) Current.MainWindow.WindowState = WindowState.Normal;
                Current?.MainWindow?.Show();
                Current?.MainWindow?.Activate();
            }
            catch (Exception e)
            {
                Logger.Error("SignalExternal", e);
            }
            return true;
        }

        private bool CanRun()
        {
            if (Modules.SelfUpdater.Security.Status.Block)
                MessageBox.Show(Encoding.UTF8.GetString(Convert.FromBase64String("UGxlYXNlIGRvd25sb2FkIHRoZSBuZXcgdmVyc2lvbiBmcm9tIHNreW1wLmlv")),
                    Encoding.UTF8.GetString(Convert.FromBase64String("SXMgbm90IGEgYnVn")), MessageBoxButton.OK, MessageBoxImage.Warning);

            return !Modules.SelfUpdater.Security.Status.Block;
        }
        //====================================================
        protected override void OnExit(ExitEventArgs e)
        {
            if (mainInstance) SingleInstance<App>.Cleanup();
            base.OnExit(e);
        }

        private void InitApp()
        {
            ApplicationInitialize = ApplicationInit;
        }

        public static void ExitApp()
        {
            Environment.Exit(0);
        }

        private void StartLuancher()
        {
            IO.DeleteFile(Updater.PathToFileUpdate);

            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Invoker)delegate
            {
                Logger.ActivateMetrica();
                MainWindow = new MainWindow();
                MainWindow.Show();
            });
        }
    }
}
