using UnityEngine;

public class MaterialScriptExample : MonoBehaviour
{
    public float speed = 0.0f;
    public float alpha = 0.0f;
    Renderer renderer;
    Material material;
    void Start()
    {
        renderer = GetComponent<Renderer>();
        material = renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat("_Speed", speed);
        material.SetFloat("_Alpha", alpha);
    }
}
