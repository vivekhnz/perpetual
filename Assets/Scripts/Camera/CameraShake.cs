using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float BaseShake = 0.01f;

    public void DirectionalShake(Vector2 direction, float amount)
    {
        transform.Translate(
            direction.x * BaseShake * amount,
            direction.y * BaseShake * amount, 0);
    }

    public void RandomShake(float amount)
    {
        DirectionalShake(new Vector2(
            Mathf.Sign(Random.Range(-1, 1)),
            Mathf.Sign(Random.Range(-1, 1))), amount);
    }
}
