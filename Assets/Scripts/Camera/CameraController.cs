using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>Class <c>CameraController</c> is responsible for manual camera control.
/// This script is assigned to the Main Camera.</summary>
public class CameraController : MonoBehaviour
{
    #region Initializations
    //========================================================================================================//
    //================================= TWEAKABLE VALUES =====================================================//
    //========================================================================================================//
    public float panSpeed = 20f;

    public float turnSpeed = 5f;

    public float zoomAmount = 10f;

    //========================================================================================================//
    //========================================== BUTTONS =====================================================//
    //========================================================================================================//
    public Button ZoomInButton;
    public Button ZoomOutButton;
    public Button PanUpButton;
    public Button PanLeftButton;
    public Button PanRightButton;
    public Button PanDownButton;
    public Button TurnLeftButton;
    public Button TurnRightButton;


    //========================================================================================================//
    //==================================== HELPER VALUES =====================================================//
    //========================================================================================================//
    public bool zoomActive;
    Vector3 cameraPos;
    Vector3 cameraTurn;
    public GameObject target;
    private Vector3 targetPoint;
    float zoom;
    float tempMouseX;
    float tempMouseY;
    bool rotating;
    bool panning;
    UIShowAndHide UIScript;

    #endregion

    #region Start
    void Start()
    {
        targetPoint = target.transform.position;
        cameraPos = transform.position;
        cameraTurn = transform.eulerAngles;
        zoomActive = true;

        UIScript = GameObject.FindWithTag("System").GetComponent<UIShowAndHide>();
    }
    #endregion

    #region Update: Handling Input

    //set up different key options --> could be optimized
    //TODO some controls missing like panning
    void Update()
    {
        if (Input.GetKey("w"))
        {
            TaskOnClickPanUp();
        }
        if (Input.GetKey("s"))
        {
            TaskOnClickPanDown();
        }
        if (Input.GetKey("a"))
        {
            TaskOnClickPanLeft();
        }
        if (Input.GetKey("d"))
        {
            TaskOnClickPanRight();
        }
        if (Input.GetKey("+"))
        {
            TaskOnClickZoomIn();
        }
        if (Input.GetKey("-"))
        {
            TaskOnClickZoomOut();
        }
        if (Input.GetKey("q"))
        {
            TaskOnClickTurnLeft();
        }
        if (Input.GetKey("e"))
        {
            TaskOnClickTurnRight();
        }

        /*******************************************************************************************************/
        /*************************************** ZOOMING *******************************************************/
        /*******************************************************************************************************/
        if (!Input.GetMouseButtonDown(1) && zoomActive)
        {
            var zoomValue = Camera.main.orthographicSize;
            var axisInput = Input.GetAxis("Mouse ScrollWheel") * zoomAmount;
            if ((zoomValue - axisInput >= 5f && axisInput > 0) || (zoomValue - axisInput <= 10f && axisInput < 0))
            {
                Camera.main.orthographicSize -= axisInput;
                if (axisInput > 0)
                {
                    UIScript.OnCameraInteractionStart("zoomIn");
                }
                if (axisInput < 0)
                {
                    UIScript.OnCameraInteractionStart("zoomOut");
                }
            }
        }
        if (!Input.GetMouseButtonDown(1) && !zoomActive)
        {
            UIScript.OnCameraInteractionStop();
        }
        if (Input.GetMouseButtonDown(1) && zoomActive)
        {
            tempMouseX = Input.mousePosition.x;
            tempMouseY = Input.mousePosition.y;
            if (Input.GetKeyDown(KeyCode.LeftCommand) || Input.GetKeyDown(KeyCode.RightCommand))
            {
                rotating = true;
            }
            else
            {
                rotating = true;
            }
        }
        if ((Input.GetMouseButtonUp(1)))
        {
            rotating = false;
            panning = false;
            UIScript.OnCameraInteractionStop();
            UIScript.cameraRotationYNew = Camera.main.transform.localEulerAngles.y;
            UIScript.RotateCompass();

        }
        if (rotating && zoomActive)
        {
            Debug.Log("Trying to ratate: tempX: " + tempMouseX + " actualX: " + Input.mousePosition.x);

            var diffX = Input.mousePosition.x - tempMouseX;
            var diffY = Input.mousePosition.y - tempMouseY;
            if (diffX > 20)
            {
                TaskOnClickTurnRight();
                tempMouseX += diffX;
            }
            if (diffX < -20)
            {
                TaskOnClickTurnLeft();
                tempMouseX += diffX;
            }
        }
        if (panning && zoomActive)
        {
            Debug.Log("Trying to pan: tempX: " + tempMouseX + " actualX: " + Input.mousePosition.x);

            var diffX = Input.mousePosition.x - tempMouseX;
            var diffY = Input.mousePosition.y - tempMouseY;
            if (diffX > 30)
            {
                TaskOnClickPanRight();
                tempMouseX += diffX;
            }
            if (diffX < -30)
            {
                TaskOnClickPanLeft();
                tempMouseX += diffX;
            }
            if (diffY > 50)
            {
                TaskOnClickPanUp();
                tempMouseX += diffX;
            }
            if (diffY < -50)
            {
                TaskOnClickPanDown();
                tempMouseX += diffX;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftCommand))
        {
            if (Input.GetMouseButtonDown(1))
            {
                rotating = true;
            }
        }
    }
    #endregion

