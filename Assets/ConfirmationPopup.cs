using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationPopup : BaseUI
{
    // Referensi UI element yang digunakan dalam popup
    [Header("UI Elements")] public TextMeshProUGUI messageText; // Untuk menampilkan pesan di popup

    public Button confirmButton; // Tombol konfirmasi
    public Button cancelButton; // Tombol pembatalan

    /// <summary>
    ///     Setup popup dengan parameter yang diperlukan.
    /// </summary>
    /// <param name="message">Pesan yang ditampilkan pada popup</param>
    /// <param name="onConfirm">Action yang dijalankan saat tombol konfirmasi ditekan</param>
    /// <param name="onCancel">Action yang dijalankan saat tombol batal ditekan</param>
    public void Setup(string message, UnityAction onConfirm, UnityAction onCancel)
    {
        // Set teks pada popup
        if (messageText != null)
            messageText.text = message;

        // Setup event tombol konfirmasi
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(onConfirm); // Tambahkan action onConfirm
            confirmButton.onClick.AddListener(ClosePopup); // Tutup popup setelah konfirmasi
        }

        // Setup event tombol pembatalan
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(onCancel); // Tambahkan action onCancel
            cancelButton.onClick.AddListener(ClosePopup); // Tutup popup setelah pembatalan
        }
    }

    /// <summary>
    ///     Fungsi untuk menutup popup.
    /// </summary>
    private void ClosePopup()
    {
        Destroy(gameObject); // Hapus popup dari scene
    }
}
