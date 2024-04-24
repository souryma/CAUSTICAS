using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
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
    [SerializeField] private GameObject _bubulles;
    [SerializeField] private GameObject UI;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject endingText;
    [SerializeField] private FirstPersonMovement playerMovement;

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

    private bool oxygenSliderActivation = false;

    private Color underWaterColor = new Color(0, 0.3607843f, 0.7176471f);

    public void SetUnderWaterColor()
    {
        _portalCard.material.color = underWaterColor;
        _portalCard.material.SetFloat("_Alpha", 1);
        _backgroundSource.clip = _underwaterAudioClip;
        _backgroundSource.loop = true;
        _backgroundSource.Play();
        _bubulles.SetActive(true);

        playerRB.drag = 5;

        oxygenSliderActivation = true;
    }

    private float _fadeDistance = 40;

    private void Update()
    {
        if (oxygenSliderActivation)
        {
            slider.value -= 0.06f * Time.deltaTime;
            float h, s, v;
            Color.RGBToHSV(underWaterColor, out h, out s, out v);
            v -= 0.06f * Time.deltaTime;
            underWaterColor = Color.HSVToRGB(h, s, v);
            _portalCard.material.SetColor("_Color", underWaterColor);
            _fadeDistance -= 0.2f;
            if (_fadeDistance < 0)
                _fadeDistance = 0;
            _portalCard.material.SetFloat("_FadeDistance", _fadeDistance);

            if (slider.value == 0)
            {
                endingText.SetActive(true);
                slider.gameObject.SetActive(false);
                playerMovement.speed = 0;
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GetDivingSuit()
    {
        hasDivingSuit = true;

        _lensDistortion.active = true;
        _vignette.active = true;
        _chromaticAberration.active = true;

        UI.SetActive(true);
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