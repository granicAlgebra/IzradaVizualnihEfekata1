// =====================================================================
//  IZRADA VIZUALNIH EFEKATA 1 – Vježbe 2
//  Efekt VFX_Udar: prsten naboja -> bljesak -> iskre -> dim
//
//  KAKO KORISTITI
//  1. Spremi ovu datoteku u:  Assets/_VFX1/Editor/VFX_Udar_Builder.cs
//     (mora biti u mapi "Editor")
//  2. U Unityju: izbornik  VFX1 -> Kreiraj VFX_Udar
//     (napredno: VFX1 -> Kreiraj VFX_Udar_Stil)
//
//  Skripta napravi:
//   - teksture T_SoftDot, T_Dim, T_Prsten u Assets/_VFX1/Textures/
//     (ako već postoje, koristi tvoje i samo im postavi import postavke)
//   - materijale M_Iskre (Additive), M_Dim (Alpha), M_Prsten (Additive)
//   - hijerarhiju VFX_Udar s 4 sloja i timingom preko Start Delay
//   - prefab u Assets/_VFX1/Effects/
// =====================================================================
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class VFX_Udar_Builder
{
    const string RootDir = "Assets/_VFX1";
    const string TexDir  = RootDir + "/Textures";
    const string MatDir  = RootDir + "/Materials";
    const string FxDir   = RootDir + "/Effects";

    enum BlendType { Alpha, Additive }

    struct Mats { public Material Iskre, Dim, Prsten; }

    // =================================================================
    //  VFX_Udar – realistični slojevi (iskre, dim) + stilizirani prsten
    // =================================================================
    [MenuItem("VFX1/Kreiraj VFX_Udar")]
    public static void BuildUdar()
    {
        Mats m = PripremiAssete();

        // Root: Emission OFF, Renderer OFF, Looping OFF – samo drži grupu
        GameObject root = NoviRoot("VFX_Udar", 3.2f);

        // ---- 1) PRSTEN NABOJA – anticipacija 0.0 – 0.6 s ---------------
        {
            var ps = NoviSloj(root, "Prsten", m.Prsten, 1, 0f);
            var main = ps.main;
            main.duration      = 0.6f;
            main.startLifetime = 0.4f;
            main.startSpeed    = 0f;
            main.startSize     = 3f;
            main.startColor    = Hex("#3FE0FF");             // zasićeni cijan

            Burst(ps, 1, 3, 0.1f);                           // 3 prstena, razmak 0.1 s
            BezShapea(ps);                                   // sve iz središta

            // 1 -> 0, ease-in (sporo pa brzo) = "usisavanje" energije
            SizeOL(ps, new AnimationCurve(new Keyframe(0f, 1f, 0f, 0f),
                                          new Keyframe(1f, 0f, -2f, -2f)));
            ColorOL(ps, Jednobojno(Color.white),
                    A(0f, 0f), A(1f, 0.2f), A(1f, 0.8f), A(0f, 1f));
        }

        // ---- 2) BLJESAK – najkraći sloj, 0.6 – 0.7 s -------------------
        {
            var ps = NoviSloj(root, "Bljesak", m.Iskre, 3, 0.6f);
            var main = ps.main;
            main.duration      = 0.1f;
            main.startLifetime = 0.1f;
            main.startSpeed    = 0f;
            main.startSize     = 2.5f;
            main.startColor    = new Color(1f, 1f, 0.85f);   // bijela / blijedo žuta

            Burst(ps, 1);
            BezShapea(ps);
            SizeOL(ps, Linear(0.6f, 1f));
            ColorOL(ps, Jednobojno(Color.white), A(1f, 0f), A(0f, 1f));
        }

        // ---- 3) ISKRE – 0.6 – ~1.2 s -----------------------------------
        {
            var ps = NoviSloj(root, "Iskre", m.Iskre, 2, 0.6f);
            var main = ps.main;
            main.duration        = 1f;
            main.startLifetime   = R(0.3f, 0.6f);
            main.startSpeed      = R(6f, 12f);
            main.startSize       = R(0.03f, 0.06f);
            main.gravityModifier = 1f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            Burst(ps, 40);
            Sfera(ps, 0.05f);

            ColorOL(ps,
                new[] {
                    C(Color.white,                  0f),
                    C(new Color(1f, 0.85f, 0.2f),   0.4f),   // žuta
                    C(new Color(1f, 0.45f, 0.05f),  1f)      // narančasta
                },
                A(1f, 0f), A(0f, 1f));

            // Napredno (isključeno): uključi kvačicu u Inspectoru da iskre usporavaju
            var lv = ps.limitVelocityOverLifetime;
            lv.limit  = 1f;
            lv.dampen = 0.1f;
            lv.enabled = false;

            Stretch(ps, 0.03f);
        }

        // ---- 4) DIM – 0.65 – ~3.15 s -----------------------------------
        {
            var ps = NoviSloj(root, "Dim", m.Dim, 0, 0.65f);
            var main = ps.main;
            main.duration        = 2f;
            main.startLifetime   = R(1.5f, 2.5f);
            main.startSpeed      = R(0.3f, 1.2f);
            main.startSize       = R(0.8f, 1.4f);
            main.startRotation   = R(0f, 360f * Mathf.Deg2Rad);   // API je u radijanima
            main.gravityModifier = -0.05f;                         // lagano se diže
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startColor      = Hex("#808080");

            Burst(ps, 10);
            Hemisfera(ps, 0.3f);

            SizeOL(ps, Linear(0.5f, 1f));                          // raste
            ColorOL(ps, Jednobojno(Color.white),
                    A(0f, 0f), A(180f / 255f, 0.1f), A(0f, 1f));

            var rot = ps.rotationOverLifetime;
            rot.enabled = true;
            rot.z = R(-20f * Mathf.Deg2Rad, 20f * Mathf.Deg2Rad);
        }

        SpremiPrefab(root);
    }

    // =================================================================
    //  NAPREDNO: VFX_Udar_Stil – cijeli efekt u stiliziranom jeziku
    //  paleta: cijan / magenta / bijela (+ tamnoljubičasti dim)
    //  timing: dulja anticipacija (0.9 s), brža akcija, kraći rezidual
    //  oblici: prstenovi + horizontalni udarni val, manje čestica
    // =================================================================
    [MenuItem("VFX1/Kreiraj VFX_Udar_Stil (napredno)")]
    public static void BuildStil()
    {
        Mats m = PripremiAssete();
        Color cijan      = Hex("#3FE0FF");
        Color magenta    = Hex("#FF2E9A");
        Color ljubicasta = Hex("#3A1F5C");
        const float udar = 0.9f;                       // trenutak akcije

        GameObject root = NoviRoot("VFX_Udar_Stil", 2.2f);

        // Prsten naboja: 4 prstena, oštriji ease-in, 0.0 – 0.9 s
        {
            var ps = NoviSloj(root, "Prsten", m.Prsten, 1, 0f);
            var main = ps.main;
            main.duration      = udar;
            main.startLifetime = 0.45f;
            main.startSpeed    = 0f;
            main.startSize     = 3.5f;
            main.startColor    = cijan;

            Burst(ps, 1, 4, 0.15f);
            BezShapea(ps);
            SizeOL(ps, new AnimationCurve(new Keyframe(0f, 1f, 0f, 0f),
                                          new Keyframe(1f, 0f, -3f, -3f)));
            ColorOL(ps, Jednobojno(Color.white),
                    A(0f, 0f), A(1f, 0.15f), A(1f, 0.85f), A(0f, 1f));
        }

        // Bljesak: samo 0.06 s
        {
            var ps = NoviSloj(root, "Bljesak", m.Iskre, 4, udar);
            var main = ps.main;
            main.duration      = 0.06f;
            main.startLifetime = 0.06f;
            main.startSpeed    = 0f;
            main.startSize     = 3f;
            main.startColor    = Color.white;

            Burst(ps, 1);
            BezShapea(ps);
            SizeOL(ps, Linear(0.8f, 1.2f));
            ColorOL(ps, Jednobojno(Color.white), A(1f, 0f), A(0f, 1f));
        }

        // Udarni val: magenta prsten koji se širi po tlu (ease-out)
        {
            var ps = NoviSloj(root, "UdarniVal", m.Prsten, 2, udar);
            var main = ps.main;
            main.duration      = 0.3f;
            main.startLifetime = 0.3f;
            main.startSpeed    = 0f;
            main.startSize     = 4f;
            main.startColor    = magenta;

            Burst(ps, 1);
            BezShapea(ps);
            SizeOL(ps, new AnimationCurve(new Keyframe(0f, 0f, 3f, 3f),
                                          new Keyframe(1f, 1f, 0f, 0f)));
            ColorOL(ps, Jednobojno(Color.white), A(1f, 0f), A(1f, 0.4f), A(0f, 1f));
            ps.GetComponent<ParticleSystemRenderer>().renderMode =
                ParticleSystemRenderMode.HorizontalBillboard;
        }

        // Iskre: manje, deblje, brzo pa naglo stanu; stepenasta paleta
        {
            var ps = NoviSloj(root, "Iskre", m.Iskre, 3, udar);
            var main = ps.main;
            main.duration        = 0.6f;
            main.startLifetime   = R(0.25f, 0.4f);
            main.startSpeed      = R(10f, 16f);
            main.startSize       = R(0.08f, 0.12f);
            main.gravityModifier = 0.5f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            Burst(ps, 16);
            Sfera(ps, 0.05f);
            ColorOL(ps,
                new[] {
                    C(Color.white, 0f),  C(Color.white, 0.2f),
                    C(cijan, 0.25f),     C(cijan, 0.6f),
                    C(magenta, 0.65f),   C(magenta, 1f)
                },
                A(1f, 0f), A(1f, 0.8f), A(0f, 1f));

            var lv = ps.limitVelocityOverLifetime;
            lv.enabled = true;
            lv.limit   = 2f;
            lv.dampen  = 0.15f;

            Stretch(ps, 0.04f);
        }

        // Dim: malo velikih, "grafičkih" oblačića, kraći rezidual
        {
            var ps = NoviSloj(root, "Dim", m.Dim, 0, udar + 0.05f);
            var main = ps.main;
            main.duration        = 1f;
            main.startLifetime   = R(0.7f, 1f);
            main.startSpeed      = R(0.6f, 1.2f);
            main.startSize       = R(1f, 1.4f);
            main.startRotation   = R(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = -0.1f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startColor      = ljubicasta;

            Burst(ps, 6);
            Hemisfera(ps, 0.3f);
            SizeOL(ps, new AnimationCurve(new Keyframe(0f, 0.4f, 2f, 2f),
                                          new Keyframe(1f, 1f, 0f, 0f)));
            ColorOL(ps, Jednobojno(Color.white),
                    A(0f, 0f), A(0.9f, 0.1f), A(0.9f, 0.5f), A(0f, 1f));

            var rot = ps.rotationOverLifetime;
            rot.enabled = true;
            rot.z = R(-45f * Mathf.Deg2Rad, 45f * Mathf.Deg2Rad);
        }

        SpremiPrefab(root);
    }

    // =================================================================
    //  Hijerarhija i prefab
    // =================================================================
    static GameObject NoviRoot(string name, float duration)
    {
        var go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Kreiraj " + name);

        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration    = duration;
        main.loop        = false;
        main.playOnAwake = true;

        var em = ps.emission; em.enabled = false;
        var sh = ps.shape;    sh.enabled = false;
        go.GetComponent<ParticleSystemRenderer>().enabled = false;
        return go;
    }

    static ParticleSystem NoviSloj(GameObject root, string name, Material mat, int orderInLayer, float startDelay)
    {
        var go = new GameObject(name);
        go.transform.SetParent(root.transform, false);   // lokalno 0,0,0
        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.loop        = false;                         // udar se ne ponavlja
        main.playOnAwake = true;
        main.startDelay  = startDelay;                    // timing sloja

        var r = go.GetComponent<ParticleSystemRenderer>();
        r.renderMode      = ParticleSystemRenderMode.Billboard;
        r.sharedMaterial  = mat;
        r.sortingOrder    = orderInLayer;                 // dim 0, iskre više
        r.maxParticleSize = 3f;                           // da se veliki bljesak ne "reže" izbliza
        return ps;
    }

    static void SpremiPrefab(GameObject root)
    {
        string path = $"{FxDir}/{root.name}.prefab";
        bool postoji = AssetDatabase.LoadAssetAtPath<GameObject>(path) != null;
        if (postoji && !EditorUtility.DisplayDialog("VFX1",
                $"{path} već postoji.\nŽeliš li ga prepisati?", "Prepiši", "Ne (samo u sceni)"))
        {
            Odaberi(root);
            return;
        }
        PrefabUtility.SaveAsPrefabAssetAndConnect(root, path, InteractionMode.UserAction);
        AssetDatabase.SaveAssets();
        Odaberi(root);
        Debug.Log($"[VFX1] {root.name} gotov → {path}");
    }

    static void Odaberi(GameObject go)
    {
        Selection.activeGameObject = go;
        if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.FrameSelected();
    }

    // =================================================================
    //  Moduli – pomoćne funkcije
    // =================================================================
    static void Burst(ParticleSystem ps, short count, int cycles = 1, float interval = 0.01f)
    {
        var em = ps.emission;
        em.enabled = true;
        em.rateOverTime = 0f;                             // udar = Rate 0 + burst
        em.rateOverDistance = 0f;
        em.SetBursts(new[] { new ParticleSystem.Burst(0f, count, count, cycles, interval) });
    }

    static void BezShapea(ParticleSystem ps) { var s = ps.shape; s.enabled = false; }

    static void Sfera(ParticleSystem ps, float radius)
    {
        var s = ps.shape;
        s.enabled = true;
        s.shapeType = ParticleSystemShapeType.Sphere;
        s.radius = radius;
    }

    static void Hemisfera(ParticleSystem ps, float radius)
    {
        var s = ps.shape;
        s.enabled = true;
        s.shapeType = ParticleSystemShapeType.Hemisphere;
        s.radius = radius;
        s.rotation = new Vector3(-90f, 0f, 0f);           // polukugla gleda prema gore
    }

    static void Stretch(ParticleSystem ps, float speedScale)
    {
        var r = ps.GetComponent<ParticleSystemRenderer>();
        r.renderMode    = ParticleSystemRenderMode.Stretch;
        r.velocityScale = speedScale;                     // "Speed Scale" u Inspectoru
        r.lengthScale   = 2f;
    }

    static void SizeOL(ParticleSystem ps, AnimationCurve c)
    {
        var s = ps.sizeOverLifetime;
        s.enabled = true;
        s.size = new ParticleSystem.MinMaxCurve(1f, c);   // krivulja množi Start Size
    }

    static void ColorOL(ParticleSystem ps, GradientColorKey[] boje, params GradientAlphaKey[] alfe)
    {
        var g = new Gradient();
        g.SetKeys(boje, alfe);
        var col = ps.colorOverLifetime;
        col.enabled = true;
        col.color = new ParticleSystem.MinMaxGradient(g);
    }

    static ParticleSystem.MinMaxCurve R(float a, float b) => new ParticleSystem.MinMaxCurve(a, b);
    static AnimationCurve Linear(float a, float b) => AnimationCurve.Linear(0f, a, 1f, b);
    static GradientColorKey C(Color c, float t) => new GradientColorKey(c, t);
    static GradientAlphaKey A(float a, float t) => new GradientAlphaKey(a, t);
    static GradientColorKey[] Jednobojno(Color c) => new[] { C(c, 0f), C(c, 1f) };

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color c);
        return c;
    }

    // =================================================================
    //  Mape, teksture, materijali
    // =================================================================
    static Mats PripremiAssete()
    {
        Mapa(RootDir); Mapa(TexDir); Mapa(MatDir); Mapa(FxDir);

        Texture2D tSoft   = Tekstura("T_SoftDot", TexSoftDot);
        Texture2D tDim    = Tekstura("T_Dim",     TexDim);
        Texture2D tPrsten = Tekstura("T_Prsten",  TexPrsten);

        return new Mats
        {
            Iskre  = Materijal("M_Iskre",  tSoft,   BlendType.Additive),
            Dim    = Materijal("M_Dim",    tDim,    BlendType.Alpha),
            Prsten = Materijal("M_Prsten", tPrsten, BlendType.Additive)
        };
    }

    static void Mapa(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        Mapa(parent);
        AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
    }

    static string NadjiAsset(string name, string type, string folder)
    {
        foreach (string guid in AssetDatabase.FindAssets($"{name} t:{type}", new[] { folder }))
        {
            string p = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(p) == name) return p;
        }
        return null;
    }

    // Ako tekstura ne postoji, generira se bijela tekstura s alfom (256×256).
    // U oba slučaja postavljaju se import postavke sa slajda "Zadane teksture".
    static Texture2D Tekstura(string name, System.Func<float, float, float> alpha)
    {
        string path = NadjiAsset(name, "Texture2D", TexDir);
        if (path == null)
        {
            path = $"{TexDir}/{name}.png";
            const int S = 256;
            var tex = new Texture2D(S, S, TextureFormat.RGBA32, false);
            var px = new Color32[S * S];
            for (int y = 0; y < S; y++)
            for (int x = 0; x < S; x++)
            {
                float u = (x + 0.5f) / S * 2f - 1f;
                float v = (y + 0.5f) / S * 2f - 1f;
                byte a = (byte)Mathf.RoundToInt(Mathf.Clamp01(alpha(u, v)) * 255f);
                px[y * S + x] = new Color32(255, 255, 255, a);
            }
            tex.SetPixels32(px);
            tex.Apply();
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
        }

        var imp = (TextureImporter)AssetImporter.GetAtPath(path);
        imp.textureType         = TextureImporterType.Default;
        imp.alphaSource         = TextureImporterAlphaSource.FromInput;   // Input Texture Alpha
        imp.alphaIsTransparency = true;
        imp.wrapMode            = TextureWrapMode.Clamp;
        imp.maxTextureSize      = 256;
        imp.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    static float Smooth(float e0, float e1, float x)
    {
        float t = Mathf.Clamp01((x - e0) / (e1 - e0));
        return t * t * (3f - 2f * t);
    }

    static float TexSoftDot(float u, float v)
    {
        float t = Mathf.Clamp01(1f - Mathf.Sqrt(u * u + v * v));
        return t * t;
    }

    static float TexPrsten(float u, float v)
    {
        float d = (Mathf.Sqrt(u * u + v * v) - 0.78f) / 0.07f;
        return Mathf.Exp(-d * d);
    }

    static float TexDim(float u, float v)
    {
        float r = Mathf.Sqrt(u * u + v * v);
        float n = 0f, amp = 0.5f, f = 2.5f;
        for (int i = 0; i < 5; i++)
        {
            n += amp * Mathf.PerlinNoise(u * f + 31.7f, v * f + 12.3f);
            amp *= 0.5f;
            f *= 2f;
        }
        float rub = 1f - Smooth(0.45f, 0.95f, r + (n - 0.48f) * 0.6f);
        return Mathf.Clamp01(rub * (0.45f + 0.8f * n));
    }

    // URP / Particles / Unlit · Transparent · Additive ili Alpha
    static Material Materijal(string name, Texture2D tex, BlendType blend)
    {
        string path = $"{MatDir}/{name}.mat";
        bool add = blend == BlendType.Additive;

        Shader urp = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        Shader sh = urp != null
            ? urp
            : Shader.Find(add ? "Legacy Shaders/Particles/Additive"
                              : "Legacy Shaders/Particles/Alpha Blended");   // rezerva bez URP-a

        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null) { mat = new Material(sh); AssetDatabase.CreateAsset(mat, path); }
        else mat.shader = sh;

        if (urp != null)
        {
            mat.SetTexture("_BaseMap", tex);
            mat.SetColor("_BaseColor", Color.white);
            mat.SetFloat("_Surface", 1f);                         // Transparent
            mat.SetFloat("_Blend", add ? 2f : 0f);                // 0 Alpha, 2 Additive
            mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            mat.SetFloat("_DstBlend", (float)(add ? BlendMode.One : BlendMode.OneMinusSrcAlpha));
            mat.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
            mat.SetFloat("_DstBlendAlpha", (float)(add ? BlendMode.One : BlendMode.OneMinusSrcAlpha));
            mat.SetFloat("_ZWrite", 0f);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.DisableKeyword("_ALPHAMODULATE_ON");
            mat.renderQueue = (int)RenderQueue.Transparent;
        }
        else
        {
            mat.SetTexture("_MainTex", tex);
        }

        EditorUtility.SetDirty(mat);
        return mat;
    }
}
