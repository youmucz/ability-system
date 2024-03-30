using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minikit.Abilities
{
    public enum MKModifierOperation
    {
        Add,
        Multiply,
        Override
    }

    public class MKModifier
    {
        public MKTag tag { get; private set; }
        public MKModifierOperation operation { get; private set; }
        public float value { get; private set; } = 0f;


        public MKModifier(MKTag _tag, MKModifierOperation _operation, float _value)
        {
            tag = _tag;
            operation = _operation;
            value = _value;
        }
    }
} // Minicrit.MAS namespace
