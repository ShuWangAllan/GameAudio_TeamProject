using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SYC_questTriggeredConversation : MonoBehaviour
{
    public GameObject pressToTalkUI;
    public GameObject Narration1UI;
    public GameObject Narration2UI;
    public EventReference NarrationAudioEventStart;
    public EventReference NarrationAudioEventEnd;
    public EventInstance narrationInstance;
    private bool narrationPlayedOnce = false;
    private bool EPressed = false;
    private bool playerInsideConversationZone;

    [SerializeField] QuestLogic questLogicScp;

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
            if (EPressed)
            {
                narrationPlayedOnce = true;
                EPressed = false;
            }
            Narration1UI.SetActive(false);
            Narration2UI.SetActive(false);
            pressToTalkUI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        narrationInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        if (playerInsideConversationZone)
        {
            if (Input.GetKeyDown(KeyCode.E) && !EPressed)
            {
                //Debug.Log("E trigger");
                EPressed = true;
                if (narrationInstance.isValid())
                {
                    narrationInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
                narrationInstance = RuntimeManager.CreateInstance(NarrationAudioEventStart);

                //Add your conversation here
                if (!questLogicScp.questComplete)
                {
                    //RuntimeManager.PlayOneShot(Narration1AudioEvent, this.transform.position);
                    narrationInstance = RuntimeManager.CreateInstance(NarrationAudioEventStart);
                    Narration1UI.SetActive(true);
                }
                else
                {
                    //RuntimeManager.PlayOneShot(Narration2AudioEvent, this.transform.position);
                    narrationInstance = RuntimeManager.CreateInstance(NarrationAudioEventEnd);
                    Narration2UI.SetActive(true);
                }
                narrationInstance.start();
                narrationInstance.release();
                pressToTalkUI.SetActive(false);
            }
        }
    }
}
