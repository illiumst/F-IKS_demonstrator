using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AgentController : MonoBehaviour
{

    public Vector3 currentpos;
    public Vector3 goalPosition;

    [SerializeField] float playBackSpeed;

    public float speed;

    GameObject system;

    public float sliderValue;

    Button playButton;
    Button pauseButton;


    public bool playingSequence;
    public bool valid;
    public bool backwardBuffer;

    bool manualRobotControl;

    public Animator animator;

    Agent parentAgent;

    GameObject agentCanvas;
    GameObject speechBubble;
    GameObject speechBubbleContent;
    GameObject speechBubbleInvalidityImage;
    GameObject speechBubbleDirectionText;

    Toggle showValidActionsToggle;
    Toggle showInvalidActionsToggle;

    Texture2D cleaningTex;
    //Texture2D cleaningInvalidTex;
    //Texture2D wallColTex;
    //Texture2D agentColTex;
    Texture2D itemPickTex;
    //Texture2D itemOffTex;
    Texture2D directionTex;

    Sprite cleaningSprite;
    //Sprite cleaningInvalidSprite;
    //Sprite wallColSprite;
    //Sprite agentColSprite;
    Sprite itemPickSprite;
    //Sprite itemOffSprite;
    Sprite directionSprite;

    // Start is called before the first frame update
    void Start()
    {
        currentpos = this.transform.position;
        goalPosition = currentpos;
        sliderValue = 0;
        valid = true;
        backwardBuffer = false;

        system = GameObject.FindWithTag("System");

        playingSequence = false;

        animator = GetComponent<Animator>();

        manualRobotControl = false;

        agentCanvas = FindGameObjectInChildWithTag(gameObject, "AgentCanvas");
        speechBubble = FindGameObjectInChildWithTag(agentCanvas, "SpeechBubble");
        speechBubbleContent = FindGameObjectInChildWithTag(speechBubble, "SpeechBubbleContent");
        speechBubbleInvalidityImage = FindGameObjectInChildWithTag(speechBubble, "SpeechBubbleInvalid");
        speechBubbleDirectionText = FindGameObjectInChildWithTag(speechBubble, "SpeechBubbleDirectionText");

        showValidActionsToggle = GameObject.FindWithTag("ValidActionsToggle").GetComponent<Toggle>();
        showInvalidActionsToggle = GameObject.FindWithTag("InvalidActionsToggle").GetComponent<Toggle>();

        cleaningTex = Resources.Load<Texture2D>("Sprites/cleaningNew");
        //cleaningInvalidTex = Resources.Load<Texture2D>("Sprites/cleaningNewInvalid");
        //wallColTex = Resources.Load<Texture2D>("Sprites/wallCol");
        //agentColTex = Resources.Load<Texture2D>("Sprites/agentCol");
        itemPickTex = Resources.Load<Texture2D>("Sprites/boxUp");
        //itemOffTex = Resources.Load<Texture2D>("Sprites/boxDown");
        directionTex = Resources.Load<Texture2D>("Sprites/direction");


        cleaningSprite = Sprite.Create(cleaningTex, new Rect(0.0f, 0.0f, cleaningTex.width, cleaningTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        //cleaningInvalidSprite = Sprite.Create(cleaningInvalidTex, new Rect(0.0f, 0.0f, cleaningInvalidTex.width, cleaningInvalidTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        //wallColSprite = Sprite.Create(wallColTex, new Rect(0.0f, 0.0f, wallColTex.width, wallColTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        //agentColSprite = Sprite.Create(agentColTex, new Rect(0.0f, 0.0f, agentColTex.width, agentColTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        itemPickSprite = Sprite.Create(itemPickTex, new Rect(0.0f, 0.0f, itemPickTex.width, itemPickTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        //itemOffSprite = Sprite.Create(itemOffTex, new Rect(0.0f, 0.0f, itemOffTex.width, itemOffTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        directionSprite = Sprite.Create(directionTex, new Rect(0.0f, 0.0f, directionTex.width, directionTex.height), new Vector2(0.5f, 0.5f), 100.0f);

        speechBubble.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        currentpos = this.transform.position;

        playBackSpeed = system.GetComponent<EnvironmentStateMachine>().playBackSpeed;

        //sequence with playbutton is being played
        if (playingSequence && valid)
        {
            speed = playBackSpeed * 3;
            MoveRobotTo(goalPosition);
        }

        if (!playingSequence && currentpos != goalPosition && goalPosition != null && valid)
        {
            MoveRobotTo(goalPosition);
        }

        if (manualRobotControl)
        {
            if (Input.GetKey("left"))
            {
                MoveRobotLeft();
            }
            if (Input.GetKey("right"))
            {
                MoveRobotRight();
            }
            if (Input.GetKey("up"))
            {
                MoveRobotUp();
            }
            if (Input.GetKey("down"))
            {
                MoveRobotDown();
            }
        }

    }

    void MoveRobotLeft()
    {
        Vector3 position = this.transform.position;
        position.x -= Time.deltaTime * speed;
        this.transform.position = position;
    }
    void MoveRobotRight()
    {
        Vector3 position = this.transform.position;
        position.x += Time.deltaTime * speed;
        this.transform.position = position;
    }
    void MoveRobotUp()
    {
        Vector3 position = this.transform.position;
        position.z += Time.deltaTime * speed;
        this.transform.position = position;
    }
    void MoveRobotDown()
    {
        Vector3 position = this.transform.position;
        position.z -= Time.deltaTime * speed;
        this.transform.position = position;
    }

    public void MoveRobotTo(Vector3 position)
    {
        animator.SetBool("moving", true);

        this.transform.position = Vector3.MoveTowards(transform.position, goalPosition, Time.deltaTime * speed);
        //StartCoroutine(LerpPosition(goalPosition, 1));

        if (backwardBuffer)
        {
            this.transform.Rotate(new Vector3(0, 180, 0), Space.Self);
        }
        Vector3 difference = goalPosition - currentpos;
        var name = this.transform.GetChild(4);
        var wallVisualiser = this.transform.GetChild(6);
        if (difference != Vector3.zero)
        {
            this.transform.forward = difference;
            name.transform.eulerAngles = Vector3.zero;
            wallVisualiser.eulerAngles = Vector3.zero;
        }
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    public Vector3 GetRecalculatedPosition(float x, float y, float z)
    {
        Vector3 center = system.GetComponent<EnvironmentStateMachine>().environmentCenter;
        return new Vector3(x - center.x, y, z - center.z);
    }

    public void UpdateAgentListItems(GameObject agentObject, GameObject listItem, int x, int y, string name, string action, bool valid)
    {
        var agentBody = agentObject.transform.GetChild(0).gameObject;
        var itemHeader = listItem.transform.GetChild(0).gameObject;
        var itemHeaderText = itemHeader.transform.GetChild(1).gameObject;
        itemHeaderText.GetComponent<Text>().text = name;

        var itemContent = listItem.transform.GetChild(1).gameObject;
        var data = itemContent.transform.GetChild(1).gameObject;
        var contentPositionText = data.transform.GetChild(0).gameObject;
        contentPositionText.GetComponent<Text>().text = "x: " + x + " y: " + y;
        var contentActionText = data.transform.GetChild(1).gameObject;
        contentActionText.GetComponent<Text>().text = action;
        var contentValidityText = data.transform.GetChild(2).gameObject;
        string validityString = "invalid";
        var color = Color.red;
        var textColor = Color.red;
        if (valid)
        {
            textColor = new Color32(0, 160, 20, 255);
            color = Color.green;
            validityString = "valid";
        }
        contentValidityText.GetComponent<Text>().text = validityString;
        contentValidityText.GetComponent<Text>().color = textColor;

        var validityLight = agentBody.transform.GetChild(5).gameObject;
        validityLight.GetComponent<Light>().color = color;

        var canvas = agentBody.transform.GetChild(4).gameObject;
        var nameTag = canvas.transform.GetChild(0).gameObject;
        nameTag.GetComponent<TextMeshProUGUI>().color = textColor;
    }

    public void UpdateSpeechBubble(string action, bool valid)
    {
        var rectTrans = speechBubbleContent.GetComponent<RectTransform>();
        var cam = Camera.main;
        if (valid && showInvalidActionsToggle.isOn)
        {
            speechBubbleInvalidityImage.SetActive(false);
            speechBubble.GetComponent<Image>().color = new Color32(190, 255, 200, 255);

            switch (action)
            {
                case "Action[CLEAN_UP]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = cleaningSprite;
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    //rectTrans.Rotate(new Vector3(0, 0, 0));
                    speechBubbleDirectionText.SetActive(false);
                    speechBubble.SetActive(true);
                    break;
                case "Action[ITEM_ACTION]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = itemPickSprite;
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    //rectTrans.Rotate(new Vector3(0, 0, 0));
                    speechBubbleDirectionText.SetActive(false);
                    speechBubble.SetActive(true);
                    break;
                case "Action[NORTH]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "N";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    //rectTrans.Rotate(new Vector3(0, 0, 0));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[NORTHEAST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "N/E";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 45));
                    //rectTrans.Rotate(new Vector3(0, 0, 45));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[EAST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "E";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 90));
                    //rectTrans.Rotate(new Vector3(0, 0, 90));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[SOUTHEAST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "S/E";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 135));
                    //rectTrans.Rotate(new Vector3(0, 0, 135));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[SOUTH]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "S";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 180));
                    //rectTrans.Rotate(new Vector3(0, 0, 180));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[SOUTHWEST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "S/W";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 225));
                    //rectTrans.Rotate(new Vector3(0, 0, 225));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[WEST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "W";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 270));
                    //rectTrans.Rotate(new Vector3(0, 0, 270));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[NORTHWEST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "N/W";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 315));
                    //rectTrans.Rotate(new Vector3(0, 0, 315));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;

                default: speechBubble.SetActive(false); break;
            }
        }
        else if (!valid && showInvalidActionsToggle.isOn)
        {
            speechBubbleInvalidityImage.SetActive(true);
            speechBubble.GetComponent<Image>().color = new Color32(255, 190, 190, 255);
            switch (action)
            {
                case "Action[CLEAN_UP]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = cleaningSprite;
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    //rectTrans.Rotate(new Vector3(0, 0, 0));
                    speechBubbleDirectionText.SetActive(false);
                    speechBubble.SetActive(true);
                    break;
                case "Action[ITEM_ACTION]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = itemPickSprite;
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    //rectTrans.Rotate(new Vector3(0, 0, 0));
                    speechBubbleDirectionText.SetActive(false);
                    speechBubble.SetActive(true);
                    break;
                case "Action[NORTH]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "N";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    //rectTrans.Rotate(new Vector3(0, 0, 0));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[NORTHEAST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "N/E";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 45));
                    //rectTrans.Rotate(new Vector3(0, 0, 45));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[EAST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "E";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 90));
                    //rectTrans.Rotate(new Vector3(0, 0, 90));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[SOUTHEAST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "S/E";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 135));
                    //rectTrans.Rotate(new Vector3(0, 0, 135));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[SOUTH]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "S";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 180));
                    //rectTrans.Rotate(new Vector3(0, 0, 180));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[SOUTHWEST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "S/W";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 225));
                    //rectTrans.Rotate(new Vector3(0, 0, 225));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[WEST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "W";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 270));
                    //rectTrans.Rotate(new Vector3(0, 0, 270));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[NORTHWEST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "N/W";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 315));
                    //rectTrans.Rotate(new Vector3(0, 0, 315));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;

                default: speechBubble.SetActive(false); break;
            }
        }
        else
        {
            speechBubble.SetActive(false);
        }

    }

    public static GameObject FindGameObjectInChildWithTag(GameObject parent, string tag)
    {
        Transform t = parent.transform;

        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).gameObject.tag == tag)
            {
                return t.GetChild(i).gameObject;
            }

        }

        return null;
    }

}
