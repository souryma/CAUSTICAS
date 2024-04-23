using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] private Volume _postProcessData;
    [SerializeField] private AudioSource _hitSound;
    private LensDistortion _lensDistortion;
    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;

    private void Start()
    {
        _postProcessData.profile.TryGet(out _lensDistortion);
        _postProcessData.profile.TryGet(out _vignette);
        _postProcessData.profile.TryGet(out _chromaticAberration);
        
        _lensDistortion.active = false;
        _vignette.active = false;
        _chromaticAberration.active = false;
    }

    //get and set player gameobject
    public GameObject GetPlayer()
    {
        return Player;
    }

    public void GetDivingSuit()
    {
        hasDivingSuit = true;
        
        _lensDistortion.active = true;
        _vignette.active = true;
        _chromaticAberration.active = true;
    }

    public void PlayHitSound()
    {
        _hitSound.volume = 5;
        _hitSound.Play();
    }
}
