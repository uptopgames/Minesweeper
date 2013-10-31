//Mobile Optimised Wet Shader
//Ninja Penguin Studios(Modified by)

Shader "NINJAPENGUINSTUDIOS/Wet/WET" {
Properties {
        _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
}
SubShader { 
        Tags { "RenderType"="Opaque" }
        LOD 300
        
        
        Pass {
                Name "FORWARD"
                Tags { "LightMode" = "ForwardBase" }
Program "vp" {
// Vertex combos: 2
//  opengl - ALU: 33 to 39
//  d3d9 - ALU: 36 to 42
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_OFF" }
Bind "vertex" Vertex
Bind "tangent" ATTR14
Bind "normal" Normal
Bind "texcoord" TexCoord0
Vector 2 [unity_Scale]
Vector 3 [_WorldSpaceCameraPos]
Vector 4 [_WorldSpaceLightPos0]
Matrix 9 [_World2Object]
Vector 17 [_MainTex_ST]
"!!ARBvp1.0
# 33 ALU
PARAM c[18] = { { 1 },
                state.lightmodel.ambient,
                program.local[2..12],
                state.matrix.mvp,
                program.local[17] };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
MOV R0.xyz, vertex.attrib[14];
MUL R1.xyz, vertex.normal.zxyw, R0.yzxw;
MAD R0.xyz, vertex.normal.yzxw, R0.zxyw, -R1;
MUL R2.xyz, R0, vertex.attrib[14].w;
MOV R0.xyz, c[3];
MOV R0.w, c[0].x;
DP4 R1.z, R0, c[11];
DP4 R1.x, R0, c[9];
DP4 R1.y, R0, c[10];
MAD R1.xyz, R1, c[2].w, -vertex.position;
DP3 R0.y, R1, R2;
DP3 R0.x, R1, vertex.attrib[14];
DP3 R0.z, vertex.normal, R1;
MOV R1, c[4];
DP3 R0.w, R0, R0;
DP4 R3.z, R1, c[11];
DP4 R3.x, R1, c[9];
DP4 R3.y, R1, c[10];
RSQ R0.w, R0.w;
DP3 R1.y, R2, R3;
DP3 R1.x, vertex.attrib[14], R3;
DP3 R1.z, vertex.normal, R3;
MAD R0.xyz, R0.w, R0, R1;
DP3 R0.w, R0, R0;
RSQ R0.w, R0.w;
MUL result.texcoord[1].xyz, R0.w, R0;
MOV result.texcoord[2].xyz, R1;
MOV result.texcoord[3].xyz, c[1];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[17], c[17].zwzw;
DP4 result.position.w, vertex.position, c[16];
DP4 result.position.z, vertex.position, c[15];
DP4 result.position.y, vertex.position, c[14];
DP4 result.position.x, vertex.position, c[13];
END
# 33 instructions, 4 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_OFF" }
Bind "vertex" Vertex
Bind "tangent" TexCoord2
Bind "normal" Normal
Bind "texcoord" TexCoord0
Vector 12 [glstate_lightmodel_ambient]
Matrix 0 [glstate_matrix_mvp]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Vector 15 [_WorldSpaceLightPos0]
Matrix 8 [_World2Object]
Vector 16 [_MainTex_ST]
"vs_2_0
; 36 ALU
def c17, 1.00000000, 0, 0, 0
dcl_position0 v0
dcl_tangent0 v1
dcl_normal0 v2
dcl_texcoord0 v3
mov r0.xyz, v1
mul r1.xyz, v2.zxyw, r0.yzxw
mov r0.xyz, v1
mad r0.xyz, v2.yzxw, r0.zxyw, -r1
mul r3.xyz, r0, v1.w
mov r0.w, c17.x
mov r0.xyz, c14
dp4 r1.z, r0, c10
dp4 r1.x, r0, c8
dp4 r1.y, r0, c9
mad r0.xyz, r1, c13.w, -v0
dp3 r2.y, r0, r3
dp3 r2.x, r0, v1
dp3 r2.z, v2, r0
dp3 r1.x, r2, r2
mov r0, c10
dp4 r4.z, c15, r0
mov r0, c9
rsq r2.w, r1.x
mov r1, c8
dp4 r4.y, c15, r0
dp4 r4.x, c15, r1
dp3 r0.y, r3, r4
dp3 r0.x, v1, r4
dp3 r0.z, v2, r4
mad r1.xyz, r2.w, r2, r0
dp3 r0.w, r1, r1
rsq r0.w, r0.w
mul oT1.xyz, r0.w, r1
mov oT2.xyz, r0
mov oT3.xyz, c12
mad oT0.xy, v3, c16, c16.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "xbox360 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_OFF" }
Bind "vertex" Vertex
Bind "tangent" TexCoord2
Bind "normal" Normal
Bind "texcoord" TexCoord0
Vector 12 [_MainTex_ST]
Matrix 8 [_World2Object]
Vector 6 [_WorldSpaceCameraPos]
Vector 7 [_WorldSpaceLightPos0]
Vector 4 [glstate_lightmodel_ambient]
Matrix 0 [glstate_matrix_mvp]
Vector 5 [unity_Scale]
"vs_360
backbbabaaaaacbiaaaaaccmaaaaaaaaaaaaaaceaaaaabjiaaaaabmaaaaaaaaa
aaaaaaaaaaaaabhaaaaaaabmaaaaabgcpppoadaaaaaaaaahaaaaaabmaaaaaaaa
aaaaabflaaaaaakiaaacaaamaaabaaaaaaaaaaleaaaaaaaaaaaaaameaaacaaai
aaaeaaaaaaaaaaneaaaaaaaaaaaaaaoeaaacaaagaaabaaaaaaaaaapmaaaaaaaa
aaaaabamaaacaaahaaabaaaaaaaaaaleaaaaaaaaaaaaabcbaaacaaaeaaabaaaa
aaaaaaleaaaaaaaaaaaaabdmaaacaaaaaaaeaaaaaaaaaaneaaaaaaaaaaaaabep
aaacaaafaaabaaaaaaaaaaleaaaaaaaafpengbgjgofegfhifpfdfeaaaaabaaad
aaabaaaeaaabaaaaaaaaaaaafpfhgphcgmgedcepgcgkgfgdheaaklklaaadaaad
aaaeaaaeaaabaaaaaaaaaaaafpfhgphcgmgefdhagbgdgfedgbgngfhcgbfagphd
aaklklklaaabaaadaaabaaadaaabaaaaaaaaaaaafpfhgphcgmgefdhagbgdgfem
gjghgihefagphddaaaghgmhdhegbhegffpgmgjghgihegngpgegfgmfpgbgngcgj
gfgoheaaghgmhdhegbhegffpgngbhehcgjhifpgnhghaaahfgogjhehjfpfdgdgb
gmgfaahghdfpddfpdaaadccodacodbdbdgdcdgcodaaaklklaaaaaaaaaaaaaaab
aaaaaaaaaaaaaaaaaaaaaabeaapmaabaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
aaaaaaeaaaaaabomaadbaaahaaaaaaaaaaaaaaaaaaaacmieaaaaaaabaaaaaaae
aaaaaaaeaaaaacjaaabaaaafaaaagaagaaaadaahaacafaaiaaaadafaaaabhbfb
aaachcfcaaadhdfdaaaababkaaaabachaaaabacdaaaababjaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaapaffeaafaaaabcaa
mcaaaaaaaaaaeaajaaaabcaameaaaaaaaaaagaangabdbcaabcaaaaaaaaaagabj
gabpbcaabcaaaaaaaaaadacfaaaaccaaaaaaaaaaafpicaaaaaaaagiiaaaaaaaa
afpifaaaaaaaagiiaaaaaaaaafpieaaaaaaaaoiiaaaaaaaaafpigaaaaaaaapmi
aaaaaaaamiapaaaaaabliiaakbacadaamiapaaaaaamgiiaaklacacaamiapaaaa
aalbdejeklacabaamiapiadoaagmaadeklacaaaamiaoaaaaaaimmgaacbakahaa
miahaaahaamamgmaalakagalmiahaaabaalelbaacbajahaamiaoaaabaapmgmim
claiahabmiahaaadaalogfaaobaeafaamiahaaahaalelbleclajagahmiahaaah
aamagmleclaiagahmiahaaadabgflomaolaeafadmiahaaadaamablaaobadafaa
miaoaaacabpmblpmklahafacmiabaaacaamdloaapaacafaamiaiaaadaalomdaa
paadacaamiahiaadaamamaaaccaeaeaamiadiaaaaalalabkilagamamaibcabac
aamdloblpaacaeadmiabaaaaaagngngmnbacacppmiapaaaaaaojcfaaoaabaaaa
miahaaabaamablliclalahaamiabaaaaaaloloaapaabafaamiacaaaaaaloloaa
paabaeaafiiiabaaaalolomgpaadabiamiaeaaaaaakhkhaaopadabaamiahiaac
aaliliaaocaaaaaamiakaaaaaalmbllmolacabaamiabaaaaaamdmdaapaaaaaaa
fibaaaaaaaaaaagmocaaaaiamiahiaabaabfgmaaobaaaaaaaaaaaaaaaaaaaaaa
aaaaaaaa"
}

SubProgram "ps3 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_OFF" }
Vector 467 [glstate_lightmodel_ambient]
Matrix 256 [glstate_matrix_mvp]
Bind "vertex" Vertex
Bind "tangent" TexCoord2
Bind "normal" Normal
Bind "texcoord" TexCoord0
Vector 466 [unity_Scale]
Vector 465 [_WorldSpaceCameraPos]
Vector 464 [_WorldSpaceLightPos0]
Matrix 264 [_World2Object]
Vector 463 [_MainTex_ST]
"sce_vp_rsx // 30 instructions using 5 registers
[Configuration]
8
0000001e41050500
[Microcode]
480
00009c6c005d000d8186c0836041fffc401f9c6c005d300c0186c0836041dfa8
00011c6c00400e0c0106c0836041dffc00019c6c005d100c0186c0836041dffc
401f9c6c011cf808010400d740619f9c401f9c6c01d0300d8106c0c360403f80
00001c6c0190a00c0686c0c360405ffc00001c6c0190900c0686c0c360409ffc
00001c6c0190800c0686c0c360411ffc00021c6c01d0a00d8286c0c360405ffc
00021c6c01d0900d8286c0c360409ffc00019c6c00800243011842436041dffc
00011c6c010002308121826301a1dffc00019c6c011d200c00bfc0e30041dffc
00011c6c00800e0c04bfc0836041dffc00001c6c0140020c0106034360405ffc
00001c6c01400e0c0686008360411ffc00001c6c0140000c0686024360409ffc
00021c6c01d0800d8286c0c360411ffc00001c6c0140000c0086004360403ffc
00009c6c0140020c0106044360405ffc00009c6c21400e0c0106045fe023007c
00009c6c0140000c0486044360409ffc00001c6c0100007f8086004300a1dffc
401f9c6c01d0200d8106c0c360405f8000001c6c0140000c0086004360403ffc
401f9c6c01d0100d8106c0c360409f80401f9c6c21d0000d8106c0dfe0230000
401f9c6c0040000c0286c0836041dfa4401f9c6c0080007f808600436041dfa1
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define glstate_light0_diffuse _glesLightSource[0].diffuse
#define glstate_light1_diffuse _glesLightSource[1].diffuse
#define glstate_light2_diffuse _glesLightSource[2].diffuse
#define glstate_light3_diffuse _glesLightSource[3].diffuse
#define glstate_light0_position _glesLightSource[0].position
#define glstate_light1_position _glesLightSource[1].position
#define glstate_light2_position _glesLightSource[2].position
#define glstate_light3_position _glesLightSource[3].position
#define glstate_light0_spotDirection _glesLightSource[0].spotDirection
#define glstate_light1_spotDirection _glesLightSource[1].spotDirection
#define glstate_light2_spotDirection _glesLightSource[2].spotDirection
#define glstate_light3_spotDirection _glesLightSource[3].spotDirection
#define glstate_light0_attenuation _glesLightSource[0].atten
#define glstate_light1_attenuation _glesLightSource[1].atten
#define glstate_light2_attenuation _glesLightSource[2].atten
#define glstate_light3_attenuation _glesLightSource[3].atten
#define glstate_lightmodel_ambient _glesLightModel.ambient
#define gl_LightSource _glesLightSource
#define gl_LightSourceParameters _glesLightSourceParameters
struct _glesLightSourceParameters {
  vec4 diffuse;
  vec4 position;
  vec3 spotDirection;
  vec4 atten;
};
uniform _glesLightSourceParameters _glesLightSource[4];
#define gl_LightModel _glesLightModel
#define gl_LightModelParameters _glesLightModelParameters
struct _glesLightModelParameters {
  vec4 ambient;
};
uniform _glesLightModelParameters _glesLightModel;
#define gl_FrontMaterial _glesFrontMaterial
#define gl_BackMaterial _glesFrontMaterial
#define gl_MaterialParameters _glesMaterialParameters
struct _glesMaterialParameters {
  vec4 emission;
  vec4 ambient;
  vec4 diffuse;
  vec4 specular;
  float shininess;
};
uniform _glesMaterialParameters _glesFrontMaterial;

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;


uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _MainTex_ST;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.xyz = normalize (_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  vec3 tmpvar_2;
  tmpvar_2 = normalize (_glesNormal);
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mat3 tmpvar_6;
  tmpvar_6[0] = tmpvar_1.xyz;
  tmpvar_6[1] = (cross (tmpvar_2, tmpvar_1.xyz) * tmpvar_1.w);
  tmpvar_6[2] = tmpvar_2;
  mat3 tmpvar_7;
  tmpvar_7[0].x = tmpvar_6[0].x;
  tmpvar_7[0].y = tmpvar_6[1].x;
  tmpvar_7[0].z = tmpvar_6[2].x;
  tmpvar_7[1].x = tmpvar_6[0].y;
  tmpvar_7[1].y = tmpvar_6[1].y;
  tmpvar_7[1].z = tmpvar_6[2].y;
  tmpvar_7[2].x = tmpvar_6[0].z;
  tmpvar_7[2].y = tmpvar_6[1].z;
  tmpvar_7[2].z = tmpvar_6[2].z;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_4 = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_10;
  tmpvar_10 = normalize ((tmpvar_8 + normalize ((tmpvar_7 * (((_World2Object * tmpvar_9).xyz * unity_Scale.w) - _glesVertex.xyz)))));
  tmpvar_3 = tmpvar_10;
  highp vec3 tmpvar_11;
  tmpvar_11 = gl_LightModel.ambient.xyz;
  tmpvar_5 = tmpvar_11;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = tmpvar_5;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform mediump float _Shininess;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  lowp vec3 tmpvar_2;
  tmpvar_2 = tmpvar_1.xyz;
  lowp float tmpvar_3;
  tmpvar_3 = tmpvar_1.w;
  lowp vec3 tmpvar_4;
  tmpvar_4 = ((texture2D (_BumpMap, xlv_TEXCOORD0).xyz * 2.0) - 1.0);
  lowp vec4 c_i0;
  lowp float spec;
  lowp float tmpvar_5;
  tmpvar_5 = max (0.0, dot (tmpvar_4, xlv_TEXCOORD1));
  mediump float tmpvar_6;
  tmpvar_6 = (pow (tmpvar_5, (_Shininess * 128.0)) * tmpvar_3);
  spec = tmpvar_6;
  c_i0.xyz = ((((tmpvar_2 * _LightColor0.xyz) * max (0.0, dot (tmpvar_4, xlv_TEXCOORD2))) + (_LightColor0.xyz * spec)) * 2.0);
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_2 * xlv_TEXCOORD3));
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_SCREEN" }
Bind "vertex" Vertex
Bind "tangent" ATTR14
Bind "normal" Normal
Bind "texcoord" TexCoord0
Vector 2 [_ProjectionParams]
Vector 3 [unity_Scale]
Vector 4 [_WorldSpaceCameraPos]
Vector 17 [_WorldSpaceLightPos0]
Matrix 9 [_World2Object]
Vector 18 [_MainTex_ST]
"!!ARBvp1.0
# 39 ALU
PARAM c[19] = { { 1, 0.5 },
                state.lightmodel.ambient,
                program.local[2..12],
                state.matrix.mvp,
                program.local[17..18] };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
