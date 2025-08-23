using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCore.API
{
    [JsonObject]
    public class ModuleInput
    {
        public string Name;
        public Guid Id;
        public Type Type;
        public Guid OutputGuid;
    }
}
