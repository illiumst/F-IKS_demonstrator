using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;



public class AgentControllerNew : MonoBehaviour
{

    #region Attribute declarations
    public Vector3 currentpos;
    public Vector3 goalPosition;
    [SerializeField] float playBackSpeed;
    public float speed;
    GameObject system;
    public float sliderValue;
    public bool playingSequence;
    public bool valid;
    public bool backwardBuffer;
    bool manualRobotControl;
    public Animator animator;
    GameObject agentCanvas;
    GameObject speechBubble;
    GameObject speechBubbleContent;
    GameObject speechBubbleInvalidityImage;
    GameObject speechBubbleDirectionText;
    GameObject speechBubbleAgentName;
    public GameObject speechBubblePositionText;
    Texture2D cleaningTex;
    Texture2D itemPickTex;
    Texture2D directionTex;
    Sprite cleaningSprite;
    Sprite itemPickSprite;
    Sprite directionSprite;
    //TODO initialize in EnvironmentStateManager
    Agent agentModel;
    public bool finished = false;
    public string action;

    //Animation states
    string currentAnimationState = "";
    const string AGENT_IDLE = "Agent_idle";
    const string AGENT_PICKUP = "Agent_pickup";
    const string AGENT_MOVING = "Agent_moving";
    const string AGENT_INVALID = "Agent_invalid";
    const string AGENT_CLEANING = "Agent_cleaning";
    const string AGENT_DAMAGE = "Agent_damage";

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Initilizations
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
        speechBubblePositionText = FindGameObjectInChildWithTag(speechBubble, "SpeechBubblePositiontext");
        speechBubbleAgentName = FindGameObjectInChildWithTag(speechBubble, "SpeechBubbleAgentNumber");
        SetAgentNameInSpeechBubble(this.transform.parent.gameObject.name);

        cleaningTex = Resources.Load<Texture2D>("Sprites/cleaningNew");
        itemPickTex = Resources.Load<Texture2D>("Sprites/boxUp");
        directionTex = Resources.Load<Texture2D>("Sprites/direction");


