using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool hasDivingSuit = false;
    public bool hasKey = false;
    public bool vaccumIsAlive = true;

    [SerializeField] private GameObject Player;

    //get and set player gameobject
    public GameObject GetPlayer()
    {
        return Player;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
