using Godot;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Minikit.AbilitySystem.Internal;

namespace Minikit.AbilitySystem
{
    public partial class MKAbilityComponent : Node2D 
    {
        private List<Tag> looseGrantedTags = new();
        private List<MKAbility> grantedAbilities = new();
        private Dictionary<Tag, MKEffect> effectsByTag = new();
        private Dictionary<Tag, MKAggregateAttribute> attributesByTag = new();


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

        public override void _Process(double delta)
        {
            UpdateInternal(delta);
        }

        protected virtual void UpdateInternal(double delta)
        {
            foreach (MKAbility ability in IterateAbilities().ToArray())
            {
                ability.Tick(delta);
            }

            foreach (MKEffect effect in effectsByTag.Values.ToArray())
            {
                effect.Tick(delta);
            }
        }


        public bool AddAttribute(MKAggregateAttribute attribute)
        {
            return attributesByTag.TryAdd(attribute.Tag, attribute);
        }

        public bool RemoveAttribute(Tag tag)
        {
            if (attributesByTag.ContainsKey(tag))
            {
                attributesByTag.Remove(tag);
                return true;
            }

            return false;
        }

        public MKAggregateAttribute GetAttribute(Tag tag)
        {
            return attributesByTag.GetValueOrDefault(tag);
        }

        public bool AddEffectStacks(Tag effectTag, int stacks = 1)
        {
            if (stacks <= 0)
            {
                return false;
            }

            if (effectsByTag.ContainsKey(effectTag))
            {
                if (effectsByTag[effectTag].AddStacks(stacks) > 0)
                {
                    return true; // Successfully added more stacks to an existing effect
                }
            }

            return false;
        }

        public bool AddEffect(MKEffect effect, int stacks = 1)
        {
            effectsByTag.Add(effect.TypeTag, effect);
            effectsByTag[effect.TypeTag].Added(this);
            effectsByTag[effect.TypeTag].AddStacks(stacks);

            return true;
        }

        public bool RemoveEffect(Tag tag)
        {
            if (effectsByTag.ContainsKey(tag))
            {
                MKEffect effect = effectsByTag[tag];
                effectsByTag.Remove(tag);
                effect.Removed();
                return true;
            }

            return false;
        }

        public bool AddAbility(MKAbility ability)
        {
            if (HasAbility(ability))
            {
                return false;
            }

            grantedAbilities.Add(ability);
            ability.Added(this);
            OnAddedAbility(ability);
            return true;
        }

        protected virtual void OnAddedAbility(MKAbility ability)
        {

        }

        public bool RemoveAbility(Tag tag)
        {
            foreach (MKAbility ability in IterateAbilities().ToArray())
            {
                if (ability.TypeTag == tag)
                {
                    if (ability.Active)
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
                    if (ability.Active)
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

        protected virtual void OnRemovedAbility(MKAbility ability)
        {

        }

        public bool RemoveAbilities(List<Tag> tagList)
        {
            int numberRemoved = 0;
            foreach (Tag tag in tagList)
            {
                if (RemoveAbility(tag))
                {
                    numberRemoved++;
                }
            }

            return numberRemoved > 0;
        }

        public bool RemoveAbilities(List<MKAbility> abilities)
        {
            int numberRemoved = 0;
            foreach (MKAbility ability in abilities)
            {
                if (RemoveAbility(ability))
                {
                    numberRemoved++;
                }
            }

            return numberRemoved > 0;
        }

        public bool ActivateAbility(Tag tag, params object[] @params)
        {
            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability.TypeTag == tag)
                {
                    if (ability.CanActivate())
                    {
                        ability.Activate(@params);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ActivateAbility(MKAbility _ability, params object[] @params)
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
                        ability.Activate(@params);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CancelAbility(Tag tag)
        {
            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability.TypeTag == tag
                    && ability.Active)
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
                    && ability.Active)
                {
                    ability.Cancel();
                    return true;
                }
            }

            return false;
        }

        public bool CancelAbilities(List<Tag> tagList)
        {
            int numberCancelled = 0;
            foreach (Tag tag in tagList)
            {
                if (CancelAbility(tag))
                {
                    numberCancelled++;
                }
            }

            return numberCancelled > 0;
        }

        public bool CancelAbilities(List<MKAbility> abilities)
        {
            int numberCancelled = 0;
            foreach (MKAbility ability in abilities)
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

        public bool HasAbility(Tag tag)
        {
            return grantedAbilities.FirstOrDefault(a => a.TypeTag == tag) != null;
        }

        public bool HasAbility(MKAbility ability)
        {
            return grantedAbilities.Contains(ability);
        }

        public List<Tag> GetAllActiveAbilities()
        {
            List<Tag> tagList = new();
            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability.Active)
                {
                    tagList.Add(ability.TypeTag);
                }
            }

            return tagList;
        }

        public List<Tag> GetAllActiveAbilitiesWithTags(List<Tag> _tagList = null)
        {
            List<Tag> tagList = new();
            foreach (var ability in IterateAbilities())
            {
                if (ability.Active && _tagList != null && _tagList.Contains(ability.TypeTag)) // If we don't supply a valid tag list, fail the check
                {
                    tagList.Add(ability.TypeTag);
                }
            }

            return tagList;
        }

        public void AddGrantedLooseTag(Tag tag)
        {
            looseGrantedTags.Add(tag);

            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability.Active
                    && ability.CancelledByGrantedLooseTags.Contains(tag))
                {
                    ability.Cancel();
                }
            }
        }

        public void RemoveGrantedLooseTag(Tag tag)
        {
            looseGrantedTags.Remove(tag);
        }

        public List<Tag> GetGrantedTags()
        {
            List<Tag> tagList = new();
            tagList.AddRange(looseGrantedTags);
            foreach (MKAbility ability in IterateAbilities())
            {
                if (ability.Active)
                {
                    tagList.AddRange(ability.GrantedTags);
                }
            }
            foreach (MKEffect effect in effectsByTag.Values)
            {
                tagList.AddRange(effect.GrantedTags);
            }

            return tagList;
        }

        public bool HasGrantedTag(Tag _tag)
        {
            foreach (Tag tag in GetGrantedTags())
            {
                if (tag == _tag)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasAnyGrantedTags(List<Tag> tagList)
        {
            foreach (Tag tag in GetGrantedTags())
            {
                if (tagList.Contains(tag))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasAllGrantedTags(List<Tag> tagList)
        {
            foreach (Tag tag in GetGrantedTags())
            {
                if (!tagList.Contains(tag))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