MOV R0.xyz, vertex.attrib[14];
MUL R1.xyz, vertex.normal.zxyw, R0.yzxw;
MAD R0.xyz, vertex.normal.yzxw, R0.zxyw, -R1;
MUL R2.xyz, R0, vertex.attrib[14].w;
MOV R0.xyz, c[4];
MOV R0.w, c[0].x;
DP4 R1.z, R0, c[11];
DP4 R1.x, R0, c[9];
DP4 R1.y, R0, c[10];
MAD R1.xyz, R1, c[3].w, -vertex.position;
DP3 R0.y, R1, R2;
DP3 R0.x, R1, vertex.attrib[14];
DP3 R0.z, vertex.normal, R1;
MOV R1, c[17];
DP3 R0.w, R0, R0;
DP4 R3.z, R1, c[11];
DP4 R3.x, R1, c[9];
DP4 R3.y, R1, c[10];
DP3 R1.y, R2, R3;
DP3 R1.x, vertex.attrib[14], R3;
DP3 R1.z, vertex.normal, R3;
RSQ R0.w, R0.w;
MAD R0.xyz, R0.w, R0, R1;
DP3 R0.w, R0, R0;
RSQ R0.w, R0.w;
MUL result.texcoord[1].xyz, R0.w, R0;
DP4 R0.w, vertex.position, c[16];
DP4 R0.z, vertex.position, c[15];
DP4 R0.x, vertex.position, c[13];
DP4 R0.y, vertex.position, c[14];
MUL R2.xyz, R0.xyww, c[0].y;
MOV result.texcoord[2].xyz, R1;
MOV R1.x, R2;
MUL R1.y, R2, c[2].x;
ADD result.texcoord[4].xy, R1, R2.z;
MOV result.position, R0;
MOV result.texcoord[4].zw, R0;
MOV result.texcoord[3].xyz, c[1];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[18], c[18].zwzw;
END
# 39 instructions, 4 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_SCREEN" }
Bind "vertex" Vertex
Bind "tangent" TexCoord2
Bind "normal" Normal
Bind "texcoord" TexCoord0
Vector 12 [glstate_lightmodel_ambient]
Matrix 0 [glstate_matrix_mvp]
Vector 13 [_ProjectionParams]
Vector 14 [_ScreenParams]
Vector 15 [unity_Scale]
Vector 16 [_WorldSpaceCameraPos]
Vector 17 [_WorldSpaceLightPos0]
Matrix 8 [_World2Object]
Vector 18 [_MainTex_ST]
"vs_2_0
; 42 ALU
def c19, 1.00000000, 0.50000000, 0, 0
dcl_position0 v0
dcl_tangent0 v1
dcl_normal0 v2
dcl_texcoord0 v3
mov r0.xyz, v1
mul r1.xyz, v2.zxyw, r0.yzxw
mov r0.xyz, v1
mad r0.xyz, v2.yzxw, r0.zxyw, -r1
mul r3.xyz, r0, v1.w
mov r0.w, c19.x
mov r0.xyz, c16
dp4 r1.z, r0, c10
dp4 r1.x, r0, c8
dp4 r1.y, r0, c9
mad r0.xyz, r1, c15.w, -v0
dp3 r2.y, r0, r3
dp3 r2.x, r0, v1
dp3 r2.z, v2, r0
dp3 r1.x, r2, r2
mov r0, c10
dp4 r4.z, c17, r0
mov r0, c9
rsq r2.w, r1.x
mov r1, c8
dp4 r4.x, c17, r1
dp4 r4.y, c17, r0
dp3 r0.y, r3, r4
dp3 r0.x, v1, r4
dp3 r0.z, v2, r4
mad r1.xyz, r2.w, r2, r0
dp3 r0.w, r1, r1
rsq r0.w, r0.w
mul oT1.xyz, r0.w, r1
dp4 r1.w, v0, c3
dp4 r1.z, v0, c2
dp4 r1.x, v0, c0
dp4 r1.y, v0, c1
mul r2.xyz, r1.xyww, c19.y
mov oT2.xyz, r0
mov r0.x, r2
mul r0.y, r2, c13.x
mad oT4.xy, r2.z, c14.zwzw, r0
mov oPos, r1
mov oT4.zw, r1
mov oT3.xyz, c12
mad oT0.xy, v3, c18, c18.zwzw
"
}

