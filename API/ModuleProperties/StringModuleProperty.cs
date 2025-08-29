using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryCore.API.ModuleValues
{
    public class StringModuleProperty : ModuleProperty
    {
        protected string DefaultValue;
        protected int MaxLength;
        public StringModuleProperty(string name, string defaultValue, int maxLength)
        {
            DefaultValue = defaultValue;
            MaxLength = maxLength;
            Name = name;
        }
        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("StringModuleValue", 0, 0, 1000, 100));
            panel.AddText(new Info("Text", -200, 0, 500, 50), $"{Name}", 50, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing();
            panel.AddInputField(new Info("Input", 250, 0, 400, 50), Module.GetValue<string>(Name).ToString(), VanillaSprites.BlueInsertPanel, new Action<string>((value) =>
            {
                Module.SetValue(value, Name);
            }), 30).InputField.characterLimit = MaxLength;
            return panel;
        }
        public override void LoadData()
        {
            if (!Module.HasValue(Name))
                Module.SetValue(DefaultValue, Name);
        }
    }
}
