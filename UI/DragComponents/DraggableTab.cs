namespace FactoryCore.UI.Components
{
    [RegisterTypeInIl2Cpp(false)]
    public class DraggableTab : DragComponent
    {
        public DraggableTab(IntPtr ptr) : base(ptr) { }

        public ModuleRootMonoBehavior moduleRoot;

        public void Init(GameObject rootObject)
        {
            moduleRoot = rootObject.GetComponent<ModuleRootMonoBehavior>();
        }
        public override void UpdateDrag(Vector3 dragDifference)
        {
            if (moduleRoot == null)
                return;
            moduleRoot.transform.position += dragDifference;
        }
    }
}
