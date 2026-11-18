# VFX1 - referentni projekt za vjezbe

Svaka vjezba ima svoju mapu u `Assets/Vjezbe/` s podmapama Scenes, Effects, Materials, Textures, Shaders, Scripts, Editor (samo one koje trebaju).
Zajednicki dio (zidovi, pod, Ref_Covjek) je u `Assets/Zajednicko/`, URP postavke u `Assets/Settings/`.
Svaka vjezba moze koristiti assete iz prethodnih (kao i studenti).

| Vjezba | Scena | Sadrzaj |
|---|---|---|
| V1 | V1_Lab/Scenes/VFX_Lab_V1 | Lab: tamna i svijetla strana, Ref_Covjek 1,8 m, testni Particle System (VFX_Test) |
| V2 | V2_Cestice/Scenes/VFX_Lab_V2 | VFX_Udar (prsten, bljesak, iskre, dim), VFX_Udar_Stil; M_Iskre (Additive), M_Dim (Alpha), M_Prsten; Editor/VFX_Udar_Builder (izbornik VFX1) |
| V3 | V3_Noise_Flipbook/Scenes/VFX_Lab_V3 | VFX_Udar_V3 (Noise na dimu, Fireball flipbook, Trails, Iskre_Raspad sub-emitter), VFX_Udar_V3_Referenca; V3_Demo_Tragovi: Trail Renderer (Kruzi.cs) i Line Renderer (munja) |
| V4 | V4_Teksture/Scenes/VFX_Lab_V4 | sRGB rampa ON/OFF, usporedba flipbookova 2x2 / 4x4 / 4x4 256px / 8x8, T_Maske_RGB po kanalima (VFX1_MaskaKanal.shader), VFX_Udar_V4 (Bljesak s T_Odsjaj), VFX_Udar_Stil_V4 (Udarni_Val_Puf, T_Puf_Stil 4x2) |
| V5 | V5_ShaderGraph/Scenes/VFX_Lab_V5 | SG_Magija + M_Magija na quadu i na cesticama (VFX_Magija) |
| V6 | V6_Dissolve_Fresnel/Scenes/VFX_Lab_V6 | SG_Vjezbalica (Step/Smoothstep/One Minus/Remap), SG_Dissolve (Lit, Alpha Clip) s M_Dissolve_Teleport i M_Dissolve_Izgaranje + PingPongSvojstvo.cs, SG_Stit (Fresnel + vertex val), SG_Portal (polarne koordinate) |
| V7 | V7_Dubina_Distorzija/Scenes/VFX_Lab_V7 | Soft Particles OFF/ON (VFX_Dim_Test, M_Dim_Soft), SG_Presjek (sfera u podu), SG_Distorzija + VFX_Udarni_Val ispred T_Mreza, SG_Magija_V7 + VFX_Magija_V7 (StableRandom.x u UV0.z), VFX_Udar_V7 (soft dim + Udarni_Val) |
| V8 | V8_VFXGraph/Scenes/VFX_Lab_V8 | VFX Graph (paket com.unity.visualeffectgraph): VFX_Roj_Iskri (2000/s, exposed Broj Iskri / Boja Iskri / Snaga Turbulencije) vs PS_Roj_Iskri (CPU), VFX_Udar_GPU (burst 3000, Collide with Plane, Orient Along Velocity), VFX_Udar_V8 hibrid (PS udar + Iskre_GPU, PonavljajUdar.cs), dodatak: VFX_Energetska_Kugla (Conform to Sphere + Turbulence), VFX_Roj_Magija (SG_Magija_VFX, Support VFX Graph, ParticleColor/ParticleAlpha iz atributa) |

`Assets/_Arhiva/` - neiskoristene i duplicirane teksture/materijali (mogu se obrisati).
