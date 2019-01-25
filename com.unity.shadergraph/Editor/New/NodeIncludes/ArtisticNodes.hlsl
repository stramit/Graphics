#ifndef UNITY_SHADER_GRAPH_ARTISTIC_NODES_INCLUDED
#define UNITY_SHADER_GRAPH_ARTISTIC_NODES_INCLUDED

/****************************************************************
Hue
****************************************************************/

void Unity_Hue_real(real3 In, real Offset, real Mode, out real3 Out)
{
    // RGB to HSV
    real4 K = real4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    real4 P = lerp(real4(In.bg, K.wz), real4(In.gb, K.xy), step(In.b, In.g));
    real4 Q = lerp(real4(P.xyw, In.r), real4(In.r, P.yzx), step(P.x, In.r));
    real D = Q.x - min(Q.w, Q.y);
    real E = 1e-10;
    real3 hsv = real3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);

    real divisor = lerp(360, 1, Mode);
    real hue = hsv.x + Offset / divisor;
    hsv.x = (hue < 0)
            ? hue + 1
            : (hue > 1)
                ? hue - 1
                : hue;

    // HSV to RGB
    real4 K2 = real4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    real3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
    Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
}

void Unity_Hue_float(float3 In, float Offset, float Mode, out float3 Out)
{
    // RGB to HSV
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 P = lerp(float4(In.bg, K.wz), float4(In.gb, K.xy), step(In.b, In.g));
    float4 Q = lerp(float4(P.xyw, In.r), float4(In.r, P.yzx), step(P.x, In.r));
    float D = Q.x - min(Q.w, Q.y);
    float E = 1e-10;
    float3 hsv = float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);

    float divisor = lerp(360, 1, Mode);
    float hue = hsv.x + Offset / divisor;
    hsv.x = (hue < 0)
            ? hue + 1
            : (hue > 1)
                ? hue - 1
                : hue;

    // HSV to RGB
    float4 K2 = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
    Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
}

void Unity_Hue_half(half3 In, half Offset, half Mode, out half3 Out)
{
    // RGB to HSV
    half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    half4 P = lerp(half4(In.bg, K.wz), half4(In.gb, K.xy), step(In.b, In.g));
    half4 Q = lerp(half4(P.xyw, In.r), half4(In.r, P.yzx), step(P.x, In.r));
    half D = Q.x - min(Q.w, Q.y);
    half E = 1e-10;
    half3 hsv = half3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);

    half divisor = lerp(360, 1, Mode);
    half hue = hsv.x + Offset / divisor;
    hsv.x = (hue < 0)
            ? hue + 1
            : (hue > 1)
                ? hue - 1
                : hue;

    // HSV to RGB
    half4 K2 = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    half3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
    Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
}

#endif // UNITY_SHADER_GRAPH_ARTISTIC_NODES_INCLUDED
