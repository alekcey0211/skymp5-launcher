using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UpdatesClient.Modules.Debugger;

namespace UpdatesClient.Core
{
    public class IniFile
    {
        private readonly string Path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string IniPath)
        {
            Path = new FileInfo(IniPath).ToString();
        }

        public string ReadINI(string Section, string Key)
        {
            try
            {
                var RetVal = new StringBuilder(65500);
                GetPrivateProfileString(Section, Key, "", RetVal, 65500, Path);
                return RetVal.ToString();
            }
            catch (Exception Ex)
            {
                Logger.Error("IniFile_Read", Ex);
                return "0";
            }


        }
        //Записываем в ini-файл. Запись происходит в выбранную секцию в выбранный ключ.
        public void WriteINI(string Section, string Key, string Value)
        {
            try
            {
                WritePrivateProfileString(Section, Key, Value, Path);
            }
            catch (Exception Ex)
            {
                Logger.Error("IniFile_Write", Ex);
            }

        }

        //Удаляем ключ из выбранной секции.
        public void DeleteKey(string Key, string Section = null)
        {
            try
            {
                WriteINI(Section, Key, null);
            }
            catch (Exception Ex)
            {
                Logger.Error("IniFile_DeleteKey", Ex);
            }

        }
        //Удаляем выбранную секцию
        public void DeleteSection(string Section = null)
        {
            try
            {
                WriteINI(Section, null, null);
            }
            catch (Exception Ex)
            {
                Logger.Error("IniFile_DeleteSection", Ex);
            }

        }
        //Проверяем, есть ли такой ключ, в этой секции
        public bool KeyExists(string Section, string Key)
        {
            try
            {
                return ReadINI(Section, Key).Length > 0;
            }
            catch (Exception Ex)
            {
                Logger.Error("IniFile_KeyExists", Ex);
                return false;
            }
        }
    }
}
