using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCore.API
{
    
    public abstract class Template
    {
        public List<Module> modules = new List<Module>();

        public void AddModule(Module module)
        {
            if (module.Id == Guid.Empty)
                module.Id = Guid.NewGuid();
            modules.Add(module);
        }

        public List<T> GetModulesOfType<T>() where T : Module
        {
            var list = new List<T>();
            foreach (var module in modules)
            {
                if (typeof(T) == module.GetType())
                    list.Add((T)module);
            }
            return list;
        }

        public void LoadModules()
        {
            foreach (var module in modules)
            {
                module.Template = this;
            }
        }
    }
}
