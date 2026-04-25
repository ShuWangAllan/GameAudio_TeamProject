using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class FootstepTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerSounds playerSounds;
    [SerializeField] private ThirdPersonCharacter character;

    [Header("Foot")]
    [SerializeField] private FootSide footSide = FootSide.Left;
    [SerializeField] private Vector3 localSoleOffset = Vector3.zero;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float castStartHeight = 0.35f;
    [SerializeField] private float castBelowFootDistance = 0.55f;
    [SerializeField] private float sphereRadius = 0.09f;

    [Header("Step Detection")]
    [SerializeField] private float plantedHeight = 0.07f;
    [SerializeField] private float liftBeforeNextStep = 0.14f;
    [SerializeField] private float minTimeBetweenThisFoot = 0.38f;

    [Header("Slow Walk Support")]
    [SerializeField] private float minMovementAmount = 0.03f;
    [SerializeField] private bool requireDownwardMovement = false;
    [SerializeField] private float requiredDownwardSpeed = 0.015f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private Collider[] ownColliders;

    private bool hasLiftedSinceLastStep = true;
    private float nextAllowedStepTime;
    private float previousHeightAboveGround;
    private bool hasPreviousHeight;

    private void Awake()
    {
        if (playerSounds == null)
            playerSounds = GetComponentInParent<PlayerSounds>();

        if (character == null)
            character = GetComponentInParent<ThirdPersonCharacter>();

        ownColliders = GetComponentsInParent<Collider>();

        if (playerSounds == null)
            Debug.LogError(gameObject.name + ": Could not find PlayerSounds.");

        if (character == null)
            Debug.LogError(gameObject.name + ": Could not find ThirdPersonCharacter.");
    }

    private void Update()
    {
        if (playerSounds == null || character == null)
            return;

        if (!character.IsGrounded || character.MovementAmount < minMovementAmount)
        {
            ResetStepState();
            return;
        }

        Vector3 solePosition = transform.TransformPoint(localSoleOffset);

        if (!TryFindGround(solePosition, out RaycastHit hit))
        {
            ResetStepState();
            return;
        }

        float heightAboveGround = solePosition.y - hit.point.y;

        bool footIsMovingDown = true;

        if (requireDownwardMovement)
        {
            footIsMovingDown = false;

            if (hasPreviousHeight)
            {
                float heightChange = previousHeightAboveGround - heightAboveGround;
                footIsMovingDown = heightChange > requiredDownwardSpeed * Time.deltaTime;
            }
        }

        previousHeightAboveGround = heightAboveGround;
        hasPreviousHeight = true;

        if (heightAboveGround >= liftBeforeNextStep)
        {
            hasLiftedSinceLastStep = true;
        }

        bool footIsPlanted = heightAboveGround <= plantedHeight;

        if (
            hasLiftedSinceLastStep &&
            footIsPlanted &&
            footIsMovingDown &&
            Time.time >= nextAllowedStepTime
        )
        {
            bool played = playerSounds.PlayFootstep(hit.point, footSide, hit.collider);

            if (played)
            {
                hasLiftedSinceLastStep = false;
                nextAllowedStepTime = Time.time + minTimeBetweenThisFoot;

                if (debugLogs)
                    Debug.Log(gameObject.name + ": played " + footSide + " footstep.");
            }
        }

        if (debugLogs)
        {
            Debug.Log(
                gameObject.name +
                " | Foot: " + footSide +
                " | Move: " + character.MovementAmount.ToString("F3") +
                " | Height: " + heightAboveGround.ToString("F3") +
                " | Planted: " + footIsPlanted +
                " | Lifted: " + hasLiftedSinceLastStep +
                " | MovingDown: " + footIsMovingDown +
                " | Ground: " + hit.collider.name
            );
        }
    }

    private void ResetStepState()
    {
        hasLiftedSinceLastStep = true;
        hasPreviousHeight = false;
    }

    private bool TryFindGround(Vector3 solePosition, out RaycastHit bestHit)
    {
        Vector3 origin = solePosition + Vector3.up * castStartHeight;
        float totalDistance = castStartHeight + castBelowFootDistance;

        Debug.DrawRay(origin, Vector3.down * totalDistance, Color.red);

        RaycastHit[] hits = Physics.SphereCastAll(
            origin,
            sphereRadius,
            Vector3.down,
            totalDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        bestHit = default;

        if (hits == null || hits.Length == 0)
            return false;

        float closestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider hitCollider = hits[i].collider;

            if (hitCollider == null)
                continue;

            if (IsOwnCollider(hitCollider))
                continue;

            if (hits[i].distance < closestDistance)
            {
                closestDistance = hits[i].distance;
                bestHit = hits[i];
            }
        }

        return bestHit.collider != null;
    }

    private bool IsOwnCollider(Collider other)
    {
        if (ownColliders == null)
            return false;

        for (int i = 0; i < ownColliders.Length; i++)
        {
            if (ownColliders[i] == other)
                return true;
        }

        return false;
    }

    private Vector3 GetSolePosition()
    {
        return transform.TransformPoint(localSoleOffset);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 solePosition = GetSolePosition();
        Vector3 origin = solePosition + Vector3.up * castStartHeight;
        float totalDistance = castStartHeight + castBelowFootDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(solePosition, 0.04f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, sphereRadius);
        Gizmos.DrawLine(origin, origin + Vector3.down * totalDistance);
        Gizmos.DrawWireSphere(origin + Vector3.down * totalDistance, sphereRadius);
    }
}