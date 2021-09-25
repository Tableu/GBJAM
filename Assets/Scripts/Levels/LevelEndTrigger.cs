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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animCont.SetTrigger("Open");
            if (MapManager.Instance && !MapManager.Instance.IsEndingLevel)
            {
                MapManager.Instance.EndLevel();
            }
        }
    }
}
