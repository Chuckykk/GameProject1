using UnityEngine;

public class PlayerRefs : MonoBehaviour
{
    [Header("Player Components")]
    public PlayerHealth2D health;
    public CharacterMovement2D move;
    public PlayerShooting2D shooting;
    public PlayerXP2D xp;

    private void Reset()        { AutoWire(); }
    private void OnValidate()   { if (!Application.isPlaying) AutoWire(); }

    private void AutoWire()
    {
        if (!health)  health  = GetComponent<PlayerHealth2D>()     ?? GetComponentInChildren<PlayerHealth2D>(true);
        if (!move)    move    = GetComponent<CharacterMovement2D>()?? GetComponentInChildren<CharacterMovement2D>(true);
        if (!shooting)shooting= GetComponent<PlayerShooting2D>()   ?? GetComponentInChildren<PlayerShooting2D>(true);
        if (!xp)      xp      = GetComponent<PlayerXP2D>()         ?? GetComponentInChildren<PlayerXP2D>(true);
    }
}
