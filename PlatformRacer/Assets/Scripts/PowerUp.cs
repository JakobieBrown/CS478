using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpEffect effect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
        effect.Apply(collision.gameObject);
    }
}