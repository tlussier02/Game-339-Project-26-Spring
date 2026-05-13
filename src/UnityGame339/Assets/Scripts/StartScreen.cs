using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public GameObject startScreen;

    public void HideStartScreen()
    {
        Debug.Log("StartScreen button clicked. Starting FarmMatch BGM.");
        AudioManager.Resolve()?.PlayGameBgm();

        if (startScreen != null)
        {
            startScreen.SetActive(false);
        }
    }
}
