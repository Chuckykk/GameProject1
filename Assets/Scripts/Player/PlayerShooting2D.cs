using UnityEngine;

[DisallowMultipleComponent]
public class PlayerShooting2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;      // Där kulan skjuts från
    [SerializeField] private GameObject bulletPrefab;  // Prefab att skjuta

    [Header("Shooting Settings")]
    [SerializeField] public float bulletSpeed = 12f;  // Hur snabbt kulorna flyger
    [SerializeField] public float fireRate = 0.2f;    // Tid mellan skott (0.2 = 5 skott/sek)
    [SerializeField] public int damage = 1;           // Grundskada per skott
    [SerializeField] private bool autoFire = true;     // Håll in musknappen för automatisk eld

    private float _nextFireTime;

    // === Publica egenskaper (så UpgradeOption kan ändra dessa) ===
    public float FireRate
    {
        get => fireRate;
        set => fireRate = Mathf.Clamp(value, 0.02f, 2f); // skydda mot för låga värden
    }

    public int Damage
    {
        get => damage;
        set => damage = Mathf.Max(0, value);
    }

    private void Update()
    {
        bool firePressed = autoFire ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (firePressed && Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (firePoint == null || bulletPrefab == null) return;

        // Skapa kulan
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Spela ljud
        AudioManager.Play("shoot");

        // Skicka iväg kulan i rätt riktning
        if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
            rb.velocity = firePoint.right * bulletSpeed;

        // --- Valfritt ---
        // Om du senare vill ge kulor damage, lägg till ett script "PlayerBullet2D"
        // och avkommentera dessa rader:
        //
        // if (bullet.TryGetComponent<PlayerBullet2D>(out var proj))
        //     proj.damage = damage;
    }
}
