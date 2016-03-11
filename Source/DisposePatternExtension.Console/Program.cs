using DisposePatternExtension.Console.Example;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisposePatternExtension.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //instantiate the container and the extension
            var container = new UnityContainer();
            container.AddNewExtension<DisposeExtension>();

            container.RegisterType<ICustomObject, CustomObject>(new DisposeInjectionMember());

            var obj = container.Resolve<ICustomObject>();

            obj.DoSomething();

            if(obj is IDisposable)
            {
                (obj as IDisposable).Dispose();
            }

            System.Console.WriteLine("Boo Ya");
        }
    }
}
