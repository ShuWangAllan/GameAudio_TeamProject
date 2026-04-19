using UnityEngine;

public class SwordPickupAudio : MonoBehaviour
{
    public void PlaySwordPickupSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Weapons/Sword pick up", transform.position);
    }
}
