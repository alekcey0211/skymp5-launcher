using System;
using System.Collections.Generic;

namespace UpdatesClient.Modules.Debugger
{
    public static class Logger
    {
        public static void Init(Version version)
        {
            SentryManager.Init(version);
            YandexMetricaManager.Init(version);
        }

        public static void Error(string message, Exception exception, IEnumerable<KeyValuePair<string, string>> extraTags = null)
        {
            SentryManager.Error(message, exception, extraTags);
        }

        public static void FatalError(string message, Exception exception)
        {
            SentryManager.FatalError(message, exception);
        }

        public static void Event(string message)
        {
            SentryManager.Event(message);
        }

        public static void ActivateMetrica()
        {
            YandexMetricaManager.Activate();
        }

        public static void DisableCrashTracking()
        {
            YandexMetricaManager.SetCrashTracking(false);
        }

        public static void ReportMetricaEvent(string text)
        {
            YandexMetricaManager.ReportEvent(text);
        }
    }
}