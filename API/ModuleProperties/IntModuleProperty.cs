using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            ModHelperInputField field = null;
            field = panel.AddInputField(new Info("Input", 250, 0, 400, 50), Module.GetValue<int>(Name).ToString(), VanillaSprites.BlueInsertPanel, new Action<string>((value) =>
            {
                
            }), 30, Il2CppTMPro.TMP_InputField.CharacterValidation.Integer);

            field.InputField.onEndEdit.AddListener(new Action<string>((value) =>
            {
                if (int.TryParse(value, out int result))
                {
                    result = Math.Clamp(result, MinValue, MaxValue);
                    field?.SetText(result.ToString(), false);
                    Module.SetValue(result, Name);
                }
            }));

            field.InputField.characterLimit = 9;

            return panel;
        }
        public override void LoadData()
        {
            if (!Module.HasValue(Name))
                Module.SetValue(DefaultValue, Name);
        }
    }
}
