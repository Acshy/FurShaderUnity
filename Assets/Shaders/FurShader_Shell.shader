Shader "FurShader/FurShader_Shell"
{
    Properties
    {
        //基本颜色
        _MainTex ("Texture", 2D) = "white" { }
        _Color ("FurColor", Color) = (1, 1, 1, 1)
        _RootColor ("FurRootColor", Color) = (0.5, 0.5, 0.5, 1)

        //光照相关参数
        _Specular ("Specular", Color) = (1, 1, 1, 1)
        _Shininess ("Shininess", Range(0.01, 256.0)) = 8.0       
        _RimColor ("Rim Color", Color) = (0, 0, 0, 1)
        _RimPower ("Rim Power", Range(0.0, 8.0)) = 6.0

        //毛发参数
        _FurTex ("Fur Pattern", 2D) = "white" { }     
        _FurLength ("Fur Length", Range(0.0, 1)) = 0.5
        _FurShadow ("Fur Shadow Intensity", Range(0.0, 1)) = 0.25

        
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent" }
        
        Cull Off 
        ZWrite On
        //ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha


        Pass
        {
            CGPROGRAM
            #include "Lighting.cginc"   
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            
            sampler2D _MainTex;
            half4 _MainTex_ST; 
            fixed4 _Color;
            fixed4 _RootColor;
            fixed4 _Specular;
            fixed _Shininess;

            sampler2D _FurTex;
            half4 _FurTex_ST;
            fixed _FurLength;
            fixed _FurShadow;

            float3 _FurOffset;

            fixed4 _RimColor;
            half _RimPower;

            float _LayerOffset;


            struct a2v {
                float4 vertex : POSITION;//顶点位置
                float3 normal : NORMAL;//发现
                float4 texcoord : TEXCOORD0;//纹理坐标
                float4 texcoord2 : TEXCOORD1;//纹理坐标
            };

           struct v2f
           {
               float4 pos: SV_POSITION;
               half4 uv: TEXCOORD0;
               float3 worldNormal: TEXCOORD1;
               float3 worldPos: TEXCOORD2;
           };

            v2f vert(a2v v)
            {
                v2f o;
                float3 OffetVertex = v.vertex.xyz + v.normal * _LayerOffset *_FurLength;//顶点外扩
                OffetVertex += mul(unity_WorldToObject, _FurOffset);//顶点受力偏移

                o.pos = UnityObjectToClipPos(float4(OffetVertex, 1.0));
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord2, _FurTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            fixed4 frag(v2f i): SV_Target
            {
                
              

                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                fixed3 worldHalf = normalize(worldView + worldLight);

                fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color;
                half vdotn = 1.0 - saturate(dot(worldView, worldNormal));
                fixed3 rim = _RimColor.rgb *  _RimColor.a * saturate( 1-pow( 1-vdotn, _RimPower));

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                fixed3 diffuse = _LightColor0.rgb * albedo * (0.5f*saturate(dot(worldNormal, worldLight))+0.5f);
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal, worldHalf)), _Shininess);

                fixed3 color = ambient + diffuse + specular + rim;
                color = lerp(_RootColor,color,saturate(pow( _LayerOffset,_FurShadow)));

                fixed3 noise = tex2D(_FurTex, i.uv.zw).rgb;
                fixed alpha = saturate(noise - (_LayerOffset * _LayerOffset));

    
                return fixed4(color, alpha);
            }
            ENDCG
        }
    }
}
