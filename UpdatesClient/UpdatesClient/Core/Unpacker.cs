using SharpCompress.Archives;
using SharpCompress.Common;
using System.IO;
using System.Linq;
using UpdatesClient.Modules.Configs;

namespace UpdatesClient.Core
{
    public class Unpacker
    {
        public static bool UnpackArchive(string file, string extractTo, string extractFromSub = "")
        {
            if (!File.Exists(file)) return false;

            string tmpFiles = $"{Settings.PathToSkyrimTmp}files\\";
            IO.RemoveDirectory(tmpFiles);
            IO.CreateDirectory(tmpFiles);

            using (IArchive archive = ArchiveFactory.Open(file))
            {
                foreach (IArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(tmpFiles, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }

            IO.RecursiveCopy($"{tmpFiles}{extractFromSub}\\", extractTo);

            IO.RemoveDirectory(tmpFiles);
            File.Delete(file);

            return true;
        }
    }
}
