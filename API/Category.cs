using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCore.API
{
    public abstract class Category
    { 
        public abstract string Name { get; }

        public abstract Type[] Modules { get; }
    }
}
