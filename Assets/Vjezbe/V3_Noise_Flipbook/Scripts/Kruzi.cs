using UnityEngine;

// Referenca za V3, slajd 12. U namespaceu da se ne sudari s tvojim Kruzi.cs.
namespace Referenca
{
    public class Kruzi : MonoBehaviour
    {
        [SerializeField] float polumjer = 1.5f;
        [SerializeField] float brzina = 3f;     // radijana u sekundi
        Vector3 centar;

        void Start() => centar = transform.position;

        void Update()
        {
            float a = Time.time * brzina;
            transform.position = centar + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * polumjer;
        }
    }
}
