using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.UI.Components;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppSystem.Security.Cryptography;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FactoryCore.UI
{
    [RegisterTypeInIl2Cpp]
    internal class ModuleOutputHolder : DragComponent
    {
        public static Dictionary<Type, Color> ColorByLinkType = new Dictionary<Type, Color>()
        {
            { typeof(float), Color.red },
            { typeof(bool), Color.blue }
        };
        public bool isInput;

        public RectTransform tempLine;

        public ModuleOutput link;

        public Dictionary<ModuleInputHolder, GameObject> linkConnections = new Dictionary<ModuleInputHolder, GameObject>();

        public static Dictionary<Guid, ModuleOutputHolder> GuidToHolder = new Dictionary<Guid, ModuleOutputHolder>();
        public ModuleOutputHolder(IntPtr ptr) : base(ptr)
        {

        }
        public void OnDestroy()
        {
            GuidToHolder.Remove(link.Id);
        }
        public void Start()
        {
            foreach (var guid in link.InputsGuids)
            {
                if (ModuleInputHolder.GuidToHolder.TryGetValue(guid, out var value) && value.linkedOutput == null)
                {
                    CreateConnection(value);
                }
            }
        }
        public override void StartDrag()
        {
            if (isInput)
                return;

            var obj = new GameObject("TempLine");
            obj.transform.SetParent(transform);
            var canvas = obj.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 5;
            var image = obj.gameObject.AddComponent<Image>();
            tempLine = obj.GetComponent<RectTransform>();
            obj.transform.localScale = Vector3.one;
            image.color = ColorByLinkType.ContainsKey(link.Type) ? ColorByLinkType[link.Type] : Color.white;
        }
        public void Update()
        {
            if (isInput)
                return;

            if (tempLine != null)
            {
                tempLine.transform.position = (transform.position + Input.mousePosition) / 2;
                Vector3 dif = Input.mousePosition - tempLine.position;
                tempLine.sizeDelta = new Vector3(dif.magnitude * 4.75f, 25);
                tempLine.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
            }
        }
        public override void EndDrag()
        {
            if (isInput)
                return;

            var target = EditorUI.Instance.HoveredGameObject?.gameObject.GetComponent<ModuleInputHolder>();

            AttemptConnection(target);

            Destroy(tempLine.gameObject);
            tempLine = null;
        }

        public void AttemptConnection(ModuleInputHolder target)
        {
            if (target == null) return;
            if (target.link.Type != link.Type) return;
            if (linkConnections.ContainsKey(target)) return;

            CreateConnection(target);
        }
        public void CreateConnection(ModuleInputHolder target)
        {
            var obj = LineBetweenObjects.Create(ColorByLinkType.ContainsKey(link.Type) ? ColorByLinkType[link.Type] : Color.white, transform, target.transform, transform);
            linkConnections.Add(target, obj);
            target.LinkOutput(this, obj);
            if (!link.InputsGuids.Contains(target.link.Id))
                link.InputsGuids.Add(target.link.Id);
        }
        public void RemoveConnections()
        {
            link.InputsGuids.Clear();
            foreach (var link in linkConnections)
            {
                link.Key.RemoveConnection();
            }
        }
        public void RemoveConnection(ModuleInputHolder target)
        {
            if (linkConnections.TryGetValue(target, out var value))
            {
                MelonLogger.Msg("Delete obj");
                Destroy(value);
            }
            linkConnections.Remove(target);
            link.InputsGuids.RemoveAll(a => a == target.link.Id);
        }
    }
}
