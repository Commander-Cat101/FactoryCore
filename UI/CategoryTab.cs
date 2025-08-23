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
    internal class CategoryTab : MonoBehaviour
    {
        public CategoryTab(IntPtr ptr) : base(ptr) { }

        public Category category;
    }
}
