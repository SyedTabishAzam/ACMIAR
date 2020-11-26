using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Image))]
public class tabbutton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{

    public TabGroup tabGroup;
    public Image backgroundImg;

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.onTabSelected(this);
        }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.onTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.onTabExit(this);
    }

    // Use this for initialization
    void Start () {
        backgroundImg = GetComponent<Image>();
        tabGroup.subscribe(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
