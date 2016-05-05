using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Patterns.Dispose
{
    public class DisposeBase : IDisposable
    {
        public virtual void Dispose()
        {
            Console.WriteLine("Hello IDisposable! By the way, this IDisposable is a " + this.GetType().BaseType);

            var fieldInfos =
                this.GetType()
                    .BaseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                        BindingFlags.FlattenHierarchy);

            var propertyInfos = this.GetType().BaseType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                        BindingFlags.FlattenHierarchy);


            foreach (var fieldInfo in fieldInfos)
            {
                DisposeField(fieldInfo);
            }

            foreach (var propertyInfo in propertyInfos)
            {
                DisposeProperty(propertyInfo);
            }
        }

        private void DisposeField(FieldInfo fieldInfo)
        {
            var resolvedValue = fieldInfo.GetValue(this);

            if (resolvedValue == null)
            { return; } //Good to go!

            DisposeValue(resolvedValue);

            fieldInfo.SetValue(this, GetDefaultValue(fieldInfo.FieldType));
        }

        private void DisposeProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo.CanRead == false)
            { return; } //write only field - let's not touch this

            var resolvedValue = propertyInfo.GetValue(this);

            if (resolvedValue == null)
            { return; } //we're done!

            DisposeValue(resolvedValue);

            if (propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(this, GetDefaultValue(propertyInfo.PropertyType));
            }
        }

        private void DisposeValue(object value)
        {
            if (value is IDisposable)
            {
                ((IDisposable) value).Dispose();
                return;
            }

            if (value is IEnumerable)
            {
                foreach (var item in (value as IEnumerable))
                {
                    DisposeValue(item);
                }
            }

            (value as ICollection<object>)?.Clear();
        }

        private object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }
    }
}
