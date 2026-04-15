using FMODUnity;
using UnityEngine;

public class SYC_enterConversationZone : MonoBehaviour
{
    public GameObject pressToTalkUI;
    public GameObject Narration1UI;
    public GameObject Narration2UI;
    public EventReference Narration1AudioEvent;
    public EventReference Narration2AudioEvent;
    private bool narrationPlayedOnce = false;
    private bool EPressed = false;
    private bool playerInsideConversationZone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            playerInsideConversationZone = true;
            pressToTalkUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            playerInsideConversationZone = false;
            EPressed = false;
            Narration1UI.SetActive(false);
            Narration2UI.SetActive(false);
            pressToTalkUI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInsideConversationZone)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E trigger");
                EPressed = true;
                //Add your conversation here
                if (!narrationPlayedOnce)
                {
                    RuntimeManager.PlayOneShot(Narration1AudioEvent, this.transform.position);
                    Narration1UI.SetActive(true);
                    pressToTalkUI.SetActive(false);
                    narrationPlayedOnce = true;
                }
                else
                {
                    RuntimeManager.PlayOneShot(Narration2AudioEvent, this.transform.position);
                    Narration2UI.SetActive(true);
                    pressToTalkUI.SetActive(false);
                }
            }
        }
    }
}
