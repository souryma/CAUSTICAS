using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class VacuumComponent : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [SerializeField, Range(0, 0.3f)] private float speed = 0.03f;

    [SerializeField] private MeshRenderer eyeRenderer;

    [SerializeField] private float rotationTime = 0.2f;
    [SerializeField] private int rotationAngle = 10;

    [SerializeField] private AudioSource _vacuumAspireSource;
    [SerializeField] private AudioSource _vacuumVoiceSource;

    [SerializeField] private GameObject _explodedVacuum;
    [SerializeField] private BoxCollider _colliderComponent;
    [SerializeField] private Rigidbody _rbComponent;

    private RaycastHit frontRay;
    private RaycastHit rightRay;
    private RaycastHit leftRay;

    private bool isVaccumAngry = false;
    private bool isTurning = false;
    private bool isColorRed = false;

    private int life = 3;


    private void Update()
    {
        PlayVacuumSound();

        if (isVaccumAngry)
        {
            if (isColorRed == false)
            {
                eyeRenderer.material.color = Color.red;
                eyeRenderer.material.SetColor("_EmissionColor", Color.red);
                isColorRed = true;
            }

            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);

            transform.LookAt(target.transform);
        }
        else
        {
            if (isTurning == false)
            {
                frontRay = DrawSphereCast(transform.forward);
                rightRay = DrawSphereCast(transform.right);
                leftRay = DrawSphereCast(-transform.right);

                transform.position = Vector3.MoveTowards(transform.position, frontRay.point, speed);

                //transform.LookAt(frontRay.point);

                if (rightRay.distance > frontRay.distance)
                {
                    isTurning = true;
                    transform.DORotate(
                        new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + rotationAngle,
                            transform.localEulerAngles.z), rotationTime).onComplete = TweenCompleted;
                }
                else if (leftRay.distance > frontRay.distance)
                {
                    isTurning = true;
                    transform.DORotate(
                        new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - rotationAngle,
                            transform.localEulerAngles.z), rotationTime).onComplete = TweenCompleted;
                }
            }
        }
    }


    // Play loop if angry, otherwise, play at random
    private void PlayVacuumSound()
    {
        if (isVaccumAngry)
        {
            if (!_vacuumAspireSource.isPlaying)
                _vacuumAspireSource.Play();

            if (!_vacuumVoiceSource.isPlaying)
                _vacuumVoiceSource.Play();
                
        }
        else
        {
            if (Random.Range(0, 100) == 10)
            {
                if (!_vacuumAspireSource.isPlaying)
                    _vacuumAspireSource.Play();
            }
        }
    }


    private void TweenCompleted()
    {
        isTurning = false;
    }

    private RaycastHit DrawSphereCast(Vector3 direction)
    {
        RaycastHit hit;

        Physics.SphereCast(new Vector3(transform.position.x, 1f, transform.position.z), 0.7f, direction,
            out hit);

        hit.point = new Vector3(hit.point.x, 0, hit.point.z);

        return hit;
    }

    // Do the vaccuum life system. The vaccum has 3 points of life. When the player hit the vaccum the first time,
    // it goes angry and it remove a life point. We have to hit it 2 times more to destroy it.


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Weapon"))
        {
            collider.GetComponent<WeaponController>().DisableCollider();

            GameManager.instance.PlayHitSound();

            life--;

            //Make the vaccum angry
            target = GameManager.instance.GetPlayer();
            isVaccumAngry = true;


            if (life == 0)
            {
                GameManager.instance.hasKey = true;
                killVacuum();
                //Destroy(gameObject);
            }
        }
    }

    private void killVacuum()
    {
        Destroy(_rbComponent);
        Destroy(_colliderComponent);
        eyeRenderer.transform.parent.gameObject.SetActive(false);
        Destroy(_vacuumAspireSource.gameObject);
        Destroy(_vacuumVoiceSource.gameObject);
        _explodedVacuum.SetActive(true);
        Destroy(this);
    }
}