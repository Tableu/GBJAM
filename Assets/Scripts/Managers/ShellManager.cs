using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class ShellManager : MonoBehaviour
{
    [SerializeField] private GameObject snailShell;
    [SerializeField] private GameObject spikyShell;
    [SerializeField] private GameObject conchShell;
    private Hashtable shellHashtable;
    private GameObject[] shellArray;
    private int redeemIndex;

    public const string REDEEM_SHELL = "RedeemShell";
    
    private void Awake()
    {
        redeemIndex = PlayerPrefs.GetInt(REDEEM_SHELL, 0);
        shellHashtable = new Hashtable();
        shellHashtable.Add(PlayerController.SNAIL_SHELL, 0);
        shellHashtable.Add(PlayerController.SPIKY_SHELL, 1);
        shellHashtable.Add(PlayerController.CONCH_SHELL, 2);
        shellArray = new[] {snailShell, spikyShell, conchShell};
    }

    public GameObject GetShell(string key)
    {
        if (shellHashtable.ContainsKey(key))
        {
            return shellArray[(int)shellHashtable[key]];
        }
        return null;
    }

    public ShellStats RedeemShell(GameObject player)
    {
        ShellStats shell =  Instantiate(shellArray[redeemIndex], player.transform.position, Quaternion.identity).GetComponent<ShellStats>();
        shell.armor = 1;
        redeemIndex++;
        if (redeemIndex > shellArray.Length)
        {
            redeemIndex = 0;
        }
        return shell;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(REDEEM_SHELL, redeemIndex);
    }
}
