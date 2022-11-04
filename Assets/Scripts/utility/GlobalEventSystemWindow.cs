using UnityEngine;
using UnityEditor;

public class GlobalEventSystemWindow : EditorWindow {

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Global Event System")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(GlobalEventSystemWindow));
    }
    
    void OnGUI()
    {
        var status = GlobalEventSystem.GetMapperStatus();

        foreach (var pair in status)
        {
            EditorGUILayout.BeginFoldoutHeaderGroup(true, pair.Key + ": " + pair.Value.Item2.Name);
            foreach (var actionStatus in pair.Value.Item1)
            {
                EditorGUILayout.LabelField(actionStatus);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}