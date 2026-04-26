using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerBreathing : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private EventReference breathingEvent;

    [Header("FMOD Parameters")]
    [SerializeField] private string breathIntensityParameterName = "BreathIntensity";
    [SerializeField] private string reverbParameterName = "ReverbAmount";

    [Header("Character Reference")]
    [SerializeField] private ThirdPersonCharacter character;

    [Header("Breathing Values")]
    [SerializeField] private float idleBreathing = 0.03f;
    [SerializeField] private float walkBreathing = 0.15f;
    [SerializeField] private float runBreathing = 0.85f;
    [SerializeField] private float crouchBreathing = 0.08f;

    [Header("Smoothing")]
    [SerializeField] private float increaseSpeed = 3f;
    [SerializeField] private float decreaseSpeed = 0.6f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private EventInstance breathingInstance;
    private float currentIntensity;
    private float currentReverbAmount = 0f;

    private void Awake()
    {
        if (character == null)
            character = GetComponentInParent<ThirdPersonCharacter>();
    }

    private void Start()
    {
        if (breathingEvent.IsNull)
        {
            Debug.LogError("PlayerBreathing: Breathing event is not assigned.");
            return;
        }

        breathingInstance = RuntimeManager.CreateInstance(breathingEvent);
        breathingInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        breathingInstance.start();
    }

    private void Update()
    {
        if (!breathingInstance.isValid())
            return;

        breathingInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        float targetIntensity = GetTargetBreathIntensity();

        float smoothingSpeed = targetIntensity > currentIntensity
            ? increaseSpeed
            : decreaseSpeed;

        currentIntensity = Mathf.MoveTowards(
            currentIntensity,
            targetIntensity,
            smoothingSpeed * Time.deltaTime
        );

        SetFMODParameter(breathIntensityParameterName, currentIntensity);
        SetFMODParameter(reverbParameterName, currentReverbAmount);

        if (debugLogs)
        {
            Debug.Log(
                "PlayerBreathing: BreathIntensity = " +
                currentIntensity.ToString("F2") +
                " | ReverbAmount = " +
                currentReverbAmount.ToString("F2")
            );
        }
    }

    public void SetBreathingReverb(float amount)
    {
        currentReverbAmount = Mathf.Clamp01(amount);

        if (debugLogs)
        {
            Debug.Log("PlayerBreathing: ReverbAmount set to " + currentReverbAmount.ToString("F2"));
        }
    }

    private float GetTargetBreathIntensity()
    {
        if (character == null)
            return idleBreathing;

        bool crouching = character.IsCrouching;
        float speed = character.AudioSpeed;

        if (crouching)
            return crouchBreathing;

        if (speed > 0.75f)
            return runBreathing;

        if (speed > 0.1f)
            return walkBreathing;

        return idleBreathing;
    }

    private void SetFMODParameter(string parameterName, float value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            return;

        FMOD.RESULT result = breathingInstance.setParameterByName(parameterName, value);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning(
                "PlayerBreathing: Could not set FMOD parameter '" +
                parameterName +
                "' to " +
                value +
                ". Result: " +
                result
            );
        }
    }

    private void OnDestroy()
    {
        if (breathingInstance.isValid())
        {
            breathingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            breathingInstance.release();
        }
    }
}