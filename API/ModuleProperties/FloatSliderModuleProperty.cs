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
    public class FloatSliderModuleProperty : FloatModuleProperty
    {
        protected float StepSize;

        public FloatSliderModuleProperty(string name, float defaultValue, float minValue, float maxValue, float stepSize) : base(name, defaultValue, minValue, maxValue)
        {
            DefaultValue = defaultValue;
            MinValue = minValue;
            MaxValue = maxValue;
            StepSize = stepSize;
            Name = name;
        }

        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("FloatSliderModuleValue", 0, 0, 1000, 100));
            panel.AddText(new Info("Text", -200, 0, 500, 50), $"{Name}", 50, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing();
            var slider = panel.AddSlider(new Info("Slider", 250, 0, 400, 40), Module.GetValue<float>(Name), MinValue, MaxValue, StepSize, new Vector2(40, 40), new Action<float>((value) =>
            {
                Module.SetValue(value, Name);
            }));
            slider.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
            return panel;
        }
    }
}
