using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScorePopupController : MonoBehaviour
{
    public float Speed = 0.1f;
    private Vector2 velocity;

    public void Initialize(string text, Vector3 position, Vector2 direction)
    {
        GetComponent<Text>().text = text;
        transform.position = position;
        transform.localScale = new Vector3(1, 1, 1);
        this.velocity = direction * Speed;
    }

    void FixedUpdate()
    {
        transform.Translate(this.velocity);
        this.velocity *= 0.9f;
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
