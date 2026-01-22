namespace FactoryCore.UI.ContextMenus
{
    [RegisterTypeInIl2Cpp(false)]
    internal class ContextMenuPanel : ModHelperPanel
    {
        internal ContextMenuInfo contextInfo;

        ContextMenuEntryMonoBehavior hoveredEntry;

        List<ContextMenuEntryMonoBehavior> entryBehaviors = new List<ContextMenuEntryMonoBehavior>();
        public ContextMenuPanel(IntPtr ptr) : base(ptr)
        {

        }
        public static ContextMenuPanel CreateContextPanel(ContextMenuInfo info)
        {
            var panel = Create<ContextMenuPanel>(new Info("Panel", info.Width, 1000) { Pivot = new Vector2(0, 1) }, VanillaSprites.MainBGPanelBlue);
            panel.contextInfo = info;

            panel.FitContent(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize);

            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 10;
            layout.padding = new RectOffset() { top = 30, bottom = 30, left = 20, right = 20 };


            for (int i = 0; i < info.Entries.Count; i++)
            {
                panel.AddModHelperComponent(info.Entries[i].CreatePanel(info.Width, out var behavior));
                behavior.contextMenu = panel;
                panel.entryBehaviors.Add(behavior);

                if (i == info.Entries.Count - 1)
                    continue;
                var bottomLine = panel.AddImage(new Info("BottomLine", 0, 0, info.Width - 100, 10, new UnityEngine.Vector2(0.5f, 0)), VanillaSprites.WhiteSquareGradient);
                bottomLine.Image.color = new UnityEngine.Color(0, 0, 0, 0.5f);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(panel);

            return panel;
        }
        public void DestroyNestedMenus(ContextMenuEntryMonoBehavior callingBehavior)
        {
            foreach (var behavior in entryBehaviors)
            {
                if (behavior == callingBehavior)
                    continue;
                behavior.RemoveNestedMenu();
            }
        }
    }
}
