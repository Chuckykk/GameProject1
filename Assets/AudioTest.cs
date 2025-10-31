using UnityEngine;
public class AudioTest : MonoBehaviour
{
    void Start() {
        AudioManager.PlayMusic("level1_theme");
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            AudioManager.Play("enemy_die");
    }
}
