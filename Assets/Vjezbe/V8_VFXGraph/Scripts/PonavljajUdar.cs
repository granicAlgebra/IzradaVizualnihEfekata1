using UnityEngine;
using UnityEngine.VFX;

// Ponavlja udar (Particle System + VFX Graph) svakih N sekundi - za demonstraciju hibrida.
public class PonavljajUdar : MonoBehaviour
{
    public float interval = 3f;
    ParticleSystem ps;
    VisualEffect[] vfx;
    float t;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        vfx = GetComponentsInChildren<VisualEffect>(true);
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t < interval) return;
        t = 0f;
        if (ps) ps.Play(true);
        foreach (var v in vfx) v.Reinit();
    }
}
