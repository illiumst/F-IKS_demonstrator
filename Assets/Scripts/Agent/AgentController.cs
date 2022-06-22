using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;



public class AgentController : MonoBehaviour
{

    #region Attribute declarations

    //========================================================================================================//
    //================================= GLOBAL OBJECTS =======================================================//
    //========================================================================================================//

    GameObject system;
    public Animator animator;
    Agent agentModel;

    //========================================================================================================//
    //================================= UI ELEMENTS ==========================================================//
    //========================================================================================================//

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

    //========================================================================================================//
    //================================= HELPING FIELDS =======================================================//
    //========================================================================================================//

    public Vector3 currentpos;
    public Vector3 goalPosition;
    public float playBackSpeed;
    public float bufferAcceleration = 1f;
    public float sliderValue;
    public bool playingSequence;
    public bool valid;
    public bool backwardBuffer;
    public bool finished = false;
    public string action;
    float currentActionLength = 0;
    string currentAnimationState = "";

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

        agentCanvas = FindGameObjectInChildWithTag(gameObject, "AgentCanvas");
        speechBubble = FindGameObjectInChildWithTag(agentCanvas, "SpeechBubble");
        speechBubbleContent = FindGameObjectInChildWithTag(speechBubble, "SpeechBubbleContent");
        speechBubbleInvalidityImage = FindGameObjectInChildWithTag(speechBubble, "SpeechBubbleInvalid");
        speechBubbleInvalidityImage.SetActive(false);
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

    void Update()
    {
        currentpos = this.transform.position;
        var stateManager = system.GetComponent<EnvironmentStateManager>();

        playBackSpeed = stateManager.playBackSpeed * bufferAcceleration;
        animator.SetFloat("animSpeed", playBackSpeed);

        //sequence with playbutton is being played
        if (playingSequence && valid)
        {
            MoveRobotTo(goalPosition);
        }

        if (!playingSequence && currentpos != goalPosition && goalPosition != null && valid)
        {
            MoveRobotTo(goalPosition);
        }

        if (!playingSequence)
        {
            ChangeAnimationState(AgentConstants.ANIMATION_IDLE);
            speechBubble.SetActive(false);
        }
    }
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
            stateManager.AddToInValidCounter(1);
            ChangeAnimationState(AgentConstants.ANIMATION_INVALID);
        }
        if (agentModel.valid)
        {
            valid = true;
            stateManager.AddToValidCounter(1);
            switch (agentModel.action)
            {
                case AgentConstants.ACTION_CLEAN_UP:
                    StartCoroutine(BubblesAction());
                    ChangeAnimationState(AgentConstants.ANIMATION_CLEANING); break;
                case AgentConstants.ACTION_ITEM:
                    ChangeAnimationState(AgentConstants.ANIMATION_PICKUP); break;
                case AgentConstants.ACTION_NORTH:
                case AgentConstants.ACTION_NORTHEAST:
                case AgentConstants.ACTION_NORTHWEST:
                case AgentConstants.ACTION_EAST:
                case AgentConstants.ACTION_SOUTHEAST:
                case AgentConstants.ACTION_SOUTH:
                case AgentConstants.ACTION_SOUTHWEST:
                case AgentConstants.ACTION_WEST:
                    ChangeAnimationState(AgentConstants.ANIMATION_MOVING); break;
                default: ChangeAnimationState(AgentConstants.ANIMATION_IDLE); break;
            }
        }
        else
        {
            Debug.Log(" No validity data");
        }
    }

    float GetActionLength(string action)
    {
        //Debug.Log("__Getting Action length: playbackSpeed " + playBackSpeed);
        switch (action)
        {
            case AgentConstants.ANIMATION_CLEANING:
                return 2f / playBackSpeed;
            case AgentConstants.ANIMATION_PICKUP:
                return 2f / playBackSpeed;
            case AgentConstants.ANIMATION_MOVING:
                return 0.3f / playBackSpeed;
            case AgentConstants.ANIMATION_INVALID:
                return 1f / playBackSpeed;
            default: return 0.5f / playBackSpeed;
        }
    }


    IEnumerator BubblesAction()
    {
        var stateManager = system.GetComponent<EnvironmentStateManager>();
        yield return StartCoroutine(WaitFor.Frames((int)(10 / playBackSpeed)));
        stateManager.SpawnCleaningBubbles(goalPosition);
    }

    public void MoveRobotTo(Vector3 position)
    {
        var speed = 3f * playBackSpeed;
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
        finished = false;
        if (currentAnimationState == state)
        {
            StartCoroutine(WaitToFinishAnimation(state));
        }
        animator.Play(state);
        currentAnimationState = state;
        StartCoroutine(WaitToFinishAnimation(state));
    }

    IEnumerator WaitToFinishAnimation(string action)
    {
        currentActionLength = GetActionLength(action);
        yield return new WaitForSeconds(currentActionLength);
        Debug.Log("Action finished...");
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
        string validityString = AgentConstants.INVALID;
        var color = Color.red;
        var textColor = Color.red;
        if (valid)
        {
            textColor = new Color32(0, 160, 20, 255);
            color = Color.green;
            validityString = AgentConstants.VALID;
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

        switch (action)
        {
            case AgentConstants.ACTION_CLEAN_UP:
                SetSpeechBubbleContent(cleaningSprite, contentRotation, "", 0, false); break;

            case AgentConstants.ACTION_ITEM:
                SetSpeechBubbleContent(itemPickSprite, contentRotation, "", 0, false); break;

            case AgentConstants.ACTION_NORTH:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_NORTH, 0, true); break;

            case AgentConstants.ACTION_NORTHEAST:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_NORTHEAST, 315, true); break;

            case AgentConstants.ACTION_EAST:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_EAST, 270, true); break;

            case AgentConstants.ACTION_SOUTHEAST:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_SOUTHEAST, 225, true); break;

            case AgentConstants.ACTION_SOUTH:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_SOUTH, 180, true); break;

            case AgentConstants.ACTION_SOUTHWEST:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_SOUTHWEST, 135, true); break;

            case AgentConstants.ACTION_WEST:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_WEST, 90, true); break;

            case AgentConstants.ACTION_NORTHWEST:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_NORTHWEST, 45, true); break;

            default: speechBubble.SetActive(false); break;
        }
        if (valid)
        {
            speechBubble.GetComponent<Image>().color = new Color32(60, 255, 140, 255);

        }
        else if (!valid)
        {
            speechBubble.GetComponent<Image>().color = new Color32(255, 60, 80, 255);
        }
        else
        {
            speechBubble.SetActive(false);
        }

    }

    void SetSpeechBubbleContent(Sprite sprite, Vector3 contentRotation, string text, float rotation, bool directionText)
    {
        speechBubbleContent.transform.GetComponent<Image>().sprite = sprite;
        speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = text;
        speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + rotation));
        speechBubbleDirectionText.SetActive(directionTex);
        speechBubble.SetActive(true);
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

    public float GetCurrentActionLength()
    {
        return this.currentActionLength;
    }
    #endregion

}
