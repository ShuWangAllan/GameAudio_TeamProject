using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class QuestLogic : MonoBehaviour
{
    public GameObject emperorsSword;
    public GameObject emperorsSwordEnd;
    public EventReference SwordPickUp;
    public GameObject pickupTrigger;

    public GameObject QuestBeginNarrationUI;
    public GameObject QuestEndNarrationUI;
    public GameObject pickupUI;
    public GameObject giveUI;
    public GameObject talkUI;
    //public GameObject dontHaveSwordUI;
    public GameObject pickupInventory;

    public GameObject endquestTrigger;
    public bool questComplete;

    public bool playerHasEnteredSwordTrigger;
    public bool playerHasSword;
    public bool voiceLinePlaying;

    public EventReference QuestStartNpcNarrationAudioEvent;
    public EventReference QuestEndNpcNarrationAudioEvent;
    private EventInstance currentVoiceLine;
    private Vector3 senseiPos;
    public GameObject swordSensei;
    public bool ETriggered;

    // Start is called before the first frame update

    void Start()
    {
        playerHasSword = false;
        pickupUI.SetActive(false);
        giveUI.SetActive(false);
        talkUI.SetActive(false);
        pickupInventory.SetActive(false);
        questComplete = false;
        emperorsSwordEnd.SetActive(false); 
        senseiPos = swordSensei.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        DisplayUI();
        PickupSword();
        EndQuest();
    }

    void PickupSword()
    {
        if (pickupTrigger.GetComponent<SwordPickup>().playerCanPickupSword == true && Input.GetKeyDown(KeyCode.E))
        {
            RuntimeManager.PlayOneShot(SwordPickUp, transform.position);
            Destroy(emperorsSword);
            playerHasSword = true;
            //dontHaveSwordUI.SetActive(false);
            pickupUI.SetActive(false);
            pickupInventory.SetActive(true);
        }
    }

    void EndQuest()
    {
        if (endquestTrigger.GetComponent<EndQuestTrigger>().playerInEndTrigger == true)
        {
            if  (playerHasSword) //Player close to sensei with sword
            {
                if (!ETriggered)
                {
                    giveUI.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        ETriggered = true;
                        giveUI.SetActive(false);
                        questComplete = true;
                        emperorsSwordEnd.SetActive(true);
                        pickupUI.SetActive(false);
                        playerHasSword = false;
                        pickupInventory.SetActive(false);
                        if (!voiceLinePlaying)
                        {
                            voiceLinePlaying = true;
                            QuestEndNarrationUI.SetActive(true);
                            if (currentVoiceLine.isValid())
                            {
                                currentVoiceLine.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                                currentVoiceLine.release();
                            }

                            currentVoiceLine = RuntimeManager.CreateInstance(QuestEndNpcNarrationAudioEvent);
                            currentVoiceLine.set3DAttributes(RuntimeUtils.To3DAttributes(senseiPos));
                            currentVoiceLine.start();
                        }
                    }
                }                
            }
            else if (!questComplete) //Player close to sensei without sword before completing quest
            {
                if (!ETriggered)
                {
                    talkUI.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        ETriggered = true;
                        talkUI.SetActive(false);
                        if (!voiceLinePlaying)
                        {
                            voiceLinePlaying = true;
                            QuestBeginNarrationUI.SetActive(true);
                            if (currentVoiceLine.isValid())
                            {
                                currentVoiceLine.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                                currentVoiceLine.release();
                            }

                            currentVoiceLine = RuntimeManager.CreateInstance(QuestStartNpcNarrationAudioEvent);
                            currentVoiceLine.set3DAttributes(RuntimeUtils.To3DAttributes(senseiPos));
                            currentVoiceLine.start();
                        }
                    }
                }
            }
            else if (questComplete) //Quest complete
            {
                if (!ETriggered)
                {
                    talkUI.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        ETriggered = true;
                        talkUI.SetActive(false);
                        if (!voiceLinePlaying)
                        {
                            voiceLinePlaying = true;
                            QuestEndNarrationUI.SetActive(true);
                            if (currentVoiceLine.isValid())
                            {
                                currentVoiceLine.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                                currentVoiceLine.release();
                            }

                            currentVoiceLine = RuntimeManager.CreateInstance(QuestEndNpcNarrationAudioEvent);
                            currentVoiceLine.set3DAttributes(RuntimeUtils.To3DAttributes(senseiPos));
                            currentVoiceLine.start();
                        }
                    }
                }
            }
        }
        else if (!endquestTrigger.GetComponent<EndQuestTrigger>().playerInEndTrigger)
        {
            ETriggered = false;
            voiceLinePlaying = false;
            QuestBeginNarrationUI.SetActive(false);
            QuestEndNarrationUI.SetActive(false) ;
            giveUI.SetActive(false);
            talkUI.SetActive(false);
        }
    }
    void DisplayUI()
    {
        //UI for Pickup
        if (pickupTrigger.GetComponent<SwordPickup>().playerCanPickupSword == true && !playerHasSword)
        {
            pickupUI.SetActive(true);
        }
        else if (pickupTrigger.GetComponent<SwordPickup>().playerCanPickupSword == false && !playerHasSword)
        {
            pickupUI.SetActive(false);
        }

        /*
        //UI for End
        if (endquestTrigger.GetComponent<EndQuestTrigger>().playerInEndTrigger == true && playerHasSword && !questComplete)
        {
            if (voiceLinePlaying)
            {
                giveUI.SetActive(true);
            }
            dontHaveSwordUI.SetActive(false);

        }
        else if (endquestTrigger.GetComponent<EndQuestTrigger>().playerInEndTrigger == true && !playerHasSword && !questComplete)
        {
            if (voiceLinePlaying)
            {
                dontHaveSwordUI.SetActive(true);
            }
            giveUI.SetActive(false);
        }
        else if (endquestTrigger.GetComponent<EndQuestTrigger>().playerInEndTrigger == false)
        {
            giveUI.SetActive(false);
            dontHaveSwordUI.SetActive(false);
        }
                        */
        //Hide all UI when complete
        if (questComplete == true)
        {
            pickupUI.SetActive(false);
        }
    }
}
