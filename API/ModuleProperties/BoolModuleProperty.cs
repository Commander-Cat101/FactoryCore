using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API.ModuleValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FactoryCore.API.ModuleProperties
{
    public class BoolModuleProperty : ModuleProperty
    {
        internal bool DefaultValue;
        public BoolModuleProperty(string name, bool defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }

        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("BoolModuleValue", 0, 0, 1000, 100));
            panel.AddText(new Info("Text", -200, 0, 500, 50), $"{Name}", 50, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing();
            ModHelperButton? button = null;
            button = panel.AddButton(new Info("Toggle", -80, 0, 60, 60, new Vector2(1, 0.5f)), "", new Action(() =>
            {
                OnClick(button);
            }));
            var value = Module.GetValue<bool>(Name);
            button.Image.SetSprite(value ? Assets.OnBtn : Assets.OffBtn);
            return panel;
        }
        public override void LoadData()
        {
            if (!Module.HasValue(Name))
                Module.SetValue(DefaultValue, Name);
        }
        public void OnClick(ModHelperButton? button)
        {
            var value = !Module.GetValue<bool>(Name);
            Module.SetValue(value, Name);
            button?.Image.SetSprite(value ? Assets.OnBtn : Assets.OffBtn);
        }
    }
}
