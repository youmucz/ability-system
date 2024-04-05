using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minikit.AbilitySystem.Internal;

namespace Minikit.AbilitySystem
{
    public class MKAbilityComponent : MonoBehaviour 
    {
        private List<MKTag> looseGrantedTags = new();
        private List<MKAbility> grantedAbilities = new();
        private Dictionary<MKTag, MKEffect> effectsByTag = new();
        private Dictionary<MKTag, MKAggregateAttribute> attributesByTag = new();


        private void Awake()
        {
            AwakeInternal();
        }

        protected virtual void AwakeInternal()
        {

        }

        private void Start()
        {
            StartInternal();
        }

        protected virtual void StartInternal()
        {

        }

        private void Update()
        {
            UpdateInternal();
        }

        protected virtual void UpdateInternal()
        {
            foreach (MKAbility ability in IterateAbilities().ToArray())
            {
                ability.Tick(Time.deltaTime);
            }

            foreach (MKEffect effect in effectsByTag.Values.ToArray())
            {
                effect.Tick(Time.deltaTime);
            }
        }


        public bool AddAttribute(MKAggregateAttribute _attribute)
        {
            if (!attributesByTag.ContainsKey(_attribute.tag))
            {
                attributesByTag.Add(_attribute.tag, _attribute);
                return true;
            }

            return false;
        }

        public bool RemoveAttribute(MKTag _tag)
        {
            if (attributesByTag.ContainsKey(_tag))
            {
                attributesByTag.Remove(_tag);
                return true;
            }

            return false;
        }

        public MKAggregateAttribute GetAttribute(MKTag _tag)
        {
            if (attributesByTag.ContainsKey(_tag))
            {
                return attributesByTag[_tag];
            }

            return null;
        }

        public bool AddEffect(MKEffect _effect, int _stacks = 1)
        {
            if (effectsByTag.ContainsKey(_effect.typeTag))
            {
                if (effectsByTag[_effect.typeTag].AddStacks(_stacks) > 0)
                {
                    return true; // Successfully added more stacks to an existing effect
                }
                else
                {
                    return false; // Failed to add any stacks to an existing effect
                }
            }
            else
            {
                effectsByTag.Add(_effect.typeTag, _effect);
                effectsByTag[_effect.typeTag].Added(this);
                return true;
            }
        }

        public bool RemoveEffect(MKTag _tag)
        {
            if (effectsByTag.ContainsKey(_tag))
            {
                MKEffect effect = effectsByTag[_tag];
                effectsByTag.Remove(_tag);
                effect.Removed();
                return true;
            }

            return false;
        }

        public bool AddAbility(MKAbility _ability)
        {
            if (HasAbility(_ability))
            {
                return false;
            }

            grantedAbilities.Add(_ability);
            _ability.Added(this);
            OnAddedAbility(_ability);
            return true;
        }

        protected virtual void OnAddedAbility(MKAbility _ability)
        {

        }

        public bool RemoveAbility(MKTag _tag)
        {
            foreach (MKAbility ability in IterateAbilities().ToArray())
            {
                if (ability.typeTag == _tag)
                {
                    if (ability.active)
                    {
                        ability.Cancel();
                    }
                    grantedAbilities.Remove(ability);
                    OnRemovedAbility(ability);

                    return true;
                }
            }

            return false;
        }

        public bool RemoveAbility(MKAbility _ability)
        {
            if (!IterateAbilities().Contains(_ability))
            {
                return false;
            }

            foreach (MKAbility ability in IterateAbilities().ToArray())
            {
                if (ability == _ability)
                {
                    if (ability.active)
                    {
                        ability.Cancel();
                    }
                    grantedAbilities.Remove(ability);
                    OnRemovedAbility(ability);

                    return true;
                }
            }

            return false;
        }

        protected virtual void OnRemovedAbility(MKAbility _ability)
        {

        }

        public bool RemoveAbilities(List<MKTag> _tagList)
        {
            int numberRemoved = 0;
            foreach (MKTag tag in _tagList)
            {
                if (RemoveAbility(tag))
                {
                    numberRemoved++;
                }
            }

            return numberRemoved > 0;
        }

