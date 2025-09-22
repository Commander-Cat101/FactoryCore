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
            return GetModulesOfType(typeof(T)).Select(a => (T)a).ToList();
        }
        public List<Module> GetModulesOfType(Type moduleType)
        {
            var list = new List<Module>();
            foreach (var module in modules)
            {
                if (moduleType == module.GetType() || module.GetType().IsSubclassOf(moduleType))
                    list.Add(module);
            }
            return list;
        }

        public void LoadModules()
        {
            foreach (var module in modules)
            {
                module.Init();
            }
        }
        public void SetReferences()
        {
            foreach (var module in modules)
            {
                module.Template = this;
            }
        }
    }
}
