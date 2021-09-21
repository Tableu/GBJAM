using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapManager : MonoBehaviour
{
    Transform confinerContainer;
    PlayerController player;
    void Start()
    {
        confinerContainer = transform.Find("CameraConfiner");

        player = FindObjectOfType<PlayerController>();

        HUDManager.Instance.Show(true);
        if (player)
        {
            HUDManager.Instance.UpdateHealth(player.Health);
            HUDManager.Instance.UpdateArmor(player.Armor);
            //TODO: Update coins.
        }


        if (confinerContainer)
        {
            CinemachineConfiner2D camConfiner = FindObjectOfType<CinemachineConfiner2D>();
            camConfiner.m_BoundingShape2D = confinerContainer.GetComponent<PolygonCollider2D>();
        }
    }
}
