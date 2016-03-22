using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisposePatternExtension
{
    public class ExampleObject : DisposeBase
    {
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
