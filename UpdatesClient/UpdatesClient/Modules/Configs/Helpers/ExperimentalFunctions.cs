using System;
using UpdatesClient.Modules.Debugger;

namespace UpdatesClient.Modules.Configs.Helpers
{
    public class ExperimentalFunctions
    {
        public static void Use(string message, Action action)
        {
            if (!Settings.Loaded) throw new TypeUnloadedException("Settings not loaded");

            try
            {
                if (Settings.ExperimentalFunctions == true)
                {
                    action.Invoke();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ExpFunc_{message}", e);
            }
        }
        public static void IfUse(string message, Action isTrue, Action isFalse)
        {
            if (!Settings.Loaded) throw new TypeUnloadedException("Settings not loaded");

            if (Settings.ExperimentalFunctions == true)
            {
                try
                {
                    isTrue.Invoke();
                }
                catch (Exception e)
                {
                    Logger.Error($"ExpFunc_{message}", e);
                }
            }
            else
            {
                isFalse.Invoke();
            }
        }
        public static bool HasExperimentalFunctions()
        {
            if (!Settings.Loaded) throw new TypeUnloadedException("Settings not loaded");
            return Settings.ExperimentalFunctions == true;
        }
    }
}
