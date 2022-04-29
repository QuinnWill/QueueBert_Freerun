using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<UsableItem> usableItems { get; private set; }


    public UsableItem currentPrimary;
    public UsableItem currentSecondary;



    protected void Start()
    {

        usableItems = new List<UsableItem>();

        UsableItem[] items = GetComponentsInChildren<UsableItem>();
        List<UsableItem> activePrimaries = new List<UsableItem>();
        List<UsableItem> activeSecondaries = new List<UsableItem>();

        for (int i = 0; i < items.Length; i++)
        {
            items[i].gameObject.SetActive(false);
            usableItems.Add(items[i]);
        }

        if (activePrimaries.Count > 0)
        {
            currentPrimary = activePrimaries[Random.Range(0, activePrimaries.Count)];
        }

        if (activeSecondaries.Count > 0)
        {
            currentSecondary = activeSecondaries[Random.Range(0, activeSecondaries.Count)];
        }

    }

    public void SwitchActive(UsableItem newActive, bool isPrimary = true)
    {
        if (newActive == null)
        {
            if (isPrimary)
            {
                currentPrimary?.gameObject.SetActive(false);
                currentPrimary = null;
            }
            else 
            {
                currentSecondary?.gameObject.SetActive(false);
                currentSecondary = null;
            }
            return;
        }

        if (isPrimary)
        {
            currentPrimary?.gameObject.SetActive(false);
            newActive.isPrimary = isPrimary;
            newActive.gameObject.SetActive(true);
            currentPrimary = newActive;
        }
        else 
        {
            
            currentSecondary?.gameObject.SetActive(false);
            newActive.isPrimary = isPrimary;
            newActive.gameObject.SetActive(true);
            currentSecondary = newActive;
        }
    }

    public List<UsableItem> getInactive()
    {
        List<UsableItem> inactiveItems = new List<UsableItem>();

        foreach (UsableItem i in usableItems)
        {
            if (!i.gameObject.activeSelf)
            {
                inactiveItems.Add(i);
            }
        }
        return inactiveItems;
    }


    





}
