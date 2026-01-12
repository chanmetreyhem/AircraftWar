using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
public class MenuController : MonoBehaviour
{
    [SerializeField] private Transform touchEffect;
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0) { 
            TouchControl primaryTouch = Touchscreen.current.primaryTouch;
            if (primaryTouch != null) {
                if (primaryTouch.isInProgress)
                {
                    Vector2 touchPos = primaryTouch.position.ReadValue();
                    touchEffect.position = touchPos;
                }
            }
        }
    }
}
