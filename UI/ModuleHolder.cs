using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryCore.UI
{
    [RegisterTypeInIl2Cpp]
    public class ModuleHolder : MonoBehaviour
    {
        public Module Module;
        public GameObject Root;
        public ModHelperPanel Panel;

        public ModuleHolder Init(Module module, GameObject root, ModHelperPanel panel)
        {
            Module = module;
            Root = root;
            Panel = panel;
            module.SetupVisual(panel, this);
            return this;
        }
        public void Update()
        {
            Module.Position = Root.transform.position;
        }
        public void Delete()
        {
            foreach (var input in GetComponentsInChildren<ModuleInputHolder>())
            {
                input.RemoveConnections();
            }
            foreach (var output in GetComponentsInChildren<ModuleOutputHolder>())
            {
                output.RemoveConnections();
            }
            Root.DestroyAllChildren();
            Destroy(Root);
        }
    }
}
