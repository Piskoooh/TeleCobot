///Modified to import right handed point cloud.
///https://github.com/Piskoooh/Pcx-RosPointcloud2
///Original repository is linked bellow.

// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

using UnityEngine;
using UnityEditor;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace PcxRosPointcloud2
{
    // Note: Not sure why but EnumPopup doesn't work in ScriptedImporterEditor,
    // so it has been replaced with a normal Popup control.

    [CustomEditor(typeof(PlyImporter))]
    class PlyImporterInspector : ScriptedImporterEditor
    {
        SerializedProperty _containerType;
        SerializedProperty _coordinateType;

        string[] _containerTypeNames;
        string[] _coordinateTypeNames;

        protected override bool useAssetDrawPreview { get { return false; } }

        public override void OnEnable()
        {
            base.OnEnable();

            _containerType = serializedObject.FindProperty("_containerType");
            _containerTypeNames = System.Enum.GetNames(typeof(PlyImporter.ContainerType));
            _coordinateType = serializedObject.FindProperty("_coordinateType");
            _coordinateTypeNames = System.Enum.GetNames(typeof(PlyImporter.CoordinateType));

        }

        public override void OnInspectorGUI()
        {
            _containerType.intValue = EditorGUILayout.Popup(
                "Container Type", _containerType.intValue, _containerTypeNames);

            _coordinateType.intValue = EditorGUILayout.Popup(
                "Coordinate Type", _coordinateType.intValue, _coordinateTypeNames);

            base.ApplyRevertGUI();
        }
    }
}
