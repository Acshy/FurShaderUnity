using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorleyNoise))]
public class WorleyNoiseCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WorleyNoise creator = (WorleyNoise)target;

        if (GUILayout.Button("生成噪声贴图"))
        {
            creator.CreateWolyTexture();
        }
        if (GUILayout.Button("保存噪声贴图"))
        {
            creator.SaveTexture();
        }
    }
}