using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct StoryScene 
{
    public Sprite bgImage;
    public Sprite characterPortrait; // Tambahan: Gambar karakter yang sedang bicara
    public string speakerName;
    [TextArea(3, 5)] public string dialogueText;
    public string sceneLabel;
}

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image backgroundUI;
    public Image characterPortraitUI; // Tambahan: UI Image untuk portrait
    public TextMeshProUGUI speakerNameUI;
    public TextMeshProUGUI dialogueUI;
    public TextMeshProUGUI sceneLabelUI;
    public CanvasGroup fadeGroup;
    public GameObject dialoguePanel;

    [Header("Story Data")]
    public StoryScene[] scenes;
    public string nextSceneName = "Scene_Gameplay_Level1";
    public float typingSpeed = 0.03f;

    private int currentSceneIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        StartCoroutine(FadeInAndPlayScene(currentSceneIndex));
    }

    public void OnScreenTapped()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueUI.text = scenes[currentSceneIndex].dialogueText;
            isTyping = false;
        }
        else
        {
            currentSceneIndex++;
            if (currentSceneIndex < scenes.Length)
            {
                StartCoroutine(TransitionToNextScene());
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    IEnumerator FadeInAndPlayScene(int index)
    {
        SetupSceneData(index);
        
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime * 1.5f;
            fadeGroup.alpha = t;
            yield return null;
        }
    }

    IEnumerator TransitionToNextScene()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 1.5f;
            fadeGroup.alpha = t;
            yield return null;
        }

        SetupSceneData(currentSceneIndex);

        t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime * 1.5f;
            fadeGroup.alpha = t;
            yield return null;
        }
    }

    void SetupSceneData(int index)
    {
        StoryScene scene = scenes[index];
        backgroundUI.sprite = scene.bgImage;
        speakerNameUI.text = scene.speakerName;
        sceneLabelUI.text = scene.sceneLabel;
        dialogueUI.text = ""; 

        // Logika untuk menampilkan/menyembunyikan Portrait Karakter
        if (scene.characterPortrait != null)
        {
            characterPortraitUI.sprite = scene.characterPortrait;
            characterPortraitUI.gameObject.SetActive(true); // Munculkan jika ada gambar
        }
        else
        {
            characterPortraitUI.gameObject.SetActive(false); // Sembunyikan jika kosong (misal: NARRATOR)
        }

        isTyping = true;
        typingCoroutine = StartCoroutine(TypeText(scene.dialogueText));
    }

    IEnumerator TypeText(string textToType)
    {
        foreach (char c in textToType.ToCharArray())
        {
            dialogueUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false; 
    }
}