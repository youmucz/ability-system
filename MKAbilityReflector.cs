using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace Minikit.AbilitySystem.Internal
{
    public static class MKAbilityReflector
    {
        private static readonly Dictionary<Tag, Type> RegisteredAbilities = new();
        private static readonly Dictionary<Tag, Type> RegisteredEffects = new();
        
        static MKAbilityReflector()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(MKAbility))
                        && !type.IsAbstract) // Ignore abstract ability classes since we don't want to register them
                    {
                        var tagFieldInfo = type.GetField(MKAbility.__typeTagFieldName);
                        var abilityTypeTag = (Tag)tagFieldInfo!.GetValue(null);
                        if (abilityTypeTag != null)
                        {
                            RegisteredAbilities.Add(abilityTypeTag, type);

                            continue;
                        }
                        else
                        {
                            GD.PrintErr($"Failed to register {nameof(MKAbility)} because field {MKAbility.__typeTagFieldName} wasn't overridden");
                            continue;
                        }
                    }

                    if (type.IsSubclassOf(typeof(MKEffect))
                        && !type.IsAbstract)
                    {
                        FieldInfo tagFieldInfo = type.GetField(MKEffect.__typeTagFieldName);
                        Tag abilityTypeTag = (Tag)tagFieldInfo!.GetValue(null);
                        if (abilityTypeTag != null)
                        {
                            RegisteredEffects.Add(abilityTypeTag, type);

                            continue;
                        }
                        else
                        {
                            GD.PrintErr($"Failed to register {nameof(MKEffect)} because field {MKEffect.__typeTagFieldName} wasn't overridden");
                            continue;
                        }
                    }
                }
            }
        }
        
        public static Type GetRegisteredAbilityType(Tag tag)
        {
            if (RegisteredAbilities.TryGetValue(tag, out var type))
            {
                return type;
            }

            GD.PrintErr($"Failed to get registered {nameof(MKAbility)} type from tag {tag.Key}");
            return null;
        }

        public static Type GetRegisteredEffectType(Tag tag)
        {
            if (RegisteredEffects.TryGetValue(tag, out var type))
            {
                return type;
            }

            GD.PrintErr($"Failed to get registered {nameof(MKEffect)} type from tag {tag.Key}");
            return null;
        }
    }
}
