using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public GameObject startScreen;

    public void HideStartScreen()
    {
        if (startScreen != null)
            {
                startScreen.SetActive(false);
            }
    }
}
