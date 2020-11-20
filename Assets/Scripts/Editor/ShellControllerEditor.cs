using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShellController))]
public class ShellControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShellController shellController = (ShellController)target;
        if (GUILayout.Button("更新Shell"))
        {
            shellController.UpdateShellEditor();
        }
        if (GUILayout.Button("删除Shell"))
        {
            shellController.ClearShellEditor();
        }
    }
}
