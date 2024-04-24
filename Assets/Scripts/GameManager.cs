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
    [SerializeField] private AudioSource _waterSound;
    [SerializeField] private MeshRenderer _portalCard;
    [SerializeField] private AudioSource _backgroundSource;
    [SerializeField] private AudioClip _underwaterAudioClip;
    [SerializeField] private Rigidbody playerRB;

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

    public void SetUnderWaterColor()
    {
        _portalCard.material.color = new Color(0, 0.3607843f, 0.7176471f);
        _backgroundSource.clip = _underwaterAudioClip;

        playerRB.drag = 5;
    }

    public void GetDivingSuit()
    {
        hasDivingSuit = true;
        
        _lensDistortion.active = true;
        _vignette.active = true;
        _chromaticAberration.active = true;
    }
    
    public void PlayerWaterFillingSound()
    {
        _waterSound.Play();
        _waterSound.loop = false;
    }

    public void PlayHitSound()
    {
        _hitSound.volume = 5;
        _hitSound.Play();
    }
}