SubProgram "xbox360 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_SCREEN" }
Bind "vertex" Vertex
Bind "tangent" TexCoord2
Bind "normal" Normal
Bind "texcoord" TexCoord0
Vector 14 [_MainTex_ST]
Vector 5 [_ProjectionParams]
Vector 6 [_ScreenParams]
Matrix 10 [_World2Object]
Vector 8 [_WorldSpaceCameraPos]
Vector 9 [_WorldSpaceLightPos0]
Vector 4 [glstate_lightmodel_ambient]
Matrix 0 [glstate_matrix_mvp]
Vector 7 [unity_Scale]
"vs_360
backbbabaaaaacgmaaaaacfmaaaaaaaaaaaaaaceaaaaaboaaaaaacaiaaaaaaaa
aaaaaaaaaaaaabliaaaaaabmaaaaabkkpppoadaaaaaaaaajaaaaaabmaaaaaaaa
aaaaabkdaaaaaanaaaacaaaoaaabaaaaaaaaaanmaaaaaaaaaaaaaaomaaacaaaf
aaabaaaaaaaaaanmaaaaaaaaaaaaaapoaaacaaagaaabaaaaaaaaaanmaaaaaaaa
aaaaabamaaacaaakaaaeaaaaaaaaabbmaaaaaaaaaaaaabcmaaacaaaiaaabaaaa
aaaaabeeaaaaaaaaaaaaabfeaaacaaajaaabaaaaaaaaaanmaaaaaaaaaaaaabgj
aaacaaaeaaabaaaaaaaaaanmaaaaaaaaaaaaabieaaacaaaaaaaeaaaaaaaaabbm
aaaaaaaaaaaaabjhaaacaaahaaabaaaaaaaaaanmaaaaaaaafpengbgjgofegfhi
fpfdfeaaaaabaaadaaabaaaeaaabaaaaaaaaaaaafpfahcgpgkgfgdhegjgpgofa
gbhcgbgnhdaafpfdgdhcgfgfgofagbhcgbgnhdaafpfhgphcgmgedcepgcgkgfgd
heaaklklaaadaaadaaaeaaaeaaabaaaaaaaaaaaafpfhgphcgmgefdhagbgdgfed
gbgngfhcgbfagphdaaklklklaaabaaadaaabaaadaaabaaaaaaaaaaaafpfhgphc
gmgefdhagbgdgfemgjghgihefagphddaaaghgmhdhegbhegffpgmgjghgihegngp
gegfgmfpgbgngcgjgfgoheaaghgmhdhegbhegffpgngbhehcgjhifpgnhghaaahf
gogjhehjfpfdgdgbgmgfaahghdfpddfpdaaadccodacodbdbdgdcdgcodaaaklkl
aaaaaaaaaaaaaaabaaaaaaaaaaaaaaaaaaaaaabeaapmaabaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaeaaaaaacbmaaebaaahaaaaaaaaaaaaaaaaaaaadmkf
aaaaaaabaaaaaaaeaaaaaaagaaaaacjaaabaaaafaaaagaagaaaadaahaacafaai
aaaadafaaaabhbfbaaachcfcaaadhdfdaaaepefeaaaababnaaaabaclaaaabach
aaaababmaaaaaablaaaabacaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadpaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaapaffeaafaaaabcaamcaaaaaaaaaafaajaaaabcaa
meaaaaaaaaaagaaogabebcaabcaaaaaaaaaagabkgacabcaabcaaaaaaaaaagacg
aaaaccaaaaaaaaaaafpigaaaaaaaagiiaaaaaaaaafpifaaaaaaaagiiaaaaaaaa
afpieaaaaaaaaoiiaaaaaaaaafpicaaaaaaaacdpaaaaaaaamiapaaaaaabliiaa
kbagadaamiapaaaaaamgnapiklagacaamiapaaaaaalbdepiklagabaamiapaaah
aagmnajeklagaaaamiapiadoaananaaaocahahaamiahaaaaaamamgmaalamaian
miahaaabaalelbaacbalajaamiaoaaabaapmgmimclakajabbeahaaadaalogfgm
mbaeafammiahaaaaaalelbleclalaiaamianaaaaaapagmieclakaiaamiahaaad
abgflomaolaeafadamchaaadaamablmgmbadafajmiahaaagabbeblmaklaaahag
beabaaacaalolomgnaagafamamecaaacaalolomgnaagaeajbeaiaaadaalololb
naadagamamihaaagaamagmmgibahppajmiamiaaeaanlnlaaocahahaamiahiaad
aamamaaaccaeaeaamiadiaaaaabklabkilacaoaomiabaaaaaagngnlbnbacacpp
aibiabagaalbgmblkbagafadmiadiaaeaamgbkbiklagagagmiapaaaaaaojcfaa
oaabaaaamiahaaabaamablliclanajaamiabaaaaaaloloaapaabafaamiacaaaa
aaloloaapaabaeaafiiiabaaaalolomgpaadabiamiaeaaaaaakhkhaaopadabaa
miahiaacaaliliaaocaaaaaamiakaaaaaalmbllmolacabaamiabaaaaaamdmdaa
paaaaaaafibaaaaaaaaaaagmocaaaaiamiahiaabaabfgmaaobaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaa"
}

