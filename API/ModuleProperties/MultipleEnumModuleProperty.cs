using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using FactoryCore.API.ModuleValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCore.API.ModuleProperties
{
    internal class MultipleEnumModuleProperty : ModuleProperty
    {
        public string[] Options;
        public int DefaultValue;
        public MultipleEnumModuleProperty(string name, string[] options, int defaultValue)
        {
            Name = name;
            Options = options;
            DefaultValue = defaultValue;
        }
        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("MultipleEnumModuleProperty", 0, 0, 1000, 100));

            panel.AddText(new Info("Text", -250, 0, 400, 50), $"{Name}", 50, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing();

            var text = panel.AddText(new Info("Type", 200, 0, 400, 50), Options[Module.GetValue<int>(Name)]);
            return panel;
        }
        internal void SetText(ModHelperText text)
        {
            text.SetText(Options[Module.GetValue<int>(Name)]);
        }
        internal void ChangeValue(int change)
        {
            var value = Module.GetValue<int>(Name) + change;
            while (value < 0)
                value = Options.Length;
            while (value > Options.Length)
                value = 0;
            Module.SetValue(value, Name);
        }
        public override void LoadData()
        {
            if (!Module.HasValue(Name))
                Module.SetValue(DefaultValue, Name);
        }
    }
}
