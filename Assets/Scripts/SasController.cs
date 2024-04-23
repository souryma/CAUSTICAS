using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SasController : MonoBehaviour
{

    [SerializeField] private bool needKey = false;

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
                        mAnimator.SetTrigger("TrOpen");
                        //Desactivate the collider
                        mCollider.enabled = false;
                    }
                    return;
                }

                mAnimator.SetTrigger("TrOpen");
                //Desactivate the collider
                mCollider.enabled = false;


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
                        mAnimator.SetTrigger("TrClose");
                        //Activate the collider
                        mCollider.enabled = true;
                    }
                }

                mAnimator.SetTrigger("TrClose");
                //Activate the collider
                mCollider.enabled = true;

            }
        }
    }       
}
