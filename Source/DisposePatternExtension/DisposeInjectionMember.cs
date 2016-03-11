using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;

namespace DisposePatternExtension
{
    public class DisposeInjectionMember : InjectionMember
    {
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            var policy = new DisposePolicy();
            var key = new NamedTypeBuildKey(implementationType, name);
            policies.Set(policy, key);
        }
    }
}
