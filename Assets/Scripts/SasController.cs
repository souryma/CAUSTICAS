using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SasController : MonoBehaviour
{

    [SerializeField] private bool needKey = false;
    [SerializeField] private GameObject OKSign;
    [SerializeField] private bool needDivingSuit = false;

    private Animator mAnimator;
    private Collider mCollider;

    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
        mCollider = GetComponent<Collider>();
    }


    private void OnTriggerEnter(Collider collider)
    {

        //if the collider is the player
        if (collider.gameObject.tag == "Player")
        {
            if (mAnimator != null)
            {
                if(needKey)
                {
                    if (GameManager.instance.hasKey)
                    {
                        OKSign.SetActive(true);
                        StartCoroutine(Open());
                    }
                    return;
                }

                if(needDivingSuit)
                {
                    if(GameManager.instance.hasDivingSuit)
                    {
                        StartCoroutine(Open());
                    }
                    return;
                }


                StartCoroutine(Open());

            }
            
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        //if the collider is the player
        if (collider.gameObject.tag == "Player")
        {
            if (mAnimator != null)
            {
                if(needKey)
                {
                    if (GameManager.instance.hasKey)
                    {
                        StartCoroutine(Close());
                    }
                    return;
                }

                if (needDivingSuit)
                {
                    if (GameManager.instance.hasDivingSuit)
                    {
                        StartCoroutine(Close());
                    }
                    return;
                }

                StartCoroutine(Close());

            }
        }
    }

    IEnumerator Open()
    {

        // Play the animation
        mAnimator.SetTrigger("TrOpen");

        yield return new WaitForSeconds(1.0f);
        mCollider.enabled = false;

    }

    IEnumerator Close()
    {

        // Play the animation
        mAnimator.SetTrigger("TrClose");

        yield return new WaitForSeconds(1.0f);
        mCollider.enabled = true;

    }
}