        public bool RemoveAbilities(List<MKAbility> _abilities)
        {
            int numberRemoved = 0;
            foreach (MKAbility ability in _abilities)
            {
                if (RemoveAbility(ability))
                {
                    numberRemoved++;
                }
            }

            return numberRemoved > 0;
        }

        public bool ActivateAbility(MKTag _tag, params object[] _params)
        {
            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability.typeTag == _tag)
                {
                    if (ability.CanActivate())
                    {
                        ability.Activate(_params);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ActivateAbility(MKAbility _ability, params object[] _params)
        {
            if (!IterateAbilities().Contains(_ability))
            {
                return false;
            }

            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability == _ability)
                {
                    if (ability.CanActivate())
                    {
                        ability.Activate(_params);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CancelAbility(MKTag _tag)
        {
            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability.typeTag == _tag
                    && ability.active)
                {
                    ability.Cancel();
                    return true;
                }
            }

            return false;
        }

        public bool CancelAbility(MKAbility _ability)
        {
            if (!IterateAbilities().Contains(_ability))
            {
                return false;
            }

            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability == _ability
                    && ability.active)
                {
                    ability.Cancel();
                    return true;
                }
            }

            return false;
        }

        public bool CancelAbilities(List<MKTag> _tagList)
        {
            int numberCancelled = 0;
            foreach (MKTag tag in _tagList)
            {
                if (CancelAbility(tag))
                {
                    numberCancelled++;
                }
            }

            return numberCancelled > 0;
        }

        public bool CancelAbilities(List<MKAbility> _abilities)
        {
            int numberCancelled = 0;
            foreach (MKAbility ability in _abilities)
            {
                if (CancelAbility(ability))
                {
                    numberCancelled++;
                }
            }

            return numberCancelled > 0;
        }

        public IEnumerable<MKAbility> IterateAbilities()
        {
            return grantedAbilities;
        }

        public bool HasAbility(MKTag _tag)
        {
            return grantedAbilities.FirstOrDefault(a => a.typeTag == _tag) != null;
        }

        public bool HasAbility(MKAbility _ability)
        {
            return grantedAbilities.Contains(_ability);
        }

        public List<MKTag> GetAllActiveAbilities()
        {
            List<MKTag> tagList = new();
            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability.active)
                {
                    tagList.Add(ability.typeTag);
                }
            }

            return tagList;
        }

        public List<MKTag> GetAllActiveAbilitiesWithTags(List<MKTag> _tagList = null)
        {
            List<MKTag> tagList = new();
            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability.active
                    && _tagList != null ? _tagList.Contains(ability.typeTag) : false) // If we don't supply a valid tag list, fail the check
                {
                    tagList.Add(ability.typeTag);
                }
            }

            return tagList;
        }

        public void AddGrantedLooseTag(MKTag _tag)
        {
            looseGrantedTags.Add(_tag);

            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability.active
                    && ability.cancelledByGrantedLooseTags.Contains(_tag))
                {
                    ability.Cancel();
                }
            }
        }

        public void RemoveGrantedLooseTag(MKTag _tag)
        {
            looseGrantedTags.Remove(_tag);
        }

        public List<MKTag> GetGrantedTags()
        {
            List<MKTag> tagList = new();
            tagList.AddRange(looseGrantedTags);
            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability.active)
                {
                    tagList.AddRange(ability.grantedTags);
                }
            }
            foreach (MKEffect effect in effectsByTag.Values)
            {
                tagList.AddRange(effect.grantedTags);
            }

            return tagList;
        }

        public bool HasGrantedTag(MKTag _tag)
        {
            foreach (MKTag tag in GetGrantedTags())
            {
                if (tag == _tag)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasAnyGrantedTags(List<MKTag> _tagList)
        {
            foreach (MKTag tag in GetGrantedTags())
            {
                if (_tagList.Contains(tag))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasAllGrantedTags(List<MKTag> _tagList)
        {
            foreach (MKTag tag in GetGrantedTags())
            {
                if (!_tagList.Contains(tag))
                {
                    return false;
                }
            }

            return true;
        }
    }
} // Minikit.AbilitySystem namespace
