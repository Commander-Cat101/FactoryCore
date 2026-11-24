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
        public override ModHelperPanel CreateTemplate()
        {
            var panel = ModHelperPanel.Create(new Info("IntModuleValue", 0, 0, 1000, 100));
            panel.AddText(new Info("Text", -200, 0, 500, 50), $"Property Name", 50, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing();
            
            ModHelperInputField field = panel.AddInputField(new Info("Input", 250, 0, 400, 50), "0", VanillaSprites.BlueInsertPanel, null, 30, Il2CppTMPro.TMP_InputField.CharacterValidation.Integer);
            field.InputField.characterLimit = 9;

            return panel;
        }

        public override void SetupProperty(ModHelperPanel root)
        {
            root.transform.Find("Text").GetComponent<ModHelperText>().SetText(Name);
            var field = root.transform.Find("Input").GetComponent<ModHelperInputField>();
            field.SetText(Module.GetValue<int>(Name).ToString());
            field.InputField.onEndEdit.AddListener(new Action<string>(value =>
            {
                if (int.TryParse(value, out int result))
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
