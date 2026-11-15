using UnityEngine;

public class DissolveTest : MonoBehaviour
{
    [SerializeField] string _materialPropertyName = "";
    [SerializeField] float _duration = 2f;

    private Material _material;

    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        _material.SetFloat(_materialPropertyName, Mathf.PingPong(Time.time / _duration, 1f));
    }
}
