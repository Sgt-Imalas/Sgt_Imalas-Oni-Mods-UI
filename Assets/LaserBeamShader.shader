// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,cgin:,cpap:True,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,atwp:True,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:33777,y:32346,varname:node_1873,prsc:2|emission-5493-OUT;n:type:ShaderForge.SFN_Color,id:5983,x:33055,y:32493,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4,c2:0.4,c3:1,c4:1;n:type:ShaderForge.SFN_OneMinus,id:6229,x:32548,y:32613,varname:node_6229,prsc:2|IN-1864-V;n:type:ShaderForge.SFN_Power,id:2982,x:32737,y:32613,varname:node_2982,prsc:2|VAL-6229-OUT,EXP-6862-OUT;n:type:ShaderForge.SFN_Vector1,id:6862,x:32548,y:32740,varname:node_6862,prsc:2,v1:5;n:type:ShaderForge.SFN_Power,id:3111,x:32723,y:32838,varname:node_3111,prsc:2|VAL-1864-V,EXP-9405-OUT;n:type:ShaderForge.SFN_TexCoord,id:1864,x:32297,y:32711,varname:node_1864,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector1,id:9405,x:32575,y:32872,varname:node_9405,prsc:2,v1:5;n:type:ShaderForge.SFN_Add,id:6028,x:32910,y:32682,varname:node_6028,prsc:2|A-2982-OUT,B-3111-OUT;n:type:ShaderForge.SFN_OneMinus,id:955,x:33073,y:32682,varname:node_955,prsc:2|IN-6028-OUT;n:type:ShaderForge.SFN_Power,id:666,x:33245,y:32682,varname:node_666,prsc:2|VAL-955-OUT,EXP-2714-OUT;n:type:ShaderForge.SFN_Vector1,id:2714,x:33073,y:32837,varname:node_2714,prsc:2,v1:5;n:type:ShaderForge.SFN_Multiply,id:1121,x:33307,y:32493,varname:node_1121,prsc:2|A-5983-RGB,B-666-OUT;n:type:ShaderForge.SFN_Tex2d,id:9453,x:32427,y:32459,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_9453,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Vector1,id:4742,x:32792,y:33063,varname:node_4742,prsc:2,v1:5;n:type:ShaderForge.SFN_ValueProperty,id:7369,x:33335,y:32869,ptovrint:False,ptlb:LaserTHicc,ptin:_LaserTHicc,varname:node_7369,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:5493,x:33521,y:32493,varname:node_5493,prsc:2|A-1121-OUT,B-7369-OUT;proporder:5983-9453-7369;pass:END;sub:END;*/

Shader "Shader Forge/LaserBeamShader" {
    Properties {
        _Color ("Color", Color) = (0.4,0.4,1,1)
        _MainTex ("MainTex", 2D) = "bump" {}
        _LaserTHicc ("LaserTHicc", Float ) = 1
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        _Stencil ("Stencil ID", Float) = 0
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilOpFail ("Stencil Fail Operation", Float) = 0
        _StencilOpZFail ("Stencil Z-Fail Operation", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            Stencil {
                Ref [_Stencil]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilOp]
                Fail [_StencilOpFail]
                ZFail [_StencilOpZFail]
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma target 3.0
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float4, _Color)
                UNITY_DEFINE_INSTANCED_PROP( float, _LaserTHicc)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _Color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Color );
                float _LaserTHicc_var = UNITY_ACCESS_INSTANCED_PROP( Props, _LaserTHicc );
                float3 emissive = ((_Color_var.rgb*pow((1.0 - (pow((1.0 - i.uv0.g),5.0)+pow(i.uv0.g,5.0))),5.0))*_LaserTHicc_var);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
