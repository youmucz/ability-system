using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minikit.AbilitySystem.Internal;

namespace Minikit.AbilitySystem
{
    public abstract class MKEffect
    {
        // ----- INTERNAL -----
        // --------------------
        public static string __typeTagFieldName = "__typeTag"; // NOTE: Internal, do not edit
        public static MKTag __typeTag = null; // Override in child classes with the new keyword
        // ------------------------
        // ----- END INTERNAL -----

        // ----- SETTINGS -----
        // --------------------
        /// <summary> A unique tag for this ability's class type </summary>
        public MKTag typeTag { get; private set; } = null;
        /// <summary> Tags that are granted to the owning MASComponent while this effect is applied </summary>
        public MKTagContainer grantedTags { get; } = new();
        public int maxStacks { get; protected set; } = 1;
        protected float duration = 0f;
        // ------------------------
        // ----- END SETTINGS -----

        // ----- INSTANCE -----
        // --------------------
        public int stacks { get; private set; } = 0;

        protected MKAbilityComponent abilityComponent;
        protected float timeOfApplied = 0f;
        // ------------------------
        // ----- END INSTANCE -----


        public MKEffect(MKTag _typeTag)
        {
            typeTag = _typeTag;
        }
        public void PostConstruct()
        {

        }


        public void Added(MKAbilityComponent _abilityComponent)
        {
            abilityComponent = _abilityComponent;
            timeOfApplied = Time.time;

            OnAdded();
        }

        protected virtual void OnAdded()
        {

        }

        public void Tick(float _deltaTime)
        {
            OnActiveTick(_deltaTime);

            if (GetDurationRemaining() < 0f)
            {
                abilityComponent.RemoveEffect(typeTag);
            }
        }

        protected virtual void OnActiveTick(float _deltaTime)
        {

        }

        public void Removed()
        {

        }

        protected virtual void OnRemoved()
        {

        }

        public int AddStacks(int _stacks)
        {
            if (_stacks < 0)
            {
                return 0;
            }

            int oldStacks = stacks;
            stacks = Mathf.Clamp(stacks + _stacks, 0, maxStacks);

            return stacks - oldStacks;
        }

        public float GetDuration()
        {
            return duration;
        }

        public float GetDurationRemaining()
        {
            return GetDuration() - (Time.time - timeOfApplied);
        }


        public static MKEffect Create(MKTag _typeTag)
        {
            Type effectType = MKAbilityReflector.GetRegisteredEffectType(_typeTag);
            if (effectType != null)
            {
                MKEffect effectInstance = Activator.CreateInstance(effectType, _typeTag) as MKEffect;
                if (effectInstance != null)
                {
                    effectInstance.PostConstruct();
                    return effectInstance;
                }
                else
                {
                    Debug.LogError($"Failed to create instance of {typeof(MKEffect).Name} because created instance was null");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"Failed to create instance of {typeof(MKEffect).Name} because type was null");
                return null;
            }
        }
    }
} // Minikit.AbilitySystem namespace
