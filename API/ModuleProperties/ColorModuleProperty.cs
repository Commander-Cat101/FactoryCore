using FactoryCore.API.ModuleValues;

namespace FactoryCore.API.ModuleProperties;

[JsonObject]
public class SavedColor(float r, float g, float b, float a)
{
    public float R = r;
    public float G = g;
    public float B = b;
    public float A = a;
    public static implicit operator Color(SavedColor color) => new Color(color.R, color.G, color.B, color.A);
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
        sliderR.SetCurrentValue(Module.GetValue<SavedColor>(Name).R * 255, false);
        sliderR.Slider.onValueChanged.AddListener(value =>
        {
            var color = Module.GetValue<SavedColor>(Name);
            color.R = value / 255;
            Module.SetValue(color, Name);
            image.Image.color = color;
        });
        
        var sliderG = root.transform.Find("SliderG").GetComponent<ModHelperSlider>();
        sliderG.SetCurrentValue(Module.GetValue<SavedColor>(Name).G * 255, false);
        sliderG.Slider.onValueChanged.AddListener(value =>
        {
            var color = Module.GetValue<SavedColor>(Name);
            color.G = value / 255;
            Module.SetValue(color, Name);
            image.Image.color = color;
        });
        
        var sliderB = root.transform.Find("SliderB").GetComponent<ModHelperSlider>();
        sliderB.SetCurrentValue(Module.GetValue<SavedColor>(Name).B * 255, false);
        sliderB.Slider.onValueChanged.AddListener(value =>
        {
            var color = Module.GetValue<SavedColor>(Name);
            color.B = value / 255;
            Module.SetValue(color, Name);
            image.Image.color = color;
        });

        if (UseAlpha)
        {
            var sliderA = root.transform.Find("SliderA").GetComponent<ModHelperSlider>();
            sliderA.SetCurrentValue(Module.GetValue<SavedColor>(Name).A * 255, false);
            sliderA.Slider.onValueChanged.AddListener(value =>
            {
                var color = Module.GetValue<SavedColor>(Name);
                color.A = value / 255;
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
