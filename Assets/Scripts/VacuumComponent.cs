using System;
using DG.Tweening;
using UnityEngine;

public class VacuumComponent : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [SerializeField, Range(0, 0.3f)] private float speed = 0.03f;

    [SerializeField] private MeshRenderer eyeRenderer;

    [SerializeField] private float rotationTime = 0.2f;
    [SerializeField] private int rotationAngle = 10;

    private RaycastHit frontRay;
    private RaycastHit rightRay;
    private RaycastHit leftRay;

    private bool isVaccumAngry = false;
    private bool isTurning = false;
    private bool isColorRed = false;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            isVaccumAngry = true;

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

    private void TweenCompleted()
    {
        isTurning = false;
    }

    private RaycastHit DrawSphereCast(Vector3 direction)
    {
        RaycastHit hit;

        Physics.SphereCast(new Vector3(transform.position.x, 1f, transform.position.z), 0.7f, direction, out hit);

        hit.point = new Vector3(hit.point.x, 0, hit.point.z);

        return hit;
    }
}