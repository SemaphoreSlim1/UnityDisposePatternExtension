using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisposePatternExtension.Console.Example
{
    public class CustomObject : ICustomObject
    {
        public void DoSomething()
        {
            System.Console.WriteLine("Hello World from Do Something!");
        }
    }
}
