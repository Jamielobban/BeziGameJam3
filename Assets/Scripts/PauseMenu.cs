using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject stuff;
    private InputSystem_Actions controls;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Player.PauseNew.performed += ctx => TogglePause();
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void TogglePause()
    {
        bool isPaused = stuff.activeSelf;

        stuff.SetActive(!isPaused);
        Time.timeScale = isPaused ? 1f : 0f;
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        stuff.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
