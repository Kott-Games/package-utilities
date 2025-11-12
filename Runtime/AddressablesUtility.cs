#if UNITY_ADDRESSABLES

using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Kott.Utilities
{
    public static class AddressablesUtility
    {
        public static AsyncOperationHandle LoadAssets(IEnumerable<string> addresses)
        {
            return Addressables.LoadAssetsAsync<UnityEngine.Object>(
                addresses, null, Addressables.MergeMode.Union);
        }
    }
}

#endif
