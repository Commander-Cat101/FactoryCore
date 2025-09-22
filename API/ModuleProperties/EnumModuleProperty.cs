using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using FactoryCore.API.ModuleValues;
using System;
using UnityEngine;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

namespace FactoryCore.API.ModuleProperties
{
    public class EnumModuleProperty : ModuleProperty
    {
        public string[] Options;
        public int DefaultValue;

        public Action<int> OnValueChanged;
        public EnumModuleProperty(string name, string[] options, int defaultValue, Action<int> onValueChanged = null)
        {
            Name = name;
            Options = options;
            DefaultValue = defaultValue;
            OnValueChanged = onValueChanged;
        }
        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            int value = Module.GetValue<int>(Name);

            var panel = root.AddPanel(new Info("EnumModuleProperty", 0, 0, 1000, 100));

            panel.AddText(new Info("Text", -250, 0, 400, 100), $"{Name}", 50, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing();

            var text = panel.AddText(new Info("Type", 200, 0, 300, 50), Options[value]);
            text.Text.overflowMode = Il2CppTMPro.TextOverflowModes.Overflow;

            var left = panel.AddButton(new Info("Left", 0, 0, 75, 75), VanillaSprites.Arrow, new Action(() =>
            {
                ChangeValue(-1);
                SetText(text);
            }));
            left.transform.rotation = Quaternion.Euler(0, 0, 180);

            panel.AddButton(new Info("Right", 400, 0, 75, 75), VanillaSprites.Arrow, new Action(() =>
            {
                ChangeValue(1);
                SetText(text);
            })).transform.rotation.SetEulerAngles(0, 0, -90);


            return panel;
        }
        internal void SetText(ModHelperText text)
        {
            text.SetText(Options[Module.GetValue<int>(Name)]);
        }
        internal void ChangeValue(int change)
        {
            var value = Module.GetValue<int>(Name) + change;
            if (value < 0)
                value = Options.Length - 1;
            if (value >= Options.Length)
                value = 0;
            Module.SetValue(value, Name);
            OnValueChanged?.Invoke(value);
        }
        public override void LoadData()
        {
            if (!Module.HasValue(Name))
                Module.SetValue(DefaultValue, Name);

            var value = Module.GetValue<int>(Name);

            if (value < 0 || value >= Options.Length)
                Module.SetValue(DefaultValue, Name);
        }
    }
}
