using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    public void EndAttack()
    {
        playerController.EndAttack();
    }
    public void HitAttack()
    {
        PlayerCombat playerCombat = GetComponentInParent<PlayerCombat>();
        playerCombat.HitAttack();
    }
}