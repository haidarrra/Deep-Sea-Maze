using UnityEngine;
using System.Collections.Generic;

public class EnemyMarlo : MonoBehaviour
{
    [Header("Pengaturan Patroli")]
    public List<Transform> titikPatroli; 
    public float kecepatanPatroli = 5f;
    public float jarakToleransi = 1.5f; 
    private int indeksTitik = 0; 
    private Transform titikTujuanSaatIni;

    [Header("Pengaturan Pengejaran (Visi)")]
    public float kecepatanKejar = 10f;
    public float jarakPenglihatan = 15f; 
    
    private Transform targetPemain;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool sedangMengejar = false;
    private bool baruSelesaiNgejar = false; // Tanda dia baru kehilangan jejak

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject pemain = GameObject.FindGameObjectWithTag("Player");
        if (pemain != null) targetPemain = pemain.transform;

        if (titikPatroli.Count > 0) titikTujuanSaatIni = titikPatroli[0];
    }

    void FixedUpdate()
    {
        if (targetPemain == null) return;

        bool melihatPemain = CekPenglihatanLebar();

        if (melihatPemain)
        {
            sedangMengejar = true;
            baruSelesaiNgejar = true; // Set penanda kalau dia habis ngejar
            KejarPemain();
        }
        else
        {
            // Jika baru kehilangan jejak, cari titik terdekat dulu sebelum lanjut rute
            if (sedangMengejar && baruSelesaiNgejar)
            {
                CariTitikTerdekat();
                baruSelesaiNgejar = false; 
            }
            
            sedangMengejar = false;
            Patroli();
        }
    }

    // Fungsi diubah jadi bool agar gampang dibaca
    bool CekPenglihatanLebar()
    {
        float jarak = Vector2.Distance(transform.position, targetPemain.position);
        if (jarak <= jarakPenglihatan)
        {
            Vector2 arahPusat = (targetPemain.position - transform.position).normalized;
            int layerPenghalang = LayerMask.GetMask("Penghalang");

            Vector2 arahKiri = Quaternion.Euler(0, 0, 15) * arahPusat;  
            Vector2 arahKanan = Quaternion.Euler(0, 0, -15) * arahPusat;

            RaycastHit2D hitPusat = Physics2D.Raycast(transform.position, arahPusat, jarakPenglihatan, layerPenghalang);
            RaycastHit2D hitKiri = Physics2D.Raycast(transform.position, arahKiri, jarakPenglihatan, layerPenghalang);
            RaycastHit2D hitKanan = Physics2D.Raycast(transform.position, arahKanan, jarakPenglihatan, layerPenghalang);

            if (hitPusat.collider == null || hitKiri.collider == null || hitKanan.collider == null)
            {
                return true; // Kelihatan!
            }
        }
        return false; // Gak kelihatan
    }

    void Patroli()
    {
        if (titikPatroli.Count == 0 || titikTujuanSaatIni == null) return;

        BergerakKeArah(titikTujuanSaatIni.position, kecepatanPatroli);

        if (Vector2.Distance(transform.position, titikTujuanSaatIni.position) <= jarakToleransi)
        {
            indeksTitik++;
            if (indeksTitik >= titikPatroli.Count) indeksTitik = 0; 
            titikTujuanSaatIni = titikPatroli[indeksTitik];
        }
    }

    // FUNGSI BARU: Cari titik paling dekat dari posisi Marlo saat ini
    void CariTitikTerdekat()
    {
        if (titikPatroli.Count == 0) return;

        float jarakTerdekat = Mathf.Infinity;
        int indeksTerdekat = 0;

        for (int i = 0; i < titikPatroli.Count; i++)
        {
            float jarak = Vector2.Distance(transform.position, titikPatroli[i].position);
            if (jarak < jarakTerdekat)
            {
                jarakTerdekat = jarak;
                indeksTerdekat = i;
            }
        }

        indeksTitik = indeksTerdekat;
        titikTujuanSaatIni = titikPatroli[indeksTitik];
    }

    void KejarPemain()
    {
        BergerakKeArah(targetPemain.position, kecepatanKejar);
    }

    void BergerakKeArah(Vector2 targetPosisi, float speed)
    {
        Vector2 arah = (targetPosisi - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + arah * speed * Time.fixedDeltaTime);
        
        if (Mathf.Abs(arah.x) > Mathf.Abs(arah.y) + 0.1f) 
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); 
            if (arah.x > 0.1f) spriteRenderer.flipX = false;      
            else if (arah.x < -0.1f) spriteRenderer.flipX = true; 
        }
        else if (Mathf.Abs(arah.y) > Mathf.Abs(arah.x) + 0.1f)
        {
            spriteRenderer.flipX = false; 
            if (arah.y > 0.1f) transform.rotation = Quaternion.Euler(0, 0, 90);  
            else if (arah.y < -0.1f) transform.rotation = Quaternion.Euler(0, 0, -90); 
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("DIMAKAN MARLO! GAME OVER!");
            FindObjectOfType<LevelManager>().TriggerGameOver(); // Panggil UI Game Over
        }
    }
}