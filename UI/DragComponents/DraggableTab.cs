using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryCore.UI.Components
{
    [RegisterTypeInIl2Cpp(false)]
    public class DraggableTab : DragComponent
    {
        public DraggableTab(IntPtr ptr) : base(ptr) { }

        private Transform RootObject;

        public void Init(Transform rootObject)
        {
            RootObject = rootObject;
        }
        public override void UpdateDrag(Vector3 dragDifference)
        {
            if (RootObject == null)
                return;
            RootObject.position += dragDifference;
        }
    }
}
