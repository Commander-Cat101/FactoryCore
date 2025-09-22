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
    [RegisterTypeInIl2Cpp(false)]
    internal class ModuleLinkHolder : DragComponent
    {
        public bool isInput;
        public ModuleLinkHolder(IntPtr ptr) : base(ptr) { }

        public RectTransform tempLine;

        public ModuleLink link;

        public Dictionary<ModuleLinkHolder, GameObject> linkConnections = new Dictionary<ModuleLinkHolder, GameObject>();

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
            image.color =  ValueColors.ColorByLinkType.ContainsKey(link.Type) ? ValueColors.ColorByLinkType[link.Type] : Color.white;
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

            var target = EditorUI.Instance.HoveredGameObject?.gameObject.GetComponent<ModuleLinkHolder>();

            AttemptConnection(target);
            
            Destroy(tempLine.gameObject);
            tempLine = null;
        }
        
        public void AttemptConnection(ModuleLinkHolder target)
        {
            if (target == null) return;
            if (!target.isInput) return;
            if (target.link.Type != link.Type) return;
            if (linkConnections.ContainsKey(target)) return;

            var obj = LineBetweenObjects.Create(ValueColors.ColorByLinkType.ContainsKey(link.Type) ? ValueColors.ColorByLinkType[link.Type] : Color.white, transform, target.transform, transform);
            linkConnections.Add(target, obj);
            target.linkConnections.Add(this, obj);
        }
    }
}
