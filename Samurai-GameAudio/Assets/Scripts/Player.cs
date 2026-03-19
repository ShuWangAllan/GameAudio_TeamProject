using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerSounds playerSounds;

    private void PlayFootsteps()
    {
        playerSounds.PlayFootsteps();
    }
}
