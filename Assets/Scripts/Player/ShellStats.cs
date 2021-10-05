using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellStats : MonoBehaviour
{
    [SerializeField] public Vector2 speed;
    [SerializeField] public int armor;
    [SerializeField] public AttackScriptableObject attackConfig;
    [SerializeField] public Sprite shellSprite;
    [SerializeField] public Sprite damagedShellSprite;
    [SerializeField] public int shell;

    public void Equipped(Transform player)
    {
        transform.parent = player;
        gameObject.SetActive(false);
    }
    public void Dropped(int playerArmor, float playerDirection)
    {
        transform.localPosition = Vector3.zero;
        transform.gameObject.SetActive(true);
        armor = playerArmor;
        transform.SetParent(null);
        GetComponent<Rigidbody2D>().velocity = new Vector2(0,7f);
        transform.localScale = new Vector3(playerDirection, transform.localScale.y, transform.localScale.z);
    }
    public void BreakShell(Transform player)
    {
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(true);
        transform.localScale = player.localScale;
        transform.parent = null;
        GetComponent<ShellAnimator>().SetIsDead();
        Destroy(gameObject, 0.5f);
    }

    public Sprite SwitchSprite(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer.sprite == shellSprite)
        {
            return damagedShellSprite;
        }
        return shellSprite;
    }
}
