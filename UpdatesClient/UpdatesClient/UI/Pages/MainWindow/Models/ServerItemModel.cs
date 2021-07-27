using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using UpdatesClient.Core;
using UpdatesClient.Core.Models;
using UpdatesClient.Modules.Configs;
using UpdatesClient.Modules.GameManager;
using UpdatesClient.Modules.GameManager.Models.ServerManifest;

namespace UpdatesClient.UI.Pages.MainWindow.Models
{
    public class ServerItemModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly bool inited = false;
        private readonly Action resort;

        public ServerModel Server;
        private bool selected;
        private bool favorite;
        private bool hasSkyEye;
        private string ping;
        private string description;

        private List<string> mods;

        private BitmapImage serverIcon;

        public BitmapImage ServerIcon
        {
            get { return serverIcon; }
            set { serverIcon = value; OnPropertyChanged(); }
        }


        public List<string> Mods
        {
            get { return mods; }
            set { mods = value; OnPropertyChanged(); }
        }

        public string ViewName
        {
            get => Server.Name.Length > 28 ? Server.Name.Substring(0, 28) + "..." : Server.Name;
        }

        public string Players { get => $"{Server.Online} / {Server.MaxPlayers}"; }

        public string Address { get => $"{Server.Address}"; }

        public string Description { get => description; set { description = value; OnPropertyChanged(); } }

        public Visibility HasMicrophone { get => Visibility.Collapsed; }
        public Visibility HasSkyEye { get => hasSkyEye ? Visibility.Visible : Visibility.Collapsed; }

        public string Locale { get => $"Ru"; }

        public string Ping { get { return ping; } set { ping = value; OnPropertyChanged(); } }
        public bool Selected { get { return selected; } set { selected = value; OnPropertyChanged(); OnPropertyChanged("SelectedRect"); } }

        public Visibility SelectedRect { get => selected ? Visibility.Visible : Visibility.Hidden; }

        public bool Favorite
        {
            get { return favorite; }
            set { favorite = value; OnPropertyChanged(); SetFavorite(); }
        }

        public ServerItemModel(ServerModel server, Action action = null)
        {
            Server = server;
            resort = action;
            Mods = new List<string>();
            ServerIcon = new BitmapImage(new Uri("pack://application:,,,/UpdatesClient;component/Assets/Images/ServerIcon.png"));
            if (Settings.FavoriteServers.Contains(Server.ID))
            {
                Favorite = true;
            }
            GetPing();
            GetAntiCheat();
            GetServerIcon();

            inited = true;
        }

        private static CancellationTokenSource cancelTokenSource;
        public async void GetManifest()
        {
            if (cancelTokenSource != null) cancelTokenSource.Cancel();
            cancelTokenSource = new CancellationTokenSource();

            ServerModsManifest mods;
            try
            {
                mods = await GameUtilities.GetManifest(Server.AddressData);
            }
            catch (HttpRequestException) { return; }
            catch (WebException) { return; }
            catch (TaskCanceledException) { return; }

            if (cancelTokenSource == null || cancelTokenSource.IsCancellationRequested) return;

            GetMods(mods);

            cancelTokenSource = null;
        }
        private void GetMods(ServerModsManifest mods)
        {
            List<string> WhiteListFiles = new List<string>(5)
            {
                "Skyrim.esm",
                "Update.esm",
                "Dawnguard.esm",
                "HearthFires.esm",
                "Dragonborn.esm"
            };

            List<string> lmods = new List<string>();

            mods.Mods.RemoveAll(r => WhiteListFiles.Contains(r.FileName));
            Mods.Clear();
            if (mods.Mods.Count != 0)
            {
                lmods.AddRange(mods.Mods.ConvertAll(c => c.FileName.Substring(0, c.FileName.LastIndexOf('.'))));
            }
            else
            {
                lmods.Add("<no mods>");
            }
            Mods = lmods;
        }

        public async void GetDesc()
        {
            string d = "<Empty>";
            Description = d;
            try
            {
                d = await Net.RequestHttp($"http://{Server.AddressData}/desc.txt", "GET", false, null);
            }
            catch (HttpRequestException) { return; }
            catch (WebException) { return; }
            catch (TaskCanceledException) { return; }

            if (string.IsNullOrWhiteSpace(d)) d = "<Empty>";
            Description = d;
        }

        private async void GetPing()
        {
            try
            {
                Ping ping = new Ping();
                PingOptions options = new PingOptions();
                byte[] buffer = new byte[16] { 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48 };
                const int timeout = 600;
                PingReply reply = await ping.SendPingAsync(Server.IP, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    Ping = reply.RoundtripTime.ToString();
                }
                else
                {
                    Ping = "-";
                }
            }
            catch { Ping = "-"; }
        }

        private async void GetAntiCheat()
        {
            try
            {
                string res = "false";
                try
                {
                    res = await Net.RequestHttp($"http://{Server.AddressData}/SkyEye", "GET", false, null);
                }
                catch (HttpRequestException) { return; }
                catch (WebException) { return; }
                catch (TaskCanceledException) { return; }
                if (res.Length == 36) hasSkyEye = true;
            }
            catch { hasSkyEye = false; }
            OnPropertyChanged(nameof(HasSkyEye));
        }

        private void GetServerIcon()
        {
            try
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();

                logo.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                logo.CacheOption = BitmapCacheOption.Default;
                logo.UriCachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.CacheIfAvailable);

                logo.UriSource = new Uri($"http://{Server.AddressData}/servericon.png");
                logo.EndInit();

                logo.DownloadCompleted += (s, e) =>
                {
                    ServerIcon = logo;
                };
            }
            catch { }
        }

        private void SetFavorite()
        {
            if (inited)
            {
                if (favorite)
                {
                    Settings.FavoriteServers.Add(Server.ID);
                    resort?.Invoke();
                }
                else if (Settings.FavoriteServers.Contains(Server.ID))
                {
                    Settings.FavoriteServers.Remove(Server.ID);
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
