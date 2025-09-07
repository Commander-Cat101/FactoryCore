using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryCore.API.ModuleValues
{
    public class IntSliderModuleProperty : IntModuleProperty
    {
        public IntSliderModuleProperty(string name, int defaultValue, int minValue, int maxValue) : base(name, defaultValue, minValue, maxValue)
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
            var slider = panel.AddSlider(new Info("Slider", 250, 0, 400, 40), DefaultValue, MinValue, MaxValue, 1, new Vector2(40, 40), new Action<float>((value) =>
            {
                Module.SetValue((int)Math.Round(value), Name);
            }));
            slider.SetCurrentValue(Module.GetValue<int>(Name));
            slider.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
            return panel;
        }
    }
}
