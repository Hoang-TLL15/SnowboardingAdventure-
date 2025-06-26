using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance { get; private set; }

    private readonly int[] skinPrices = { 0, 6, 6, 10 };

    public int SkinCount => skinPrices.Length;

    [Header("Skin Select Buttons")]
    [SerializeField] private Button skin1Button;
    [SerializeField] private Button skin2Button;
    [SerializeField] private Button skin3Button;
    [SerializeField] private Button skin4Button;

    [Header("Skin Button Labels")]
    [SerializeField] private Text skin1Label;
    [SerializeField] private Text skin2Label;
    [SerializeField] private Text skin3Label;
    [SerializeField] private Text skin4Label;

    [Header("Star Count UI")]
    [SerializeField] private Text starCountText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Assign button listeners
        if (skin1Button != null) skin1Button.onClick.AddListener(() => OnSkinButtonClicked(0));
        if (skin2Button != null) skin2Button.onClick.AddListener(() => OnSkinButtonClicked(1));
        if (skin3Button != null) skin3Button.onClick.AddListener(() => OnSkinButtonClicked(2));
        if (skin4Button != null) skin4Button.onClick.AddListener(() => OnSkinButtonClicked(3));

        UpdateSkinButtons();
    }

    private void UpdateSkinButtons()
    {
        UpdateSkinButton(0, skin1Label);
        UpdateSkinButton(1, skin2Label);
        UpdateSkinButton(2, skin3Label);
        UpdateSkinButton(3, skin4Label);

        // Update star count display
        if (starCountText != null)
        {
            starCountText.text = $"{GetStars()}";
        }
    }

    private void UpdateSkinButton(int index, Text label)
    {
        if (label == null) return;
        if (IsSkinUnlocked(index))
        {
            if (GetSelectedSkin() == index)
                label.text = "Selected";
            else
                label.text = "Select";
        }
        else
        {
            label.text = $"{skinPrices[index]} Stars";
        }
    }

    private void OnSkinButtonClicked(int skinIndex)
    {
        if (IsSkinUnlocked(skinIndex))
        {
            SelectSkin(skinIndex);
        }
        else
        {
            if (TryBuySkin(skinIndex))
            {
                SelectSkin(skinIndex);
            }
        }
        UpdateSkinButtons();
    }

    public bool IsSkinUnlocked(int skinIndex)
    {
        if (skinIndex == 0) return true; // Skin 1 luôn free
        return PlayerPrefs.GetInt($"SkinUnlocked_{skinIndex}", 0) == 1;
    }

    public bool TryBuySkin(int skinIndex)
    {
        if (IsSkinUnlocked(skinIndex)) return true;

        int stars = PlayerPrefs.GetInt("Stars", 0);
        int price = skinPrices[skinIndex];
        if (stars >= price)
        {
            stars -= price;
            PlayerPrefs.SetInt("Stars", stars);
            PlayerPrefs.SetInt($"SkinUnlocked_{skinIndex}", 1);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    public bool SelectSkin(int skinIndex)
    {
        if (IsSkinUnlocked(skinIndex))
        {
            PlayerPrefs.SetInt("SelectedSkin", skinIndex);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    public int GetSelectedSkin()
    {
        return PlayerPrefs.GetInt("SelectedSkin", 0);
    }

    public int GetStars()
    {
        return PlayerPrefs.GetInt("Stars", 0);
    }
}