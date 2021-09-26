using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    Animator animCont;

    private void Awake()
    {
        animCont = GetComponent<Animator>();
    }

    public void OpenChest()
    {
        animCont.SetTrigger("Open");
        if (MapManager.Instance && !MapManager.Instance.IsEndingLevel)
        {
            MapManager.Instance.EndLevel();
        }
    }
}
