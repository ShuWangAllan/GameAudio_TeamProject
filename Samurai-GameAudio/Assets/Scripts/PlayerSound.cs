using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference _footsteps;
    private FMOD.Studio.EventInstance footsteps;

    private void Awake()
    {
        if (!_footsteps.IsNull)
        {
            footsteps = FMODUnity.RuntimeManager.CreateInstance(_footsteps);
        }
    }

    public void PlayFootsteps()
    {
        if (footsteps.isValid())
        {
            footsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            GroundSwitch();
            footsteps.start();
        }
    }

    private void GroundSwitch()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction * 1.5f, Color.red); // shows ray in Scene view

        if (Physics.Raycast(ray, out hit, 1.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            GameObject hitObject = hit.collider.gameObject;

            int layer = hitObject.layer;
            string layerName = LayerMask.LayerToName(layer);

            Debug.Log("Standing on: " + hitObject.name + " | Layer: " + layerName);

            if (layer == LayerMask.NameToLayer("Concrete"))
                footsteps.setParameterByName("Footsteps", 0);
            else if (layer == LayerMask.NameToLayer("Dirt"))
                footsteps.setParameterByName("Footsteps", 1);
            else if (layer == LayerMask.NameToLayer("Water"))
                footsteps.setParameterByName("Footsteps", 4);
            else if (layer == LayerMask.NameToLayer("Grass"))
                footsteps.setParameterByName("Footsteps", 2);
            else if (layer == LayerMask.NameToLayer("Gravel"))
                footsteps.setParameterByName("Footsteps", 3);
            else if (layer == LayerMask.NameToLayer("Wood"))
                footsteps.setParameterByName("Footsteps", 5);
            else
                footsteps.setParameterByName("Footsteps", 0);
        }
    }
}