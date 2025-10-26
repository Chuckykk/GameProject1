using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyFlashOnHit : MonoBehaviour
{
    [Header("Flash settings")]
    public Color flashColor = Color.red;      // färgen vid träff
    public float flashDuration = 0.1f;          // hur länge blinkar
    public bool useMaterialInstance = true;     // skapa kopia av materialet

    private SpriteRenderer _sr;
    private Color _originalColor;
    private Material _materialInstance;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;

        if (useMaterialInstance)
        {
            _materialInstance = Instantiate(_sr.material);
            _sr.material = _materialInstance;
        }
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        _sr.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        _sr.color = _originalColor;
    }
}
