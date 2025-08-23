using FactoryCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCore
{
    internal class BloonCategory : Category
    {
        public override string Name => "Bloons";

        public override Type[] Modules => [ typeof(BloonModule), typeof(BloonModule), typeof(BloonModule), typeof(BloonModule) ];
    }
}
