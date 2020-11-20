using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTextureMixer : MonoBehaviour
{
    [Header("贴图文件名")]
    public string FileName = "noise";

    [System.Serializable]
    public class TextureInfo
    {
        public Texture2D Texture;
        public float Weight;
    }
    [SerializeField]
    [Header("噪声混合组：手动调节权重")]
    public List<TextureInfo> TextureInfoList;


    Texture2D mixedNoiseTexture;

    private void OnValidate()
    {
        MixNoiseTexture();
    }
    public void MixNoiseTexture()
    {
        if (TextureInfoList.Count <= 0)
            Debug.LogError("贴图未加载");
        
        mixedNoiseTexture = new Texture2D(TextureInfoList[0].Texture.width, TextureInfoList[0].Texture.height);

        for (int y = 0; y < TextureInfoList[0].Texture.height; y++)
        {
            for (int x = 0; x < TextureInfoList[0].Texture.width; x++)
            {
                Color pixel = Color.white;
                pixel = Color.black;

                for (int i = 0; i < TextureInfoList.Count; i++)
                {
                    pixel += TextureInfoList[i].Texture.GetPixel(x, y) * TextureInfoList[i].Weight;
                }
                pixel.a = 1;
                mixedNoiseTexture.SetPixel(x, y, pixel);
            }
        }

        mixedNoiseTexture.Apply();

        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().sharedMaterial = new Material(GetComponent<Renderer>().sharedMaterial.shader);
            GetComponent<Renderer>().sharedMaterial.mainTexture = mixedNoiseTexture;
        }
    }

    public void SaveTexture()
    {
        System.IO.File.WriteAllBytes(Application.dataPath + "/Texture" + FileName + ".png", mixedNoiseTexture.EncodeToPNG());

#if UNITY_EDITOR || UNITY_EDITOR_WIN
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.LogError("贴图保存成功！文件路径：" + Application.dataPath + "/Texture" + FileName + ".png");
    }

    public void LoadTexture()
    {
        TextureInfoList = new List<TextureInfo>();
        for (int i = 0; i < transform.childCount; i++)
        {
            TextureInfo textureInfo = new TextureInfo();
            textureInfo.Texture = transform.GetChild(i).GetComponent<WorleyNoise>().NoiseTexture;
            textureInfo.Weight = 1.0f / transform.childCount;
            TextureInfoList.Add(textureInfo);
        }
        MixNoiseTexture();
    }

}