    #region Single Navigation Functions

    //========================================================================================================//
    //=========================== SINGLE NAVIGATION FUNCTIONS ================================================//
    //========================================================================================================//

    void TaskOnClickZoomIn()
    {
        Camera.main.orthographicSize -= zoomAmount * Time.deltaTime;
        UIScript.OnCameraInteractionStart("zoomIn");
    }

    void TaskOnClickZoomOut()
    {
        Camera.main.orthographicSize += zoomAmount * Time.deltaTime;
        UIScript.OnCameraInteractionStart("zoomOut");
    }

    void TaskOnClickPanUp()
    {
        cameraPos.z -= panSpeed * Time.deltaTime * 0.5f;
        transform.position = cameraPos;
        UIScript.OnCameraInteractionStart("pan");

    }

    void TaskOnClickPanLeft()
    {
        cameraPos.x += panSpeed * Time.deltaTime;
        transform.position = cameraPos;
        UIScript.OnCameraInteractionStart("pan");

    }

    void TaskOnClickPanRight()
    {
        cameraPos.x -= panSpeed * Time.deltaTime;
        transform.position = cameraPos;
        UIScript.OnCameraInteractionStart("pan");

    }

    void TaskOnClickPanDown()
    {
        cameraPos.z += panSpeed * Time.deltaTime * 0.5f;
        transform.position = cameraPos;
        UIScript.OnCameraInteractionStart("pan");

    }

    void TaskOnClickTurnLeft()
    {
        transform.RotateAround(targetPoint, new Vector3(0.0f, 1.0f, 0.0f), 5 * Time.deltaTime * turnSpeed);
        UIScript.OnCameraInteractionStart("rotate");

    }

    void TaskOnClickTurnRight()
    {
        transform.RotateAround(targetPoint, new Vector3(0.0f, 1.0f, 0.0f), -5 * Time.deltaTime * turnSpeed);
        UIScript.OnCameraInteractionStart("rotate");

    }
    void TaskOnClickTurnUp()
    {
        transform.RotateAround(targetPoint, new Vector3(1.0f, 0.0f, 0.0f), 5 * Time.deltaTime * turnSpeed);
        UIScript.OnCameraInteractionStart("rotate");

    }
    void TaskOnClickTurnDown()
    {
        transform.RotateAround(targetPoint, new Vector3(1.0f, 0.0f, 0.0f), -5 * Time.deltaTime * turnSpeed);
        UIScript.OnCameraInteractionStart("rotate");

    }

    #endregion
}
