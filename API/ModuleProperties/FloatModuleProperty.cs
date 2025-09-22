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
    public class FloatModuleProperty : ModuleProperty
    {
        protected float DefaultValue;
        protected float MinValue;
        protected float MaxValue;
        public FloatModuleProperty(string name, float defaultValue, float minValue, float maxValue)
        {
            DefaultValue = defaultValue;
            MinValue = minValue;
            MaxValue = maxValue;
            Name = name;
        }
        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("FloatModuleValue", 0, 0, 1000, 100));
            panel.AddText(new Info("Text", -200, 0, 500, 50), $"{Name}", 50, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing();
            ModHelperInputField field = null;
            field = panel.AddInputField(new Info("Input", 250, 0, 400, 50), Module.GetValue<float>(Name).ToString(), VanillaSprites.BlueInsertPanel, new Action<string>((value) =>
            {
                
            }), 30, Il2CppTMPro.TMP_InputField.CharacterValidation.Decimal);

            field.InputField.onEndEdit.AddListener(new Action<string>((value) =>
            {
                if (float.TryParse(value, out float result))
                {
                    result = Math.Clamp(result, MinValue, MaxValue);
                    field?.SetText(result.ToString(), false);
                    Module.SetValue(result, Name);
                }
            }));

            return panel;
        }
        public override void LoadData()
        {
            if (!Module.HasValue(Name))
                Module.SetValue(DefaultValue, Name);
        }
    }
}
