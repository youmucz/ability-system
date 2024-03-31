using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minikit.AbilitySystem
{
    public enum MKTagQueryCondition
    {
        Any,
        All,
        None
    }

    [System.Serializable]
    public class MKTagQuery
    {
        [SerializeField] private MKTagQueryCondition condition = MKTagQueryCondition.Any;
        [SerializeField] private List<MKTag> tagContainer = new();


        public MKTagQuery(MKTagQueryCondition _condition, List<MKTag> _tagList)
        {
            condition = _condition;
            tagContainer = _tagList;
        }


        public bool Test(MKTag _tag)
        {
            return Test(new List<MKTag>() { _tag });
        }

        public bool Test(List<MKTag> _tagList)
        {
            switch (condition)
            {
                case MKTagQueryCondition.Any:
                    foreach (MKTag tag in _tagList)
                    {
                        if (tagContainer.Contains(tag))
                        {
                            return true;
                        }
                    }
                    return false;
                case MKTagQueryCondition.All:
                    foreach (MKTag tag in _tagList)
                    {
                        if (!tagContainer.Contains(tag))
                        {
                            return false;
                        }
                    }
                    return true;
                case MKTagQueryCondition.None:
                    foreach (MKTag tag in _tagList)
                    {
                        if (tagContainer.Contains(tag))
                        {
                            return false;
                        }
                    }
                    return true;
            }

            return false;
        }
    }
} // Minikit.AbilitySystem namespace
