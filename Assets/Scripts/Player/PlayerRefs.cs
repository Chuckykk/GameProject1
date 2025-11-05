using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRefs : MonoBehaviour
{
    [Header("Player Components")]
    public PlayerHealth2D health;       // dra in PlayerHealth2D
    public CharacterMovement2D move;    // dra in ditt rörelsescript
    public PlayerShooting2D shooting;         // dra in ditt skjutscript
    public PlayerXP2D xp;               // dra in PlayerXP2D
}
