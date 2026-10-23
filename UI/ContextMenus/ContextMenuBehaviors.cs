namespace FactoryCore.UI.ContextMenus;

[RegisterTypeInIl2Cpp(false)]
    internal class ContextMenuEntryMonoBehavior : MonoBehaviour
    {
        public bool isHovered = false;
        public ContextMenuEntryMonoBehavior(IntPtr ptr) : base(ptr)
        {
        }

        public ModHelperPanel panel;

        public ModHelperText text;

        public ContextMenuEntry entry;

        public ContextMenuPanel contextMenu;

        public ContextMenuPanel nestedMenu;
        public void UnHover()
        {
            isHovered = false;
            panel.Background.color = Color.clear;
        }
        public void Hover()
        {
            isHovered = true;
            panel.Background.color = new Color(0, 0, 0, 0.25f);

            if (entry is ContextMenuNestedMenu nestedEntry)
            {
                if (nestedMenu != null)
                    return;
                
                contextMenu.DestroyNestedMenus(this);
                nestedMenu = panel.AddModHelperComponent(ContextMenuPanel.CreateContextPanel(nestedEntry.NestedMenu));
                
                var panelPosition = panel.transform.position + (EditorUI.Instance.ShouldNestedContextMenusGoLeft ? -new Vector3((nestedMenu.RectTransform.sizeDelta.x + contextMenu.RectTransform.sizeDelta.x / 2) * EditorUI.Instance.Canvas.scaleFactor, -30) : new Vector3(contextMenu.contextInfo.Width * EditorUI.Instance.MagicalScalingNumber, 30));
                
                nestedMenu.transform.position = EditorUI.ClampPositionInsideCanvasSpace(panelPosition, new Vector2(nestedMenu.RectTransform.sizeDelta.x, nestedEntry.NestedMenu.GetEstimatedHeight()));
                TaskScheduler.ScheduleTask(() =>
                {
                    nestedMenu.transform.position = EditorUI.ClampPositionInsideCanvasSpace(panelPosition, nestedMenu.RectTransform.sizeDelta);
                });

            }
        }
        public void RemoveNestedMenu()
        {
            if (nestedMenu != null)
            {
                nestedMenu.DeleteObject();
                nestedMenu = null;
            }
        }
        public void Update()
        {
            var hover = EditorUI.Instance.HoveredGameObject;
            if (hover?.GetComponent<ContextMenuEntryMonoBehavior>() == this || hover?.GetComponent<ModHelperText>() == text)
            {
                if (!isHovered)
                    Hover();
            }
            else
            {
                if (isHovered)
                    UnHover();
            }

            if (Input.GetMouseButtonDown(0) && isHovered)
            {
                if (entry is ContextMenuButton buttonEntry)
                {
                    buttonEntry.onClick.Invoke();
                    EditorUI.Instance.CloseContextMenu();
                }
            }
        }
    }