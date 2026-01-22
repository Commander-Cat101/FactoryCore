namespace FactoryCore.API.ModuleValues
{
    public abstract class ModuleProperty
    {
        public Module Module;
        public string Name { get; set; }

        public abstract ModHelperPanel GetVisual(ModHelperPanel root);

        public abstract void LoadData();
    }
}
