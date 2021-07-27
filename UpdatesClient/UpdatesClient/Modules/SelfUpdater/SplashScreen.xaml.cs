using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using UpdatesClient.Core.Enums;
using UpdatesClient.Core.Network;
using UpdatesClient.Modules.Configs;
using UpdatesClient.Modules.Debugger;
using Res = UpdatesClient.Properties.Resources;

namespace UpdatesClient.Modules.SelfUpdater
{
    /// <summary>
    /// Логика взаимодействия для SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private bool WaitOk = false;
        private bool Ok = false;

        private int bannerIdx = 0;
        private int bannerDelay = 0;

        public SplashScreen()
        {
            try
            {
                if (Settings.Locale == 0)
                {
                    SelectLanguage language = new SelectLanguage();
                    language.ShowDialog();
                    if (string.IsNullOrEmpty(language.LanguageBase))
                    {
                        Close();
                        return;
                    }

                    Settings.Locale = GetLocaleByName(language.LanguageBase);
                    Settings.Save();
                }
                try
                {
                    Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(GetLocaleDescription(Settings.Locale));
                }
                catch (Exception e) { Logger.Error("SetLanguage", e); }
            }
            catch (Exception e)
            {
                Logger.Error("SelectLanguage", e);
            }

            InitializeComponent();
            Loaded += new RoutedEventHandler(Splash_Loaded);
        }

        private Locales GetLocaleByName(string name)
        {
            switch (name)
            {
                case "ru-RU": return Locales.ru_RU;
                case "en-US": return Locales.en_US;
                default: return Locales.nul;
            }
        }
        private string GetLocaleDescription(Locales locale)
        {
            MemberInfo[] memInfo = typeof(Locales).GetMember(locale.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return "en-US";
        }

        void Splash_Loaded(object sender, RoutedEventArgs e)
        {
            IAsyncResult result = null;

            void initCompleted(IAsyncResult ar)
            {
                App.Current.ApplicationInitialize.EndInvoke(result);
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Invoker)delegate { Close(); });
            }
            result = App.Current.ApplicationInitialize.BeginInvoke(this, initCompleted, null);

            ShowBanners();
        }

        public void SetProgress(double Value, double LenFile, double prDown)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Invoker)delegate
            {
                progBar.Value = prDown / 100;
                Status.Text = $"{(int)prDown}% ({Value}КБ/{LenFile}КБ)";
            });
        }
        public void SetProgressMode(bool IsIdenterminate)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Invoker)delegate { progBar.IsIndeterminate = IsIdenterminate; });
        }
        public void SetStatus(string text)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Invoker)delegate { Status.Text = text; });
        }

        public void Ready()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Invoker)delegate
            {
                auth.SignIn += Auth_SignIn;
                DispReady();
            });
        }

        private async void DispReady()
        {
            try
            {
                SetProgressMode(true);
                Status.Text = Res.Authorization;
                await Task.Run(() => Auth());
                SetProgressMode(false);
            }
            catch
            {
                Animation();
            }
        }

        private async void ShowBanners()
        {
            await Task.Yield();
            if (NetworkSettings.Banners == "null") return;
            string[] bannersUrl = NetworkSettings.Banners.Split('|');
            List<ImageSource> banners = new List<ImageSource>(bannersUrl.Length);
            foreach (string url in bannersUrl)
            {
                banners.Add(await GetBanner(url));

                Ellipse ellipse = new Ellipse()
                {
                    Height = 9,
                    Width = 9,
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Stroke = new BrushConverter().ConvertFromString("#FF969696") as SolidColorBrush,
                    Margin = new Thickness(0, 0, 8, 0)
                };
                ellipse.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
                bannersButton.Children.Add(ellipse);
            }
            banners.Reverse();

            AutoSet();
            while (!WaitOk)
            {
                if (banner.Source != banners[bannerIdx])
                    banner.Source = banners[bannerIdx];
                await Task.Delay(100);
            }
        }
        private async void AutoSet()
        {
            while (!WaitOk)
            {
                for (int i = bannersButton.Children.Count - 1; i >= 0;)
                {
                    if (bannerDelay > 0)
                    {
                        bannerDelay -= 50;
                        await Task.Delay(50);
                    }
                    else
                    {
                        Ellipse_MouseLeftButtonUp(bannersButton.Children[i], null);
                        i--;
                    }
                }
            }
        }
        private void Ellipse_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (Ellipse b in bannersButton.Children)
            {
                b.StrokeThickness = 1;
                b.Fill = Brushes.Transparent;
            }
            Ellipse el = (Ellipse)sender;
            el.StrokeThickness = 0;
            el.Fill = Brushes.White;
            bannerIdx = bannersButton.Children.IndexOf(el);
            bannerDelay = 5000;
        }

        public async Task<ImageSource> GetBanner(string url)
        {
            ImageSource image = BitmapFrame.Create(new Uri("pack://application:,,,/Assets/Images/Banners/Default.png"), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None);

            try
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();

                logo.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                logo.CacheOption = BitmapCacheOption.None;
                logo.UriCachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Reload);

                logo.UriSource = new Uri(url);
                logo.EndInit();
                while (logo.IsDownloading) await Task.Delay(10);
                image = logo;
            }
            catch { }

            return image;
        }

        private async Task Auth()
        {
            string username = await Account.GetLogin();
            Settings.UserName = username;
            Ok = true;
            WaitOk = true;
        }

        private async void Auth_SignIn()
        {
            try
            {
                await Auth();
            }
            catch { }
        }

        public bool Wait()
        {
            //Условие выхода - авторизация
            while (!WaitOk) Thread.Sleep(100);
            return Ok;
        }

        public async void Animation()
        {
            progressBarGrid.Visibility = Visibility.Collapsed;
            header.MoveIsEnabled = false;
            const double width = 1054;
            TimeSpan span = new TimeSpan(0, 0, 0, 0, 750);

            DoubleAnimation animWidth = new DoubleAnimation(width, new Duration(span))
            {
                FillBehavior = FillBehavior.HoldEnd
            };
            BeginAnimation(WidthProperty, animWidth);
            BeginAnimation(MinWidthProperty, animWidth);
            DoubleAnimation animLeft = new DoubleAnimation(Left - (width - 469) / 2, new Duration(span))
            {
                FillBehavior = FillBehavior.Stop
            };
            BeginAnimation(LeftProperty, animLeft);

            await Task.Delay(span);
            header.MoveIsEnabled = true;
        }
    }
}
