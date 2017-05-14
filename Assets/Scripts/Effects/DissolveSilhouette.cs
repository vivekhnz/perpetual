using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class DissolveSilhouette : MonoBehaviour
{
    public Color SilhouetteColor = Color.white;
    public float DissolveAmount = 0.0f;

    private SpriteRenderer spriteRenderer;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateDissolve();
    }

    void OnDisable()
    {
        UpdateDissolve();
    }

    void Update()
    {
        UpdateDissolve();
    }

    void UpdateDissolve()
    {
        var mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", SilhouetteColor);
        mpb.SetFloat("_DissolveAmount", DissolveAmount);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}
