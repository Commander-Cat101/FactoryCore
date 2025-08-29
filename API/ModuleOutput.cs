using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCore.API
{
    [JsonObject]
    public class ModuleOutput
    {
        public string Name;
        public Guid Id;
        public Type Type;
        public List<Guid> InputsGuids = new List<Guid>();
        public Func<object> OutputFunc;
        
    }
}
