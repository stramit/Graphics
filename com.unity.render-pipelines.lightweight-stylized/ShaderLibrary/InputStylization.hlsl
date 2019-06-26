#ifndef TOON_INPUT_STYLIZATION_INCLUDED
#define TOON_INPUT_STYLIZATION_INCLUDED

TEXTURE2D(_GlobalDirectLighting);       SAMPLER(sampler_GlobalDirectLighting);
TEXTURE2D(_LocalDirectLighting);        SAMPLER(sampler_LocalDirectLighting);

TEXTURE2D(_GlobalShadow);               SAMPLER(sampler_GlobalShadow);
TEXTURE2D(_LocalShadow);                SAMPLER(sampler_LocalShadow);

TEXTURE2D(_GlobalFog);                  SAMPLER(sampler_GlobalFog);
TEXTURE2D(_LocalFog);                   SAMPLER(sampler_LocalFog);

half4 _GlobalIndirectLighting;
half4 _LocalIndirectLighting;

half4 _GlobalRimlight;
half4 _LocalRimlight;

#endif // TOON_INPUT_STYLIZATION_INCLUDED
