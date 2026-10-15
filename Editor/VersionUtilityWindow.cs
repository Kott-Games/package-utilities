using System;
using UnityEditor;
using UnityEngine;

namespace Kott.Utilities
{
    public class VersionUtilityWindow : EditorWindow
    {
        private readonly GUIContent _majorVersionLabel = new("Major Version");
        private readonly GUIContent _minorVersionLabel = new("Minor Version");
        private readonly GUIContent _buildVersionLabel = new("Build Version");

        private readonly GUIContent _applyButtonLabel = new("Apply Version");

        private const int MajorVersionBase = 0;
        private const int MinorVersionBase = 1;
        private const int BuildVersionBase = 1;

        private int _majorVersion = MajorVersionBase;
        private int _minorVersion = MinorVersionBase;
        private int _buildVersion = BuildVersionBase;

        [MenuItem("Tools/Version", priority = 0)]
        private static void Open() => GetWindow<VersionUtilityWindow>("Version", true).Show();

        private void OnEnable()
        {
            string versionString = PlayerSettings.bundleVersion;
            if (Version.TryParse(versionString, out Version version))
            {
                _majorVersion = version.Major;
                _minorVersion = version.Minor;
                _buildVersion = version.Build;
            }

            ClampVersionNumbers();
        }

        private void OnGUI()
        {
            _majorVersion = EditorGUILayout.DelayedIntField(_majorVersionLabel, _majorVersion);
            _minorVersion = EditorGUILayout.DelayedIntField(_minorVersionLabel, _minorVersion);
            _buildVersion = EditorGUILayout.DelayedIntField(_buildVersionLabel, _buildVersion);

            ClampVersionNumbers();

            EditorGUILayout.Space();

            Version version = new(_majorVersion, _minorVersion, _buildVersion);
            string versionString = VersionUtility.GetPseudoRandomVersionString(version);

            EditorGUILayout.HelpBox($"Generated version: {version} ({versionString})", MessageType.Info);

            if (GUILayout.Button(_applyButtonLabel))
            {
                PlayerSettings.bundleVersion = version.ToString(3);
            }

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox($"Current version string: {Application.version}", MessageType.Info);
        }

        private void ClampVersionNumbers()
        {
            _majorVersion = Mathf.Max(_majorVersion, MajorVersionBase);
            _minorVersion = Mathf.Max(_minorVersion, MinorVersionBase);
            _buildVersion = Mathf.Max(_buildVersion, BuildVersionBase);
        }
    }
}