SubProgram "ps3 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_SCREEN" }
Vector 467 [glstate_lightmodel_ambient]
Matrix 256 [glstate_matrix_mvp]
Bind "vertex" Vertex
Bind "tangent" TexCoord2
Bind "normal" Normal
Bind "texcoord" TexCoord0
Vector 466 [_ProjectionParams]
Vector 465 [unity_Scale]
Vector 464 [_WorldSpaceCameraPos]
Vector 463 [_WorldSpaceLightPos0]
Matrix 264 [_World2Object]
Vector 462 [_MainTex_ST]
"sce_vp_rsx // 36 instructions using 6 registers
[Configuration]
8
0000002441050600
[Defaults]
1
461 1
3f000000
[Microcode]
576
00009c6c005cf00d8186c0836041fffc401f9c6c005d300c0186c0836041dfa8
00019c6c00400e0c0106c0836041dffc00011c6c005d000c0186c0836041dffc
401f9c6c011ce808010400d740619f9c00001c6c01d0300d8106c0c360403ffc
00001c6c01d0200d8106c0c360405ffc00001c6c01d0100d8106c0c360409ffc
00001c6c01d0000d8106c0c360411ffc00021c6c01d0a00d8286c0c360405ffc
00021c6c01d0900d8286c0c360409ffc00021c6c01d0800d8286c0c360411ffc
00009c6c0190a00c0486c0c360405ffc00009c6c0190900c0486c0c360409ffc
00009c6c0190800c0486c0c360411ffc00011c6c00800243011843436041dffc
00011c6c01000230812183630121dffc00009c6c011d100c02bfc0e30041dffc
401f9c6c0040000d8086c0836041ff8000019c6c009cd00e008000c36041dffc
00011c6c00800e0c04bfc0836041dffc00029c6c0140020c0106014360405ffc
00029c6c01400e0c0286008360411ffc00029c6c0140000c0286024360409ffc
00009c6c0140020c0106044360405ffc00009c6c0140000c0a86054360409ffc
00009c6c01400e0c0106044360411ffc00001c6c204000000686c08aa0a300fc
00009c6c0140000c0486044360409ffc00011c6c0100007f8286054300a1dffc
00001c6c009d202a868000c360409ffc00009c6c0140000c0486024360403ffc
401f9c6c00c000080086c09541a19fac401f9c6c204000558086c09fe0a260ac
401f9c6c0040000c0286c0836041dfa4401f9c6c0080007f828602436041dfa1
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_SCREEN" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define glstate_light0_diffuse _glesLightSource[0].diffuse
#define glstate_light1_diffuse _glesLightSource[1].diffuse
#define glstate_light2_diffuse _glesLightSource[2].diffuse
#define glstate_light3_diffuse _glesLightSource[3].diffuse
#define glstate_light0_position _glesLightSource[0].position
#define glstate_light1_position _glesLightSource[1].position
#define glstate_light2_position _glesLightSource[2].position
#define glstate_light3_position _glesLightSource[3].position
#define glstate_light0_spotDirection _glesLightSource[0].spotDirection
#define glstate_light1_spotDirection _glesLightSource[1].spotDirection
#define glstate_light2_spotDirection _glesLightSource[2].spotDirection
#define glstate_light3_spotDirection _glesLightSource[3].spotDirection
#define glstate_light0_attenuation _glesLightSource[0].atten
#define glstate_light1_attenuation _glesLightSource[1].atten
#define glstate_light2_attenuation _glesLightSource[2].atten
#define glstate_light3_attenuation _glesLightSource[3].atten
#define glstate_lightmodel_ambient _glesLightModel.ambient
#define gl_LightSource _glesLightSource
#define gl_LightSourceParameters _glesLightSourceParameters
struct _glesLightSourceParameters {
  vec4 diffuse;
  vec4 position;
  vec3 spotDirection;
  vec4 atten;
};
uniform _glesLightSourceParameters _glesLightSource[4];
#define gl_LightModel _glesLightModel
#define gl_LightModelParameters _glesLightModelParameters
struct _glesLightModelParameters {
  vec4 ambient;
};
uniform _glesLightModelParameters _glesLightModel;
#define gl_FrontMaterial _glesFrontMaterial
#define gl_BackMaterial _glesFrontMaterial
#define gl_MaterialParameters _glesMaterialParameters
struct _glesMaterialParameters {
  vec4 emission;
  vec4 ambient;
  vec4 diffuse;
  vec4 specular;
  float shininess;
};
uniform _glesMaterialParameters _glesFrontMaterial;

