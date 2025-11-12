using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kott.Utilities
{
    public static class TransformExtensions
    {
        public static IEnumerable<Transform> GetChildren(this Transform transform, bool deep = false)
        {
            foreach (Transform child in IterateChildren(transform))
            {
                yield return child;
                if (!deep) continue;
                foreach (Transform grandChild in child.GetChildren(true))
                {
                    yield return grandChild;
                }
            }
        }

        public static void ForEachChild(this Transform transform, Action<Transform> action, bool deep = false)
        {
            if (action == null) return;
            foreach (Transform child in IterateChildren(transform))
            {
                action(child);
                if (!deep) continue;
                child.ForEachChild(action, true);
            }
        }

        private static IEnumerable<Transform> IterateChildren(Transform transform)
        {
            if (!transform) yield break;

            int count = transform.childCount;
            if (count < 0) yield break;

            for (int i = 0; i < count; i++)
            {
                Transform child = transform.GetChild(i);
                yield return child;
            }
        }
    }
}
