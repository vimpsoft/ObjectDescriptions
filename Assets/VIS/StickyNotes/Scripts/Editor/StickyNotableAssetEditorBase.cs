﻿using UnityEditor;
using VIS.ObjectDescription.ScriptableObjects;

namespace VIS.ObjectDescription.Editor
{
    internal abstract class StickyNotableAssetEditorBase : StickyNoteEditorBase
    {
        private SerializedObject _targetCache;

        protected sealed override SerializedProperty findProperty(int index, string propertyName)
        {
            if (_targetCache == null)
                setRightTarget();
            return _targetCache.FindProperty(propertyName);
        }

        protected override void applyModifiedProperties(int index)
        {
            if (_targetCache == null)
                setRightTarget();
            _targetCache.ApplyModifiedProperties();
        }

        protected override bool needToDrawBaseInspector => true;

        private void setRightTarget()
        {
            var assetPath = AssetDatabase.GetAssetPath(target);
            var asset = AssetDatabase.LoadAssetAtPath<StickyNote>(assetPath);
            _targetCache = new SerializedObject(asset);
        }
    }
}
