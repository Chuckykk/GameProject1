using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Damage & Lifetime")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 3f;

    [Header("Meta")]
    [Tooltip("Visas på dödsskärmen som 'from <killerName>'.")]
    public string killerName = "Enemy2";

    [Header("Behaviour")]
    [Tooltip("Förstör projektilen vid träff.")]
    [SerializeField] private bool destroyOnHit = true;

    private bool hasHit;               // skydd mot dubbelträffar
    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Säkerställ rätt fysikinställningar för en 2D-projektil:
        if (rb != null)
        {
            rb.gravityScale = 0f;      // ingen gravitation
            rb.freezeRotation = true;  // rotera inte
        }
        if (col != null)
        {
            col.isTrigger = true;      // vi använder OnTriggerEnter2D
        }
    }

    private void OnEnable()
    {
        // Nollställ flagga om du senare poolar projektiler
        hasHit = false;

        // Självdö efter en stund
        if (lifeTime > 0f)
            Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return; // redan registrerad träff

        // 1) Träffar spelare
        if (other.CompareTag("Player"))
        {
            hasHit = true;

            // Sätt vem som dödade
            DeathContext.KillerName = string.IsNullOrEmpty(killerName) ? "Enemy" : killerName;

            // Gör skadan
            var playerHealth = other.GetComponent<PlayerHealth2D>();
            if (playerHealth != null)
                playerHealth.TakeDamage(Mathf.Max(0, damage));

            if (destroyOnHit)
                Destroy(gameObject);

            return;
        }

        // 2) Träffar vägg/ban-geometri (byt/utöka taggar efter ditt projekt)
        if (other.CompareTag("Wall"))
        {
            if (destroyOnHit)
                Destroy(gameObject);
            return;
        }

        // 3) Ignorera egna fiender (vanligt om bullets föds nära sin ägare)
        if (other.CompareTag("Enemy"))
        {
            // Gör inget — låt projektilen flyga vidare
            return;
        }
    }

    // (Valfritt) Om kameran lämnar projektilen – städa upp
    private void OnBecameInvisible()
    {
        // Endast om du INTE poolar bullets
        Destroy(gameObject);
    }

    // Publika hjälpare (om du vill sätta detta vid spawn)
    public void SetDamage(int newDamage) => damage = Mathf.Max(0, newDamage);
    public void SetKillerName(string name) => killerName = name;
}
