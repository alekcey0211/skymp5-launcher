using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UpdatesClient.Core;
using UpdatesClient.Core.Enums;
using UpdatesClient.Core.Models;
using UpdatesClient.Modules.Configs;
using UpdatesClient.Modules.Debugger;
using UpdatesClient.Modules.GameManager;
using UpdatesClient.Modules.GameManager.AntiCheat;
using UpdatesClient.Modules.GameManager.Model;
using UpdatesClient.Modules.ModsManager;
using UpdatesClient.Modules.Notifications;
using UpdatesClient.Modules.Recovery;
using UpdatesClient.UI.Pages.MainWindow.Models;
using UpdatesClient.UI.Windows;
using Res = UpdatesClient.Properties.Resources;

namespace UpdatesClient.UI.Pages.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для ServerList.xaml
    /// </summary>
    public partial class ServerList : UserControl
    {
        public static bool ShowProgressBar { get => model.MainButtonProgressBar; set => model.MainButtonProgressBar = value; }
        private static ServerListModel model;
        private bool blockMainBtn = false;

        public ServerList()
        {
            InitializeComponent();
            model = new ServerListModel();
            DataContext = model;

            Init();
        }
        public async void Init()
        {
            await Task.Yield();
            while (true)
            {
                FillServerList();
                await Task.Delay(30 * 1000);
            }
        }
        public async void PostInit()
        {
            await Task.Yield();
            await CheckGame();
            ModVersion.Load();
            model.MainButtonEnabled = !ModVersion.SKSEDisabled && !ModVersion.ModsDisabled;
            FileWatcher.Init();
        }
        public async Task CheckGame()
        {
            ResultGameVerification result = GameVerification.CheckSkyrim();
            Mods.Init();

            if (result.NeedInstall)
            {
                model.MainButtonStatus = MainButtonStatus.Install;
            }
            else
            {
                await CheckClientUpdates();

                if (!Mods.CheckModFiles("SKSE"))
                {
                    model.MainButtonProgressBar = true;
                    await ModUtilities.InstallSKSE();
                    model.MainButtonProgressBar = false;
                }

                if (!Mods.CheckModFiles("RuFixConsole")) await ModUtilities.InstallRuFixConsole();
                
            }
        }

        private async Task CheckClientUpdates()
        {
            try
            {
                string lastVersion = await Net.GetLastestVersion();
                string version = Mods.GetModHash("SkyMPCore");
                if (string.IsNullOrEmpty(version) || lastVersion != version || !Mods.CheckModFiles("SkyMPCore")) model.MainButtonStatus = MainButtonStatus.Update;
                else model.MainButtonStatus = MainButtonStatus.Play;
            }
            catch (WebException we)
            {
                NotifyController.Show(we);
                model.MainButtonStatus = MainButtonStatus.Retry;
            }
            catch (Exception e)
            {
                Logger.Error("CheckClient", e);
                NotifyController.Show(e);
                model.MainButtonStatus = MainButtonStatus.Retry;
            }
        }

        private async void FillServerList()
        {
            try
            {
                string servers = await ServerModel.GetServers();

                List<ServerItemModel> list = ServerModel.ParseServersToList(servers)
                    .ConvertAll(c => new ServerItemModel(c, model.SortServerList)).ToList();
                list.RemoveAll(x => x.Server.IsEmpty());

                model.ServersList = list;
                model.SelectedServer = list.Find(x => x.Server.ID == Settings.LastServerID);
            }
            catch (HttpRequestException) { }
            catch (WebException) { }
            catch (Exception e)
            {
                Logger.Error("FillServerList", e);
            }
        }

        private void ServerListDataGrid_Click(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is ServerItemModel serverModel)
            {
                Settings.LastServerID = serverModel.Server.ID;
                model.SelectedServer = serverModel;
            }
        }

        private async void mainButton_Click(object sender, RoutedEventArgs e)
        {
            if (!blockMainBtn)
            {
                blockMainBtn = true;
                switch (model.MainButtonStatus)
                {
                    case MainButtonStatus.Install:
                        await Install();
                        break;
                    case MainButtonStatus.Play:
                        await Play(model.SelectedServer.Server);
                        break;
                    case MainButtonStatus.Update:
                        model.MainButtonProgressBar = true;
                        if (await ModUtilities.InstallClient(true)) goto case MainButtonStatus.Retry;
                        else model.MainButtonStatus = MainButtonStatus.Retry;
                        break;
                    case MainButtonStatus.Retry:
                        await CheckClientUpdates();
                        break;
                }
                model.MainButtonProgressBar = false;
                blockMainBtn = false;
            }
        }

        private async Task Install()
        {
            InstallerWindow iw = new InstallerWindow();
            iw.ShowDialog();
            if (iw.InstallReady)
            {
                ResultGameVerification result = iw.Result;

                await GameCleaner.CreateGameManifest(Settings.PathToSkyrim);

                bool skse = true, rufix = true, client = true;
                model.MainButtonProgressBar = true;
                if (!result.IsSKSEFound) skse = await ModUtilities.InstallSKSE();
                if (!result.IsRuFixConsoleFound) rufix = await ModUtilities.InstallRuFixConsole();
                client = await ModUtilities.InstallClient();
                model.MainButtonProgressBar = false;

                if (skse && rufix && client)
                {
                    NotifyController.Show(Res.InstallationCompleted);
                    await CheckGame();
                }
            }
        }

        //TODO: Уведомление об ошибках
        private async Task Play(ServerModel server)
        {
            if (!File.Exists($"{Settings.PathToSkyrim}\\skse64_loader.exe"))
            {
                await CheckGame();
                return;
            }

            if (!await ModUtilities.ActivateCoreMod()) return;

            Settings.LastServerID = server.ID;
            Settings.Save();

            await GameUtilities.Play(server, Window.GetWindow(this));
        }
    }
}
