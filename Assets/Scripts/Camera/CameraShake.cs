using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float BaseShake = 0.01f;

    public void RandomShake(float amount)
    {
        transform.Translate(
            Mathf.Sign(Random.Range(-1, 1)) * BaseShake * amount,
            Mathf.Sign(Random.Range(-1, 1)) * BaseShake * amount, 0);
    }
}
