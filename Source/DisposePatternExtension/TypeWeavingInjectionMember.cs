using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;

namespace DisposePatternExtension
{
    public class WeaveInjectionMember<TInterface, TImplementation> : InjectionMember
        where TImplementation : TInterface
    {
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            var policy = new TypeWeavePolicy();
            policy.WeaveInterfaceType = typeof(TInterface);
            policy.WeaveProviderType = typeof(TImplementation);

            var key = new NamedTypeBuildKey(implementationType, name);
            policies.Set(policy, key);
        }
    }
}
