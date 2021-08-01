using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AgentCollision : MonoBehaviour
{

    Text warningText;
    Image redScreenFlashImage;

    public float flashDelay = 1f;

    Animator animator;

    bool collision = false;

    private void Start()
    {
        var system = GameObject.FindWithTag("System");
        redScreenFlashImage = system.GetComponent<UIGlobals>().redScreenFlashImage;
        animator = GetComponent<Animator>();
    }
    void OnTriggerEnter(Collider collider)
    {
        collision = true;
        Debug.Log("---------------------------------We hit: " + collider.name);
        if (collider.tag == "Trash")
        {
            warningText.text = "Found Trash!!!!";
            warningText.gameObject.SetActive(true);
            animator.SetBool("foundDirt", collision);

        }
        if (collider.tag == "Wall")
        {
            warningText.text = "Hit Wall!!!!";
            warningText.gameObject.SetActive(true);
            FlashWhenHit(flashDelay);
            animator.SetBool("collision", collision);

        }

        if (collider.tag == "Agent")
        {
            warningText.text = "Hit another Agent!!!!";
            warningText.gameObject.SetActive(true);
            animator.SetBool("collision", collision);

        }
    }

    void OnTriggerExit()
    {
        warningText.gameObject.SetActive(false);
        collision = false;
        animator.SetBool("collision", collision);
        animator.SetBool("foundDirt", collision);

    }

    public void FlashWhenHit(float delay)
    {
        StartCoroutine(FlashWhenHitCoroutine(delay));
    }
    IEnumerator FlashWhenHitCoroutine(float delay)
    {
        var imageColor = redScreenFlashImage.GetComponent<Image>().color;
        imageColor = new Color(180, 0, 0, 0.2f);
        redScreenFlashImage.GetComponent<Image>().color = imageColor;
        yield return new WaitForSeconds(delay);
        imageColor = new Color(180, 0, 0, 0);
        redScreenFlashImage.GetComponent<Image>().color = imageColor;
    }

    public void setWarningText(Text text)
    {
        this.warningText = text;
    }

    public bool GetCollision()
    {
        return this.collision;
    }

    public void SetCollision(bool collision)
    {
        this.collision = collision;
    }
}
