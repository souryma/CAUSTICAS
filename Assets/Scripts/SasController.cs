using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SasController : MonoBehaviour
{
    [SerializeField] private bool needKey = false;
    [SerializeField] private GameObject OKSign;
    [SerializeField] private bool needDivingSuit = false;
    [SerializeField] private AudioSource okAudioSource;
    [SerializeField] private AudioSource OpeningSource;
    [SerializeField] private AudioSource ClosingSource;
    [SerializeField] private Material GreenMaterial;
    [SerializeField] private Material RedMaterial;
    [SerializeField] private Material OrangeMaterial;
    [SerializeField] private MeshRenderer Light;
    [SerializeField] private List<GameObject> _roomLights;
    [SerializeField] private GameObject waterPlane;
    [SerializeField] private GameObject waterCollider;


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
                if (needKey)
                {
                    if (GameManager.instance.hasKey)
                    {
                        OKSign.SetActive(true);
                        okAudioSource.Play();
                        StartCoroutine(Open());
                    }

                    return;
                }

                if (needDivingSuit)
                {
                    if (GameManager.instance.hasDivingSuit)
                    {
                        StartCoroutine(OpenEndingSas());
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
                if (needKey)
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

    private IEnumerator OpenEndingSas()
    {
        Light.material = OrangeMaterial;
        foreach (var light in _roomLights)
        {
            light.SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }
        waterPlane.SetActive(true);
        waterPlane.transform.DOMoveY(326/100, 5);
        yield return new WaitForSeconds(4);
        waterCollider.SetActive(false);
        yield return new WaitForSeconds(1);
        Light.material = GreenMaterial;
        StartCoroutine(Open());
    }

    IEnumerator Open()
    {
        if (!OpeningSource.isPlaying)
            OpeningSource.Play();

        // Play the animation
        mAnimator.SetTrigger("TrOpen");

        yield return new WaitForSeconds(1.0f);
        mCollider.enabled = false;
    }

    IEnumerator Close()
    {
        if (!ClosingSource.isPlaying)
            ClosingSource.Play();

        // Play the animation
        mAnimator.SetTrigger("TrClose");

        yield return new WaitForSeconds(1.0f);
        mCollider.enabled = true;
    }
}