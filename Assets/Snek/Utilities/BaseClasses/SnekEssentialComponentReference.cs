using System;
using UnityEngine;

namespace Snek.Utilities
{
    public struct SnekEssentialComponentReference
    {
        public Type Type;
        public Component Reference;

        public SnekEssentialComponentReference(Type type, Component reference)
        {
            Type = type;
            Reference = reference;
        }
    }
}