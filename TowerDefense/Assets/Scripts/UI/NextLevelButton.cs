using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelButton : MonoBehaviour
{
    private void OnEnable()
    {
        if (GameManager.instance.CurrentLevel == Tags.LAST_LEVEL_INDEX)
            gameObject.SetActive(false);
    }
}
