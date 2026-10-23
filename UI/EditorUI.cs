using Debug = UnityEngine.Debug;

namespace FactoryCore.UI
{
    public abstract class EditorUI : ModGameMenu<HotkeysScreen>
    {
        public static EditorUI Instance;

        public abstract Dictionary<Type, Color> NodeColors { get; }
        public abstract ContextMenuInfo ContextMenu { get; }
        public abstract Type CenteredModule { get; }

        public DragComponent? DragObject;
        public bool IsDraggingBackground;
        public Vector3 MouseLastPosition;

        public static float Scaling = 1;
        public static ModHelperPanel MenuContent;
        public static ModHelperPanel ScalePanel;
        public ModHelperPanel ExtrasPanel;

        internal ContextMenuPanel contextPanel;

        Vector2 selectedContextPosition;

        public Canvas Canvas;
        public static Template Template;

        public GameObject? HoveredGameObject;
        public bool ShouldNestedContextMenusGoLeft = false;
        
        
        public float MagicalScalingNumber => Canvas.scaleFactor / 2;

        public abstract void SaveTemplate();

        public override bool OnMenuOpened(Il2CppSystem.Object data)
        {
            Scaling = 1;
            Instance = this;

            CommonForegroundHeader.SetText("Editor");

            GameMenu.transform.DestroyAllChildren();
            GameMenu.saved = true;

            Canvas = GameMenu.GetComponentInParent<Canvas>();
            Canvas.sortingOrder = 5;
            CommonForegroundScreen.instance.GetComponentInParent<Canvas>().sortingOrder = 11;

            ScalePanel = GameMenu.gameObject.AddModHelperPanel(new Info("ScalePanel"));
            MenuContent = ScalePanel.gameObject.AddModHelperPanel(new Info("RootContent", 0, 0, new Vector2(0.5f, 0.5f)));

            CreateExtras();

            var module = Template.GetModulesOfType(CenteredModule).FirstOrDefault();
            if (module != null)
                MenuContent.gameObject.transform.localPosition = -module.Position - new Vector2(500, 0);

            MelonCoroutines.Start(LoadTemplate());

            return false;
        }
        public override void OnMenuClosed()
        {
            CommonForegroundScreen.instance.GetComponentInParent<Canvas>().sortingOrder = 6;
            SaveTemplate();
        }
        public override void OnMenuUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                CloseContextMenu(true);
                IsDraggingBackground = HoveredGameObject == null;
                DragObject = HoveredGameObject?.gameObject.GetComponentInParent<DragComponent>();
                DragObject?.StartDrag();
            }
            if (Input.GetMouseButtonUp(0))
            {
                CloseContextMenu(true);
                IsDraggingBackground = false;
                DragObject?.EndDrag();
                DragObject = null;
            }

            HoveredGameObject = PollForHoveredObjects();

            if (IsDraggingBackground)
            {
                MenuContent.gameObject.transform.position += Input.mousePosition - MouseLastPosition;
            }
            DragObject?.UpdateDrag(Input.mousePosition - MouseLastPosition);
            MouseLastPosition = Input.mousePosition;

            if (Input.GetMouseButtonDown(1) && !IsDraggingBackground)
            {
                if (HoveredGameObject == null)
                    CreateContextMenu(ContextMenu);
                var tab = HoveredGameObject?.gameObject.GetComponentInParent<DraggableTab>();
                
                if (tab != null && tab.transform.parent.GetComponentInChildren<ModuleHolder>().Module.IsDuplicable)
                {
                    ContextMenuInfo contextMenuInfo = ContextMenuBuilder.Create(500)
                        .WithButton("Duplicate", () =>
                        {
                            CreateModule(tab.transform.parent.GetComponentInChildren<ModuleHolder>().Module.GetType(), tab.transform.parent.transform.position + new Vector3(100, -100));
                        });
                    CreateContextMenu(contextMenuInfo);
                }  
            }
            
