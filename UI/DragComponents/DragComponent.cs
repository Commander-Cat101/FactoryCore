using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryCore.UI.Components
{
    [RegisterTypeInIl2Cpp]
    public class DragComponent : MonoBehaviour
    {
        public DragComponent(IntPtr ptr) : base(ptr) { }

        public virtual void StartDrag()
        {

        }
        public virtual void EndDrag()
        {

        }
        public virtual void UpdateDrag(Vector3 dragDifference)
        {

        }
    }
}
