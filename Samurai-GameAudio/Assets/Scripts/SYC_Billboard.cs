using UnityEngine;

public class SYC_Billboard : MonoBehaviour
{
    private Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }


    void LateUpdate()
    {
        if (cam == null)
        {
            Debug.Log("Camera not assigned to billboard");
            return;
        }

            Vector3 direction = cam.transform.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(-direction);
    }
}
