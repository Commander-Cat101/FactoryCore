using FactoryCore.API.ModuleValues;

namespace FactoryCore.API.ModuleProperties
{
    [JsonObject]
    public class SavedColor
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public SavedColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static implicit operator Color(SavedColor color) => new Color(color.r, color.g, color.b, color.a);
        public static implicit operator SavedColor(Color color) => new SavedColor(color.r, color.g, color.b, color.a);
    }
    public class ColorModuleProperty : ModuleProperty
    {
        protected Color defaultColor;
        public bool UseAlpha;
        public ColorModuleProperty(string name, Color DefaultColor, bool useAlpha = true)
        {
            defaultColor = DefaultColor;
            Name = name;
            UseAlpha = useAlpha;
        }
        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("ColorModuleValue", 0, 0, 1000, 450));

            Sprite sprite = null;
            var image = panel.AddImage(new Info("Color", -250, 0, 300, 300), sprite);
            image.Image.color = Module.GetValue<SavedColor>(Name);

            var sliderR = panel.AddSlider(new Info("SliderR", 225, UseAlpha ? 150 : 100, 400, 50), Module.GetValue<SavedColor>(Name).r * 255, 0, 255, 1f, new Vector2(40, 40), new Action<float>((value) =>
            {
                var color = Module.GetValue<SavedColor>(Name);
                color.r = value / 255;
                Module.SetValue(color, Name);
                image.Image.color = color;
            }));
            sliderR.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
            sliderR.AddText(new Info("RText", -275, 0, 50, 50), "R:", 50);

            var sliderG = panel.AddSlider(new Info("SliderG", 225, UseAlpha ? 50 : 0, 400, 50), Module.GetValue<SavedColor>(Name).g * 255, 0, 255, 1f, new Vector2(40, 40), new Action<float>((value) =>
            {
                var color = Module.GetValue<SavedColor>(Name);
                color.g = value / 255;
                Module.SetValue(color, Name);
                image.Image.color = color;
            }));
            sliderG.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
            sliderG.AddText(new Info("GText", -275, 0, 50, 50), "G:", 50);


            var sliderB = panel.AddSlider(new Info("SliderB", 225, UseAlpha ? -50 : -100, 400, 50), Module.GetValue<SavedColor>(Name).b * 255, 0, 255, 1f, new Vector2(40, 40), new Action<float>((value) =>
            {
                var color = Module.GetValue<SavedColor>(Name);
                color.b = value / 255;
                Module.SetValue(color, Name);
                image.Image.color = color;
            }));
            sliderB.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
            sliderB.AddText(new Info("BText", -275, 0, 50, 50), "B:", 50);

            if (UseAlpha)
            {
                var sliderA = panel.AddSlider(new Info("SliderA", 225, -150, 400, 50), Module.GetValue<SavedColor>(Name).a * 255, 0, 255, 1f, new Vector2(40, 40), new Action<float>((value) =>
                {
                    var color = Module.GetValue<SavedColor>(Name);
                    color.a = value / 255;
                    Module.SetValue(color, Name);
                    image.Image.color = color;
                }));
                sliderA.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
                sliderA.AddText(new Info("AText", -275, 0, 50, 50), "A:", 50);
            }

            return panel;
        }

        public override ModHelperPanel CreateTemplate()
        {
            var panel = ModHelperPanel.Create(new Info("ColorModuleValue", 0, 0, 1000, 450));

            var image = panel.AddImage(new Info("Color", -250, 0, 300, 300), (Sprite)null);
            image.AddLayoutElement().ignoreLayout = true;

            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperRight;
            layout.childForceExpandHeight = layout.childForceExpandWidth = false;
            layout.SetPadding(50);
            layout.spacing = 50;
            layout.m_TotalPreferredSize = new Vector2(1000, 450);

            foreach (var character in new[] {"R", "G", "B", "A"})
            {
                var slider = panel.AddSlider(new Info($"Slider{character}", 400, 50), 0, 0, 255, 1f, new Vector2(40, 40), null);
                slider.DefaultNotch.transform.localScale = Vector3.one * 0.6f;
                slider.AddText(new Info($"{character}Text", -275, 0, 50, 50), $"{character}:", 50);
            }
            
            return panel;
        }

        public override void SetupProperty(ModHelperPanel root)
        {
            var image = root.transform.Find("Color").GetComponent<ModHelperImage>();
            image.Image.color = Module.GetValue<SavedColor>(Name);
            
            var sliderR = root.transform.Find("SliderR").GetComponent<ModHelperSlider>();
            sliderR.SetCurrentValue(Module.GetValue<SavedColor>(Name).r * 255, false);
            sliderR.Slider.onValueChanged.AddListener(value =>
            {
                var color = Module.GetValue<SavedColor>(Name);
                color.r = value / 255;
                Module.SetValue(color, Name);
                image.Image.color = color;
            });
            
            var sliderG = root.transform.Find("SliderG").GetComponent<ModHelperSlider>();
            sliderG.SetCurrentValue(Module.GetValue<SavedColor>(Name).g * 255, false);
            sliderG.Slider.onValueChanged.AddListener(value =>
            {
                var color = Module.GetValue<SavedColor>(Name);
                color.g = value / 255;
                Module.SetValue(color, Name);
                image.Image.color = color;
            });
            
            var sliderB = root.transform.Find("SliderB").GetComponent<ModHelperSlider>();
            sliderB.SetCurrentValue(Module.GetValue<SavedColor>(Name).b * 255, false);
            sliderB.Slider.onValueChanged.AddListener(value =>
            {
                var color = Module.GetValue<SavedColor>(Name);
                color.b = value / 255;
                Module.SetValue(color, Name);
                image.Image.color = color;
            });

            if (UseAlpha)
            {
                var sliderA = root.transform.Find("SliderA").GetComponent<ModHelperSlider>();
                sliderA.SetCurrentValue(Module.GetValue<SavedColor>(Name).a * 255, false);
                sliderA.Slider.onValueChanged.AddListener(value =>
                {
                    var color = Module.GetValue<SavedColor>(Name);
                    color.a = value / 255;
                    Module.SetValue(color, Name);
                    image.Image.color = color;
                });
            }
            else
                root.transform.Find("SliderA").gameObject.SetActive(false);
        }

        public override void LoadData()
        {
            if (!Module.HasValue(Name))
                Module.SetValue((SavedColor)defaultColor, Name);
        }
    }
}
