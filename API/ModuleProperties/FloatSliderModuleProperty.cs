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
        public override ModHelperPanel CreateTemplate()
        {
            var panel = ModHelperPanel.Create(new Info("FloatSliderModuleValue", 0, 0, 1000, 100));
            panel.AddText(new Info("Text", -200, 0, 500, 50), $"Property Name", 50, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing();
            
            return panel;
        }

        public override void SetupProperty(ModHelperPanel root)
        {
            root.transform.Find("Text").GetComponent<ModHelperText>().SetText(Name);
            var slider = root.AddSlider(new Info("Slider", 250, 0, 400, 40), DefaultValue, MinValue, MaxValue, StepSize, new Vector2(40, 40), new Action<float>((value) =>
            {
                Module.SetValue(value, Name);
            }));
            slider.SetCurrentValue(Module.GetValue<float>(Name));
            slider.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
        }
    }
}
