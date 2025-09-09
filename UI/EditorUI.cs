using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.UI.Components;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppAssets.Scripts.Unity.UI_New.Settings;
using Il2CppNinjaKiwi.Common;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FactoryCore.UI
{
    public abstract class EditorUI : ModGameMenu<HotkeysScreen>
    {
        public static EditorUI Instance;
        public abstract List<Category> Categories { get; }

        public DragComponent? DragObject;
        public bool IsDraggingBackground;
        public Vector3 MouseLastPosition;

        public float Scaling = 1;
        public static ModHelperPanel MenuContent;
        public ModHelperPanel ExtrasPanel;

        public Canvas Canvas;
        public static Template Template;

        public GameObject? HoveredGameObject;
        public override bool OnMenuOpened(Il2CppSystem.Object data)
        {
            Instance = this;

            CommonForegroundScreen.instance.GetComponentInParent<Canvas>().sortingOrder = 11;
            CommonForegroundHeader.SetText("Editor");
            GameMenu.transform.DestroyAllChildren();
            Canvas = GameMenu.GetComponentInParent<Canvas>();
            Canvas.sortingOrder = 9;
            MenuContent = GameMenu.gameObject.AddModHelperPanel(new Info("RootContent", 0, 0, new Vector2(0.5f, 0.5f)));

            CreateExtras();
            LoadTemplateUI();
            return false;
        }
        public override void OnMenuClosed()
        {
            CommonForegroundScreen.instance.GetComponentInParent<Canvas>().sortingOrder = 3;
            SaveTemplate();
        }
        public override void OnMenuUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                IsDraggingBackground = HoveredGameObject == null;
                DragObject = HoveredGameObject?.gameObject.GetComponentInParent<DragComponent>();
                DragObject?.StartDrag();
            }
            if (Input.GetMouseButtonUp(0))
            {
                IsDraggingBackground = false;
                DragObject?.EndDrag();
                DragObject = null;
            }

            if (IsDraggingBackground)
            {
                MenuContent.gameObject.transform.position += Input.mousePosition - MouseLastPosition;
            }
            DragObject?.UpdateDrag(Input.mousePosition - MouseLastPosition);
            MouseLastPosition = Input.mousePosition;

            var pointer = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            Il2CppSystem.Collections.Generic.List<RaycastResult> raycastResultsIL2CPP = new Il2CppSystem.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResultsIL2CPP);
            List<RaycastResult> raycastResults = raycastResultsIL2CPP.ToList().Where(a => a.gameObject.GetComponentInParent<CommonBackgroundScreen>() == null).ToList();
            HoveredGameObject = raycastResults.FirstOrDefault()?.gameObject;
        }
        public void CreateExtras()
        {
            ExtrasPanel = GameMenu.gameObject.AddModHelperPanel(new Info("ExtrasPanel", InfoPreset.FillParent));

            var panel = ExtrasPanel.AddPanel(new Info("Categories", 250, -500, 450, 1000, Vector2.zero), VanillaSprites.MainBGPanelBlue);
            var tab = panel.AddComponent<CreateModuleMenu>();
            tab.panel = panel;

            var addbutton = panel.AddButton(new Info("AddComponentButton", 100, 150, 200, 200, new Vector2(0, 1)), VanillaSprites.AddMoreBtn, new Action(() =>
            {
                tab.ToggleTab();
            }));
            addbutton.AddLayoutElement().ignoreLayout = true;

            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.padding = new RectOffset() { top = 50, bottom = 50 };
            layout.spacing = 50;
            panel.FitContent(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize);

            foreach (var category in Categories)
            {
                CreateCategoryTab(category).transform.SetParent(panel, false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
            tab.ToggleTab();
        }
        public ModHelperPanel CreateCategoryTab(Category category)
        {
            var panel = ModHelperPanel.Create(new Info(category.Name, 550, 60));
            panel.AddComponent<CategoryTab>().category = category;
            panel.AddText(new Info("Text", 550, 60), category.Name + "  >", 40).EnableAutoSizing();
            return panel;
        }
        public void CreateModuleUI(Module module)
        {
            module.Init();

            var moduleRoot = MenuContent.AddPanel(new Info("ModuleRoot", 0, 0, 0, 0));
            moduleRoot.transform.localPosition = new Vector3(module.XPosition, module.YPosition, 0);
            moduleRoot.AddComponent<VerticalLayoutGroup>();

            var component = moduleRoot.AddPanel(new Info("Component", 1000, 700, new Vector2(0.5f, 0.5f)), VanillaSprites.MainBGPanelBlue, RectTransform.Axis.Vertical, 0, 0);
            var holder = component.AddComponent<ModuleHolder>().Init(module, moduleRoot, component);
            component.FitContent(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize);

            var dragBar = moduleRoot.AddPanel(new Info("DragBar", 0, 0, 1000, 150, new Vector2(0.5f, 0.5f)), VanillaSprites.MainBGPanelBlue);
            dragBar.AddComponent<DraggableTab>().Init(moduleRoot.transform);
            dragBar.AddText(new Info("Text", 0, 0, 900, 100, new Vector2(0.5f, 0.5f)), module.Name, 60, Il2CppTMPro.TextAlignmentOptions.MidlineLeft).EnableAutoSizing();
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
        }
        public void CreateModule(Type moduleType)
        {
            Module module = (Module)Activator.CreateInstance(moduleType);
            module.Template = Template;
            Template.AddModule(module);
            module.Position = Canvas.transform.position;
            CreateModuleUI(module);
        }
        public void LoadTemplateUI()
        {
            foreach (var module in Template.modules)
            {
                CreateModuleUI(module);
            }
        }
        public abstract void SaveTemplate();
    }
}
