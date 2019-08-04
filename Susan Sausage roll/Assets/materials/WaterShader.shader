Shader "Unlit/WaterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            // Found this on GLSL sandbox. I really liked it, changed a few things and made it tileable.
// :)
// by David Hoskins.


// Water turbulence effect by joltz0r 2013-07-04, improved 2013-07-07


// Redefine below to see the tiling...
//#define SHOW_TILING

float2 mod(float2 x, float2 y){
    return x-y*floor(x/y);
}

#define TAU 200
#define MAX_ITER 3
 fixed4 frag (v2f ia) : SV_Target
 {
              
	float time = _Time.y * 0.25+23.0;
    // uv should be the 0-1 uv of texture...
	float2 uv = ia.uv;
    
#ifdef SHOW_TILING
	float2 p = mod(uv*TAU*2.0, TAU)-500.0;
#else
    float2 p = mod(7*uv*TAU, TAU)-250.0;
#endif
	float2 i = float2(p);
	float c = 0.5;
	float inten = .009;

	for (int n = 0; n < MAX_ITER; n++) 
	{
		float t = time * (1.0 - (3.5 / float(n+1)));
		i = p + float2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
		c += 1.0/length(float2(p.x / (sin(i.x+t)/inten),p.y / (cos(i.y+t)/inten)));
	}
	c /= float(MAX_ITER);
	c = 1.17-pow(c, 1.4);
	float3 colour = pow(abs(c), 50.0);
    colour = clamp(colour + float3(0.0, 0.35, 0.5), 0.0, 0.6);
    

	#ifdef SHOW_TILING
	// Flash tile borders...
	float2 pixel = 2.0 / iResolution.xy;
	uv *= 2.0;

	float f = floor(mod(_Time.y*.5, 2.0)); 	// Flash value.
	float2 first = step(pixel, uv) * f;		   	// Rule out first screen pixels and flash.
	uv  = step(frac(uv), pixel);				// Add one line of pixels per tile.
	colour = lerp(colour, float3(1.0, 1.0, 0.0), (uv.x + uv.y) * first.x * first.y); // Yellow line
	
	#endif
	return float4(colour, 1.0);
}

          
            ENDCG
        }
    }
}
