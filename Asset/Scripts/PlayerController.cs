using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro; 

public class PlayerController : MonoBehaviour
{
    [Header("Pengaturan Waktu")]
    public float sisaWaktu = 60f; 
    public TextMeshProUGUI teksWaktuUI; 

    [Header("Pengaturan Pergerakan")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;

    [Header("UI & Panel")]
    public GameObject panelGameOver;
    public TextMeshProUGUI teksPesanGameOver; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        Time.timeScale = 1f; 
        if (panelGameOver != null) panelGameOver.SetActive(false);
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (sisaWaktu > 0)
        {
            sisaWaktu -= Time.deltaTime;
            if (teksWaktuUI != null) teksWaktuUI.text = "Waktu: " + Mathf.Round(sisaWaktu);
        }
        else
        {
            sisaWaktu = 0;
            MunculkanPanelGameOver("Waktu Habis!"); 
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            this.enabled = false; 
            rb.linearVelocity = Vector2.zero;
            if (anim != null) anim.SetTrigger("Mati");
            Invoke("PesanTersengat", 1.0f); 
        }
        else if (collision.CompareTag("Item"))
        {
            sisaWaktu += 2f; 
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Mutiara"))
        {
            sisaWaktu += 5f; 
            Destroy(collision.gameObject);
        }
        // Logika Portal untuk Pindah Scene
        else if (collision.CompareTag("Portal"))
        {
            // Memuat scene berikutnya sesuai urutan di Build Settings
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    void PesanTersengat()
    {
        MunculkanPanelGameOver("Zzzt! Kamu Tersengat!");
    }

    void MunculkanPanelGameOver(string pesan)
    {
        if (panelGameOver != null) 
        {
            panelGameOver.SetActive(true);
            if (teksPesanGameOver != null) teksPesanGameOver.text = pesan;
        }
        Time.timeScale = 0f; 
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}