using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Pengaturan Waktu")]
    public float waktuTersisa = 120f;
    public bool isGameOver = false;

    [Header("Referensi UI")]
    public TextMeshProUGUI teksWaktu;
    public GameObject panelGameOver;
    public GameObject panelMenang;

    [Header("Sistem Audio")]
    public AudioSource uiSfxSource;
    public AudioClip clipMenang, clipKalah;
    public AudioSource bgmSource; // Tarik Audio Source Main Camera ke slot ini
    public float volumeTarget = 0.1f; // Volume BGM saat game over/menang

    [Header("Sistem Booster Acak")]
    public GameObject prefabBooster;
    public Transform[] titikSpawnBooster;
    private int jumlahBoosterMuncul = 0;

    void Start()
    {
        Time.timeScale = 1f;

        if (panelGameOver != null)
            panelGameOver.SetActive(false);

        if (panelMenang != null)
            panelMenang.SetActive(false);

        UpdateUITimer();
        StartCoroutine(RutinitasSpawnBooster());
    }

    void Update()
    {
        if (isGameOver)
            return;

        waktuTersisa -= Time.deltaTime;

        if (waktuTersisa <= 0)
        {
            waktuTersisa = 0;
            UpdateUITimer();
            TriggerGameOver();
            return;
        }

        UpdateUITimer();
    }

    void UpdateUITimer()
    {
        if (teksWaktu != null)
        {
            teksWaktu.text = "Waktu " + Mathf.FloorToInt(waktuTersisa);
        }
    }

    // Fungsi untuk mengecilkan BGM secara perlahan (fade out)
    IEnumerator FadeOutBGM()
    {
        if (bgmSource != null)
        {
            while (bgmSource.volume > volumeTarget)
            {
                bgmSource.volume -= 0.05f;
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
    }

    // ==========================
    // GAME OVER
    // ==========================
    public void TriggerGameOver()
    {
        if (isGameOver)
            return;

        Debug.Log("GAME OVER DIPANGGIL");

        StartCoroutine(FadeOutBGM()); // Panggil fade out sebelum pause
        
        if (uiSfxSource != null && clipKalah != null)
            uiSfxSource.PlayOneShot(clipKalah);

        isGameOver = true;
        Time.timeScale = 0f;

        if (panelGameOver != null)
            panelGameOver.SetActive(true);
    }

    // ==========================
    // MENANG
    // ==========================
    public void TriggerMenang()
    {
        if (isGameOver)
            return;

        Debug.Log("TRIGGER MENANG DIPANGGIL");

        StartCoroutine(FadeOutBGM()); // Panggil fade out sebelum pause

        if (uiSfxSource != null && clipMenang != null)
            uiSfxSource.PlayOneShot(clipMenang);

        isGameOver = true;
        Time.timeScale = 0f;

        if (panelMenang != null)
            panelMenang.SetActive(true);
    }

    // ==========================
    // RESTART & LANJUT
    // ==========================
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LanjutLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // ==========================
    // SPAWN BOOSTER
    // ==========================
    IEnumerator RutinitasSpawnBooster()
    {
        while (!isGameOver && jumlahBoosterMuncul < 3)
        {
            yield return new WaitForSeconds(25f);

            if (isGameOver)
                yield break;

            if (prefabBooster != null && titikSpawnBooster.Length > 0)
            {
                int index = Random.Range(0, titikSpawnBooster.Length);
                Instantiate(prefabBooster, titikSpawnBooster[index].position, Quaternion.identity);
                jumlahBoosterMuncul++;
            }
        }
    }
}