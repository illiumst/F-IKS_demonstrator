using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnMouseOverClass : MonoBehaviour
{
    GameObject toolTipCanvas;

    private void Start()
    {
        toolTipCanvas = GameObject.FindWithTag("TooltipCanvas");
        toolTipCanvas.GetComponent<Canvas>().enabled = false;

    }

    void OnMouseOver()
    {
        if (toolTipCanvas != null)
        {
            var mousePos = Input.mousePosition;
            toolTipCanvas.transform.position = new Vector3(mousePos.x, mousePos.y + 35, mousePos.z);
            FillTooltip();
            toolTipCanvas.GetComponent<Canvas>().enabled = true;
            Cursor.visible = false;
        }
        Debug.Log("Mouse over: " + this.gameObject.name);
    }

    void OnMouseExit()
    {
        if (toolTipCanvas != null)
        {
            toolTipCanvas.GetComponent<Canvas>().enabled = false;
            Cursor.visible = true;

        }
    }

    void FillTooltip()
    {
        Debug.Log("________Trying to fill tooltip: " + this.gameObject.name);
        var statemachine = GameObject.FindWithTag("System").GetComponent<EnvironmentStateManager>();

        if (this.gameObject.name.Contains("Dirt"))
        {
            var dirt = statemachine.GetDirtByName(this.gameObject.name);
            if (dirt != null)
            {
                toolTipCanvas.transform.GetChild(1).gameObject.GetComponent<Text>().text = dirt.name;
                toolTipCanvas.transform.GetChild(2).gameObject.GetComponent<Text>().text = "Position: x: " + dirt.x + " y: " + dirt.y;
                toolTipCanvas.transform.GetChild(3).gameObject.GetComponent<Text>().text = "Amount: " + dirt.amount;
            }
        }
        else if (this.gameObject.name.Contains("Item"))
        {
            var item = statemachine.GetItemByName(this.gameObject.name);
            {
                if (item != null)
                {
                    toolTipCanvas.transform.GetChild(1).gameObject.GetComponent<Text>().text = item.name;
                    toolTipCanvas.transform.GetChild(2).gameObject.GetComponent<Text>().text = "Position: x: " + item.x + " y: " + item.y;
                    toolTipCanvas.transform.GetChild(3).gameObject.GetComponent<Text>().text = "";
                }
            }
        }
        else if (this.gameObject.name.Contains("Slider"))
        {
            Debug.Log("Hovering over handle...");
        }
    }

}
