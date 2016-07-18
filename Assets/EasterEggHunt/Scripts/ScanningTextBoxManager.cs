using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ScanningTextBoxManager : MonoBehaviour {

    EventTrigger trigger;
    EventTrigger.Entry entry;
    // Use this for initialization
    void Start () {
        trigger = GetComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Select;
        entry.callback.AddListener((data) => { OnSelectDelegate((BaseEventData) data); });
        EnableOnSelect();
    }

    public void OnSelectDelegate(BaseEventData eventData)
    {
        Debug.Log("OnPointerDownDelegate called.");
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void EnableOnSelect()
    {
        trigger.triggers.Add(entry);

    }

    public void DisableOnSelect()
    {
        trigger.triggers.Remove(entry);

    }
}