            if (Input.mouseScrollDelta.y != 0)
            {
                CloseContextMenu();
                Scaling = Mathf.Clamp(Scaling + (Input.mouseScrollDelta.y / 10), 0.5f, 2.5f);
                ScalePanel.transform.localScale = Vector3.one * Scaling;
            }
        }
        public void CreateExtras()
        {
            ExtrasPanel = GameMenu.gameObject.AddModHelperPanel(new Info("ExtrasPanel", InfoPreset.FillParent));

            ExtrasPanel.AddButton(new Info("HelpButton", -100, -100, 150, 150, Vector2.one), VanillaSprites.InfoBtn2, new Action(() =>
            {
                PopupScreen.instance.SafelyQueue(screen =>
                {
                    screen.ShowPopup(PopupScreen.Placement.menuCenter, "How to use", "Right-click to add modules\nConnect nodes to add features to your bloon\nRight-click input nodes to clear connections", null, "Ok", null, null, Popup.TransitionAnim.Scale);
                });
            }));
        }
        public void CloseContextMenu(bool failIfHovering = false)
        {
            if (failIfHovering && HoveredGameObject?.GetComponentInParent<ContextMenuEntryMonoBehavior>() != null)
                return;
            
            contextPanel?.DeleteObject();
            contextPanel = null;
        }
        public void CreateContextMenu(ContextMenuInfo info)
        {
            CloseContextMenu();

            contextPanel = ExtrasPanel.AddModHelperComponent(ContextMenuPanel.CreateContextPanel(info));
            selectedContextPosition = Input.mousePosition;
            
            contextPanel.transform.position = ClampPositionInsideCanvasSpace(selectedContextPosition, new Vector2(contextPanel.RectTransform.sizeDelta.x, ContextMenu.GetEstimatedHeight()));
            
            TaskScheduler.ScheduleTask(() =>
            {
                ShouldNestedContextMenusGoLeft =
                    Canvas.GetComponent<RectTransform>().rect.width / 2 * Canvas.scaleFactor  <
                    selectedContextPosition.x;
                
                MelonLogger.Msg(ShouldNestedContextMenusGoLeft);
                
                contextPanel.transform.position = ClampPositionInsideCanvasSpace(selectedContextPosition, contextPanel.RectTransform.sizeDelta);
            });

        }
        public ModHelperPanel CreateModuleUI(Module module)
        {
            var moduleRoot = MenuContent.AddPanel(new Info("ModuleRoot", 0, 0, 0, 0));
            moduleRoot.transform.localPosition = new Vector3(module.XPosition, module.YPosition, 0);
            moduleRoot.AddComponent<VerticalLayoutGroup>();

            var component = moduleRoot.AddPanel(new Info("Component", 1000, 700, new Vector2(0.5f, 0.5f)), VanillaSprites.MainBGPanelBlue, RectTransform.Axis.Vertical, 0, 0);
            var holder = component.AddComponent<ModuleHolder>().Init(module, moduleRoot, component);
            component.FitContent(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize);

            var dragBar = moduleRoot.AddPanel(new Info("DragBar", 0, 0, 1000, 150, new Vector2(0.5f, 0.5f)), VanillaSprites.MainBGPanelBlue);
            dragBar.AddComponent<DraggableTab>().Init(moduleRoot.transform);
            dragBar.AddText(new Info("Text", 400, 0, 700, 100, new Vector2(0f, 0.5f)), module.Name, 60, Il2CppTMPro.TextAlignmentOptions.MidlineLeft).EnableAutoSizing();
            if (module.IsRemovable)
            {
                dragBar.AddButton(new Info("Delete", -75, 0, 100, 100, new Vector2(1, 0.5f)), VanillaSprites.AddRemoveBtn, new Action(() =>
                {
                    holder.Delete();
                    Template.modules.Remove(module);
                }));
            }
            if (module.Description != string.Empty)
            {
                dragBar.AddButton(new Info("Info", module.IsRemovable ? -200 : -75, 0, 100, 100, new Vector2(1, 0.5f)), VanillaSprites.InfoBtn2, new Action(() =>
                {
                    PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.inGameCenter, "Info", module.Description, null, "Ok", null, null, Popup.TransitionAnim.Scale));
                }));
            }
            dragBar.transform.SetSiblingIndex(0);

            return moduleRoot;
        }
        public void CreateModule(Type moduleType)
        {
            CreateModule(moduleType, selectedContextPosition);
        }
        public void CreateModule(Type moduleType, Vector2 position)
        {
            Module module = (Module)Activator.CreateInstance(moduleType);
            module.Template = Template;
            Template.AddModule(module);
            module.Init();
            CreateModuleUI(module).transform.position = position;
        }
        public void LoadTemplateUI()
        {
            foreach (var module in Template.modules)
            {
                CreateModuleUI(module);
            }
        }
        public IEnumerator LoadTemplate()
        {
            List<int> generateTimes = new List<int>();
            foreach (var module in Template.modules)
            {
                try
                {
                    module.Init();
                }
                catch (Exception e)
                {
                    MelonLogger.Error(e);
                }
            }
            foreach (var module in Template.modules)
            {
                var watch = Stopwatch.StartNew();
                CreateModuleUI(module);
                watch.Stop();
                generateTimes.Add((int)watch.ElapsedMilliseconds);
                yield return null;
            }
            MelonLogger.Msg($"Loaded modules in {generateTimes.Average()}ms per module");
        }

        public static GameObject? PollForHoveredObjects()
        {
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            Il2CppSystem.Collections.Generic.List<RaycastResult> raycastResultsIL2CPP = new Il2CppSystem.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResultsIL2CPP);
            List<RaycastResult> raycastResults = raycastResultsIL2CPP.ToList().Where(a => a.gameObject.GetComponentInParent<CommonBackgroundScreen>() == null).ToList();
            return raycastResults.FirstOrDefault()?.gameObject;
        }
        public static Color GetColorForNode(Type nodeType)
        {
            if (Instance.NodeColors.TryGetValue(nodeType, out var color))
            {
                return color;
            }
            return Color.white;
        }

        public static Vector2 ClampPositionInsideCanvasSpace(Vector2 position, Vector2 sizeDelta)
        {
            var canvas = Instance.Canvas;
            var canvasRect = canvas.GetComponent<RectTransform>().rect;
            
            float y = Mathf.Clamp(position.y, sizeDelta.y * canvas.scaleFactor, canvasRect.height - (sizeDelta.y / 2) * canvas.scaleFactor);
            float x = Math.Min(position.x, (canvasRect.width - sizeDelta.x) * canvas.scaleFactor);
            
            return new Vector2(x, y);
        }
    }
}
