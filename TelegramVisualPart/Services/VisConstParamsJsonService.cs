using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramVisualPart.Services
{
    public static class VisConstParamsJsonService
    {
        private static Dictionary<string, string> _dict = null;
        private  static void SetStringParams(string fileName)
        {
            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string parentPath = baseDirectoryInfo.Parent.Parent.Parent.Parent.FullName;
            string tempPath = Path.Combine(parentPath, "TelegramVisualPart");
            string jsonFilePath = Path.Combine(tempPath, fileName);

            string json = File.ReadAllText(jsonFilePath);
            _dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static int GetNumByName(string name)
        {
            const string _fileName = "VisParams.json";
            SetStringParams(_fileName);
            int.TryParse(_dict[name], out int res);

            return res;
        }

        public static string GetStringByName(string name)
        {
            const string _fileName = "VisParams.json";
            SetStringParams(_fileName);
            return _dict[name];
        }

    }
}
