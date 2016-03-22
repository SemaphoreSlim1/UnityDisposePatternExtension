using DisposePatternExtension.Console.Example;
using Microsoft.Practices.Unity;
using System;

namespace DisposePatternExtension.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //instantiate the container and the extension
            var container = new UnityContainer();
            container.AddNewExtension<WeaveExtension>();

            container.RegisterType<ICustomObject, CustomObject>(new WeaveInjectionMember<IDisposable, DisposeBase>());

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
