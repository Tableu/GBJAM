
using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    Animator animCont;
    [SerializeField] private GameObject Enemies;
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
            Destroy(Enemies);
        }
    }
}
