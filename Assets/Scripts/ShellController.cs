using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ShellController : MonoBehaviour
{
    public Renderer Target;
    public Shader ShellShader;
    private Material BaseMat;

    [Header("皮毛颜色")]
    public Color FurColor;
    public Color FurRootColor;
    public Color FurSpecularColor;
    public Color FurRimColor;

    [Header("皮毛光照")]
    [Range(0,16)]
    public float Shininess;
    [Range(0, 4)]
    public float RimPower;
    [Range(0, 2)]
    public float FurShadow;

    [Header("皮毛参数")]
    public Texture2D FurPattern;
    public int LayerCount = 10;
    public float FurLength = 0.5f;
    public Vector3 FurForce;


    [Range(1,10)]
    public float FurTenacity = 1;


    GameObject[] layers;



    void UpdateShellTrans()
    {
        if (layers == null || layers.Length == 0)
        {
            return;
        }
        for (int i = 0; i < layers.Length; i++)
        {
            //依据层数、毛发长度和毛发硬度计算lerp速度
            float lerpSpeed = (layers.Length-i) * (1.0f / layers.Length)* FurTenacity;

            //让Shell的位置和旋转Lerp到目标模型
            //后面乘的常数完全是试出来的参数……没有具体物理解释
            layers[i].gameObject.transform.position = Vector3.Lerp(layers[i].gameObject.transform.position, Target.transform.position, lerpSpeed * Time.deltaTime *20);
            layers[i].gameObject.transform.rotation = Quaternion.Lerp(layers[i].gameObject.transform.rotation, Target.transform.rotation, lerpSpeed * Time.deltaTime *10);
        }
    }
    public void UpdateShellEditor()
    {
        ClearShellEditor();
        CreateShell();
    }

    public void UpdateShellRunTime()
    {
        ClearShellRunTime();
        CreateShell();
    }

    public void ClearShellEditor()
    {
        if (layers == null || layers.Length == 0)
        {
            return;
        }

        GameObject[] shells = GameObject.FindGameObjectsWithTag("Shell");

        for (int i = 0; i < shells.Length; i++)
        {
            DestroyImmediate(shells[i].gameObject);
            //Destroy(layers[i].gameObject);
        }
    }
    void ClearShellRunTime()
    {
        if (layers == null || layers.Length == 0)
        {
            return;
        }

        for (int i = 0; i < layers.Length; i++)
        {
            Destroy(layers[i].gameObject);
        }
    }
    void CreateShell()
    {
        layers = new GameObject[LayerCount];
        float furOffset = 1.0f/ LayerCount;
        for (int i = 0; i< LayerCount; i++)
        {
            //复制渲染的原模型一遍
            GameObject layer = Instantiate(Target.gameObject, Target.transform.position, Target.transform.rotation);

            //如果不想让场景界面看起来物体太多太乱
            layer.hideFlags=HideFlags.HideInHierarchy;
            //标上Tag方便找
            layer.tag = "Shell";

            //用ShellShader创建Shell材质
            layer.GetComponent<Renderer>().sharedMaterial = new Material(ShellShader);

            //将原模型的贴图传入Shell材质
            layer.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", Target.sharedMaterial.GetTexture("_MainTex"));
            //将编辑器中的参数传进去
            layer.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", FurColor);
            layer.GetComponent<Renderer>().sharedMaterial.SetColor("_RootColor", FurRootColor);
            layer.GetComponent<Renderer>().sharedMaterial.SetColor("_RimColor", FurRimColor);
            layer.GetComponent<Renderer>().sharedMaterial.SetColor("_Specular", FurSpecularColor);
            layer.GetComponent<Renderer>().sharedMaterial.SetFloat("_Shininess", Shininess);
            layer.GetComponent<Renderer>().sharedMaterial.SetFloat("_RimPower", RimPower);
            layer.GetComponent<Renderer>().sharedMaterial.SetFloat("_FurShadow", FurShadow);
            layer.GetComponent<Renderer>().sharedMaterial.SetTexture("_FurTex",FurPattern);
            layer.GetComponent<Renderer>().sharedMaterial.SetFloat("_FurLength", FurLength);

            //不同Shell的偏移参数不一样
            layer.GetComponent<Renderer>().sharedMaterial.SetFloat("_LayerOffset", i * furOffset);
            //计算受力、层数和硬度共同影响的Shell偏移
            layer.GetComponent<Renderer>().sharedMaterial.SetVector("_FurOffset", FurForce* Mathf.Pow(  i*furOffset,FurTenacity));

            //由于在单通道渲染半透明材质,进行了深度写入，为了防止被深度剔除，所以要手动更改渲染队列
            layer.GetComponent<Renderer>().sharedMaterial.renderQueue = 3000 + i;

            layers[i] = layer;
        }
    }






    void Start()
    {
        UpdateShellRunTime();
    }


    bool isEditFur = false;
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isEditFur = !isEditFur;
        }

        if(isEditFur)
            UpdateShellRunTime();
        else
            UpdateShellTrans();
    }
}
