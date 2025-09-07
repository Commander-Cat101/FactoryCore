using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using Il2CppNinjaKiwi.Common;
using MelonLoader;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FactoryCore.UI
{
    [RegisterTypeInIl2Cpp]
    internal class CreateModuleMenu : MonoBehaviour
    {
        public bool isOpen = false;

        public ModHelperPanel? panel;

        public ModHelperPanel? hoverPanel;

        public CategoryTab? HoveredTab;
        public void ToggleTab()
        {
            isOpen = !isOpen;
            GetComponent<RectTransform>().pivot = new Vector2(0.5f, isOpen ? 0 : 1);
            GetComponent<RectTransform>().position = new Vector2(150, 0);
        }
        public void AnimToggle(float time, bool closing)
        {
            if (time < 1)
            {
                time = Mathf.Clamp(Time.deltaTime * 3 + time, 0, 1);
                float pos = Mathf.SmoothStep(-GetComponent<RectTransform>().sizeDelta.y, GetComponent<RectTransform>().sizeDelta.y, time);
                if (closing)
                    pos *= -1;
                GetComponent<RectTransform>().anchoredPosition = new Vector2(300, pos);
                TaskScheduler.ScheduleTask(() => { AnimToggle(time, closing); });
            }
        }

        public void Update()
        {
            CategoryTab? tab = EditorUI.Instance.HoveredGameObject?.GetComponentInParent<CategoryTab>();
            if (tab != null)
            {
                if (tab == HoveredTab)
                    return;

                DeleteCategory();
                HoveredTab = tab;
                CreateCategory(tab);
            }
            else
            {
                if (EditorUI.Instance.HoveredGameObject?.GetComponentInParent<CreateModuleMenu>() == null)
                    DeleteCategory();
            }

            if (Input.GetMouseButtonDown(0))
            {
                var moduleTab = EditorUI.Instance.HoveredGameObject?.GetComponentInParent<ModuleTab>();
                if (moduleTab != null)
                {
                    DeleteCategory();
                    ToggleTab();
                    EditorUI.Instance.CreateModule(moduleTab.moduleType);
                }
            }
        }
        public void DeleteCategory()
        {
            hoverPanel?.transform.DestroyAllChildren();
            Destroy(hoverPanel?.gameObject);
            hoverPanel = null;
            HoveredTab = null;
        }
        public void CreateCategory(CategoryTab tab)
        {
            hoverPanel = panel?.AddPanel(new Info("ComponentSelect", 250, 0, 500, 100, new Vector2(1, 0.5f)), VanillaSprites.MainBGPanelBlue);
            hoverPanel.AddLayoutElement().ignoreLayout = true;

            var layout = hoverPanel.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.padding = new RectOffset() { top = 50, bottom = 50 };
            layout.spacing = 50;
            hoverPanel.FitContent(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize);

            foreach (var moduleType in tab.category.Modules)
            {
                CreateModule(hoverPanel, moduleType);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
            var rect = hoverPanel.GetComponent<RectTransform>();
            float y = Mathf.Clamp(tab.transform.position.y, rect.sizeDelta.y / 4.70f, EditorUI.Instance.Canvas.GetComponent<RectTransform>().rect.height - rect.sizeDelta.y / 4.70f);
            hoverPanel.transform.position = hoverPanel.transform.position.WithY(y);
        }
        public void CreateModule(ModHelperPanel panel, Type moduleType)
        {
            var modulePanel = panel.AddPanel(new Info("Module", 0, 0, 450, 70));

            var module = (Module)Activator.CreateInstance(moduleType);
            var text = modulePanel.AddText(new Info("Text", 450, 70), module.Name, 50);
            text.Text.overflowMode = Il2CppTMPro.TextOverflowModes.Overflow;
            modulePanel.AddComponent<ModuleTab>().moduleType = moduleType;
        }
    }
}
