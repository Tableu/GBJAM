using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActivateEnd : MonoBehaviour
{
    [SerializeField] private GameObject LevelEnd;

    private void OnDestroy()
    {
        LevelEnd.GetComponent<BoxCollider2D>().enabled = true;
    }
}
