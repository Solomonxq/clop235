using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WaveUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public Spawner spawner;

    [Header("UI Текст (видимий ВСЮДИ)")]
    public TextMeshProUGUI waveText;

    [Header("UI Смужка (тільки на Арені)")]
    public GameObject waveBarBackground; 
    public RectTransform waveBarFill;     

    [Header("Налаштування Сцени")]
    public string arenaSceneName = "Arena"; 

    private float maxBarWidth;

    void Awake()
    {
        if (waveBarFill != null)
        {
            maxBarWidth = waveBarFill.rect.width;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitUI();
    }

    void Start()
    {
        InitUI();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (playerStats != null)
        {
            playerStats.OnWaveChanged -= UpdateWaveText;
        }

        if (spawner != null)
        {
            spawner.OnWaveProgressChanged -= UpdateWaveBar;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitUI();
    }

    private void InitUI()
    {
        // 1. Пошук PlayerStats
        if (playerStats == null)
        {
            playerStats = FindAnyObjectByType<PlayerStats>();
        }

        if (playerStats != null)
        {
            playerStats.OnWaveChanged -= UpdateWaveText; 
            playerStats.OnWaveChanged += UpdateWaveText;

            // Примусово викликаємо LoadData(), якщо дані ще не завантажені
            playerStats.LoadData(); 
            UpdateWaveText(playerStats.currentWave);
        }
        else
        {
            // Якщо PlayerStats ще не встиг створитися, беремо хвилю напряму з збереження
            int savedWave = PlayerPrefs.GetInt("SavedWave", 1);
            UpdateWaveText(savedWave);
        }

        // 2. Пошук Spawner
        if (spawner == null)
        {
            spawner = FindAnyObjectByType<Spawner>();
        }

        if (spawner != null)
        {
            spawner.OnWaveProgressChanged -= UpdateWaveBar;
            spawner.OnWaveProgressChanged += UpdateWaveBar;
        }

        CheckSceneVisibility();
    }

    private void CheckSceneVisibility()
    {
        bool isArena = SceneManager.GetActiveScene().name == arenaSceneName;

        if (waveText != null) 
        {
            waveText.gameObject.SetActive(true);
        }

        if (waveBarBackground != null) 
        {
            waveBarBackground.SetActive(isArena);
        }
    }

    void UpdateWaveBar(int killedCount, int totalCount)
    {
        if (waveBarFill != null && totalCount > 0)
        {
            float progress = 1f - ((float)killedCount / totalCount);

            Vector2 size = waveBarFill.sizeDelta;
            size.x = maxBarWidth * progress;
            waveBarFill.sizeDelta = size;
        }
    }

    void UpdateWaveText(int waveNumber)
    {
        if (waveText != null)
        {
            waveText.text = "Хвиля: " + waveNumber;
        }
    }
}