using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class TheLeekQuest : MonoBehaviour
{
    [Header("Triggers")]
    public GameObject leekTrigger;
    public GameObject marketSellerTrigger;

    [Header("UI Elements")]
    public GameObject pressToTalkUI;
    public GameObject pressToPickUpUI;
    public GameObject pressToGiveUI;
    public GameObject leekUIImage;
    public GameObject QuestBeginNarrationUI;
    public GameObject QuestEndNarrationUI;
    [Header("Items")]
    public GameObject leekToCollect;
    public GameObject leekToGive;
    public bool questStarted;
    bool doesPlayerHaveLeek;
    private bool questEnd = false;
    public EventReference QuestStartNpcNarrationAudioEvent;
    public EventReference QuestEndNpcNarrationAudioEvent;
    public Vector3 ReekTraderPos;


    // Start is called before the first frame update
    void Start()
    {
        questStarted = false;
        pressToGiveUI.SetActive(false);
        pressToPickUpUI.SetActive(false);
        pressToTalkUI.SetActive(false);
        leekToGive.SetActive(false);
        leekUIImage.SetActive(false);
        QuestBeginNarrationUI.SetActive(false);
        QuestEndNarrationUI.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        LeekPickup();
        MarketSellerStart();
        MarketSellerEnd();
    }

    void MarketSellerStart()
    {
        if (questStarted == false)
        {
            if (marketSellerTrigger.GetComponent<MarketSellerTrigger>().playerIsInMarketSellerTrigger == true)
            {
                pressToTalkUI.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    //Add your conversation here
                    RuntimeManager.PlayOneShot(QuestStartNpcNarrationAudioEvent, ReekTraderPos);
                    QuestBeginNarrationUI.SetActive(true);
                    questStarted = true;
                    pressToTalkUI.SetActive(false);
                }
            }
            else
            {
                pressToTalkUI.SetActive(false);
            }
        }
        if (questStarted && doesPlayerHaveLeek == false && !questEnd)
        {
            if (marketSellerTrigger.GetComponent<MarketSellerTrigger>().playerIsInMarketSellerTrigger == true)
            {
                QuestBeginNarrationUI.SetActive(true);
            }
            else
            {
                QuestBeginNarrationUI.SetActive(false);
            }
        }
    }
    void LeekPickup()
    {
        if (questStarted == true)
        {
            if (doesPlayerHaveLeek == false)
            {
                if (leekTrigger.GetComponent<LeekPickup>().playerIsInLeekTrigger == true)
                {
                    pressToPickUpUI.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        pressToPickUpUI.SetActive(false);
                        Destroy(leekToCollect);
                        doesPlayerHaveLeek = true;
                        leekUIImage.SetActive(true);
                        QuestBeginNarrationUI.SetActive(false);
                    }
                }
                else
                {
                    pressToPickUpUI.SetActive(false);
                }
            }
        }
    }
    void MarketSellerEnd()
    {
        if (doesPlayerHaveLeek == true)
        {
            if (marketSellerTrigger.GetComponent<MarketSellerTrigger>().playerIsInMarketSellerTrigger == true)
            {
                pressToGiveUI.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    RuntimeManager.PlayOneShot(QuestEndNpcNarrationAudioEvent, ReekTraderPos);
                    doesPlayerHaveLeek = false;
                    pressToGiveUI.SetActive(false);
                    leekToGive.SetActive(true);
                    leekUIImage.SetActive(false);
                    questEnd = true;
                }
            }
            else
            {
                pressToGiveUI.SetActive(false);
            }
        }
        else if (questEnd)
        {
            if (marketSellerTrigger.GetComponent<MarketSellerTrigger>().playerIsInMarketSellerTrigger == true)
            {
                QuestEndNarrationUI.SetActive(true);
            }
            else
            {
                QuestEndNarrationUI.SetActive(false);
            }
        }
    }


}

