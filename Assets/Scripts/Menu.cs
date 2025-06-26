using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private GameObject shopPanel;

    [Header("Buttons")]
    [SerializeField] private Button showLevelButton;
    [SerializeField] private Button instructionButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button iconButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button soundOnButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button quitShopButton;

    [Header("Level Buttons")]
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button level4Button;
    [SerializeField] private Button level5Button;

    [Header("Sound")]
    [SerializeField] private GameObject soundBackground;

    [SerializeField] private GameObject icon;

    private AudioSource bgmSource;

    private void Awake()
    {
        if (soundBackground != null)
        {
            bgmSource = soundBackground.GetComponent<AudioSource>();
            if (bgmSource != null && !bgmSource.isPlaying)
            {
                bgmSource.Play();
            }
        }
    }

    private void Start()
    {
        // Assign button listeners
        showLevelButton.onClick.AddListener(OnShowLevel);
        instructionButton.onClick.AddListener(OnInstruction);
        quitButton.onClick.AddListener(OnQuit);
        iconButton.onClick.AddListener(OnIcon);
        backButton.onClick.AddListener(OnBack);
        if (soundOnButton != null)
            soundOnButton.onClick.AddListener(OnSoundOn);
        if (shopButton != null)
            shopButton.onClick.AddListener(OnShop);
        if (quitShopButton != null)
            quitShopButton.onClick.AddListener(OnQuitShop);

        // Level buttons
        if (level1Button != null)
            level1Button.onClick.AddListener(() => LoadLevel("Level1"));
        if (level2Button != null)
            level2Button.onClick.AddListener(() => LoadLevel("Level2"));
        if (level3Button != null)
            level3Button.onClick.AddListener(() => LoadLevel("Level3"));
        if (level4Button != null)
            level4Button.onClick.AddListener(() => LoadLevel("Level4"));
        if (level5Button != null)
            level5Button.onClick.AddListener(() => LoadLevel("Level5"));

        // Ensure initial state
        menuScreen.SetActive(true);
        levelSelectPanel.SetActive(false);
        instructionPanel.SetActive(false);
        if (shopPanel != null)
            shopPanel.SetActive(false);

        // Check saved sound state
        bool soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        if (soundOn)
        {
            if (icon != null)
                icon.SetActive(true);
            if (soundBackground != null)
                soundBackground.SetActive(true);
            if (bgmSource != null && !bgmSource.isPlaying)
                bgmSource.Play();
            if (soundOnButton != null)
                soundOnButton.gameObject.SetActive(false);
        }
        else
        {
            if (icon != null)
                icon.SetActive(false);
            if (soundBackground != null)
                soundBackground.SetActive(false);
            if (bgmSource != null && bgmSource.isPlaying)
                bgmSource.Stop();
            if (soundOnButton != null)
                soundOnButton.gameObject.SetActive(true);
        }
    }

    private void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnShowLevel()
    {
        menuScreen.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    private void OnInstruction()
    {
        menuScreen.SetActive(false);
        instructionPanel.SetActive(true);
    }

    private void OnQuit()
    {
        Application.Quit();
    }

    private void OnIcon()
    {
        if (icon != null)
            icon.SetActive(false);
        if (soundBackground != null)
            soundBackground.SetActive(false);
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Stop();

        if (soundOnButton != null)
            soundOnButton.gameObject.SetActive(true);

        PlayerPrefs.SetInt("SoundOn", 0);
        PlayerPrefs.Save();
    }

    private void OnSoundOn()
    {
        if (icon != null)
            icon.SetActive(true);
        if (soundBackground != null)
            soundBackground.SetActive(true);
        if (bgmSource != null && !bgmSource.isPlaying)
            bgmSource.Play();

        if (soundOnButton != null)
            soundOnButton.gameObject.SetActive(false);

        PlayerPrefs.SetInt("SoundOn", 1);
        PlayerPrefs.Save();
    }

    private void OnBack()
    {
        instructionPanel.SetActive(false);
        menuScreen.SetActive(true);
    }

    private void OnShop()
    {
        menuScreen.SetActive(false);
        if (shopPanel != null)
            shopPanel.SetActive(true);
    }

    private void OnQuitShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
        menuScreen.SetActive(true);
    }
}
