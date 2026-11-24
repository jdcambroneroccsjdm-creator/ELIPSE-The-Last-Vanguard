
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingSceneManager : MonoBehaviour
{
    [Header("Loading Settings")]
    public float loadingTime = 4f;

    void Start()
    {
        StartCoroutine(LoadMainScene());
    }

    IEnumerator LoadMainScene()
    {
        yield return new WaitForSeconds(loadingTime);

        SceneManager.LoadScene("MainScene");
    }
}
