using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AgentCollision : MonoBehaviour
{

    Text warningText;
    Image redScreenFlashImage;

    public float flashDelay = 1f;

    Animator animator;

    ObjectPooler objectPooler;

    bool collision = false;

    private void Start()
    {
        var system = GameObject.FindWithTag("System");
        redScreenFlashImage = system.GetComponent<UIGlobals>().redScreenFlashImage;
        animator = GetComponent<Animator>();
        objectPooler = ObjectPooler.Instance;

    }
    void OnTriggerEnter(Collider collider)
    {
        collision = true;
        Debug.Log("---------------------------------We hit: " + collider.name);
        if (collider.tag == "TrashBoundary")
        {
            string trashName = collider.transform.parent.name;
            string trashIndexString = trashName.Substring(trashName.Length - 1);
            int trashIndex;
            int.TryParse(trashIndexString, out trashIndex);
            Debug.Log("---------------------------------We hit trash: " + trashName + " with index: " + trashIndex);

            collider.transform.parent.GetComponent<Trash>().DecreaseFillAmount();

            warningText.text = "Found Trash!!!!";
            warningText.gameObject.SetActive(true);
            animator.SetTrigger("foundTrash");

        }
        if (collider.tag == "Wall")
        {
            //FlashWhenHit(flashDelay);
            //TODO check what's wrong with animnator: null reference exception
            //animator.SetBool("collision", collision);

        }

        if (collider.tag == "Agent")
        {
            warningText.text = "Hit another Agent!!!!";
            warningText.gameObject.SetActive(true);
            animator.SetBool("collision", collision);

        }
        if (collider.tag == "Door")
        {
            var doorAn = collider.GetComponentInChildren<Animator>();
            doorAn.SetTrigger("OpenClose");
            warningText.text = "Hit Door!!!!";
            collision = false;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        Debug.Log("---------------------------------We exited: " + collider.name);

        if (collider.tag == "Door")
        {
            var doorAn = collider.GetComponentInChildren<Animator>();
            doorAn.SetTrigger("OpenClose");
        }
        else
        {
            collision = false;
            animator.SetBool("collision", collision);
        }

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
