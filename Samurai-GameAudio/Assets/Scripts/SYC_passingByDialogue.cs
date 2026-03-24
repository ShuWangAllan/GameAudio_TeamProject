using FMODUnity;
using UnityEngine;

public class SYC_passingByDialogue : MonoBehaviour
{
    public SphereCollider conversationArea;
    public GameObject pressToTalkUI;
    public GameObject Narration1;
    public GameObject Narration2;
    public EventReference dialogueAudioEvent1;
    public EventReference dialogueAudioEvent2;
    private bool firstNarrationTriggered = false;
    private bool narrationContinue = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pressToTalkUI.SetActive(false);
        Narration1.SetActive(false);
        Narration2.SetActive(false);
    }

    private void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            Debug.Log("Set Active");
            pressToTalkUI.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E) && !narrationContinue)
            {
                if (!firstNarrationTriggered)
                {
                    pressToTalkUI.SetActive(false);
                    RuntimeManager.PlayOneShot(dialogueAudioEvent1, this.transform.position);
                    Narration1.SetActive(true);
                    firstNarrationTriggered = true;
                    narrationContinue = true;
                }
                else
                {
                    pressToTalkUI.SetActive(false);
                    RuntimeManager.PlayOneShot(dialogueAudioEvent2, this.transform.position);
                    Narration2.SetActive(true);
                    narrationContinue = true;
                }
            }
        }
    }


    private void OnTriggerExit(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            Debug.Log("Exit");
            narrationContinue = false;
            pressToTalkUI.SetActive(false);
            Narration1.SetActive(false);
            Narration2.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
