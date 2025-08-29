using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryCore.UI
{
    public static class ValueColors
    {
        public static Dictionary<Type, Color> ColorByLinkType = new Dictionary<Type, Color>()
        {
            { typeof(float), Color.red },
            { typeof(bool), Color.blue }
        };
    }
}
