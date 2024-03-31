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
        private Dictionary<MKTag, MKAbility> abilitiesByTag = new();
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
            foreach (MKAbility ability in abilitiesByTag.Values.ToArray())
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
            if (HasAbility(_ability.typeTag))
            {
                return false;
            }

            abilitiesByTag.Add(_ability.typeTag, _ability);
            _ability.Added(this);
            OnAddedAbility(_ability);
            return true;
        }

        protected virtual void OnAddedAbility(MKAbility _ability)
        {

        }

        public bool RemoveAbility(MKTag _tag)
        {
            if (abilitiesByTag.ContainsKey(_tag))
            {
                MKAbility ability = abilitiesByTag[_tag];
                abilitiesByTag.Remove(_tag);
                OnRemovedAbility(ability);
                return true;
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
                if (abilitiesByTag.ContainsKey(tag))
                {
                    abilitiesByTag.Remove(tag);
                    numberRemoved++;
                }
            }

            return numberRemoved > 0;
        }

        public bool ActivateAbility(MKTag _tag, params object[] _params)
        {
            if (abilitiesByTag.ContainsKey(_tag))
            {
                if (abilitiesByTag[_tag].CanActivate())
                {
                    abilitiesByTag[_tag].Activate(_params);
                    return true;
                }
            }

            return false;
        }

        public bool CancelAbility(MKTag _tag)
        {
            if (abilitiesByTag.ContainsKey(_tag))
            {
                if (abilitiesByTag[_tag].active)
                {
                    abilitiesByTag[_tag].Cancel();
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
                if (abilitiesByTag.ContainsKey(tag)
                    && abilitiesByTag[tag].active)
                {
                    abilitiesByTag[tag].Cancel();
                    numberCancelled++;
                }
            }

            return numberCancelled > 0;
        }

        public bool HasAbility(MKTag _tag)
        {
            return abilitiesByTag.ContainsKey(_tag);
        }

        public bool IsAbilityActive(MKTag _tag)
        {
            if (abilitiesByTag.ContainsKey(_tag)
                && abilitiesByTag[_tag].active)
            {
                return true;
            }

            return false;
        }

        public bool IsAnyAbilityActive(List<MKTag> _tagList)
        {
            foreach (MKTag tag in _tagList)
            {
                if (abilitiesByTag.ContainsKey(tag)
                    && abilitiesByTag[tag].active)
                {
                    return true;
                }
            }

            return false;
        }

        public List<MKTag> GetAllActiveAbilities(List<MKTag> _tagList = null)
        {
            List<MKTag> tagList = new();
            foreach (MKAbility ability in abilitiesByTag.Values)
            {
                if (ability.active
                    && _tagList != null ? _tagList.Contains(ability.typeTag) : true)
                {
                    tagList.Add(ability.typeTag);
                }
            }

            return tagList;
        }

        public void AddGrantedLooseTag(MKTag _tag)
        {
            looseGrantedTags.Add(_tag);

            foreach (MKAbility ability in abilitiesByTag.Values)
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
            foreach (MKAbility ability in abilitiesByTag.Values)
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
