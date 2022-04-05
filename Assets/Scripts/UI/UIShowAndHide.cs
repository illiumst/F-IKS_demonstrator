using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIShowAndHide : MonoBehaviour
{
    GameObject CameraControlsCanvas;
    GameObject RobotScrollView;
    GameObject CompassCanvas;
    GameObject CameraCursorCanvas;
    GameObject ToggleMenuCanvas;
    Image CameraCursorImage;

    GameObject QuickOverview;


    Texture2D CameraCursorTextureZoomIn;
    Texture2D CameraCursorTextureZoomOut;
    Texture2D CameraCursorTextureRotate;
    Texture2D CameraCursorTexturePan;


    Sprite CameraCursorSpriteZoomIn;
    Sprite CameraCursorSpriteZoomOut;
    Sprite CameraCursorSpriteRotate;
    Sprite CameraCursorSpritePan;


    Button SettingsButton;
    Button CameraControlsButton;
    Button CameraCalibrationButton;

    Button ShowAgentActionsButton;
    Button HideAgentActionsButton;

    int camControlsCounter;
    int settingsCounter;

    Vector3 compassPosition;

    float cameraRotationY;
    public float cameraRotationYNew;

    public void InitializeUIShowAndHide()
    {

        CompassCanvas = GameObject.FindWithTag("CompassCanvas");
        QuickOverview = GameObject.FindWithTag("QuickFactsOverview");
        CameraCursorCanvas = GameObject.FindWithTag("CameraCursor");
        CameraCursorCanvas.GetComponent<Canvas>().enabled = false;
        CameraCursorImage = GameObject.FindWithTag("CameraCursorImage").transform.GetComponent<Image>();

        CameraCursorTextureZoomIn = Resources.Load<Texture2D>("Sprites/zoom-in");
        CameraCursorTextureZoomOut = Resources.Load<Texture2D>("Sprites/zoom-out");
        CameraCursorTextureRotate = Resources.Load<Texture2D>("Sprites/rotate");
        CameraCursorTexturePan = Resources.Load<Texture2D>("Sprites/movement2");

        CameraCursorSpriteZoomIn = Sprite.Create(CameraCursorTextureZoomIn, new Rect(0.0f, 0.0f, CameraCursorTextureZoomIn.width, CameraCursorTextureZoomIn.height), new Vector2(0.5f, 0.5f), 100.0f);
        CameraCursorSpriteZoomOut = Sprite.Create(CameraCursorTextureZoomOut, new Rect(0.0f, 0.0f, CameraCursorTextureZoomOut.width, CameraCursorTextureZoomOut.height), new Vector2(0.5f, 0.5f), 100.0f);
        CameraCursorSpriteRotate = Sprite.Create(CameraCursorTextureRotate, new Rect(0.0f, 0.0f, CameraCursorTextureRotate.width, CameraCursorTextureRotate.height), new Vector2(0.5f, 0.5f), 100.0f);
        CameraCursorSpritePan = Sprite.Create(CameraCursorTexturePan, new Rect(0.0f, 0.0f, CameraCursorTexturePan.width, CameraCursorTexturePan.height), new Vector2(0.5f, 0.5f), 100.0f);

        camControlsCounter = 0;
        settingsCounter = 0;
        compassPosition = CompassCanvas.transform.position;
        cameraRotationY = Camera.main.transform.localEulerAngles.y;
    }

    public void OnCameraInteractionStart(string interactionType)
    {
        if (CameraCursorCanvas != null)
        {
            var mousePos = Input.mousePosition;
            CameraCursorCanvas.transform.position = new Vector3(mousePos.x, mousePos.y + 35, mousePos.z);
            CameraCursorCanvas.GetComponent<Canvas>().enabled = true;
            Cursor.visible = false;
            switch (interactionType)
            {
                case "zoomIn": CameraCursorImage.sprite = CameraCursorSpriteZoomIn; break;
                case "zoomOut": CameraCursorImage.sprite = CameraCursorSpriteZoomOut; break;
                case "rotate": CameraCursorImage.sprite = CameraCursorSpriteRotate; break;
                case "pan": CameraCursorImage.sprite = CameraCursorSpritePan; break;

                default: break;
            }
        }
    }

    public void OnCameraInteractionStop()
    {
        if (CameraCursorCanvas != null)
        {
            CameraCursorCanvas.GetComponent<Canvas>().enabled = false;
            Cursor.visible = true;

        }
    }

    public void RecalibrateCamera()
    {
        Camera.main.transform.position = new Vector3(7.5f, 8.5f, -1);
        Camera.main.orthographicSize = 8;
        Camera.main.transform.eulerAngles = new Vector3(45, -65, 0);
        SetCompassDefaultRotation();
    }

    public void ShowHideCameraControls()
    {
        if (camControlsCounter % 2 == 0)
        {
            ShowCameraControls();
        }
        else
        {
            HideCameraControls();
        }
        camControlsCounter += 1;
    }

    public void MoveOverviewCanvasDown()
    {
        var canvas = QuickOverview;
        var newPos = new Vector3(canvas.transform.position.x, canvas.transform.position.y - 75, canvas.transform.position.z);
        canvas.transform.position = newPos;
    }
    public void MoveOverviewCanvasUp()
    {
        var canvas = QuickOverview;
        var newPos = new Vector3(canvas.transform.position.x, canvas.transform.position.y + 75, canvas.transform.position.z);
        canvas.transform.position = newPos;
    }


    public void HideCameraControls()
    {
        CameraControlsCanvas.GetComponent<Canvas>().enabled = false;
        RepositionCompass();
    }

    public void ShowCameraControls()
    {
        CameraControlsCanvas.GetComponent<Canvas>().enabled = true;
        ToggleMenuCanvas.GetComponent<Canvas>().enabled = false;
        MoveCompassLeft(Screen.width / 8);
    }

    public void ShowHideSettings()
    {
        if (settingsCounter % 2 == 0)
        {
            ShowSettings();
        }
        else
        {
            ToggleMenuCanvas.GetComponent<Canvas>().enabled = false;
            RepositionCompass();
        }
        settingsCounter += 1;
    }

    public void ShowSettings()
    {
        ToggleMenuCanvas.GetComponent<Canvas>().enabled = true;
        CameraControlsCanvas.GetComponent<Canvas>().enabled = false;
        MoveCompassLeft(Screen.width / 5.5f);
    }

    void MoveCompassLeft(float amount)
    {
        var newPos = new Vector3(compassPosition.x - amount, compassPosition.y, compassPosition.z);
        CompassCanvas.transform.position = newPos;
    }

    void RepositionCompass()
    {
        CompassCanvas.transform.position = compassPosition;
    }

    public void RotateCompass()
    {
        var diff = cameraRotationYNew - cameraRotationY;
        Debug.Log("Initial Camera rotation: " + cameraRotationY);
        Debug.Log("Current Camera rotation: " + cameraRotationYNew);
        Debug.Log("Difference Camera rotation: " + diff);

        CompassCanvas.transform.eulerAngles = new Vector3(0, 0, diff);
    }

    void SetCompassDefaultRotation()
    {
        CompassCanvas.transform.eulerAngles = new Vector3(0, 0, 20);
    }

    public void ShowAgentActions()
    {
        RobotScrollView.GetComponent<Canvas>().enabled = true;
    }
    public void HideAgentActions()
    {
        RobotScrollView.GetComponent<Canvas>().enabled = false;
    }

}
