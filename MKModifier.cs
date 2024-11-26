using System.Collections;
using System.Collections.Generic;

namespace Minikit.AbilitySystem
{
    public enum MKModifierOperation
    {
        Add,
        Multiply,
        Override
    }

    public class MKModifier
    {
        public Tag Tag { get; private set; }
        public MKModifierOperation Operation { get; private set; }
        public float Value { get; private set; } = 0f;

        public MKModifier(Tag tag, MKModifierOperation operation, float value)
        {
            Tag = tag;
            Operation = operation;
            Value = value;
        }
    }
}