        cleaningSprite = Sprite.Create(cleaningTex, new Rect(0.0f, 0.0f, cleaningTex.width, cleaningTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        itemPickSprite = Sprite.Create(itemPickTex, new Rect(0.0f, 0.0f, itemPickTex.width, itemPickTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        directionSprite = Sprite.Create(directionTex, new Rect(0.0f, 0.0f, directionTex.width, directionTex.height), new Vector2(0.5f, 0.5f), 100.0f);

        speechBubble.SetActive(false);
        #endregion
    }

    // Update is called once per frame
    //TODO update action here too
    void Update()
    {
        currentpos = this.transform.position;
        playBackSpeed = 1;


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

        if (currentpos == goalPosition && (action != "Action[CLEAN_UP]" || action != "Action[ITEM_ACTION]"))
        {
            //finished = true;
            animator.SetBool("moving", false);
        }

    }
    //TODO maybe move this functionality to agent controller
    public void UpdateAgentAction()
    {
        var stateManager = system.GetComponent<EnvironmentStateManager>();
        UpdateSpeechBubble(agentModel.action, agentModel.valid);
        speechBubblePositionText.GetComponent<Text>().text = "x:" + agentModel.x + " y:" + agentModel.y;
        action = agentModel.action;
        finished = false;

        if (!agentModel.valid)
        {
            valid = false;
            speed = 0;
            animator.SetBool("moving", false);
            animator.SetBool("invalid", true);
            stateManager.AddToValidCounter(1);
            ChangeAnimationState(AGENT_INVALID);
        }
        if (agentModel.valid)
        {
            valid = true;
            animator.SetBool("invalid", false);
            stateManager.AddToInValidCounter(1);
            switch(agentModel.action){
                case "Action[CLEAN_UP]" : 
                    StartCoroutine(BubblesAction());
                    ChangeAnimationState(AGENT_CLEANING);break;
                case "Action[ITEM_ACTION]" :  
                    ChangeAnimationState(AGENT_PICKUP); break;
                case "Action[NORTH]": 
                case "Action[NORTHEAST]": 
                case "Action[NORTHWEST]": 
                case "Action[EAST]": 
                case "Action[SOUTHEAST]": 
                case "Action[SOUTH]": 
                case "Action[SOUTHWEST]" :
                    ChangeAnimationState(AGENT_MOVING); break;
                default: ChangeAnimationState(AGENT_IDLE); break;
            }
        }
    }

    float GetActionLength(string action){
        switch(action){
                case AGENT_CLEANING : 
                    return 2f;
                case AGENT_PICKUP :  
                    return 2f;
                case AGENT_MOVING:
                    return 0.5f;
                default: return 1f;
        }
    }


    IEnumerator BubblesAction()
    {
        var stateManager = system.GetComponent<EnvironmentStateManager>();
        yield return StartCoroutine(WaitFor.Frames(10));
        stateManager.SpawnCleaningBubbles(goalPosition);
    }

    public void MoveRobotTo(Vector3 position)
    {
        animator.SetBool("moving", true);
        this.transform.position = Vector3.MoveTowards(transform.position, goalPosition, Time.deltaTime * speed);

        if (backwardBuffer)
        {
            this.transform.Rotate(new Vector3(0, 180, 0), Space.Self);
        }
        Vector3 difference = goalPosition - currentpos;
        var name = this.transform.GetChild(4);
        if (difference != Vector3.zero)
        {
            this.transform.forward = difference;
            name.transform.eulerAngles = Vector3.zero;
        }
    }

    void ChangeAnimationState(string state)
    {
        if (currentAnimationState == state)
        {
            StartCoroutine(WaitToFinishAnimation(state));
        }
        animator.Play(state);
        currentAnimationState = state;
        StartCoroutine(WaitToFinishAnimation(state));
    }

//TODO change length according to action
    IEnumerator WaitToFinishAnimation(string action)
    {
        yield return new WaitForSeconds(GetActionLength(action));
        finished = true;
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

        var canvas = agentBody.transform.GetChild(4).gameObject;
        var nameTag = canvas.transform.GetChild(0).gameObject;
    }

    public void UpdateSpeechBubble(string action, bool valid)
    {
        var rectTrans = speechBubble.GetComponent<RectTransform>();
        var camera = Camera.main;
        rectTrans.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
        Vector3 contentRotation = rectTrans.rotation.eulerAngles;

        if (valid)
        {
            speechBubbleInvalidityImage.SetActive(false);
            speechBubble.GetComponent<Image>().color = new Color32(60, 255, 140, 255);
            speechBubble.GetComponent<Blinking>().startBlinking = false;

            switch (action)
            {
                case "Action[CLEAN_UP]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = cleaningSprite;
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z));
                    speechBubbleDirectionText.SetActive(false);
                    speechBubble.SetActive(true);
                    break;
                case "Action[ITEM_ACTION]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = itemPickSprite;
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z));
                    speechBubbleDirectionText.SetActive(false);
                    speechBubble.SetActive(true);
                    break;
                case "Action[NORTH]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "N";
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[NORTHEAST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "N/E";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 315));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[EAST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "E";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 270));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[SOUTHEAST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "S/E";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 225));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[SOUTH]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "S";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 180));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[SOUTHWEST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "S/W";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 135));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[WEST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "W";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 90));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[NORTHWEST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "N/W";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 45));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;

                default: speechBubble.SetActive(false); break;
            }
        }
        else if (!valid)
        {
            speechBubble.GetComponent<Image>().color = new Color32(255, 60, 80, 255);
            switch (action)
            {
                case "Action[CLEAN_UP]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = cleaningSprite;
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z));
                    speechBubbleDirectionText.SetActive(false);
                    speechBubble.SetActive(true);
                    break;
                case "Action[ITEM_ACTION]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = itemPickSprite;
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z));
                    speechBubbleDirectionText.SetActive(false);
                    speechBubble.SetActive(true);
                    break;
                case "Action[NORTH]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "N";
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[NORTHEAST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "N/E";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 315));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[EAST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "E";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 270));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[SOUTHEAST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "S/E";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 225));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[SOUTH]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "S";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 180));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[SOUTHWEST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "S/W";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 135));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[WEST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "W";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 90));
                    speechBubbleDirectionText.SetActive(true);
                    speechBubble.SetActive(true); break;
                case "Action[NORTHWEST]":
                    speechBubbleContent.transform.GetComponent<Image>().sprite = directionSprite;
                    speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = "N/W";
                    speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + 45));
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

    public void SetAgentNameInSpeechBubble(string name)
    {
        speechBubbleAgentName.transform.GetComponent<Text>().text = GetAgentNumberFromNameAsString(name);

    }

    #region helper methods
    string GetAgentNumberFromNameAsString(string name)
    {
        string nameNew = Regex.Replace(name, "[^0-9]", "");
        return nameNew;
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
        Vector3 center = system.GetComponent<EnvironmentStateManager>().environmentCenter;
        return new Vector3(x - center.x, y, z - center.z);
    }

    public void SetAgentModel(Agent agent)
    {
        this.agentModel = agent;
    }
    #endregion

}
