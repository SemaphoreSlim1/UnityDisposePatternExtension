using Microsoft.Practices.ObjectBuilder2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisposePatternExtension
{
    public class WeavePolicy : IBuilderPolicy
    {
        public Type WeaveInterfaceType { get; set; }
        public Type WeaveProviderType { get; set; }
    }

    
}
