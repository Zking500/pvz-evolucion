using UnityEngine;

public class SphereProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 25f;
    public float lifeTime = 3f;
    public bool destroyOnHit = true;
    
    [Header("Effects")]
    public GameObject hitEffect;
    public AudioClip hitSound;
    
    private AudioSource audioSource;
    private float spawnTime;
    
    void Start()
    {
        spawnTime = Time.time;
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Auto-destruir después del tiempo de vida
        Destroy(gameObject, lifeTime);
    }
    
    void Update()
    {
        // Verificar tiempo de vida
        if (Time.time - spawnTime >= lifeTime)
        {
            DestroyProjectile();
            return;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // No dañar al jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        
        // Aplicar daño si el objeto tiene un componente de salud
        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }
        
        // Efectos de impacto
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, collision.contacts[0].point, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        // Sonido de impacto
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
        
        // Destruir el proyectil
        if (destroyOnHit)
        {
            DestroyProjectile();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Manejar triggers si es necesario
        if (other.CompareTag("Enemy"))
        {
            Health targetHealth = other.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
            
            if (destroyOnHit)
            {
                DestroyProjectile();
            }
        }
    }
    
    void DestroyProjectile()
    {
        // Desactivar el renderizador y colisionador
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
        
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        // Destruir después de un pequeño retraso para permitir efectos
        Destroy(gameObject, 0.1f);
    }
    
    // Método para establecer el daño desde scripts externos
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
    
    // Método para establecer el tiempo de vida
    public void SetLifeTime(float newLifeTime)
    {
        lifeTime = newLifeTime;
        // Reiniciar el temporizador de destrucción
        CancelInvoke("DestroyProjectile");
        Destroy(gameObject, lifeTime);
    }
}