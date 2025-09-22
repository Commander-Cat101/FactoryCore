using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.UI.Components;
using Harmony;
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
    internal class ModuleInputHolder : MonoBehaviour
    {
        public static Dictionary<Type, Color> ColorByLinkType = new Dictionary<Type, Color>()
        {
            { typeof(float), Color.red },
            { typeof(bool), Color.blue }
        };

        public static Dictionary<Guid, ModuleInputHolder> GuidToHolder = new Dictionary<Guid, ModuleInputHolder>();

        public RectTransform tempLine;

        public ModuleInput link;

        public ModuleOutputHolder? linkedOutput;
        public GameObject? linkedWire;

        public ModuleInputHolder(IntPtr ptr) : base(ptr)
        {
        }
        public void OnDestroy()
        {
            GuidToHolder.Remove(link.Id);
        }
        public void Update()
        {
            if (!Input.GetMouseButtonDown(1))
                return;

            if (EditorUI.Instance.HoveredGameObject?.gameObject != gameObject)
                return;
            RemoveConnections();
        }
        public void Start()
        {
            if (link.OutputGuid == Guid.Empty) return;
            if (linkedOutput != null) return;
            if (ModuleOutputHolder.GuidToHolder.TryGetValue(link.OutputGuid, out var value))
            {
                value.CreateConnection(this);
            }
        }
        public void LinkOutput(ModuleOutputHolder target, GameObject obj)
        {
            if (linkedWire != null)
            {
                Destroy(linkedWire);
            }
            if (linkedOutput != null)
            {
                linkedOutput.RemoveConnection(this);
            }    
            linkedOutput = target;
            linkedWire = obj;
            link.OutputGuid = target.link.Id;
        }
        public void RemoveConnections()
        {
            if (linkedOutput != null)
                linkedOutput.RemoveConnection(this);
            RemoveConnection();
        }
        public void RemoveConnection()
        {
            linkedWire = null;
            linkedOutput = null;
            link.OutputGuid = Guid.Empty;
        }
    }
}
