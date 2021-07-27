using System.Windows;
using System.Windows.Controls;
using UpdatesClient.Core;
using UpdatesClient.Core.Enums;
using UpdatesClient.Modules.Configs;
using UpdatesClient.Modules.GameManager;
using UpdatesClient.Modules.ModsManager;
using UpdatesClient.UI.Pages.MainWindow.Models;

namespace UpdatesClient.UI.Pages.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class SettingsPanel : UserControl
    {
        private SettingsPanelModel Model;

        public SettingsPanel()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            Model = new SettingsPanelModel()
            {
                Locales = new string[] { "Русский", "English" },
                SelectedLocale = GetIdByLocale(Settings.Locale),
                SkyrimPath = Settings.PathToSkyrim,
                ExpFunctions = Settings.ExperimentalFunctions ?? false,
                DisabledSKSE = ModVersion.SKSEDisabled,
                DisabledMods = ModVersion.ModsDisabled,
                CanDisabledMods = Mods.ExistMod("SkyMPCore"),
                CanDisabledSKSE = Mods.ExistMod("SKSE"),
                SelfVersion = $"Vers. {EnvParams.VersionFile}.{Settings.UserId}"
            };
            grid.DataContext = Model;
        }

        public async void Save()
        {
            bool modified = false;
            bool modifiedMv = false;
            if (Settings.PathToSkyrim != Model.SkyrimPath)
            {
                Settings.PathToSkyrim = Model.SkyrimPath;
                modified = true;
            }

            if (Settings.Locale != GetLocaleById(Model.SelectedLocale))
            {
                Settings.Locale = GetLocaleById(Model.SelectedLocale);
                modified = true;
            }

            if (Settings.ExperimentalFunctions != Model.ExpFunctions)
            {
                Settings.ExperimentalFunctions = Model.ExpFunctions;
                modified = true;
            }

            if (ModVersion.ModsDisabled != Model.DisabledMods)
            {
                ModVersion.ModsDisabled = Model.DisabledMods;
                if (ModVersion.ModsDisabled)
                {
                    await Mods.DisableAll();
                    await Mods.DisableMod("RuFixConsole");
                    await Mods.DisableMod("SkyMPCore");
                }
                else
                {
                    if (Mods.ExistMod("RuFixConsole")) await Mods.EnableMod("RuFixConsole");
                    if (Mods.ExistMod("SkyMPCore")) await Mods.EnableMod("SkyMPCore");
                }
                modifiedMv = true;
            }

            if (Model.CanDisabledSKSE && ModVersion.SKSEDisabled != Model.DisabledSKSE)
            {
                ModVersion.SKSEDisabled = Model.DisabledSKSE;
                if (ModVersion.SKSEDisabled)
                {
                    await Mods.DisableAll(true);
                }
                else
                {
                    if (ModVersion.ModsDisabled && Mods.ExistMod("SKSE")) await Mods.EnableMod("SKSE");
                    else await ModUtilities.ActivateCoreMod();
                }
                modifiedMv = true;
            }

            if (modified) Settings.Save();
            if (modifiedMv) ModVersion.Save();
        }

        private int GetIdByLocale(Locales locale)
        {
            switch (locale)
            {
                case Locales.ru_RU:
                    return 0;
                case Locales.en_US:
                    return 1;
                default: return -1;
            }
        }

        private Locales GetLocaleById(int locale)
        {
            return (Locales)(locale + 1);
        }

        private void SelectSkyrimPath(object sender, RoutedEventArgs e)
        {
            Model.SkyrimPath = GameVerification.GetGameFolder() ?? Settings.PathToSkyrim;
        }
    }
}
