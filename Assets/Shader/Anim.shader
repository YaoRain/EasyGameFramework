Shader "Unlit/Anim"
{
    Properties
    {
        _Color("Color Tint",Color)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Ramp("Ramp Texture",2D)="white"{}
        _Outline("Outline",Range(0,0.1))=0.02
        _Factor("Factor of Outline",Range(0,1))=0.5
        _OutlineColor("Outline Color",Color)=(0,0,0,1)
        _Specular("Specular",Color)=(1,1,1,1)
        _SpecularScale("Specular Scale",Range(0,0.1))=0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }   
        //此Pass渲染卡通着色效果，主要运用半兰伯特光照模型配合渐变纹理
        Pass
        {
            Tags{"LightMode"="ForwardBase"}
            Cull Back
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            //引入阴影相关的宏
            #include "AutoLight.cginc"
            //引入预设的光照变量，如_LightColor0
            #include "Lighting.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            sampler2D _Ramp;
            fixed4 _Specular;
            fixed _SpecularScale;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex:POSITION;
                float2 uv:TEXCOORD0;
                float3 normal:NORMAL;
                float4 tangent:TANGENT;
            };

            struct v2f
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
                float3 worldNormal:TEXCOORD1;
                float3 worldPos:TEXCOORD2;
                SHADOW_COORDS(3)
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos=UnityObjectToClipPos(v.vertex);
                o.uv=TRANSFORM_TEX(v.uv,_MainTex);
                o.worldNormal=mul(v.normal,(float3x3)unity_WorldToObject);
                o.worldPos=mul(unity_ObjectToWorld,v.vertex);
                TRANSFER_SHADOW(o);

                return o;
            }

            fixed4 frag(v2f i):SV_Target
            {
                fixed3 worldNormal=normalize(i.worldNormal);
                fixed3 worldLightDir=normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldViewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 worldHalfDir=normalize(worldLightDir+worldViewDir);

                //计算材质反射率
                fixed4 c=tex2D(_MainTex,i.uv);
                fixed3 albedo=c.rgb*_Color.rgb;

                //计算环境光
                fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz*albedo;

                //处理阴影
                UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos);

                //计算半兰伯特漫反射系数，亮化处理，将结果从[-1,1]映射到[0,1]，以便作为渐变纹理的采样uv
                fixed diff=dot(worldNormal,worldLightDir);
                diff=(diff*0.5+0.5)*atten;

                //卡通渲染的核心内容，对漫反射进行区域色阶的离散变化
                fixed3 diffuse=_LightColor0.rgb*albedo*tex2D(_Ramp,float2(diff,diff)).rgb;

                //计算半兰伯特高光系数，并将高光边缘的过渡进行抗锯齿处理，系数越大，过渡越明显
                fixed spec=dot(worldNormal,worldHalfDir);
                fixed w=fwidth(spec)*3.0;

                //计算高光，在[-w,w]范围内平滑插值
                fixed3 specular=_Specular.rgb*smoothstep(-w,w,spec-(1-_SpecularScale))*step(0.0001,_SpecularScale);

                return fixed4(ambient+diffuse,1.0);
            }
            ENDCG    
        }
    }
    FallBack "Diffuse" 
}