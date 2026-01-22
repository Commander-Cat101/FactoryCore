namespace FactoryCore.UI
{
    [RegisterTypeInIl2Cpp(false)]
    public class ModuleHolder : MonoBehaviour
    {
        public Module Module;
        public GameObject Root;
        public ModHelperPanel Panel;

        public ModuleHolder Init(Module module, GameObject root, ModHelperPanel panel)
        {
            Module = module;
            Root = root;
            Panel = panel;
            module.SetupVisual(panel, this);
            return this;
        }
        public void Update()
        {
            Module.Position = Root.transform.localPosition;
        }
        public void Delete()
        {
            foreach (var input in GetComponentsInChildren<ModuleInputHolder>())
            {
                input.RemoveConnections();
            }
            foreach (var output in GetComponentsInChildren<ModuleOutputHolder>())
            {
                output.RemoveConnections();
            }
            Root.DestroyAllChildren();
            Destroy(Root);
        }
    }
}
