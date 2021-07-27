using Newtonsoft.Json;
using System;
using System.IO;

namespace UpdatesClient.Core.Helpers
{
    public static class JsonModelSaver
    {
        public static void Save(this IJsonSaver obj, string path)
        {
            IO.CreateDirectory(Path.GetDirectoryName(path));
            if (obj != null) File.WriteAllText(path, JsonConvert.SerializeObject(obj));
        }

        public static T Load<T>(this IJsonSaver obj, string path) where T : new()
        {
            if (Directory.Exists(Path.GetDirectoryName(path)) && File.Exists(path))
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return new T();
        }

        //!Может не работать
        public static IJsonSaver Load(this IJsonSaver obj, string path)
        {
            Type t = obj.GetType();
            if (Directory.Exists(Path.GetDirectoryName(path)) && File.Exists(path))
                return (IJsonSaver)JsonConvert.DeserializeObject(File.ReadAllText(path), t);
            return default;
        }
    }

    public interface IJsonSaver { }
}
