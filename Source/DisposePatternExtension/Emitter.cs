using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DisposePatternExtension
{
    public static class Emitter
    {
        public static Type Weave(Type baseType, Type interfaceType, Type weaveProviderType)
        {            
            var interfaceMethodInfos = interfaceType.GetMethods();

            // create a dynamic assembly and module 
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "tmpAssembly";
            AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule("tmpModule");

            // create a new type builder            
            var typeBuilder = module.DefineType(weaveProviderType.Name + "_" + baseType.Name, TypeAttributes.Public | TypeAttributes.Class,baseType);
            CreatePassThroughConstructors(typeBuilder, baseType);

            typeBuilder.AddInterfaceImplementation(interfaceType);

            var methodBuilders = new Dictionary<string, MethodBuilder>();

            foreach(var interfaceMethodInfo in interfaceMethodInfos)
            {
                //define the method and its parameters based on the interface definition
                var methodName = interfaceMethodInfo.Name;
                var returnType = interfaceMethodInfo.ReturnType;
                var parameterTypes = interfaceMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray();

                var methodBuilder = typeBuilder.DefineMethod(methodName,
                                                            MethodAttributes.Public | MethodAttributes.Virtual,
                                                            returnType,parameterTypes);

                //set the method signature
                methodBuilder.SetSignature(returnType, null, null, parameterTypes, null, null);

                //provide a pass-through implementation

                //get a reference to the "base" method
                var weavedMethodInfo = weaveProviderType.GetMethod(methodName);

                //emit the IL
                var emitter = methodBuilder.GetILGenerator();
                emitter.Emit(OpCodes.Nop);
                emitter.Emit(OpCodes.Ldarg_0);
                emitter.Emit(OpCodes.Call, weavedMethodInfo);
                emitter.Emit(OpCodes.Nop);
                emitter.Emit(OpCodes.Ret);
                

                //then signal to the type that our new method implements the interface method
                typeBuilder.DefineMethodOverride(methodBuilder, interfaceMethodInfo);

                //save the method builder, as we'll need it later
                methodBuilders[methodName] = methodBuilder;
            }

            //now complete the type
            var weavedType = typeBuilder.CreateType();          


            //finally return our new, dynamically created, weaved type
            return weavedType;
        }

        /// <summary>Creates one constructor for each public constructor in the base class. Each constructor simply
        /// forwards its arguments to the base constructor, and matches the base constructor's signature.
        /// Supports optional values, and custom attributes on constructors and parameters.
        /// Does not support n-ary (variadic) constructors</summary>
        private static void CreatePassThroughConstructors(this TypeBuilder builder, Type baseType)
        {
            foreach (var constructor in baseType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
                {
                    //throw new InvalidOperationException("Variadic constructors are not supported");
                    continue;
                }

                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
                var requiredCustomModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
                var optionalCustomModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

                var ctor = builder.DefineConstructor(MethodAttributes.Public, constructor.CallingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var parameter = parameters[i];
                    var parameterBuilder = ctor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
                    if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0)
                    {
                        parameterBuilder.SetConstant(parameter.RawDefaultValue);
                    }

                    foreach (var attribute in BuildCustomAttributes(parameter.GetCustomAttributesData()))
                    {
                        parameterBuilder.SetCustomAttribute(attribute);
                    }
                }

                foreach (var attribute in BuildCustomAttributes(constructor.GetCustomAttributesData()))
                {
                    ctor.SetCustomAttribute(attribute);
                }

                var emitter = ctor.GetILGenerator();             
                emitter.Emit(OpCodes.Nop);

                // Load `this` and call base constructor with arguments
                emitter.Emit(OpCodes.Ldarg_0);
                for (var i = 1; i <= parameters.Length; ++i)
                {
                    emitter.Emit(OpCodes.Ldarg, i);
                }
                emitter.Emit(OpCodes.Call, constructor);

                emitter.Emit(OpCodes.Ret);                
            }
        }


        private static CustomAttributeBuilder[] BuildCustomAttributes(IEnumerable<CustomAttributeData> customAttributes)
        {
            return customAttributes.Select(attribute => {
                var attributeArgs = attribute.ConstructorArguments.Select(a => a.Value).ToArray();
                var namedPropertyInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<PropertyInfo>().ToArray();
                var namedPropertyValues = attribute.NamedArguments.Where(a => a.MemberInfo is PropertyInfo).Select(a => a.TypedValue.Value).ToArray();
                var namedFieldInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<FieldInfo>().ToArray();
                var namedFieldValues = attribute.NamedArguments.Where(a => a.MemberInfo is FieldInfo).Select(a => a.TypedValue.Value).ToArray();
                return new CustomAttributeBuilder(attribute.Constructor, attributeArgs, namedPropertyInfos, namedPropertyValues, namedFieldInfos, namedFieldValues);
            }).ToArray();
        }
    }
}
