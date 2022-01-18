using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AgentCollision : MonoBehaviour
{
    /*
   This script is probably not needed anymore!!
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
       if (collider.tag == "TrashBoundary")
       {
           string trashName = collider.transform.parent.name;
           string trashIndexString = trashName.Substring(trashName.Length - 1);
           int trashIndex;
           int.TryParse(trashIndexString, out trashIndex);
           Debug.Log("---------------------------------We hit trash: " + trashName + " with index: " + trashIndex);

           collider.transform.parent.GetComponent<Trash>().DecreaseFillAmount();

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
            animator.SetBool("collision", collision);

        }
     if (collider.tag == "Door")
     {
         var doorAn = collider.GetComponentInChildren<Animator>();
         doorAn.SetTrigger("OpenClose");
         collision = false;
     }
}

    void OnTriggerExit(Collider collider)
    {

        if (collider.tag == "Door")
         {
             var doorAn = collider.GetComponentInChildren<Animator>();
             doorAn.SetTrigger("OpenClose");
         }
        //else
        //{
        collision = false;
        animator.SetBool("collision", collision);
        //}

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
    }*/
}
