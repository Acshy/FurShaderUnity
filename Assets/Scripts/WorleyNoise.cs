using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorleyNoise : MonoBehaviour
{

    [Header("贴图文件名")]
    public string FileName = "noise";
    [Header("随机种子")]
    public int RandSeed = 0;
    [Header("细胞最大间距")]
    [Range(2,128)]
    public int MaxRange=32;
    [Header("Pow/变化锐利度")]
    [Range(0.1f, 10)]
    public float Power = 2;
     [Header("贴图分辨率")]
    public int resolution = 256;//贴图分辨率
    [Header("噪声贴图")]
    public Texture2D NoiseTexture;


    private int pointNumber = 4;
    private Vector2Int[,] points = null;



    /*—————————— 编辑器接口 ————————————*/

    //创建纹理（编辑器中调用
    public void CreateWolyTexture()
    {
        NoiseTexture = GenerateWorlyMap(); 
        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().sharedMaterial = new Material(GetComponent<Renderer>().sharedMaterial.shader);
            GetComponent<Renderer>().sharedMaterial.mainTexture = NoiseTexture;
        }

    }

    //保存纹理（编辑器中调用
    public void SaveTexture()
    {
        System.IO.File.WriteAllBytes(Application.dataPath + "/Texture" + FileName + ".png", NoiseTexture.EncodeToPNG());

        #if UNITY_EDITOR || UNITY_EDITOR_WIN
                UnityEditor.AssetDatabase.Refresh();
        #endif

        Debug.LogError("贴图保存成功！文件路径：" + Application.dataPath + "/Texture" + FileName + ".png");
    }

    private void OnValidate() {
        CreateWolyTexture();
    }

    /*—————————— 纹理生成相关 ————————————*/

    //噪声生成
    private Texture2D GenerateWorlyMap()
    {
        //初始化
        if(RandSeed>0)
            Random.InitState(RandSeed);
        Texture2D result = new Texture2D(resolution, resolution);

        //初始化细胞点
        pointNumber = resolution / MaxRange;
        points = new Vector2Int[pointNumber, pointNumber];
        for (int y = 0; y < pointNumber; y++)
        {
            for (int x = 0; x < pointNumber; x++)
            {
                Vector2Int p = new Vector2Int(Random.Range(0, MaxRange), Random.Range(0, MaxRange));
                points[x, y] = p;
            }
        }

        //计算每个像素点最短距离，设置像素值
        for (int y = 0; y < resolution; y++)
            for (int x = 0; x < resolution; x++)
            {
                float dis = GetMinDis(x, y) / MaxRange;
                dis = Mathf.Pow(dis, Power);
                
                result.SetPixel(x, y,(1- dis) * Color.white);
            }

        result.Apply();
        return result;
    }

    //获取随机点
    private Vector2Int GetPoint(int x, int y)
    {
        int maxRange = resolution / pointNumber;

        Vector2Int off = new Vector2Int(maxRange * x, +maxRange * y);

        if (x >= pointNumber)
            x = 0;
        if (x < 0) x = pointNumber - 1;

        if (y >= pointNumber)
            y = 0;
        if (y < 0) y = pointNumber - 1;

        return points[x, y] + off;
    }

    //获取最短距离
    private float GetMinDis(int x, int y)
    {
        Vector2Int currentPoint = new Vector2Int(x, y);
        int gridSize = resolution / pointNumber;

        float minDis = float.MaxValue;
        int indexX = x / gridSize;
        int indexY = y / gridSize;
        
        for (int j = -1; j < 2; j++)
            for (int i = -1; i < 2; i++)
            {
                Vector2Int p = GetPoint(indexX + i, indexY + j);

                float dis = Vector2Int.Distance(p, currentPoint);
                if (dis < minDis)
                {
                    minDis = dis;
                }

            }
        return minDis;
    }
}