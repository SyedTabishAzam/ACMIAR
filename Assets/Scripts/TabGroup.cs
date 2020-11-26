using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TabGroup : MonoBehaviour {

    public List<tabbutton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public tabbutton selectedTab;
    public List<GameObject> objectsToSwap;

    public void subscribe(tabbutton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<tabbutton>();
        }
        tabButtons.Add(button);
    }

    public void onTabEnter(tabbutton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.backgroundImg.sprite = tabHover;
        }
    }

    public void onTabExit(tabbutton button)
    {
        ResetTabs();

    }

    public void onTabSelected(tabbutton button)
    {
        selectedTab = button;
        ResetTabs();
        button.backgroundImg.sprite = tabActive;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach(tabbutton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) { continue; }
            button.backgroundImg.sprite = tabIdle;
            {

            }
            button.backgroundImg.sprite = tabIdle;


        }

    }
}
