namespace FactoryCore.API.ModuleValues
{
    public class SpaceModuleProperty : ModuleProperty
    {
        float Height;

        public SpaceModuleProperty(float Height)
        {
            this.Height = Height;
        }

        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            return root.AddPanel(new Info("Space", 0, 0, 1000, Height));
        }

        public override ModHelperPanel CreateTemplate()
        {
            return ModHelperPanel.Create(new Info("Space", 0, 0, 1000, Height));
        }

        public override void SetupProperty(ModHelperPanel root)
        {
            root.RectTransform.sizeDelta = new Vector2(1000, Height);
        }

        public override void LoadData()
        { 

        }
    }
}
