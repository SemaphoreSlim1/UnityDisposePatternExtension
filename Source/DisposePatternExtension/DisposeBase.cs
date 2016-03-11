using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisposePatternExtension
{
    public class DisposeBase : IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("Type injected disposable!");
        }
    }
}
