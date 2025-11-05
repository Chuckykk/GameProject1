using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class XPOrb2D : MonoBehaviour
{
    [Header("XP-värde")]
    [Min(1)] public int xpValue = 1;

    [Header("Magnet (valfritt)")]
    public float magnetRange = 3f;     // hur nära spelaren orben börjar dras
    public float magnetSpeed = 6f;     // hastighet på draget

    private Transform player;
    private PlayerXP2D playerXP;

    private void Start()
    {
        // Hitta spelaren automatiskt via tag
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null)
        {
            player = pObj.transform;
            playerXP = pObj.GetComponent<PlayerXP2D>();
        }
        else
        {
            Debug.LogWarning("[XPOrb2D] Hittar ingen spelare med tag 'Player'.");
        }

        // Se till att collidern är trigger
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Om spelaren är inom magnetisk radie → dra orben mot spelaren
        if (distance <= magnetRange)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                magnetSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Om vi redan har referensen till PlayerXP2D, använd den
        if (playerXP != null)
        {
            playerXP.AddXP(xpValue);
        }
        else if (other.TryGetComponent(out PlayerXP2D xp))
        {
            xp.AddXP(xpValue);
        }

        Destroy(gameObject);
    }
}
