
using UnityEngine;

public class ClockTowerActivation : MonoBehaviour
{
    [SerializeField] private Animator clockAnimator;

    private bool activated;

    public void ActivateClockTower()
    {
        if (activated)
            return;

        activated = true;

        if (clockAnimator != null)
        {
            clockAnimator.SetTrigger("Activate");
        }

        Debug.Log("Clock Tower Activated!");
    }
}
