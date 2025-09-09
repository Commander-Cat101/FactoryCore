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
    public class IntModuleProperty : ModuleProperty
    {

        protected int DefaultValue;
        protected int MinValue;
        protected int MaxValue;
        public IntModuleProperty(string name, int defaultValue, int minValue, int maxValue)
        {
            DefaultValue = defaultValue;
            MinValue = minValue;
            MaxValue = maxValue;
            Name = name;
        }
        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("IntModuleValue", 0, 0, 1000, 100));
            panel.AddText(new Info("Text", -200, 0, 500, 50), $"{Name}", 50, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing();
            panel.AddInputField(new Info("Input", 250, 0, 400, 50), Module.GetValue<int>(Name).ToString(), VanillaSprites.BlueInsertPanel, new Action<string>((value) =>
            {
                if (int.TryParse(value, out int result))
                    Module.SetValue(result, Name);
            }), 30, Il2CppTMPro.TMP_InputField.CharacterValidation.Integer).InputField.characterLimit = 9;
            return panel;
        }
        public override void LoadData()
        {
            if (!Module.HasValue(Name))
                Module.SetValue(DefaultValue, Name);
        }
    }
}
