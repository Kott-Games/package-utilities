using System;
using UnityEngine;

namespace Kott.Utilities.Progress
{
    public sealed class DebugProgressReporter : IProgress<ProgressReport>
    {
        public static DebugProgressReporter Instance { get; } = new DebugProgressReporter();

        private DebugProgressReporter() { }

        public void Report(ProgressReport value)
        {
#if UNITY_EDITOR
            Debug.Log($"[Bootstrap] {value.Progress * 100f,5:##0.0}% - {value.Info}");
#endif
        }
    }
}