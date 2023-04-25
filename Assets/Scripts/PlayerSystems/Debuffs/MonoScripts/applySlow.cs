using Unity.VisualScripting;
using UnityEngine;

public class applySlow : MonoBehaviour
{
    [SerializeField]
    private float duration;

    [SerializeField]
    private float percentage;

    private Transform playerTrans;

    private void Start()
    {
        playerTrans = (Transform)Variables.ActiveScene.Get("player");
    }

    public void DoSlow()
    {
        playerTrans.GetComponent<DebuffsManager>().AddDebuff(new Slow(duration, percentage));
    }
}