varying highp vec4 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;


uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _MainTex_ST;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.xyz = normalize (_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  vec3 tmpvar_2;
  tmpvar_2 = normalize (_glesNormal);
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (gl_ModelViewProjectionMatrix * _glesVertex);
  mat3 tmpvar_7;
  tmpvar_7[0] = tmpvar_1.xyz;
  tmpvar_7[1] = (cross (tmpvar_2, tmpvar_1.xyz) * tmpvar_1.w);
  tmpvar_7[2] = tmpvar_2;
  mat3 tmpvar_8;
  tmpvar_8[0].x = tmpvar_7[0].x;
  tmpvar_8[0].y = tmpvar_7[1].x;
  tmpvar_8[0].z = tmpvar_7[2].x;
  tmpvar_8[1].x = tmpvar_7[0].y;
  tmpvar_8[1].y = tmpvar_7[1].y;
  tmpvar_8[1].z = tmpvar_7[2].y;
  tmpvar_8[2].x = tmpvar_7[0].z;
  tmpvar_8[2].y = tmpvar_7[1].z;
  tmpvar_8[2].z = tmpvar_7[2].z;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_4 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_11;
  tmpvar_11 = normalize ((tmpvar_9 + normalize ((tmpvar_8 * (((_World2Object * tmpvar_10).xyz * unity_Scale.w) - _glesVertex.xyz)))));
  tmpvar_3 = tmpvar_11;
  highp vec3 tmpvar_12;
  tmpvar_12 = gl_LightModel.ambient.xyz;
  tmpvar_5 = tmpvar_12;
  highp vec4 o_i0;
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_6 * 0.5);
  o_i0 = tmpvar_13;
  highp vec2 tmpvar_14;
  tmpvar_14.x = tmpvar_13.x;
  tmpvar_14.y = (tmpvar_13.y * _ProjectionParams.x);
  o_i0.xy = (tmpvar_14 + tmpvar_13.w);
  o_i0.zw = tmpvar_6.zw;
  gl_Position = tmpvar_6;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = tmpvar_5;
  xlv_TEXCOORD4 = o_i0;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform mediump float _Shininess;
