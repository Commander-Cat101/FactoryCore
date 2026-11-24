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

        public override ModHelperPanel CreateTemplate()
        {
            var panel = ModHelperPanel.Create(new Info("FloatModuleValue", 0, 0, 1000, 100));
            panel.AddText(new Info("Text", -200, 0, 500, 50), $"Property Name", 50, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing();
            var slider = panel.AddSlider(new Info("Slider", 250, 0, 400, 40), 0, 0, 1, 1, new Vector2(40, 40), null);
            
            slider.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
            return panel;
        }

        public override void SetupProperty(ModHelperPanel root)
        {
            root.transform.Find("Text").GetComponent<ModHelperText>().SetText(Name);
            var slider = root.transform.Find("Slider").GetComponent<ModHelperSlider>();
            slider.Slider.onValueChanged.AddListener(new Action<float>(value =>
            {
                Module.SetValue((int)Math.Round(value), Name);
            }));
            slider.Slider.maxValue = MaxValue;
            slider.Slider.minValue = MinValue;
            slider.SetCurrentValue(DefaultValue, false);
        }
    }
}
