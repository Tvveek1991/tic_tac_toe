using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
[InitializeOnLoad]
public class AssignKey : ScriptableObject
{
    [SerializeField] private KeyData key;
    [SerializeField] private bool autoSetup;

    static EditorCoroutine editorCoroutine;

    static AssignKey()
    { 
        if(editorCoroutine == null)
            editorCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(Assign());
    }

    static IEnumerator Assign()
    {
        yield return new EditorWaitForSeconds(0);

        string[] keys = AssetDatabase.FindAssets("t:AssignKey");

        if (keys.Length > 0)
        {
            AssignKey assignKey = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(keys[0]), typeof(AssignKey)) as AssignKey;

            if (assignKey)
                if (assignKey.autoSetup)
                    assignKey.SetupKey();
        }

        editorCoroutine = null;
    }

    void OnValidate()
    {
        SetupKey();
    }

    public void SetupKey()
    {
#if UNITY_ANDROID
        if (key == null)
            return;

        PlayerSettings.Android.useCustomKeystore = true;

        PlayerSettings.Android.keyaliasName = key.Key.Alias;
        PlayerSettings.Android.keyaliasPass = key.Key.Password;

        PlayerSettings.Android.keystorePass = key.Key.Password;
        PlayerSettings.Android.keystoreName = Application.dataPath + "/" + key.Key.Path;
#endif
    }
}

[CustomEditor(typeof(AssignKey))]
[CanEditMultipleObjects]
public class AssignKeyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AssignKey key = (AssignKey)target;

        if (GUILayout.Button("Setup Key"))
        {
            key.SetupKey();
        }

    }
}