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

    [Header("Character Reference")]
    [SerializeField] private ThirdPersonCharacter character;

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

        if (footstepsEvent.IsNull)
            Debug.LogError("PlayerSounds: Footsteps Event is not assigned.");

        if (character == null)
            Debug.LogWarning("PlayerSounds: ThirdPersonCharacter not found. Speed and crouch parameters will default to 0.");
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
                " | FMOD Speed: " +
                GetMovementSpeedForFMOD().ToString("F2") +
                " | Crouch: " +
                GetCrouchValue().ToString("F2")
            );
        }

        return true;
    }

    private void SetMovementParameters(EventInstance footstep)
    {
        TrySetParameter(footstep, crouchParameterName, GetCrouchValue());
        TrySetParameter(footstep, speedParameterName, GetMovementSpeedForFMOD());
    }

    private bool IsCrouching()
    {
        return character != null && character.IsCrouching;
    }

    private float GetCrouchValue()
    {
        return IsCrouching() ? 1f : 0f;
    }

    private float GetMovementSpeedForFMOD()
    {
        if (character == null)
            return 0f;

        if (IsCrouching())
            return character.AudioCrouchSpeed;

        return character.AudioSpeed;
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