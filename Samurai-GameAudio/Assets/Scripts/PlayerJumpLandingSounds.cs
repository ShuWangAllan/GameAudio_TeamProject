using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerJumpLandingSounds : MonoBehaviour
{
    [Header("FMOD Events")]
    [SerializeField] private EventReference jumpEvent;

    [Header("FMOD Parameters")]
    [SerializeField] private string jumpIntensityParameterName = "JumpIntensity";
    [SerializeField] private string reverbParameterName = "ReverbAmount";

    [Header("References")]
    [SerializeField] private ThirdPersonCharacter character;

    [Header("Jump Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float jumpIntensity = 0.7f;

    [SerializeField] private float jumpSoundCooldown = 0.25f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private float currentReverbAmount = 0f;
    private float nextAllowedJumpSoundTime = 0f;

    private void Awake()
    {
        if (character == null)
            character = GetComponent<ThirdPersonCharacter>();

        if (character == null)
            character = GetComponentInParent<ThirdPersonCharacter>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (debugLogs)
            {
                Debug.Log(
                    "PlayerJumpLandingSounds: Space pressed. Grounded = " +
                    (character != null && character.IsGrounded) +
                    " | Crouching = " +
                    (character != null && character.IsCrouching) +
                    " | ReverbAmount = " +
                    currentReverbAmount.ToString("F2")
                );
            }

            if (character == null)
                return;

            if (!character.IsGrounded)
                return;

            if (character.IsCrouching)
                return;

            if (Time.time < nextAllowedJumpSoundTime)
                return;

            PlayJump();

            nextAllowedJumpSoundTime = Time.time + jumpSoundCooldown;
        }
    }

    public void SetJumpReverb(float amount)
    {
        currentReverbAmount = Mathf.Clamp01(amount);

        if (debugLogs)
        {
            Debug.Log("PlayerJumpLandingSounds: ReverbAmount set to " + currentReverbAmount.ToString("F2"));
        }
    }

    private void PlayJump()
    {
        if (jumpEvent.IsNull)
        {
            Debug.LogWarning("PlayerJumpLandingSounds: Jump event is not assigned.");
            return;
        }

        EventInstance jump = RuntimeManager.CreateInstance(jumpEvent);

        TrySetParameter(jump, jumpIntensityParameterName, jumpIntensity);
        TrySetParameter(jump, reverbParameterName, currentReverbAmount);

        jump.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        jump.start();
        jump.release();

        if (debugLogs)
        {
            Debug.Log(
                "PlayerJumpLandingSounds: Jump sound played. JumpIntensity = " +
                jumpIntensity.ToString("F2") +
                " | ReverbAmount = " +
                currentReverbAmount.ToString("F2")
            );
        }
    }

    private void TrySetParameter(EventInstance eventInstance, string parameterName, float value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            return;

        FMOD.RESULT result = eventInstance.setParameterByName(parameterName, value);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning(
                "PlayerJumpLandingSounds: Could not set FMOD parameter '" +
                parameterName +
                "' to " +
                value +
                ". Result: " +
                result
            );
        }
    }
}