using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoiseTextureMixer))]
public class NoiseTextureMixerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NoiseTextureMixer mixer = (NoiseTextureMixer)target;
        if (GUILayout.Button("加载子物体中的噪声贴图"))
        {
            mixer.LoadTexture();
        }
        if (GUILayout.Button("混合噪声贴图"))
        {
            mixer.MixNoiseTexture();
        }
        if (GUILayout.Button("保存混合噪声贴图"))
        {
            mixer.SaveTexture();
        }
    }
}