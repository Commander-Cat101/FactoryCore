using BTD_Mod_Helper.Api.Components;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryCore.API
{
    internal class BloonModule : Module
    {
        public override string Name => "Bloon";

        public override void GetModuleProperties()
        {
            AddProperty(new IntModuleProperty("Health", 1, 1, 1000));
            AddProperty(new FloatSliderModuleProperty("Speed", 0.1f, 1, 25, 0.1f));
            AddProperty(new FloatModuleProperty("Damage", 1, 1, 25));
            AddProperty(new ColorModuleProperty("Color", Color.red));
        }
        public override void GetLinkNodes()
        {
            AddInput<float>("Number");
            AddInput<bool>("Bool");
            AddOutput<bool>("Visuals");
            AddOutput<float>("Number");
        }
    }
}
