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
    public class DisposeExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var strategy = new DisposeBuilderStrategy(Container);
            Context.Strategies.Add(strategy,UnityBuildStage.PreCreation);
        }
    }

    internal class DisposeBuilderStrategy : BuilderStrategy
    {
        private IUnityContainer container;

        public DisposeBuilderStrategy(IUnityContainer container)
        {
            this.container = container;
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            var policy = context.Policies.Get<DisposePolicy>(context.BuildKey);

            if(policy == null)
            { return; } //this isn't an object we want to intercept with our behavior            

            var targetType = context.BuildKey.Type;
            var newType = Emitter.CreateType(targetType);
            var newKey = new NamedTypeBuildKey(newType, context.BuildKey.Name);

            var inst = context.NewBuildUp(newKey);


            context.Existing = inst;
        }
    }
}
