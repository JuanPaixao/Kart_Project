using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;

    public void SetSpeedUI(float speed)
    {
        uiManager.SpeedUI((int)speed);
    }
}
