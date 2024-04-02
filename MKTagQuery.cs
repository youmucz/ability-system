using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minikit.AbilitySystem
{
    public enum MKTagQueryCondition
    {
        Any, // At least 1 tag in the query exists in the list of tags being tested against
        All, // All of the tags in the query exist in the list of tags being tested against
        None // None of the tags in the query exist in the list of tags being tested against
    }

    [System.Serializable]
    public class MKTagQuery
    {
        [SerializeField] private MKTagQueryCondition condition = MKTagQueryCondition.Any;
        [SerializeField] private List<MKTag> tagList = new();


        public MKTagQuery(MKTagQueryCondition _condition, List<MKTag> _tagList)
        {
            condition = _condition;
            tagList = _tagList;
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
                    foreach (MKTag tag in tagList)
                    {
                        if (_tagList.Contains(tag))
                        {
                            return true;
                        }
                    }
                    return false;
                case MKTagQueryCondition.All:
                    foreach (MKTag tag in tagList)
                    {
                        if (!_tagList.Contains(tag))
                        {
                            return false;
                        }
                    }
                    return true;
                case MKTagQueryCondition.None:
                    foreach (MKTag tag in tagList)
                    {
                        if (_tagList.Contains(tag))
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
