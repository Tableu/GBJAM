using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellScript : MonoBehaviour
{
    [SerializeField] private PlayerController.PlayerStats playerStats;
    public void Detached() 
    {
        //When the shell is detached (either when the player discards it or its enemy dies) enable relevant components (e.g. collider)
    }
    public void AttachedToPlayer(GameObject Player)
    {
        Player.GetComponent<PlayerController>().SetStats(playerStats);
        //Call PlayerAnimatorController to set sprite
    }
}
