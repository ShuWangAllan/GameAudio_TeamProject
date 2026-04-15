using UnityEngine;

public class SYC_UIdisable : MonoBehaviour
{
    BoxCollider2D targetCollider;
    SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null )
        {
            Debug.Log("Sprite renderer of " + gameObject.name + "cannot be found");
        }

        targetCollider = GetComponent<BoxCollider2D>();
        if (targetCollider == null )
        {
            Debug.Log("Collider of " + gameObject.name + "cannot be found");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targetCollider != null && spriteRenderer != null)
        {
            if (other.CompareTag("player"))
            {
                spriteRenderer.color = Color.white;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targetCollider != null && spriteRenderer != null)
        {
            if (other.CompareTag("player"))
            {
                spriteRenderer.color = Color.clear;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
