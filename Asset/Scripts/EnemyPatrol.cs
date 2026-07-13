using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Pengaturan Patroli")]
    public Transform[] titikPatroli; // Array untuk menampung titik A, B, dll
    public float kecepatan = 2f;     // Kecepatan gerak ubur-ubur
    
    private int indeksTitik = 0;

    void Update()
    {
        // Cegah error jika titik patroli belum diisi
        if (titikPatroli.Length == 0) return;

        // Menggerakkan Jellie secara perlahan menuju titik target saat ini
        transform.position = Vector2.MoveTowards(transform.position, titikPatroli[indeksTitik].position, kecepatan * Time.deltaTime);

        // Mengecek apakah Jellie sudah sangat dekat (hampir sampai) di titik target
        if (Vector2.Distance(transform.position, titikPatroli[indeksTitik].position) < 0.1f)
        {
            // Pindah ke titik target berikutnya
            indeksTitik++;
            
            // Jika sudah mencapai titik terakhir, kembali ke titik pertama (looping)
            if (indeksTitik >= titikPatroli.Length)
            {
                indeksTitik = 0;
            }
        }
    }
}