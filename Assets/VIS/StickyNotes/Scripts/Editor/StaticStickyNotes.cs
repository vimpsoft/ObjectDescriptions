﻿using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VIS.ObjectDescription.ScriptableObjects;
using Object = UnityEngine.Object;

namespace VIS.ObjectDescription.Editor
{
    public static class StaticStickyNotes
    {
        private const string _addMenuPath = "Assets/VIS/Add Sticky Note";
        private const string _removeMenuPath = "Assets/VIS/Remove Sticky Note";

        [MenuItem(_addMenuPath, false)]
        public static void AddStickyNoteToAsset()
        {
            var targetAsset = getAddableAsset();
            var newStickyAsset = ScriptableObject.CreateInstance<StickyNote>();
            newStickyAsset.name = "Note";
            //newStickyAsset.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontUnloadUnusedAsset;
            AssetDatabase.AddObjectToAsset(newStickyAsset, targetAsset);
            AssetDatabase.SaveAssets();

            notifyOnStickNotes();
        }

        [MenuItem(_addMenuPath, true)]
        public static bool AddableAssetValidation()
        {
            return getAddableAsset() != null;
        }

        [MenuItem(_removeMenuPath, false)]
        public static void RemoveStickyNoteFromAsset()
        {
            var targetAsset = getRemovableAsset();
            AssetDatabase.RemoveObjectFromAsset(targetAsset);
            AssetDatabase.SaveAssets();
            Object.DestroyImmediate(targetAsset);

            notifyOnUnstickNotes();
        }

        [MenuItem(_removeMenuPath, true)]
        public static bool RemovableAssetValidation()
        {
            return getRemovableAsset() != null;
        }

        private static Object getAddableAsset()
        {
            var selected = Selection.objects;
            if (selected == null || selected.Length > 1)
                return null;

            var candidate = selected[0];
            Debug.Log($"candidate = {candidate.GetType().FullName}");

            if (candidate is DefaultAsset || candidate is StickyNote || candidate is MonoScript || candidate is SceneAsset ||
                candidate is Shader || candidate is AssemblyDefinitionAsset)
                return null;

            if (!AssetDatabase.IsMainAsset(candidate) && !AssetDatabase.IsSubAsset(candidate))
                return null;

            return candidate;
        }

        private static Object getRemovableAsset()
        {
            var selected = Selection.objects;

            if (selected == null || selected.Length > 1)
                return null;

            if (!(selected[0] is StickyNote))
                return null;

            return selected[0];
        }

        private static void notifyOnStickNotes()
        {
            var allEditors = Resources.FindObjectsOfTypeAll<UnityEditor.Editor>();
            var listeners = allEditors.Where(o => o is IAssetsStickedEventsListener).Select(o => o as IAssetsStickedEventsListener).ToArray();
            for (int i = 0; i < listeners.Length; i++)
                listeners[i].OnSticked();
        }

        private static void notifyOnUnstickNotes()
        {
            var allEditors = Resources.FindObjectsOfTypeAll<UnityEditor.Editor>();
            var listeners = allEditors.Where(o => o is IAssetsStickedEventsListener).Select(o => o as IAssetsStickedEventsListener).ToArray();
            for (int i = 0; i < listeners.Length; i++)
                listeners[i].OnUnsticked();
        }
    }
}
