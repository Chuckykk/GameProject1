using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CharacterMovement2D : MonoBehaviour
{
    [Header("Movement")]                                         
    [SerializeField] private float moveSpeed = 7.5f;
    [SerializeField] private bool useRawInput = true;

    [Header("Aiming")]
    [SerializeField] private Transform aimPivot = null;
    [SerializeField] private bool rotateWholeBody = false;
    [SerializeField] private float aimOffsetDegrees = -90f;

    [Header("Visual")]
    [SerializeField] private Transform spriteRoot = null;
    [SerializeField] private bool flipOnMouseX = true;

    private Rigidbody2D _rb;
    private Collider2D _col;
    private Camera _cam;
    private Vector2 _moveInput;
    private bool _canMove = true;

    public Vector2 MoveInput => _moveInput;
    public float Speed => moveSpeed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _cam = Camera.main;

        EnsurePhysicsSetup();
    }

    private void EnsurePhysicsSetup()
    {
        // MÅSTE vara Dynamic för att blockeras av statiska väggar
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.gravityScale = 0f;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rb.freezeRotation = true;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Collidern får inte vara trigger
        _col.isTrigger = false;
    }

    private void Update()
    {
        if (_canMove)
            ReadMoveInput();

        UpdateAim();
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;

        Vector2 delta = _moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + delta);
    }

    private void ReadMoveInput()
    {
        float x = useRawInput ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
        float y = useRawInput ? Input.GetAxisRaw("Vertical")   : Input.GetAxis("Vertical");
        _moveInput = new Vector2(x, y);
        if (_moveInput.sqrMagnitude < 0.0001f) _moveInput = Vector2.zero;
    }

    private void UpdateAim()
    {
        if (_cam == null) _cam = Camera.main;
        if (_cam == null) return;

        Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 from = _rb != null ? _rb.position : (Vector2)transform.position;
        Vector2 dir = (Vector2)(mouseWorld - (Vector3)from);
        if (dir.sqrMagnitude < 1e-8f) return;

        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + aimOffsetDegrees;

        if (rotateWholeBody) transform.rotation = Quaternion.Euler(0f, 0f, ang);
        else if (aimPivot != null) aimPivot.rotation = Quaternion.Euler(0f, 0f, ang);

        if (flipOnMouseX && spriteRoot != null)
        {
            Vector3 s = spriteRoot.localScale;
            s.x = Mathf.Abs(s.x) * (dir.x >= 0f ? 1f : -1f);
            spriteRoot.localScale = s;
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        _canMove = enabled;
        if (!enabled) _moveInput = Vector2.zero;
    }

    private void OnValidate()
    {
        if (moveSpeed < 0f) moveSpeed = 0f;
        // Gör editor-självläkning också
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (_col == null) _col = GetComponent<Collider2D>();
        if (_rb != null && _col != null) EnsurePhysicsSetup();
    }
}
