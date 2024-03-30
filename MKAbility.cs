using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minikit.AbilitySystem.Internal;

namespace Minikit.AbilitySystem
{
    public abstract class MKAbility
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
        /// <summary> Tags that are granted to the owning MASComponent while this ability is active </summary>
        public MKTagContainer grantedTags { get; } = new();
        /// <summary> This ability cannot be activated if the owning MASComponent has any of these tags </summary>
        public MKTagContainer blockedByTags { get; } = new();
        /// <summary> When this ability is activated successfully, any active abilities on the owning MASComponent that matches one of these tags will be cancelled </summary>
        public MKTagContainer cancelAbilityTags { get; } = new();
        /// <summary> Tags that, when granted to the owning MASComponent, will cancel this ability (only includes grantedLooseTags) </summary>
        public MKTagContainer cancelledByGrantedLooseTags { get; } = new();
        /// <summary> The tag for the effect used to track this ability's cooldown </summary>
        public MKTag cooldownEffectTag { get; protected set; } = null;
        // -----------------------
        // ----- END SETTINGS -----

        // ----- INSTANCE -----
        // --------------------
        public MKAbilityComponent masComponent { get; private set; } = null;
        public bool active { get; private set; } = false;
        protected object[] activationParams;
        // ------------------------
        // ----- END INSTANCE -----


        public MKAbility(MKTag _typeTag)
        {
            typeTag = _typeTag;
        }
        public void OnPostConstruct()
        {
            if (cooldownEffectTag != null)
            {
                blockedByTags.AddTag(cooldownEffectTag);
            }
        }


        public void Tick(float _deltaTime)
        {
            if (active)
            {
                OnActiveTick(_deltaTime);
            }
        }

        protected virtual void OnActiveTick(float _deltaTime)
        {

        }

        public virtual bool CanActivate()
        {
            if (masComponent == null)
            {
                return false;
            }

            if (masComponent.HasAnyGrantedTags(blockedByTags))
            {
                return false;
            }

            return true;
        }

        public void Activate(params object[] _params)
        {
            activationParams = _params;

            active = true;

            MKTagContainer cancelledAbilities = masComponent.GetAllActiveAbilities(cancelAbilityTags);
            if (!cancelledAbilities.IsEmpty())
            {
                masComponent.CancelAbilities(cancelledAbilities);
            }

            OnActivate();
        }

        protected virtual void OnActivate()
        {

        }

        public void End()
        {
            if (active)
            {
                active = false;

                OnEnd(false);
            }
        }

        public void Cancel()
        {
            if (active)
            {
                active = false;

                OnEnd(true);
            }
        }

        protected virtual void OnEnd(bool _cancelled)
        {

        }

        protected void StartCooldown()
        {
            if (cooldownEffectTag != null)
            {
                masComponent.AddEffect(MKEffect.Create(cooldownEffectTag));
            }
        }

        public void Added(MKAbilityComponent _masComponent)
        {
            masComponent = _masComponent;

            OnAdded();
        }

        protected virtual void OnAdded()
        {

        }

        public void Removed(MKAbilityComponent _masComponent)
        {
            masComponent = null;

            Cancel();
            OnRemoved();
        }

        protected virtual void OnRemoved()
        {

        }

        public static MKAbility Create(MKTag _typeTag)
        {
            Type abilityType = MKAbilityReflector.GetRegisteredAbilityType(_typeTag);
            if (abilityType != null)
            {
                MKAbility abilityInstance = Activator.CreateInstance(abilityType, _typeTag) as MKAbility;
                if (abilityInstance != null)
                {
                    abilityInstance.OnPostConstruct();
                    return abilityInstance;
                }
                else
                {
                    Debug.LogError($"Failed to create instance of {typeof(MKAbility).Name} because created instance was null");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"Failed to create instance of {typeof(MKAbility).Name} because type was null");
                return null;
            }
        }
    }
} // Minicrit.MAS namespace
