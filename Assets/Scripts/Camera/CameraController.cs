using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;

    public float turnSpeed = 5f;

    public float zoomAmount = 10f;

    public bool zoomActive;

    public Button ZoomInButton;
    public Button ZoomOutButton;
    public Button PanUpButton;
    public Button PanLeftButton;
    public Button PanRightButton;
    public Button PanDownButton;
    public Button TurnLeftButton;
    public Button TurnRightButton;

    Vector3 cameraPos;
    Vector3 cameraTurn;

    public GameObject target;
    private Vector3 targetPoint;

    float zoom;

    float tempMouseX;
    float tempMouseY;

    bool rotating;
    bool panning;

    void Start()
    {
        targetPoint = target.transform.position;
        //transform.LookAt(targetPoint);
        ZoomInButton.onClick.AddListener(TaskOnClickZoomIn);
        ZoomOutButton.onClick.AddListener(TaskOnClickZoomOut);
        PanUpButton.onClick.AddListener(TaskOnClickPanUp);
        PanLeftButton.onClick.AddListener(TaskOnClickPanLeft);
        PanRightButton.onClick.AddListener(TaskOnClickPanRight);
        PanDownButton.onClick.AddListener(TaskOnClickPanDown);
        TurnLeftButton.onClick.AddListener(TaskOnClickTurnLeft);
        TurnRightButton.onClick.AddListener(TaskOnClickTurnRight);

        cameraPos = transform.position;
        cameraTurn = transform.eulerAngles;
        zoomActive = true;

    }
    // Update is called once per frame
    void Update()
    {
        //targetPoint = target.transform.position;
        //transform.LookAt(targetPoint);

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
        if (!Input.GetMouseButtonDown(1) && zoomActive)
        {
            var zoomValue = Camera.main.orthographicSize;
            var axisInput = Input.GetAxis("Mouse ScrollWheel") * zoomAmount;
            if ((zoomValue - axisInput >= 5f && axisInput > 0) || (zoomValue - axisInput <= 10f && axisInput < 0))
            {
                Camera.main.orthographicSize -= axisInput;
            }
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
                //panning = true;
                rotating = true;
            }
        }

        if ((Input.GetMouseButtonUp(1)))
        {
            rotating = false;
            panning = false;
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
            //TODO still buggy
            /*if (diffY > 40)
            {
                TaskOnClickTurnUp();
                tempMouseY += diffY;
            }
            if (diffY < 40)
            {
                TaskOnClickTurnDown();
                tempMouseY += diffY;
            }*/
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
            Debug.Log("...........Left command pressed....");
            if (Input.GetMouseButtonDown(1))
            {
                rotating = true;
            }
        }

    }

    void TaskOnClickZoomIn()
    {
        Camera.main.orthographicSize -= zoomAmount * Time.deltaTime;
    }

    void TaskOnClickZoomOut()
    {
        Camera.main.orthographicSize += zoomAmount * Time.deltaTime;
    }

    void TaskOnClickPanUp()
    {
        cameraPos.z -= panSpeed * Time.deltaTime * 0.5f;
        transform.position = cameraPos;
    }

    void TaskOnClickPanLeft()
    {
        cameraPos.x += panSpeed * Time.deltaTime;
        transform.position = cameraPos;
    }

    void TaskOnClickPanRight()
    {
        cameraPos.x -= panSpeed * Time.deltaTime;
        transform.position = cameraPos;
    }

    void TaskOnClickPanDown()
    {
        cameraPos.z += panSpeed * Time.deltaTime * 0.5f;
        transform.position = cameraPos;
    }

    void TaskOnClickTurnLeft()
    {
        //TODO 360 degrees abfragen
        //cameraTurn.y += turnSpeed * Time.deltaTime;
        //transform.eulerAngles = cameraTurn;
        transform.RotateAround(targetPoint, new Vector3(0.0f, 1.0f, 0.0f), 5 * Time.deltaTime * turnSpeed);
    }

    void TaskOnClickTurnRight()
    {
        //TODO 360 degrees abfragen
        //cameraTurn.y -= turnSpeed * Time.deltaTime;
        //transform.eulerAngles = cameraTurn;
        transform.RotateAround(targetPoint, new Vector3(0.0f, 1.0f, 0.0f), -5 * Time.deltaTime * turnSpeed);
    }
    void TaskOnClickTurnUp()
    {
        transform.RotateAround(targetPoint, new Vector3(1.0f, 0.0f, 0.0f), 5 * Time.deltaTime * turnSpeed);
    }
    void TaskOnClickTurnDown()
    {
        transform.RotateAround(targetPoint, new Vector3(1.0f, 0.0f, 0.0f), -5 * Time.deltaTime * turnSpeed);
    }
}
