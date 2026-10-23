namespace FactoryCore.UI.ContextMenus;

public struct ContextMenuInfo
    {
        internal int Width;

        internal List<ContextMenuEntry> Entries;

        internal float GetEstimatedHeight()
        {
            float height = 80 * Entries.Count + 60;
            height += (Entries.Count - 1) * 30;
            return height;
        }
    }
    internal abstract class ContextMenuEntry
    {
        public string DisplayText;

        internal abstract ModHelperPanel CreatePanel(int panelWidth, out ContextMenuEntryMonoBehavior behavior);
    }
    internal class ContextMenuNestedMenu : ContextMenuEntry
    {
        internal ContextMenuInfo NestedMenu;

        internal override ModHelperPanel CreatePanel(int panelWidth, out ContextMenuEntryMonoBehavior behavior)
        {
            var panel = ModHelperPanel.Create(new Info("NestedMenu", panelWidth - 50, 80), VanillaSprites.WhiteSquareGradient);
            panel.Background.color = Color.clear;

            behavior = panel.AddComponent<ContextMenuEntryMonoBehavior>();
            behavior.panel = panel;
            behavior.entry = this;

            var text = panel.AddText(new Info("Name", -25, 0, panelWidth - 100, 70), DisplayText, 60, Il2CppTMPro.TextAlignmentOptions.Center);
            text.Text.overflowMode = Il2CppTMPro.TextOverflowModes.Overflow;
            behavior.text = text;

            panel.AddImage(new Info("Arrow", -50, 0, 50, 40, new Vector2(1, 0.5f)), VanillaSprites.MonkeyKnowledgeArrow).transform.rotation = Quaternion.Euler(0, 0, 90);

            return panel;
        }

        public ContextMenuNestedMenu(string name)
        {
            DisplayText = name;
        }
    }
    internal class ContextMenuButton : ContextMenuEntry
    {
        public Action onClick;
        public ContextMenuButton(string name, Action onClick)
        {
            DisplayText = name;
            this.onClick = onClick;
        }
        internal override ModHelperPanel CreatePanel(int panelWidth, out ContextMenuEntryMonoBehavior behavior)
        {
            var panel = ModHelperPanel.Create(new Info("NestedMenu", panelWidth - 50, 80) , VanillaSprites.WhiteSquareGradient);
            panel.Background.color = Color.clear;

            behavior = panel.AddComponent<ContextMenuEntryMonoBehavior>();
            behavior.panel = panel;
            behavior.entry = this;


            LayoutElement layout = panel.AddLayoutElement();
            layout.minWidth = 450;
            layout.minHeight = 60;
            var text = panel.AddText(new Info("Name", panelWidth - 50, 70), DisplayText, 60, Il2CppTMPro.TextAlignmentOptions.Center);
            text.Text.overflowMode = Il2CppTMPro.TextOverflowModes.Overflow;
            behavior.text = text;

            panel.RectTransform.sizeDelta = new Vector2(text.Text.preferredWidth, 80);

            LayoutRebuilder.ForceRebuildLayoutImmediate(panel);

            return panel;
        }
    }

    public static class ContextMenuBuilder
    {
        public static ContextMenuInfo Create(int width)
        {
            return new ContextMenuInfo { Width = width, Entries = new List<ContextMenuEntry>() };
        }
        public static ContextMenuInfo WithButton(this ContextMenuInfo info, string name, Action onClick)
        {
            info.Entries.Add(new ContextMenuButton(name, onClick));
            return info;
        }
        public static ContextMenuInfo WithButton(this ContextMenuInfo info, Type moduleType)
        {
            var module = (Module)Activator.CreateInstance(moduleType);
            info.Entries.Add(new ContextMenuButton(module.Name, () => { EditorUI.Instance.CreateModule(moduleType); }));
            return info;
        }
        public static ContextMenuInfo WithNested(this ContextMenuInfo info, string name, int nestedMenuWidth, Action<ContextMenuInfo> nestedMenu)
        {
            ContextMenuInfo nestedInfo = Create(nestedMenuWidth);
            nestedMenu(nestedInfo);

            info.Entries.Add(new ContextMenuNestedMenu(name)
            {
                NestedMenu = nestedInfo
            });
            return info;
        }
    }