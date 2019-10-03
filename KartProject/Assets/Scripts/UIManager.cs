using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text speedText;

    public void SpeedUI(int speed)
    {
        speedText.text = speed.ToString();
    }
}
