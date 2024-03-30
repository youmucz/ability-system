using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minikit.Abilities
{
    public class MKTag : IEquatable<MKTag>
    {
        public string key { get; private set; }

        private static Dictionary<string, MKTag> tags = new();
        public static MKTag Invalid = MKTag.Get("");


        private MKTag(string _key)
        {
            key = _key;
            if (!tags.ContainsKey(_key))
            {
                tags.Add(_key, this);
            }
        }


        public static MKTag Get(string _key)
        {
            if (tags.ContainsKey(_key))
            {
                return tags[_key];
            }
            else
            {
                return new MKTag(_key);
            }
        }

        public override bool Equals(object _obj)
        {
            return this.Equals(_obj as MKTag);
        }

        public bool Equals(MKTag _other)
        {
            if (_other is null)
            {
                return false;
            }

            if (System.Object.ReferenceEquals(this, _other))
            {
                return true;
            }

            if (this.GetType() != _other.GetType())
            {
                return false;
            }

            return key == _other.key;
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }

        public static bool operator ==(MKTag _lhs, MKTag _rhs)
        {
            if (_lhs is null
                && _rhs is null)
            {
                return true;
            }

            if (_lhs is null
                || _rhs is null)
            {
                return false;
            }

            return _lhs.Equals(_rhs);
        }

        public static bool operator !=(MKTag _lhs, MKTag _rhs)
        {
            return !(_lhs == _rhs);
        }
    }
} // Minicrit.MAS namespace
