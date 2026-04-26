using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerSounds : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private EventReference footstepsEvent;

    [Header("FMOD Parameters")]
    [SerializeField] private string surfaceParameterName = "Footsteps";
    [SerializeField] private string speedParameterName = "Speed";
    [SerializeField] private string crouchParameterName = "Crouch";
    [SerializeField] private string crouchSpeedParameterName = "CrouchSpeed";

    [Header("Movement Reference")]
    [SerializeField] private ThirdPersonCharacter character;
    [SerializeField] private Rigidbody characterRigidbody;

    [Header("Normal Speed Tuning")]
    [SerializeField] private float normalWorldSpeedForFullSpeed = 4.5f;
    [SerializeField, Range(0f, 1f)] private float minimumNormalSpeed = 0.2f;

    [Header("Crouch Speed Tuning")]
    [SerializeField] private float crouchWorldSpeedForFullSpeed = 1.8f;
    [SerializeField, Range(0f, 1f)] private float minimumCrouchSpeed = 0.15f;

    [Header("Anti-Spam")]
    [SerializeField] private float minTimeBetweenAnyFootstep = 0.16f;
    [SerializeField] private float minTimeBetweenSameFoot = 0.32f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private float nextAllowedAnyFootstepTime;
    private float nextAllowedLeftFootstepTime;
    private float nextAllowedRightFootstepTime;

    private void Awake()
    {
        if (character == null)
            character = GetComponentInParent<ThirdPersonCharacter>();

        if (characterRigidbody == null)
            characterRigidbody = GetComponentInParent<Rigidbody>();

        if (footstepsEvent.IsNull)
            Debug.LogError("PlayerSounds: Footsteps Event is not assigned.");

        if (character == null)
            Debug.LogWarning("PlayerSounds: ThirdPersonCharacter not found.");

        if (characterRigidbody == null)
            Debug.LogWarning("PlayerSounds: Rigidbody not found.");
    }

    public bool PlayFootstep(Vector3 position, FootSide footSide, Collider groundCollider)
    {
        if (footstepsEvent.IsNull)
        {
            Debug.LogError("PlayerSounds: Footsteps Event is not assigned.");
            return false;
        }

        if (groundCollider == null)
        {
            if (debugLogs)
                Debug.LogWarning("PlayerSounds: Ground collider is null.");

            return false;
        }

        if (!CanPlayFootstep(footSide))
            return false;

        EventInstance footstep = RuntimeManager.CreateInstance(footstepsEvent);

        if (!footstep.isValid())
        {
            Debug.LogError("PlayerSounds: Could not create FMOD footstep instance.");
            return false;
        }

        SetSurfaceFromCollider(footstep, groundCollider);
        SetMovementParameters(footstep);

        footstep.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        FMOD.RESULT startResult = footstep.start();

        if (startResult != FMOD.RESULT.OK)
        {
            Debug.LogWarning("PlayerSounds: FMOD start result = " + startResult);
            footstep.release();
            return false;
        }

        footstep.release();

        SetCooldowns(footSide);

        if (debugLogs)
        {
            Debug.Log(
                "PlayerSounds: Played " +
                footSide +
                " footstep on " +
                groundCollider.name +
                " | Crouching: " +
                IsCrouching() +
                " | Speed: " +
                GetNormalSpeedParameterValue().ToString("F2") +
                " | CrouchSpeed: " +
                GetCrouchSpeedParameterValue().ToString("F2") +
                " | Crouch: " +
                GetCrouchParameterValue().ToString("F2")
            );
        }

        return true;
    }

    private void SetMovementParameters(EventInstance footstep)
    {
        float crouchValue = GetCrouchParameterValue();
        float normalSpeedValue = GetNormalSpeedParameterValue();
        float crouchSpeedValue = GetCrouchSpeedParameterValue();

        TrySetParameter(footstep, crouchParameterName, crouchValue);
        TrySetParameter(footstep, speedParameterName, normalSpeedValue);
        TrySetParameter(footstep, crouchSpeedParameterName, crouchSpeedValue);
    }

    private bool IsCrouching()
    {
        return character != null && character.IsCrouching;
    }

    private float GetCrouchParameterValue()
    {
        return IsCrouching() ? 1f : 0f;
    }

    private float GetNormalSpeedParameterValue()
    {
        if (IsCrouching())
            return 0f;

        float worldSpeed = GetHorizontalWorldSpeed();

        float value = 0f;

        if (normalWorldSpeedForFullSpeed > 0f)
            value = worldSpeed / normalWorldSpeedForFullSpeed;

        value = Mathf.Clamp01(value);

        if (worldSpeed > 0.05f)
            value = Mathf.Max(value, minimumNormalSpeed);

        return value;
    }

    private float GetCrouchSpeedParameterValue()
    {
        if (!IsCrouching())
            return 0f;

        float worldSpeed = GetHorizontalWorldSpeed();

        float value = 0f;

        if (crouchWorldSpeedForFullSpeed > 0f)
            value = worldSpeed / crouchWorldSpeedForFullSpeed;

        value = Mathf.Clamp01(value);

        if (worldSpeed > 0.03f)
            value = Mathf.Max(value, minimumCrouchSpeed);

        return value;
    }

    private float GetHorizontalWorldSpeed()
    {
        if (characterRigidbody == null)
            return 0f;

        Vector3 velocity = characterRigidbody.linearVelocity;
        velocity.y = 0f;

        return velocity.magnitude;
    }

    private bool CanPlayFootstep(FootSide footSide)
    {
        if (Time.time < nextAllowedAnyFootstepTime)
            return false;

        if (footSide == FootSide.Left && Time.time < nextAllowedLeftFootstepTime)
            return false;

        if (footSide == FootSide.Right && Time.time < nextAllowedRightFootstepTime)
            return false;

        return true;
    }

    private void SetCooldowns(FootSide footSide)
    {
        nextAllowedAnyFootstepTime = Time.time + minTimeBetweenAnyFootstep;

        if (footSide == FootSide.Left)
            nextAllowedLeftFootstepTime = Time.time + minTimeBetweenSameFoot;
        else
            nextAllowedRightFootstepTime = Time.time + minTimeBetweenSameFoot;
    }

    private void SetSurfaceFromCollider(EventInstance footstep, Collider groundCollider)
    {
        int layer = groundCollider.gameObject.layer;

        if (layer == LayerMask.NameToLayer("Concrete"))
            TrySetParameter(footstep, surfaceParameterName, 0);
        else if (layer == LayerMask.NameToLayer("Dirt"))
            TrySetParameter(footstep, surfaceParameterName, 1);
        else if (layer == LayerMask.NameToLayer("Grass"))
            TrySetParameter(footstep, surfaceParameterName, 2);
        else if (layer == LayerMask.NameToLayer("Gravel"))
            TrySetParameter(footstep, surfaceParameterName, 3);
        else if (layer == LayerMask.NameToLayer("Water"))
            TrySetParameter(footstep, surfaceParameterName, 4);
        else if (layer == LayerMask.NameToLayer("Wood"))
            TrySetParameter(footstep, surfaceParameterName, 5);
        else
            TrySetParameter(footstep, surfaceParameterName, 0);
    }

    private void TrySetParameter(EventInstance footstep, string parameterName, float value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            return;

        FMOD.RESULT result = footstep.setParameterByName(parameterName, value);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning(
                "PlayerSounds: Could not set FMOD parameter '" +
                parameterName +
                "' to " +
                value +
                ". Result: " +
                result
            );
        }
    }
}

public enum FootSide
{
    Left = 0,
    Right = 1
}