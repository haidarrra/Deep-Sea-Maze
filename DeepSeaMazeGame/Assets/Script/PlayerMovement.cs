using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Pengaturan Pergerakan")]
    public float moveSpeed = 7f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;

    [Header("Mekanik Game")]
    public int totalPlankton = 0;
    public GameObject portalFinis;

    [Header("Sistem Audio")]
    public AudioSource sfxSource;
    public AudioClip clipMakan, clipMutiara, clipPortal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (portalFinis != null)
        {
            portalFinis.SetActive(false);
        }
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (movement.y > 0)
        {
            spriteRenderer.flipX = false;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (movement.y < 0)
        {
            spriteRenderer.flipX = false;
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // =====================
        // PLANKTON
        // =====================
        if (collision.CompareTag("Plankton"))
        {
            if (sfxSource != null && clipMakan != null)
                sfxSource.PlayOneShot(clipMakan);

            Destroy(collision.gameObject);
            totalPlankton++;

            Debug.Log("Plankton dimakan! Jumlah: " + totalPlankton);

            if (totalPlankton >= 30 && portalFinis != null)
            {
                portalFinis.SetActive(true);
                Debug.Log("PORTAL FINIS TERBUKA!");
            }
        }

        // =====================
        // MUTIARA
        // =====================
        else if (collision.CompareTag("Mutiara"))
        {
            if (sfxSource != null && clipMutiara != null)
                sfxSource.PlayOneShot(clipMutiara);

            Destroy(collision.gameObject);
            Debug.Log("Mutiara Samudera Didapatkan!");
        }

        // =====================
        // BOOSTER
        // =====================
        else if (collision.CompareTag("Booster"))
        {
            Destroy(collision.gameObject);
            AcakEfekBooster();
        }

        // =====================
        // PORTAL FINISH
        // =====================
        else if (collision.CompareTag("Portal"))
        {
            if (sfxSource != null && clipPortal != null)
                sfxSource.PlayOneShot(clipPortal);

            Debug.Log("LEVEL SELESAI! KAMU MENANG!");

            LevelManager lm = FindObjectOfType<LevelManager>();

            if (lm != null)
            {
                lm.TriggerMenang();
            }
            else
            {
                Debug.LogError("LevelManager tidak ditemukan!");
            }
        }
    }

    void AcakEfekBooster()
    {
        int efekAcak = Random.Range(1, 4);

        if (efekAcak == 1)
        {
            Debug.Log("GACHA: KECEPATAN SUPER (5 Detik)!");
            StartCoroutine(DurasiEfekKecepatan(10f));
        }
        else if (efekAcak == 2)
        {
            Debug.Log("GACHA: ZONK! MELAMBAT (5 Detik)!");
            StartCoroutine(DurasiEfekKecepatan(2.5f));
        }
        else
        {
            Debug.Log("GACHA: WAKTU DITAMBAH 10 DETIK!");

            LevelManager lm = FindObjectOfType<LevelManager>();

            if (lm != null)
            {
                lm.waktuTersisa += 10f;
            }
        }
    }

    IEnumerator DurasiEfekKecepatan(float kecepatanBaru)
    {
        float kecepatanAsli = 7f;

        moveSpeed = kecepatanBaru;

        yield return new WaitForSeconds(5f);

        moveSpeed = kecepatanAsli;

        Debug.Log("Efek Booster Habis! Kecepatan normal kembali.");
    }
}