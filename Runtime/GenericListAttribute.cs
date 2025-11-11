using System;
using UnityEngine;

namespace ToG.Utility
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class GenericListAttribute : PropertyAttribute
    {
        public GenericListAttribute() : base(applyToCollection: true)
        {

        }
    }
}
