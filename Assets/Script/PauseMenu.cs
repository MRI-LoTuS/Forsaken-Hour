using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject pausePanel;
    public MonoBehaviour[] scriptDimatikan;
    [SerializeField] private InputActionReference pauseAction;
    public string menuScene = "Main Menu";


    public static bool isPaused { get; private set;  }
    void Start()
    {
        Resume();
    }

    private void OnEnable()
    {
        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPausePerformed;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.performed -= OnPausePerformed;
            pauseAction.action.Disable();
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (ForsakenClock.selesai) return;

        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause() 
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetPlayerScript(false);
    }

    public void Resume() 
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetPlayerScript(true);
    }

    public void KeMenuUtama() 
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScene);

    }

    void SetPlayerScript(bool aktif) 
    {
        foreach (var s in scriptDimatikan)
            if (s != null) s.enabled = aktif;
    }

}
