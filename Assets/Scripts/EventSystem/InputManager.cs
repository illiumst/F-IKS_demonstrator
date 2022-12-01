using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputManager : MonoBehaviour
{
    int UILayer;
    bool overUI;


    private void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");
        overUI = false;
    }

    private void Update()
    {
        overUI = IsPointerOverUIElement();
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        var stateMachine = GameObject.FindWithTag("System").GetComponent<EnvironmentStateManager>();
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
            {
                if (curRaysastResult.gameObject.name.Equals("Handle"))
                {
                    Debug.Log("Hovering over handle");
                    stateMachine.SetStepTextActive(true);
                }
                else
                {
                }
                Camera.main.GetComponent<CameraController>().zoomActive = false;
                return true;
            }
        }
        stateMachine.SetStepTextActive(false);
        Camera.main.GetComponent<CameraController>().zoomActive = true;
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
