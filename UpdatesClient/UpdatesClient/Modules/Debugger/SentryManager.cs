using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UpdatesClient.Core;
using UpdatesClient.Modules.Configs;
using UpdatesClient.Modules.SelfUpdater;

namespace UpdatesClient.Modules.Debugger
{
    public static class SentryManager
    {
        private static string uri;
        private static string AssemblyShortName
        {
            get
            {
                if (_assemblyShortName == null)
                {
                    Assembly a = typeof(Logger).Assembly;
                    _assemblyShortName = a.ToString().Split(',')[0];
                }
                return _assemblyShortName;
            }
        }
        private static string _assemblyShortName;
        private static string md5Launcher;
        private const string dsn = "https://13d9192e33aa4e86a8f9a55d89d5ffc5:2def384d727d457482cf641f234a2fc8@sentry.skyrez.su/4";

        public static void Init(Version version)
        {
            try
            {
                uri = $"pack://application:,,,/{AssemblyShortName};component/{{path}}";
                md5Launcher = Hashing.GetMD5FromFile(File.OpenRead(EnvParams.PathToFile));
            }
            catch { }

            try
            {
                SentrySdk.Init(options =>
                {
                    options.Dsn = new Dsn(dsn);
                    options.Release = version.ToString();
                    options.BeforeSend = SentryEvent;
                });
            }
            catch { }
        }

        private static SentryEvent SentryEvent(SentryEvent e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Settings.UserName) && Settings.UserId != 0)
                {
                    e.User = new Sentry.Protocol.User()
                    {
                        Id = Settings.UserId.ToString(),
                        Username = Settings.UserName
                    };
                }

                e.SetTag("Locale", Settings.Locale.ToString());
                //e.SetTag("URI", uri);
                e.SetTag("MD5", md5Launcher);
                e.SetTag("ExperimentalFunctions", Settings.ExperimentalFunctions?.ToString());
            }
            catch { }

            return e;
        }

        public static void Error(string message, Exception exception, IEnumerable<KeyValuePair<string, string>> extraTags)
        {
            SentryEvent sentryEvent = new SentryEvent(exception)
            {
                Message = message,
                Level = Sentry.Protocol.SentryLevel.Error,
            };
            if (extraTags != null) sentryEvent.SetTags(extraTags);
            sentryEvent.SetTag("HRESULT", exception.HResult.ToString());

            SentrySdk.CaptureEvent(sentryEvent);
        }

        public static void FatalError(string message, Exception exception)
        {
            SentryEvent sentryEvent = new SentryEvent(exception)
            {
                Message = message,
                Level = Sentry.Protocol.SentryLevel.Fatal
            };
            sentryEvent.SetTag("HRESULT", exception.HResult.ToString());

            SentrySdk.CaptureEvent(sentryEvent);
        }

        public static void Event(string message)
        {
            SentrySdk.CaptureMessage(message);
        }
    }
}