using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AwakeStart : MonoBehaviour
{

    void Start()
    {
        Application.targetFrameRate = 60;

    }
    public AudioSource clip;
    public IEnumerator waitSceneChange(float duration, string name)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene("Level 1");
    }

    public void ChangeScene()
    {
        clip.Play();
        StartCoroutine(waitSceneChange(0.95f, "Level 1"));
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}