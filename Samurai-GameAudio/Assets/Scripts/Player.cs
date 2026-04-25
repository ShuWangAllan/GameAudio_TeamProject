using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerSounds playerSounds;

    private void Awake()
    {
        if (playerSounds == null)
            playerSounds = GetComponent<PlayerSounds>();

        if (playerSounds == null)
            playerSounds = GetComponentInParent<PlayerSounds>();

        if (playerSounds == null)
            Debug.LogError("Player: Could not find PlayerSounds.");
    }
}