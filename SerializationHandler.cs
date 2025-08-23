using FactoryCore.API;
using MelonLoader.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCore
{
    internal static class SerializationHandler
    {
        internal static JsonSerializerSettings Settings => new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
        internal static string FolderDirectory => Path.Combine(MelonEnvironment.ModsDirectory, "Factory");
        internal static void EnsureFolderExists()
        {
            if (!Directory.Exists(FolderDirectory))
                Directory.CreateDirectory(FolderDirectory);
        }
        internal static void SaveTemplate(Template template)
        {
            EnsureFolderExists();

            var content = JsonConvert.SerializeObject(template, Settings);
            var path = Path.Combine(FolderDirectory, "Template.json");
            File.WriteAllText(path, content);
        }
        internal static Template LoadTemplate()
        {
            EnsureFolderExists();

            var path = Path.Combine(FolderDirectory, "Template.json");

            if (!File.Exists(path))
                return new Template();
            File.ReadAllText(path);
            var content = JsonConvert.DeserializeObject<Template>(File.ReadAllText(path), Settings);
            return content;
        }
    }
}