uniform sampler2D _ShadowMapTexture;
uniform sampler2D _MainTex;
uniform lowp vec4 _LightColor0;
uniform sampler2D _BumpMap;
void main ()
{
  lowp vec4 c;
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  lowp vec3 tmpvar_2;
  tmpvar_2 = tmpvar_1.xyz;
  lowp float tmpvar_3;
  tmpvar_3 = tmpvar_1.w;
  lowp vec3 tmpvar_4;
  tmpvar_4 = ((texture2D (_BumpMap, xlv_TEXCOORD0).xyz * 2.0) - 1.0);
  lowp vec4 c_i0;
  lowp float spec;
  lowp float tmpvar_5;
  tmpvar_5 = max (0.0, dot (tmpvar_4, xlv_TEXCOORD1));
  mediump float tmpvar_6;
  tmpvar_6 = (pow (tmpvar_5, (_Shininess * 128.0)) * tmpvar_3);
  spec = tmpvar_6;
  c_i0.xyz = ((((tmpvar_2 * _LightColor0.xyz) * max (0.0, dot (tmpvar_4, xlv_TEXCOORD2))) + (_LightColor0.xyz * spec)) * (texture2DProj (_ShadowMapTexture, xlv_TEXCOORD4).x * 2.0));
  c_i0.w = 0.0;
  c = c_i0;
  c.xyz = (c_i0.xyz + (tmpvar_2 * xlv_TEXCOORD3));
  gl_FragData[0] = c;
}



#endif"
}

}
Program "fp" {
// Fragment combos: 2
//  opengl - ALU: 22 to 24, TEX: 2 to 3
//  d3d9 - ALU: 25 to 26, TEX: 2 to 3
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_OFF" }
Vector 0 [_LightColor0]
Float 1 [_Shininess]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_BumpMap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 22 ALU, 2 TEX
PARAM c[3] = { program.local[0..1],
                { 2, 1, 0, 128 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1.yw, fragment.texcoord[0], texture[1], 2D;
MAD R1.xy, R1.wyzw, c[2].x, -c[2].y;
MUL R1.z, R1.y, R1.y;
MAD R1.z, -R1.x, R1.x, -R1;
MUL R2.xyz, R0, fragment.texcoord[3];
ADD R1.z, R1, c[2].y;
RSQ R1.z, R1.z;
RCP R1.z, R1.z;
DP3 R1.w, R1, fragment.texcoord[1];
MOV R2.w, c[2];
MUL R2.w, R2, c[1].x;
MAX R1.w, R1, c[2].z;
POW R1.w, R1.w, R2.w;
MUL R1.w, R0, R1;
DP3 R0.w, R1, fragment.texcoord[2];
MUL R0.xyz, R0, c[0];
MUL R1.xyz, R1.w, c[0];
MAX R0.w, R0, c[2].z;
MAD R0.xyz, R0, R0.w, R1;
MAD result.color.xyz, R0, c[2].x, R2;
MOV result.color.w, c[2].z;
END
# 22 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_OFF" }
Vector 0 [_LightColor0]
Float 1 [_Shininess]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_BumpMap] 2D
"ps_2_0
; 25 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c2, 2.00000000, -1.00000000, 1.00000000, 0.00000000
def c3, 128.00000000, 0, 0, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
texld r0, t0, s1
texld r3, t0, s0
mov r0.x, r0.w
mad_pp r4.xy, r0, c2.x, c2.y
mul_pp r0.x, r4.y, r4.y
mad_pp r0.x, -r4, r4, -r0
add_pp r0.x, r0, c2.z
rsq_pp r0.x, r0.x
rcp_pp r4.z, r0.x
mov_pp r0.x, c1
dp3_pp r1.x, r4, t1
mul_pp r0.x, c3, r0
max_pp r1.x, r1, c2.w
pow_pp r2.x, r1.x, r0.x
mov_pp r1.x, r2.x
mul_pp r1.x, r3.w, r1
mul_pp r2.xyz, r1.x, c0
dp3_pp r0.x, r4, t2
mul_pp r1.xyz, r3, c0
max_pp r0.x, r0, c2.w
mad_pp r0.xyz, r1, r0.x, r2
mul_pp r1.xyz, r3, t3
mov_pp r0.w, c2
mad_pp r0.xyz, r0, c2.x, r1
mov_pp oC0, r0
"
}

SubProgram "xbox360 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_OFF" }
Vector 0 [_LightColor0]
Float 1 [_Shininess]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_BumpMap] 2D
"ps_360
backbbaaaaaaabgaaaaaabdaaaaaaaaaaaaaaaceaaaaabaiaaaaabdaaaaaaaaa
aaaaaaaaaaaaaaoaaaaaaabmaaaaaandppppadaaaaaaaaaeaaaaaabmaaaaaaaa
aaaaaammaaaaaagmaaadaaabaaabaaaaaaaaaahiaaaaaaaaaaaaaaiiaaacaaaa
aaabaaaaaaaaaajiaaaaaaaaaaaaaakiaaadaaaaaaabaaaaaaaaaahiaaaaaaaa
aaaaaalbaaacaaabaaabaaaaaaaaaalmaaaaaaaafpechfgnhaengbhaaaklklkl
aaaeaaamaaabaaabaaabaaaaaaaaaaaafpemgjghgiheedgpgmgphcdaaaklklkl
aaabaaadaaabaaaeaaabaaaaaaaaaaaafpengbgjgofegfhiaafpfdgigjgogjgo
gfhdhdaaaaaaaaadaaabaaabaaabaaaaaaaaaaaahahdfpddfpdaaadccodacodb
dbdgdcdgcodaaaklaaaaaaaaaaaaaaabaaaaaaaaaaaaaaaaaaaaaabeabpmaaba
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaeaaaaaaapabaaaaeaaaaaaaaae
aaaaaaaaaaaacmieaaapaaapaaaaaaabaaaadafaaaaahbfbaaaahcfcaaaahdfd
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
eaaaaaaaaaaaaaaalpiaaaaadpiaaaaaedaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
aaafcaadaaaabcaameaaaaaaaaaagaafgaalbcaabcaaaaaaaaaacabbaaaaccaa
aaaaaaaabaaieaabbpbppgiiaaaaeaaababiaaabbpbpppnjaaaaeaaamiagaaaa
aagbgmmgilaapopomiabaaaaaelclcblnbaaaapokaiaaaaaaaaaaagmocaaaaia
miabaaaaaamdloaapaaaacaamiacaaaaaamdloaapaaaabaamiafaaaaaalalbaa
kcaapoaaeaciaaaaaagmgmmgcbabppiamiacaaaaaabllbaaobaaaaaadicaaaaa
aaaaaalbocaaaaaabeapaaabaajeielbkbaeaaaaamipaaabaapikmblobabaaae
aaehabaaaamamamlobaeadabmiadaaabaabllalaklaaaaabmiahmaaaaamagmma
klabpoaaaaaaaaaaaaaaaaaaaaaaaaaa"
}

