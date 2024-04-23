using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Animator mAnimator;
    private Collider weaponCollider;

    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
        weaponCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Hit());
        }

        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Reverse());
        }
    }

    IEnumerator Hit()
    { 
        if (mAnimator != null)
        {
            // Play the animation
            mAnimator.SetTrigger("TrHit");

            yield return new WaitForSeconds(0.2f);
            weaponCollider.enabled = true;
            yield return new WaitForSeconds(0.2f);
            weaponCollider.enabled = false;
        }
    }

    IEnumerator Reverse()
    {
        if (mAnimator != null)
        {
            // Play the animation
            mAnimator.SetTrigger("TrReverse");

            yield return new WaitForSeconds(0.4f);
            weaponCollider.enabled = true;
            yield return new WaitForSeconds(0.3f);
            weaponCollider.enabled = false;
        }
    }

    public void DisableCollider()
    {
        weaponCollider.enabled = false;
    }

}
