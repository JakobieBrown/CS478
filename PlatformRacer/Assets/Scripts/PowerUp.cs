using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpEffect effect;
    public AudioClip pickupSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        effect.Apply(collision.gameObject);
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
        Destroy(gameObject);
    }
}