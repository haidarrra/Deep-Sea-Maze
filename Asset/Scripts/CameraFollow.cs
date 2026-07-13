using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Kamera")]
    public Transform target; // Ini nanti diisi dengan karakter ikan

    [Header("Pengaturan Kamera")]
    public float pergerakanHalus = 5f; 
    public Vector3 offset = new Vector3(0, 0, -10f); // Z harus -10 agar kamera tetap melihat dari jauh

    void FixedUpdate()
    {
        // Pastikan target (ikan) tidak kosong
        if (target != null)
        {
            // Posisi tujuan kamera (posisi ikan + jarak kamera)
            Vector3 posisiTujuan = target.position + offset;
            
            // Bergerak dengan mulus (smooth) menuju target
            Vector3 posisiMulus = Vector3.Lerp(transform.position, posisiTujuan, pergerakanHalus * Time.fixedDeltaTime);
            transform.position = posisiMulus;
        }
    }
}