using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] 
public class CharacterMovement2D : MonoBehaviour
{
    [Header("Movement")]                                         
    [SerializeField] private float moveSpeed = 7.5f;             
    [SerializeField] private bool useRawInput = true;           

    [Header("Aiming")]                                           
    [SerializeField] private Transform aimPivot = null;          // // (Valfritt) vapenpivot som roteras mot musen
    [SerializeField] private bool rotateWholeBody = false;       // // Om true roteras hela spelaren
    [SerializeField] private float aimOffsetDegrees = -90f;      // // Kompensera sprite-riktning (t.ex. om "upp" är 90° fel)

    [Header("Visual")]                                           
    [SerializeField] private Transform spriteRoot = null;        
    [SerializeField] private bool flipOnMouseX = true;         

  
    private Rigidbody2D _rb;                                     // // Referens till Rigidbodyn (prestanda)
    private Camera _cam;                                         // // Huvudkamera för skärm->värld
    private Vector2 _moveInput;                                  // // Senaste input (Update)
    private bool _canMove = true;                                // // För att enkelt låsa rörelsen vid paus/game over

 
    public Vector2 MoveInput => _moveInput;                      // // Ger UI/animationer tillgång att läsa input
    public float Speed => moveSpeed;                             // // Exponera bara läsning av hastigheten

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();                       // // Cacha rigidbody
        _rb.freezeRotation = true;                               // // Lås Z-rotation för 2D
        _cam = Camera.main;                                      // // Cacha kamera
    }

    private void Update()
    {
        if (_canMove)                                            // // Läs input endast när vi får röra oss
            ReadMoveInput();                                     // // Tangentbordsinput (WASD/Arrow)

        UpdateAim();                                             // // Sikta/rotera (får gärna ske även när pausat)
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;                                   // // Ingen rörelse i paus/game over

        // // Direkt och snärtig rörelse: MovePosition (fysikvänligt) utan velocity/acceleration
        // // Normalisera så diagonal inte blir snabbare, multiplicera med hastighet och fixedDeltaTime
        Vector2 delta = _moveInput.normalized * moveSpeed * Time.fixedDeltaTime; 
        _rb.MovePosition(_rb.position + delta);                  // // Flytta kroppen ett steg
    }

    private void ReadMoveInput()
    {
        // // Välj mellan rå eller smoothead input – rå ger den snärtiga känslan
        float x = useRawInput ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
        float y = useRawInput ? Input.GetAxisRaw("Vertical")   : Input.GetAxis("Vertical");

        _moveInput = new Vector2(x, y);                          // // Spara inputvektor

        // // Klipp väldigt små värden (dödzon) så vi inte "kryper" vid analog input
        if (_moveInput.sqrMagnitude < 0.0001f)                   
            _moveInput = Vector2.zero;                           
    }

    private void UpdateAim()
    {
        // // Säkerställ kamera (om du bytt kameror i runtime)
        if (_cam == null) _cam = Camera.main;                    
        if (_cam == null) return;                                // // Utan kamera kan vi inte sikta korrekt

        // // Skärm -> värld (musens position)
        Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;                                       // // 2D: håll dig på z=0

        // // Riktning från spelaren till musen
        Vector2 from = _rb != null ? _rb.position : (Vector2)transform.position;
        Vector2 dir = (Vector2)(mouseWorld - (Vector3)from);
        if (dir.sqrMagnitude < 0.000001f) return;                // // Skydd om musen är exakt på spelaren

        // // Vinkel i grader, med offset för sprite-riktning
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + aimOffsetDegrees;

        // // Antingen rotera hela kroppen eller bara vapenpivot
        if (rotateWholeBody)
            transform.rotation = Quaternion.Euler(0f, 0f, ang);  
        else if (aimPivot != null)
            aimPivot.rotation = Quaternion.Euler(0f, 0f, ang);   

        // // Visuell flip beroende på musens X
        if (flipOnMouseX && spriteRoot != null)
        {
            float dx = dir.x;                                    // // Negativ = musen vänster om spelaren
            Vector3 s = spriteRoot.localScale;                   
            s.x = Mathf.Abs(s.x) * (dx >= 0f ? 1f : -1f);        // // Positiv (höger) / negativ (vänster)
            spriteRoot.localScale = s;                           
        }
    }

    // // Enkelt API för att låsa upp/låsa rörelse externt (t.ex. GameManager)
    public void SetMovementEnabled(bool enabled)
    {
        _canMove = enabled;                                      // // Sätt flagga
        if (!enabled) _moveInput = Vector2.zero;                 // // Nollställ input vid låsning
    }

    private void OnValidate()
    {
        if (moveSpeed < 0f) moveSpeed = 0f;                      // // Skydda mot negativa värden i Inspector
    }

    // // (Valfritt) Editorhjälp – rita siktlinje när objektet är valt
    private void OnDrawGizmosSelected()
    {
        if (_cam == null) _cam = Camera.main;                   
        if (_cam == null) return;                               

        Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector3 from = Application.isPlaying && _rb != null ? (Vector3)_rb.position : transform.position;
        Gizmos.DrawLine(from, mouseWorld);                       // // Linje spelare -> mus
    }
}
