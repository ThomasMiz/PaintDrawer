
float4x4 World, View, Projection;
float4 color;

float4 VS(float4 pos : POSITION0) : POSITION0
{
	return mul(mul(mul(pos, World), View), Projection);
}

float4 PS() : COLOR0
{
    return color;
}

technique Technique1
{
    pass Pass1
    {
		VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
    }
}
