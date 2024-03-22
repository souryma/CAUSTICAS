using UnityEngine;
using Random = UnityEngine.Random;

public class FishGroupBehaviour : MonoBehaviour
{
    [SerializeField] private FishGroupData fishGroup;

    private Vector3 _groupDirection;
    private GameObject target;

    [SerializeField, Range(0, 10)] private float speed = 1;

    private void Start()
    {
        target = new GameObject(fishGroup.name);

        fishGroup.SpawnFishes(gameObject);
    }

    private void Update()
    {
        target.transform.position = new Vector3(target.transform.position.x + Random.Range(-1, 1),
            target.transform.position.y + Random.Range(-1, 1),
            target.transform.position.z + Random.Range(-1, 1));
        
        Vector3 directionToTarget = target.transform.position - transform.position;

        directionToTarget = directionToTarget.normalized;

        transform.position += Time.deltaTime * speed * directionToTarget;
        transform.LookAt(target.transform.position);
    }
}