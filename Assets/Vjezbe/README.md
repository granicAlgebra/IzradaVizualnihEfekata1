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

`Assets/_Arhiva/` - neiskoristene i duplicirane teksture/materijali (mogu se obrisati).
