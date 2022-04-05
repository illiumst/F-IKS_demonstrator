using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject toolTipCanvas;
    GameObject toolTipText;

    private void Start()
    {
        toolTipCanvas = GameObject.FindWithTag("Tooltip");
        toolTipText = GameObject.FindWithTag("TooltipText");

        toolTipCanvas.GetComponent<Canvas>().enabled = false;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (toolTipCanvas != null)
        {
            var mousePos = Input.mousePosition;
            toolTipCanvas.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 35, this.transform.position.z);
            var tagName = this.gameObject.tag;
            StartCoroutine(FillTooltip(tagName));
        }
        Debug.Log("Mouse over: " + this.gameObject.name);
        if (this.gameObject.tag == "EpisodeSelectionButtonClosed")
        {
            MoveArrowDown();
        }
        else if (this.gameObject.tag == "EpisodeSelectionButtonOpen")
        {
            MoveArrowUp();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (toolTipCanvas != null)
        {
            toolTipCanvas.GetComponent<Canvas>().enabled = false;
            Cursor.visible = true;

        }
        if (this.gameObject.tag == "EpisodeSelectionButtonClosed")
        {
            MoveArrowUp();
        }
        else if (this.gameObject.tag == "EpisodeSelectionButtonOpen")
        {
            MoveArrowDown();
        }
    }

    IEnumerator FillTooltip(string tagName)
    {
        yield return new WaitForSeconds(2f); // Wait 5 seconds

        Debug.Log("________Trying to fill tooltip: " + this.gameObject.name);
        var statemachine = GameObject.FindWithTag("System").GetComponent<EnvironmentStateMachine>();
        toolTipCanvas.GetComponent<Canvas>().enabled = true;

        switch (tagName)
        {
            case "HomeButton": toolTipText.gameObject.GetComponent<Text>().text = "Home"; break;
            case "AddTabButton": toolTipText.gameObject.GetComponent<Text>().text = "New Sequence"; break;
            case "EpisodeSelectionDropDown": toolTipText.gameObject.GetComponent<Text>().text = "Select Episode"; break;
            case "CursorButton": toolTipText.gameObject.GetComponent<Text>().text = "Select Tool"; break;
            case "MoveButton": toolTipText.gameObject.GetComponent<Text>().text = "Move Tool"; break;
            case "TurnButton": toolTipText.gameObject.GetComponent<Text>().text = "Turn Tool"; break;
            case "ZoomInButton": toolTipText.gameObject.GetComponent<Text>().text = "Zoom In"; break;
            case "ZoomOutButton": toolTipText.gameObject.GetComponent<Text>().text = "Zoom Out"; break;
            case "SettingsButton": toolTipText.gameObject.GetComponent<Text>().text = "Settings"; break;
            case "FullScreenButton":
                toolTipText.gameObject.GetComponent<Text>().text = "Fullscreen";
                toolTipCanvas.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 35, this.transform.position.z);
                break;
            default:
                toolTipCanvas.GetComponent<Canvas>().enabled = false;
                break;
        }
    }

    void MoveArrowDown()
    {
        var arrow = GameObject.FindWithTag("EpisodeSelectionButtonArrow");
        arrow.transform.position += new Vector3(0, -3, 0);
    }
    void MoveArrowUp()
    {
        var arrow = GameObject.FindWithTag("EpisodeSelectionButtonArrow");
        arrow.transform.position += new Vector3(0, 3, 0);
    }

}
