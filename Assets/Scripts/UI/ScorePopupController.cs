using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScorePopupController : MonoBehaviour
{
    private Vector2 velocity;

    public void Initialize(int score, Vector3 position, Vector2 velocity)
    {
        GetComponent<Text>().text = $"+{score}";
        transform.position = position;
        transform.localScale = new Vector3(1, 1, 1);
        this.velocity = velocity;
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
