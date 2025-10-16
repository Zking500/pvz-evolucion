using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public bool destroyOnDeath = true;
    public GameObject deathEffect;
    
    [Header("Events")]
    public UnityEvent<float> onHealthChanged;
    public UnityEvent<float> onDamaged;
    public UnityEvent onDeath;
    
    private float currentHealth;
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth);
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        
        onHealthChanged?.Invoke(currentHealth);
        onDamaged?.Invoke(damage);
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        if (isDead || amount <= 0) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        onHealthChanged?.Invoke(currentHealth);
    }
    
    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
        onHealthChanged?.Invoke(currentHealth);
        
        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Efecto de muerte
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, transform.rotation);
            Destroy(effect, 3f);
        }
        
        // Invocar evento de muerte
        onDeath?.Invoke();
        
        // Destruir el objeto
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            // Desactivar componentes
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
            
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
            
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null) renderer.enabled = false;
        }
    }
    
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public bool IsAlive()
    {
        return !isDead;
    }
}