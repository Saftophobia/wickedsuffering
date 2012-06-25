//View matrix.
float4x4 xView;
//Projection Matrix.
float4x4 xProjection;
//World matrix.
float4x4 xWorld;
//Light direction.
float3 xLightDirection;
//Ambient light power.
float xAmbient;
//Chooses whether or not to enable lighting.
bool xEnableLighting;
//Current time
float xTime;
//The overcast i.e. How cloudy the sky is.
float xOvercast;
//A texture to be mapped.
Texture xTexture;
//A sampler for the texture.
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; 
minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};


//Textured Technique (for grass)

////Defines an output struct for the vertex shader.
struct TexVertexToPixel
{
    float4 Position   	: POSITION;    
    float4 Color		: COLOR0;
    float LightingFactor: TEXCOORD0;
    float2 TextureCoords: TEXCOORD1;
};
//Defines an output struct for the pixel shader.
struct TexPixelToFrame
{
    float4 Color : COLOR0;
};

//The vertex shader for the textured technique.
//Takes as input from XNA, the position, normal and texture coordinates
TexVertexToPixel TexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float2 inTexCoords: TEXCOORD0)
{	

	TexVertexToPixel Output = (TexVertexToPixel)0;
	//Multiplies the view and projection matrices, into a matrix containing information of both.
	float4x4 preViewProjection = mul (xView, xProjection);
	//Multiplies the world matrix with the matrix containing information of the view and projection matrices, made in the last line.
	//This creates a matrix containing the information of all three matrices. This matrix is needed to be able to
	//map the position of each vertex or point in the 3D world to our 2D screen.
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
	//Multiplies the input position with the World*View*Projection matrix, to map this 3D vertex to our 2D screen.
	//Sets the output position to this value, as this will be the position of the vertex on the screen.
	Output.Position = mul(inPos, preWorldViewProjection);
		
	//Sets the output texture coordinate to the input directly, as it will be chosen in XNA.
	Output.TextureCoords = inTexCoords;
	
	//Multiplies the World matrix with the unit vector of the input normal.
	float3 Normal = normalize(mul(normalize(inNormal), xWorld));
	//
	Output.LightingFactor = 1;
	if (xEnableLighting)
		Output.LightingFactor = saturate(dot(Normal, -xLightDirection));
    
	return Output;     
}

//Pixel shader for the Textured technique.
TexPixelToFrame TexturedPS(TexVertexToPixel PSIn) 
{

	TexPixelToFrame Output = (TexPixelToFrame)0;		
    //Sets the color of the pixel to the value of the Texture Sampler, at the coordinates from the input.
	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);
	//Sets the output color to the sum of the ambient light and the lighting factor of the input. 
	//Saturate changes the result to a range between 0 and 1.
	Output.Color.rgb *= saturate(PSIn.LightingFactor + xAmbient);

	return Output;
}

//Defines the Textured technique, and defines its vertex and pixel shaders.
technique Textured
{
	pass Pass0
    {   
    	VertexShader = compile vs_2_0 TexturedVS();
        PixelShader  = compile ps_2_0 TexturedPS();
    }
}

//Technique: PerlinNoise (for creating cloudmap)

//Defines an output struct for the vertex shader.
 struct PNVertexToPixel
 {    
     float4 Position         : POSITION;
     float2 TextureCoords    : TEXCOORD0;
 };
 
 //Defines an output struct for the pixel shader.
 struct PNPixelToFrame
 {
     float4 Color : COLOR0;
 };
 
 //Vertex shader for the perlin noise technique.
 PNVertexToPixel PerlinVS(float4 inPos : POSITION, float2 inTexCoords: TEXCOORD)
 {    
     PNVertexToPixel Output = (PNVertexToPixel)0;
     
     Output.Position = inPos;
     Output.TextureCoords = inTexCoords;
     
     return Output;    
 }
 
 //Pixel shader for the perlin noise technique.
 //It takes the noise image, and renders it 6 times at different resolutions.
 //It also makes the 6 images move over eachother at different speeds to give the effect that the clouds are moving and changing shape.
 PNPixelToFrame PerlinPS(PNVertexToPixel PSIn)
 {
     PNPixelToFrame Output = (PNPixelToFrame)0;    
     
     float2 move = float2(0,1);
     float4 perlin = tex2D(TextureSampler, (PSIn.TextureCoords)+xTime*move)/2;
     perlin += tex2D(TextureSampler, (PSIn.TextureCoords)*2+xTime*move)/4;
     perlin += tex2D(TextureSampler, (PSIn.TextureCoords)*4+xTime*move)/8;
     perlin += tex2D(TextureSampler, (PSIn.TextureCoords)*8+xTime*move)/16;
     perlin += tex2D(TextureSampler, (PSIn.TextureCoords)*16+xTime*move)/32;
     perlin += tex2D(TextureSampler, (PSIn.TextureCoords)*32+xTime*move)/32;    
     
     Output.Color.rgb = 1.0f-pow(perlin.r, xOvercast)*2.0f;
     Output.Color.a =1;
 
     return Output;
 }
 
 //Defines the perlin noise technique, and sets its vertex and pixel shaders.
 technique PerlinNoise
 {
     pass Pass0
     {
         VertexShader = compile vs_2_0 PerlinVS();
         PixelShader = compile ps_2_0 PerlinPS();
     }
 }

//Technique: SkyDome (for creating the skydome)

////Defines an output struct for the pixel shader.
struct SDVertexToPixel
{    
    float4 Position         : POSITION;
    float2 TextureCoords    : TEXCOORD0;
    float4 ObjectPosition    : TEXCOORD1;
};

//Defines an output struct for the pixel shader.
struct SDPixelToFrame
{
    float4 Color : COLOR0;
};

//The vertex shader for the skydome technique.
//similar to the one for the textured technique.
SDVertexToPixel SkyDomeVS( float4 inPos : POSITION, float2 inTexCoords: TEXCOORD0)
{    
    SDVertexToPixel Output = (SDVertexToPixel)0;
    float4x4 preViewProjection = mul (xView, xProjection);
    float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
    Output.Position = mul(inPos, preWorldViewProjection);
    Output.TextureCoords = inTexCoords;
    Output. ObjectPosition = inPos;
    
    return Output;    
}

//Pixel shader for the skydome technique. Defines two colors, one for the top of the sky and one for the bottom.
//This is to make the bottom look brighter to make it look more realistic, like a horizon.
SDPixelToFrame SkyDomePS(SDVertexToPixel PSIn)
{
    SDPixelToFrame Output = (SDPixelToFrame)0;        

    float4 topColor = float4(0.3f, 0.3f, 0.8f, 1);    
    float4 bottomColor = 1;    
    
    float4 baseColor = lerp(bottomColor, topColor, saturate((PSIn. ObjectPosition.y)/0.4f));
    float4 cloudValue = tex2D(TextureSampler, PSIn.TextureCoords).r;
    
    Output.Color = lerp(baseColor,1, cloudValue);        

    return Output;
}

//Defines the skydome technique and sets its vertex and pixel shaders.
technique SkyDome
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 SkyDomeVS();
        PixelShader = compile ps_2_0 SkyDomePS();
    }
}