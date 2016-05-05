using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisposePatternExtension.Console.Example
{
    public class CustomObject : ICustomObject
    {
        public string ExampleProperty { get; set; }

        protected string protectedMember;

        public CustomObject()
        {
            ExampleProperty = "Hello World";
            protectedMember = "I have protected status";
        }
        
        public void DoSomething()
        {
            System.Console.WriteLine("Hello World from Do Something!");
        }

    }
}
