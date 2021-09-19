
using UnityEngine;

public class LandSnailAttack: IState
{
    private PlayerController _player;

    public LandSnailAttack(PlayerController player)
    {
        _player = player;
    }

    public void Tick()
    {
        throw new System.NotImplementedException();
    }

    public void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        throw new System.NotImplementedException();
    }
}
