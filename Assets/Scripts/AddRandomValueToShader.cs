using UnityEngine;
using Random = UnityEngine.Random;

public class AddRandomValueToShader : MonoBehaviour
{
    public MeshRenderer wavingMaterial;

    public float offset;

    void Start()
    {
        offset = Random.Range(999f, 5000f);
        wavingMaterial.material.SetFloat("_RandomOffset", offset);
        wavingMaterial.material.SetFloat("_RandomScale", Random.Range(0.5f, 5f));
        
        Destroy(this);
    }
}