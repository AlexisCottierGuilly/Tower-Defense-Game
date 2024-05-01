using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTimeWaiter : MonoBehaviour
{
    void Update()
    {
        GameManager.instance.player.achievementStats.timeWaitedInTutorial += Time.deltaTime;
    }
}