SubProgram "ps3 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_OFF" }
Vector 0 [_LightColor0]
Float 1 [_Shininess]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_BumpMap] 2D
"sce_fp_rsx // 33 instructions using 3 registers
[Configuration]
24
ffffffff0003c020000ffff1000000000000840003000000
[Offsets]
2
_LightColor0 2 0
000001d000000120
_Shininess 1 0
00000040
[Microcode]
528
94001702c8011c9dc8000001c8003fe106820440ce001c9daa02000054020001
00000000000040000000bf80000000001080014000021c9cc8000001c8000001
000000000000000000000000000000009e021700c8011c9dc8000001c8003fe1
02800240ab041c9cab040000c800000102800440c9041c9fc9040001c9000003
02800340c9001c9d00020000c800000100003f80000000000000000000000000
08823b4001003c9cc9000001c80000011e7e7d00c8001c9dc8000001c8000001
a4800540c9041c9dc8010001c8003fe110820900ab001c9c00020000c8000001
00000000000000000000000000000000c2800540c9041c9dc8010001c8003fe1
08801d00ff041c9dc8000001c80000010e820240c8041c9dc8020001c8000001
0000000000000000000000000000000004800240ff001c9d00020000c8000001
000043000000000000000000000000001080020055001c9dab000000c8000001
ee880140c8011c9dc8000001c8003fe104801c40ff001c9dc8000001c8000001
02800900c9001c9d00020000c800000100000000000000000000000000000000
10800240c8041c9dab000000c80000010e800240c9041c9d01000000c8000001
0e8a0440ff001c9dc8021001c900000100000000000000000000000000000000
10800140c8021c9dc8000001c800000100000000000000000000000000000000
0e810440c8041c9dc9100001c9140001
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_OFF" }
"!!GLES"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_SCREEN" }
Vector 0 [_LightColor0]
Float 1 [_Shininess]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_BumpMap] 2D
SetTexture 2 [_ShadowMapTexture] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 24 ALU, 3 TEX
PARAM c[3] = { program.local[0..1],
                { 2, 1, 0, 128 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R1.yw, fragment.texcoord[0], texture[1], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TXP R1.x, fragment.texcoord[4], texture[2], 2D;
MAD R1.yz, R1.xwyw, c[2].x, -c[2].y;
MUL R1.w, R1.z, R1.z;
MAD R1.w, -R1.y, R1.y, -R1;
MOV R2.x, c[2].w;
ADD R1.w, R1, c[2].y;
RSQ R1.w, R1.w;
RCP R1.w, R1.w;
DP3 R2.y, R1.yzww, fragment.texcoord[1];
MUL R2.z, R2.x, c[1].x;
MAX R2.x, R2.y, c[2].z;
POW R2.w, R2.x, R2.z;
MUL R2.xyz, R0, c[0];
MUL R2.w, R0, R2;
DP3 R0.w, R1.yzww, fragment.texcoord[2];
MUL R1.yzw, R2.w, c[0].xxyz;
MAX R0.w, R0, c[2].z;
MAD R2.xyz, R2, R0.w, R1.yzww;
MUL R0.xyz, R0, fragment.texcoord[3];
MUL R1.xyz, R1.x, R2;
MAD result.color.xyz, R1, c[2].x, R0;
MOV result.color.w, c[2].z;
END
# 24 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_SCREEN" }
Vector 0 [_LightColor0]
Float 1 [_Shininess]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_BumpMap] 2D
SetTexture 2 [_ShadowMapTexture] 2D
"ps_2_0
; 26 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c2, 2.00000000, -1.00000000, 1.00000000, 0.00000000
def c3, 128.00000000, 0, 0, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
dcl t4
texld r0, t0, s1
texldp r5, t4, s2
texld r2, t0, s0
mov r0.x, r0.w
mad_pp r4.xy, r0, c2.x, c2.y
mul_pp r0.x, r4.y, r4.y
mad_pp r0.x, -r4, r4, -r0
add_pp r0.x, r0, c2.z
rsq_pp r0.x, r0.x
rcp_pp r4.z, r0.x
mov_pp r0.x, c1
dp3_pp r1.x, r4, t1
mul_pp r0.x, c3, r0
max_pp r1.x, r1, c2.w
pow_pp r3.x, r1.x, r0.x
mov_pp r1.x, r3.x
mul_pp r1.x, r2.w, r1
mul_pp r3.xyz, r1.x, c0
dp3_pp r0.x, r4, t2
mul_pp r1.xyz, r2, c0
max_pp r0.x, r0, c2.w
mad_pp r0.xyz, r1, r0.x, r3
mul_pp r0.xyz, r5.x, r0
mul_pp r1.xyz, r2, t3
mov_pp r0.w, c2
mad_pp r0.xyz, r0, c2.x, r1
mov_pp oC0, r0
"
}

