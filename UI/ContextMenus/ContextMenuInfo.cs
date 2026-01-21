using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using FactoryCore.API;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FactoryCore.UI.ContextMenus
{
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

                nestedMenu.transform.position = EditorUI.ClampPositionInsideCanvasSpace(panel.transform.position + new Vector3(contextMenu.contextInfo.Width * EditorUI.Instance.MagicalScalingNumber, 30), new Vector2(nestedMenu.RectTransform.sizeDelta.x, nestedEntry.NestedMenu.GetEstimatedHeight()));
                TaskScheduler.ScheduleTask(() =>
                {
                    nestedMenu.transform.position = EditorUI.ClampPositionInsideCanvasSpace(panel.transform.position + new Vector3(contextMenu.contextInfo.Width * EditorUI.Instance.MagicalScalingNumber, 30), nestedMenu.RectTransform.sizeDelta);
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
            if (nestedMenu != null)
            {
                
            }

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

    public static class ContextMenuBuilder
    {
        public static ContextMenuInfo Create(int width)
        {
            return new ContextMenuInfo() { Width = width, Entries = new List<ContextMenuEntry>() };
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
}
