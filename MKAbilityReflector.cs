using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Minikit.AbilitySystem.Internal
{
    public static class MKAbilityReflector
    {
        private static Dictionary<MKTag, Type> registeredAbilities = new();
        private static Dictionary<MKTag, Type> registeredEffects = new();


        static MKAbilityReflector()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(MKAbility))
                        && !type.IsAbstract) // Ignore abstract ability classes since we don't want to register them
                    {
                        FieldInfo tagFieldInfo = type.GetField(MKAbility.__typeTagFieldName);
                        MKTag abilityTypeTag = (MKTag)tagFieldInfo.GetValue(null);
                        if (abilityTypeTag != null)
                        {
                            registeredAbilities.Add(abilityTypeTag, type);

                            continue;
                        }
                        else
                        {
                            Debug.LogError($"Failed to register {typeof(MKAbility).Name} because field {MKAbility.__typeTagFieldName} wasn't overridden");
                            continue;
                        }
                    }

                    if (type.IsSubclassOf(typeof(MKEffect))
                        && !type.IsAbstract)
                    {
                        FieldInfo tagFieldInfo = type.GetField(MKEffect.__typeTagFieldName);
                        MKTag abilityTypeTag = (MKTag)tagFieldInfo.GetValue(null);
                        if (abilityTypeTag != null)
                        {
                            registeredEffects.Add(abilityTypeTag, type);

                            continue;
                        }
                        else
                        {
                            Debug.LogError($"Failed to register {typeof(MKEffect).Name} because field {MKEffect.__typeTagFieldName} wasn't overridden");
                            continue;
                        }
                    }
                }
            }
        }


        public static Type GetRegisteredAbilityType(MKTag _tag)
        {
            if (registeredAbilities.ContainsKey(_tag))
            {
                return registeredAbilities[_tag];
            }

            Debug.LogError($"Failed to get registered {typeof(MKAbility).Name} type from tag {_tag.key}");
            return null;
        }

        public static Type GetRegisteredEffectType(MKTag _tag)
        {
            if (registeredEffects.ContainsKey(_tag))
            {
                return registeredEffects[_tag];
            }

            Debug.LogError($"Failed to get registered {typeof(MKEffect).Name} type from tag {_tag.key}");
            return null;
        }
    }
} // Minikit.AbilitySystem namespace
