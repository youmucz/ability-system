using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Minikit.AbilitySystem
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MKNativeTagAttribute : Attribute
    {
    }
} // Minikit.AbilitySystem namespace

namespace Minikit.AbilitySystem.Internal
{
    public static class MKTagReflector
    {
        private static List<string> cachedNativelyDefinedTags = new();


        public static List<string> GetNativelyDefinedTags()
        {
            if (cachedNativelyDefinedTags.Count > 0)
            {
                return cachedNativelyDefinedTags;
            }

            List<string> nativelyDefinedTags = new();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsStatic())
                    {
                        foreach (FieldInfo fieldInfo in type.GetFields())
                        {
                            if (fieldInfo.FieldType == typeof(string)
                                && fieldInfo.GetCustomAttribute<MKNativeTagAttribute>() != null)
                            {
                                nativelyDefinedTags.Add((string)fieldInfo.GetValue(null));
                            }
                        }
                    }
                }
            }
            cachedNativelyDefinedTags = nativelyDefinedTags;

            return cachedNativelyDefinedTags;
        }
    }
} // Minikit.AbilitySystem.Internal namespace
