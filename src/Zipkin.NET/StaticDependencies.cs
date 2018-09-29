using System;
using System.Collections.Generic;

namespace Zipkin.NET
{
    public static class StaticDependencies
    {
        private static readonly Dictionary<Type, object> Dependencies;

        static StaticDependencies()
        {
            Dependencies = new Dictionary<Type, object>();
        }

        public static void TryRegister<T>(object dependency) where T : class
        {
            if (!Dependencies.ContainsKey(typeof(T)))
            {
                Dependencies.Add(typeof(T), dependency);
            }
        }

        public static T Get<T>() where T : class
        {
            if (Dependencies.ContainsKey(typeof(T)))
            {
                return Dependencies[typeof(T)] as T;
            }

            return null;
        }
    }
}
