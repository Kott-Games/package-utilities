using System;
using UnityEngine;

namespace Kott.Utilities
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class GenericListAttribute : PropertyAttribute
    {
        public GenericListAttribute() : base(applyToCollection: true)
        { }
    }
}
