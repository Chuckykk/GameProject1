using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;            // // Där kulan skjuts från (barn till AimPivot)
    [SerializeField] private GameObject bulletPrefab;        // // Prefab att skjuta (måste ha Rigidbody2D)
    
    [Header("Shooting Settings")]
    [SerializeField] private float bulletSpeed = 12f;        // // Hur snabbt kulorna flyger
    [SerializeField] private float fireRate = 0.2f;          // // Tid mellan skott (0.2 = 5 skott/sek)
    [SerializeField] private bool autoFire = true;           // // Håll in musknappen för automatisk eld

    private float _nextFireTime;                             // // Timer för skottintervall
    private Camera _cam;                                     // // För att läsa musposition

    private void Awake()
    {
        _cam = Camera.main;                                 // // Hämta kamera för säkerhets skull
    }

    private void Update()
    {
        // // Vänster musknapp skjuter
        bool firePressed = autoFire ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        // // Kontrollera cooldown
        if (firePressed && Time.time >= _nextFireTime)
        {
            Shoot();                                         // // Skjut kula
            _nextFireTime = Time.time + fireRate;            // // Starta om cooldown
        }
    }

    private void Shoot()
    {
        if (firePoint == null || bulletPrefab == null) return;

        // // Skapa kula på firePoint-position med samma rotation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // // Hämta rigidbody
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // // Skicka iväg den i riktningen firePoint pekar
            rb.velocity = firePoint.right * bulletSpeed;
        }
    }
}
