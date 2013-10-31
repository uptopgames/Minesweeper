//Chrome Shader Optimised For Mobile Games//
Shader "NINJAPENGUINSTUDIOS/Metal/Chrome" 
{
   Properties 
   {
     _EnvMap("EnvMap", 2D) = "black" {TexGen SphereMap}
   }
    
   SubShader 
   {
     SeparateSpecular On
          Pass 
          {
              Name "BASE"
              ZWrite on
              Blend One One
              BindChannels {
              Bind "Vertex", vertex
              Bind "normal", normal
          }
    
            SetTexture [_EnvMap] 
            {
              combine texture
         }
      }
   } 
   Fallback "Mobile/Diffuse"
}