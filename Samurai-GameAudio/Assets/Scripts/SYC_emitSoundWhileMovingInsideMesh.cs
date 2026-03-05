using UnityEngine;
using FMOD.Studio;
using System.Threading;
using System.Timers;
using FMODUnity;

public class SYC_emitSoundWhileMovingInsideMesh : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    public EventReference audioEvent1;
    public float audioEmitCD1;
    public EventReference audioEvent2;
    public float audioEmitCD2;
    public EventReference randomAudioEvent3;
    public float randomAudioEmitCD3;
    public int randomPlayChance = 20;
    private float timer1;
    private float timer2;
    private float timer3;
    private bool audio2Available = false;
    private bool audio3Available = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Awake()
    {
        if (!audioEvent2.IsNull)
        {
            audio2Available = true;
        }
        if (!randomAudioEvent3.IsNull)
        {
            audio3Available = true;
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timer1 = 0;
            timer2 = 0;
            timer3 = 0;
            playerRigidbody = other.GetComponent<Rigidbody>();
            RuntimeManager.PlayOneShot(audioEvent1);
            if (audio2Available)
            {
                RuntimeManager.PlayOneShot(audioEvent2);
            }
            if (audio3Available && Random.Range(0, 100) < randomPlayChance)
            {
                Debug.Log("Random audio begin play");
                RuntimeManager.PlayOneShot(randomAudioEvent3);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timer1 += Time.deltaTime;
            timer2 += Time.deltaTime;
            timer3 += Time.deltaTime;
            //Debug.Log(playerRigidbody.linearVelocity.x);
            //Debug.Log(playerRigidbody.linearVelocity.y);
            if (playerRigidbody.linearVelocity.magnitude > 1f)
            {
                if (timer1 >= audioEmitCD1)
                {
                    RuntimeManager.PlayOneShot(audioEvent1, transform.position);
                    timer1 = 0f;
                }
                if (audio2Available && timer2 >= audioEmitCD2)
                {
                    RuntimeManager.PlayOneShot(audioEvent2, transform.position);
                    timer2 = 0f;
                }
                if (audio3Available && timer3 >= randomAudioEmitCD3)
                {
                    if (Random.Range(0, 100) < randomPlayChance)
                    {
                        RuntimeManager.PlayOneShot(randomAudioEvent3, transform.position);
                        Debug.Log("Random audio play");
                    }
                    timer3 = 0f;
                }
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}

/* References:
 * Shaped by Rain Studios (2022) How to make an Audio System in Unity | Unity + FMOD Tutorial. 
 * Available at: https://youtu.be/rcBHIOjZDpk?si=xO42SvS1iYgHhwW4 [Accessed 5 March 2026]
 * */