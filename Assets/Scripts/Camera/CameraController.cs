using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 40f;

    public float turnSpeed = 10f;

    public float zoomAmount = 100f;

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

    float zoom;

    void Start()
    {
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

    }
    // Update is called once per frame
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
        cameraPos.z += panSpeed * Time.deltaTime;
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
        cameraPos.z -= panSpeed * Time.deltaTime;
        transform.position = cameraPos;
    }

    void TaskOnClickTurnLeft()
    {
        //TODO 360 degrees abfragen
        cameraTurn.y += turnSpeed * Time.deltaTime;
        transform.eulerAngles = cameraTurn;
    }

    void TaskOnClickTurnRight()
    {
        //TODO 360 degrees abfragen
        cameraTurn.y -= turnSpeed * Time.deltaTime;
        transform.eulerAngles = cameraTurn;
    }
}
