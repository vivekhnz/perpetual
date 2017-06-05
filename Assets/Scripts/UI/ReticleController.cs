using UnityEngine;

public class ReticleController : MonoBehaviour
{
    public Texture2D ReticleTexture;

    void Start()
    {
        ShowReticle();
    }

    public void ShowCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void ShowReticle()
    {
        Cursor.SetCursor(ReticleTexture, new Vector2(0.5f, 0.5f),
            CursorMode.ForceSoftware);
    }

    void OnDestroy()
    {
        ShowCursor();
    }
}