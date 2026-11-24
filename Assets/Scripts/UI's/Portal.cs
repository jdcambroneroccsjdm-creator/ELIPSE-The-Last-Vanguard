using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    [Header("Portal Settings")]
    public string nextSceneName = "Stage2";

    private bool portalUnlocked = false;

    public void UnlockPortal()
    {
        portalUnlocked = true;
        gameObject.SetActive(true);

        Debug.Log("Portal is now unlocked!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!portalUnlocked)
            return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Entering portal...");

            SceneManager.LoadScene(nextSceneName);
        }
    }
}
