using BTD_Mod_Helper.Api.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCore.API.ModuleValues
{
    public abstract class ModuleProperty
    {
        public Module Module;
        public string Name { get; set; }

        public abstract ModHelperPanel GetVisual(ModHelperPanel root);

        public abstract void LoadData();
    }
}
