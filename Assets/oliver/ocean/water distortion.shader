Shader "Sprites/Default"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
                // vec2 uv = fragCoord.xy / iResolution.xy;
                // vec4 maskColor = texture(iChannel1,uv);//Grab the mask color (this is created in the Buf A tab)
                
                // if(maskColor.r == 0.0){//Apply our funky water shader only in parts that are red in the mask
                // }
                
                // fragColor = texture(iChannel0,uv);

                float2 uv = IN.texcoord;
                float X = uv.x*20*_SinTime;
                float Y = uv.y*20*_CosTime;
                uv.y += cos(X+Y)*0.01*cos(Y);
                uv.x += sin(X-Y)*0.01*sin(Y);

                uv *= float2(128,128);
                // uv = floor(uv) + 0.5;
                uv.x /= 128;
                uv.y /= 128;
                uv = clamp(uv, 0, 1);

				fixed4 c = SampleSpriteTexture (uv) * IN.color;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}