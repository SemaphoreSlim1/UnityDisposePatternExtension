using DisposePatternExtension.Console.Example;
using Microsoft.Practices.Unity;
using System;
using System.Runtime.Serialization;
using Patterns.Dispose;

namespace DisposePatternExtension.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            UsingDispose();
        }

        private static void ExplicitDispose()
        {
            //instantiate the container and the extension
            var container = new UnityContainer();
            container.AddNewExtension<TypeWeaveExtension>();

            container.RegisterType<ICustomObject, CustomObject>(new WeaveInjectionMember<IDisposable, DisposeBase>());

            var obj = container.Resolve<ICustomObject>();

            obj.DoSomething();

            if (obj is IDisposable)
            {
                (obj as IDisposable).Dispose();
            }

            System.Console.WriteLine("Boo Ya");
            System.Console.ReadLine();
        }

        private static void UsingDispose()
        {
            using (var container = new UnityContainer())
            {
                container.AddNewExtension<TypeWeaveExtension>();

                container.RegisterType<ICustomObject, CustomObject>(new ContainerControlledLifetimeManager(),
                    new WeaveInjectionMember<IDisposable, DisposeBase>());

                var obj = container.Resolve<ICustomObject>();

                obj.DoSomething();
            }

            System.Console.WriteLine("Boo Ya");
            System.Console.ReadLine();
        }
    }
}
