using UnityEngine;
using System.Collections; // Wajib untuk bikin sistem jeda waktu

public class TeleportPortal : MonoBehaviour
{
    [Header("Titik Tujuan Teleportasi")]
    public Transform titikTujuan;

    // Variabel untuk mencegah infinite loop
    private bool bisaTeleport = true;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && bisaTeleport)
        {
            // Ambil script portal tujuan
            TeleportPortal portalTujuan = titikTujuan.GetComponent<TeleportPortal>();
            
            if (portalTujuan != null)
            {
                // Matikan sementara portal tujuan agar tidak langsung memantulkan ikan kembali
                portalTujuan.MatikanPortalSementara();
            }

            // Pindahkan ikan
            collision.gameObject.transform.position = titikTujuan.position;
        }
    }

    // Fungsi untuk mematikan portal selama 1 detik
    public void MatikanPortalSementara()
    {
        StartCoroutine(JedaTeleport());
    }

    IEnumerator JedaTeleport()
    {
        bisaTeleport = false; // Matikan sensor
        yield return new WaitForSeconds(1f); // Tunggu 1 detik
        bisaTeleport = true; // Hidupkan lagi sensornya
    }
}