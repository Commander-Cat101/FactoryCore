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
        public override ModHelperPanel CreateTemplate()
        {
            var panel = ModHelperPanel.Create(new Info("FloatModuleValue", 0, 0, 1000, 100));
            panel.AddText(new Info("Text", -200, 0, 500, 50), "Property Name", 50, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing();
            
            panel.AddInputField(new Info("Input", 250, 0, 400, 50), "0", VanillaSprites.BlueInsertPanel, null, 30, Il2CppTMPro.TMP_InputField.CharacterValidation.Decimal);
            
            return panel;
        }

        public override void SetupProperty(ModHelperPanel root)
        {
            root.transform.Find("Text").GetComponent<ModHelperText>().SetText(Name);
            
            ModHelperInputField field = root.transform.Find("Input").GetComponent<ModHelperInputField>();
            field.SetText(Module.GetValue<float>(Name).ToString());
            
            field.InputField.onEndEdit.AddListener(new Action<string>((value) =>
            {
                if (float.TryParse(value, out float result))
                {
                    result = Math.Clamp(result, MinValue, MaxValue);
                    field?.SetText(result.ToString(), false);
                    Module.SetValue(result, Name);
                }
            }));
        }

        public override void LoadData()
        {
            if (!Module.HasValue(Name))
                Module.SetValue(DefaultValue, Name);
        }
    }
}
