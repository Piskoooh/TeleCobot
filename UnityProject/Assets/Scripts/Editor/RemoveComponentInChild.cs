using UnityEngine;
using UnityEditor;

/// <summary>
/// 子オブジェクトについているコンポーネントを一括削除。エディタ上で使用するスクリプト。
/// </summary>
public class RemoveComponentInChild : MonoBehaviour
{

    [Header("削除したいコンポーネントを文字列で指定"), SerializeField]
    string componentName;

    //コンポーネントを取得して該当コンポーネントを削除
    void GetComAndDes()
    {
        Component[] components = GetComponentsInChildren<Component>();
        foreach (Component component in components)
        {
            if (component.GetType().Name == componentName)
            {
                DestroyImmediate(component);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RemoveComponentInChild))]
    public class ExampleInspector : Editor
    {
        RemoveComponentInChild rootClass;

        private void OnEnable()
        {
            rootClass = target as RemoveComponentInChild;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("一括削除"))
            {
                // 押下時に実行したい処理
                rootClass.GetComAndDes();
            }

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}