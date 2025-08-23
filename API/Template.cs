using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCore.API
{
    
    internal class Template
    {
        public List<Module> modules = new List<Module>();

        public void AddModule(Module module)
        {
            if (module.Id == Guid.Empty)
                module.Id = Guid.NewGuid();
            modules.Add(module);
        }
    }
}
