using BTD_Mod_Helper.Api.Components;
using FactoryCore.API.ModuleValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppNinjaKiwi.GUTS.Models.BossRushRandomizerSettings;
using static Il2CppSystem.Linq.Expressions.Interpreter.NullableMethodCallInstruction;

namespace FactoryCore.API.ModuleProperties
{
    public class ColorModuleProperty : ModuleProperty
    {
        protected Color defaultColor;
        public ColorModuleProperty(string name, Color DefaultColor)
        {
            defaultColor = DefaultColor;
            Name = name;
        }
        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("ColorModuleValue", 0, 0, 1000, 450));

            Sprite sprite = null;
            var image = panel.AddImage(new Info("Color", -250, 0, 300, 300), sprite);
            image.Image.color = Module.GetValue<Color>(Name);

            var sliderR = panel.AddSlider(new Info("Slider", 225, 150, 400, 50), Module.GetValue<Color>(Name).r * 255, 0, 255, 1f, new Vector2(40, 40), new Action<float>((value) =>
            {
                var color = Module.GetValue<Color>(Name);
                color.r = value / 255;
                Module.SetValue(color, Name);
                image.Image.color = color;
            }));
            sliderR.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
            sliderR.AddText(new Info("RText", -275, 0, 50, 50), "R:", 50);

            var sliderG = panel.AddSlider(new Info("Slider", 225, 50, 400, 50), Module.GetValue<Color>(Name).g * 255, 0, 255, 1f, new Vector2(40, 40), new Action<float>((value) =>
            {
                var color = Module.GetValue<Color>(Name);
                color.g = value / 255;
                Module.SetValue(color, Name);
                image.Image.color = color;
            }));
            sliderG.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
            sliderG.AddText(new Info("GText", -275, 0, 50, 50), "G:", 50);


            var sliderB = panel.AddSlider(new Info("Slider", 225, -50, 400, 50), Module.GetValue<Color>(Name).b * 255, 0, 255, 1f, new Vector2(40, 40), new Action<float>((value) =>
            {
                var color = Module.GetValue<Color>(Name);
                color.b = value / 255;
                Module.SetValue(color, Name);
                image.Image.color = color;
            }));
            sliderB.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
            sliderB.AddText(new Info("BText", -275, 0, 50, 50), "B:", 50);


            var sliderA = panel.AddSlider(new Info("Slider", 225, -150, 400, 50), Module.GetValue<Color>(Name).a * 255, 0, 255, 1f, new Vector2(40, 40), new Action<float>((value) =>
            {
                var color = Module.GetValue<Color>(Name);
                color.a = value / 255;
                Module.SetValue(color, Name);
                image.Image.color = color;
            }));
            sliderA.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
            sliderA.AddText(new Info("AText", -275, 0, 50, 50), "A:", 50);

            return panel;
        }
        public override void LoadData()
        {
            if (!Module.HasValue(Name))
                Module.SetValue(defaultColor, Name);
        }
    }
}
