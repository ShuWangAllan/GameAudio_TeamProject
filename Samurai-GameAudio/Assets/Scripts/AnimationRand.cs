using UnityEngine;

public class AnimationRand : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float minSpeed = 0.7f;
    [SerializeField] private float maxSpeed = 1.2f;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        if (animator != null)
        {
            animator.speed = Random.Range(minSpeed, maxSpeed);
        }
    }
}