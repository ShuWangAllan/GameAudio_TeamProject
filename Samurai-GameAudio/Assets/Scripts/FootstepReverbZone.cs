using UnityEngine;

public class FootstepReverbZone : MonoBehaviour
{
    [Header("Reverb")]
    [Range(0f, 1f)]
    [SerializeField] private float reverbAmount = 1f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private void Reset()
    {
        Collider zoneCollider = GetComponent<Collider>();

        if (zoneCollider != null)
            zoneCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerSounds playerSounds = FindPlayerSounds(other);
        PlayerBreathing playerBreathing = FindPlayerBreathing(other);
        PlayerJumpLandingSounds jumpLandingSounds = FindJumpLandingSounds(other);

        if (playerSounds != null)
            playerSounds.SetFootstepReverb(reverbAmount);

        if (playerBreathing != null)
            playerBreathing.SetBreathingReverb(reverbAmount);

        if (jumpLandingSounds != null)
            jumpLandingSounds.SetJumpReverb(reverbAmount);

        if (debugLogs)
        {
            Debug.Log("FootstepReverbZone: Player entered. ReverbAmount = " + reverbAmount.ToString("F2"));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerSounds playerSounds = FindPlayerSounds(other);
        PlayerBreathing playerBreathing = FindPlayerBreathing(other);
        PlayerJumpLandingSounds jumpLandingSounds = FindJumpLandingSounds(other);

        if (playerSounds != null)
            playerSounds.SetFootstepReverb(0f);

        if (playerBreathing != null)
            playerBreathing.SetBreathingReverb(0f);

        if (jumpLandingSounds != null)
            jumpLandingSounds.SetJumpReverb(0f);

        if (debugLogs)
        {
            Debug.Log("FootstepReverbZone: Player exited. ReverbAmount = 0.00");
        }
    }

    private PlayerSounds FindPlayerSounds(Collider other)
    {
        PlayerSounds playerSounds = other.GetComponent<PlayerSounds>();

        if (playerSounds != null)
            return playerSounds;

        playerSounds = other.GetComponentInChildren<PlayerSounds>();

        if (playerSounds != null)
            return playerSounds;

        return other.GetComponentInParent<PlayerSounds>();
    }

    private PlayerBreathing FindPlayerBreathing(Collider other)
    {
        PlayerBreathing playerBreathing = other.GetComponent<PlayerBreathing>();

        if (playerBreathing != null)
            return playerBreathing;

        playerBreathing = other.GetComponentInChildren<PlayerBreathing>();

        if (playerBreathing != null)
            return playerBreathing;

        return other.GetComponentInParent<PlayerBreathing>();
    }

    private PlayerJumpLandingSounds FindJumpLandingSounds(Collider other)
    {
        PlayerJumpLandingSounds jumpLandingSounds = other.GetComponent<PlayerJumpLandingSounds>();

        if (jumpLandingSounds != null)
            return jumpLandingSounds;

        jumpLandingSounds = other.GetComponentInChildren<PlayerJumpLandingSounds>();

        if (jumpLandingSounds != null)
            return jumpLandingSounds;

        return other.GetComponentInParent<PlayerJumpLandingSounds>();
    }
}