using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMove(new Vector3(255, transform.position.y, 255), 600);
    }
}