SubProgram "xbox360 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_SCREEN" }
Vector 0 [_LightColor0]
Float 1 [_Shininess]
SetTexture 0 [_ShadowMapTexture] 2D
SetTexture 1 [_MainTex] 2D
SetTexture 2 [_BumpMap] 2D
"ps_360
backbbaaaaaaabimaaaaabfeaaaaaaaaaaaaaaceaaaaabdaaaaaabfiaaaaaaaa
aaaaaaaaaaaaabaiaaaaaabmaaaaaaplppppadaaaaaaaaafaaaaaabmaaaaaaaa
aaaaaapeaaaaaaiaaaadaaacaaabaaaaaaaaaaimaaaaaaaaaaaaaajmaaacaaaa
aaabaaaaaaaaaakmaaaaaaaaaaaaaalmaaadaaabaaabaaaaaaaaaaimaaaaaaaa
aaaaaamfaaadaaaaaaabaaaaaaaaaaimaaaaaaaaaaaaaanhaaacaaabaaabaaaa
aaaaaaoeaaaaaaaafpechfgnhaengbhaaaklklklaaaeaaamaaabaaabaaabaaaa
aaaaaaaafpemgjghgiheedgpgmgphcdaaaklklklaaabaaadaaabaaaeaaabaaaa
aaaaaaaafpengbgjgofegfhiaafpfdgigbgegphhengbhafegfhihehfhcgfaafp
fdgigjgogjgogfhdhdaaklklaaaaaaadaaabaaabaaabaaaaaaaaaaaahahdfpdd
fpdaaadccodacodbdbdgdcdgcodaaaklaaaaaaaaaaaaaaabaaaaaaaaaaaaaaaa
aaaaaabeabpmaabaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaeaaaaaabbe
baaaafaaaaaaaaaeaaaaaaaaaaaadmkfaabpaabpaaaaaaabaaaadafaaaaahbfb
aaaahcfcaaaahdfdaaaapefeaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaalpiaaaaa
aaaaaaaadpiaaaaaedaaaaaaabfafaadaaaabcaameaaaaaaaaaagaaigaaobcaa
bcaaaaaaaaaacabeaaaaccaaaaaaaaaaemeaaaaaaaaaaablocaaaaaemiamaaaa
aamgkmaaobaaaeaababifaabbpbppgiiaaaaeaaaliaieaabbpbppppiaaaaeaaa
bacieaabbpbppompaaaaeaaamialaaaaaagcgcaaoaaeaeaamiadaaaeaalagmaa
kaaappaamiaeaaaaaegngnmgnbaeaeppkaeiaeabaagmblmgcbabppiamiabaaab
aaloloaapaaeabaamiacaaabaaloloaapaaeacaamiadaaacaalalbaakcabppaa
eaepaaaeaaaamagmkbafaaicmiabaaabaablmgaaobabaaaadiehabaaaamamagm
obafadabbeabaaabaamgblblobabafaeamedababaagmlamgkbabaaabmiahaaab
aamalbmaolaeacabmiahmaaaaamablmaolabaaaaaaaaaaaaaaaaaaaaaaaaaaaa
"
}

SubProgram "ps3 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_SCREEN" }
Vector 0 [_LightColor0]
Float 1 [_Shininess]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_BumpMap] 2D
SetTexture 2 [_ShadowMapTexture] 2D
"sce_fp_rsx // 33 instructions using 3 registers
[Configuration]
24
ffffffff0007c020001fffe1000000000000840003000000
[Offsets]
2
_LightColor0 2 0
000001d0000000a0
_Shininess 1 0
00000120
[Microcode]
528
94001702c8011c9dc8000001c8003fe106800440ce001c9d00020000aa020000
000040000000bf80000000000000000008800240ab001c9cab000000c8000001
0880044001001c9e01000000c90000039e021700c8011c9dc8000001c8003fe1
08800340c9001c9d00020000c800000100003f80000000000000000000000000
08803b40c9003c9d55000001c80000010e820240c8041c9dc8020001c8000001
00000000000000000000000000000000d0820540c9001c9dc8010001c8003fe1
10820900c9041c9d00020000c800000100000000000000000000000000000000
b0800540c9001c9dc8010001c8003fe102880900ff001c9daa020000c8000001
000000000000000000000000000000001080014000021c9cc8000001c8000001
00000000000000000000000000000000028a0240ff001c9d00020000c8000001
0000430000000000000000000000000002881d00c9101c9dc8000001c8000001
1080020001101c9c01140000c80000010e820240c9041c9dff040001c8000001
02801c40ff001c9dc8000001c80000011080024001001c9cc8040001c8000001
ee800240c8041c9dc8015001c8003fe102021805c8011c9dc8000001c8003fe1
0e820440ff001c9dc8020001c904000100000000000000000000000000000000
10800140c8021c9dc8000001c800000100000000000000000000000000000000
0e81044000041c9cc9041001c9000001
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "SHADOWS_SCREEN" }
"!!GLES"
}

}
        }

#LINE 51

}

FallBack "Mobile/VertexLit"
}