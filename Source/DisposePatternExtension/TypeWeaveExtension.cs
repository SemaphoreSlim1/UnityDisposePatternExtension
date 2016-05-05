using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DisposePatternExtension
{
    public class TypeWeaveExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var strategy = new WeaveBuilderStrategy(Container);
            Context.Strategies.Add(strategy,UnityBuildStage.PreCreation);
        }
    }

    internal class WeaveBuilderStrategy : BuilderStrategy
    {
        private IUnityContainer container;

        public WeaveBuilderStrategy(IUnityContainer container)
        {
            this.container = container;
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            var policy = context.Policies.Get<TypeWeavePolicy>(context.BuildKey);

            if(policy == null)
            { return; } //this isn't an object we want to intercept with our behavior            

            var targetType = context.BuildKey.Type;
            var weaveInterfaceType = policy.WeaveInterfaceType;
            var weaveProviderType = policy.WeaveProviderType;
            
            var newType = Emitter.Weave(targetType, weaveInterfaceType, weaveProviderType);

            var newKey = new NamedTypeBuildKey(newType, context.BuildKey.Name);

            var inst = context.NewBuildUp(newKey);


            context.Existing = inst;
        }
    }
}
