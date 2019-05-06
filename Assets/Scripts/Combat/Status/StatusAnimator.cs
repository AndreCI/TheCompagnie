using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity;

public class StatusAnimator : MonoBehaviour
{
    public Animator animator;
    public Animator poisonedAnimator;
    public void PlayAnimation(CombatStatus.STATUS status, float duration=0.2f)
    {
        switch (status)
        {
            case CombatStatus.STATUS.POISON:
                poisonedAnimator.SetBool("poisoned", true);
                //animator.SetTrigger("poisoned");
                break;
            case CombatStatus.STATUS.REGEN:
                animator.SetTrigger("regen");
                break;
            case CombatStatus.STATUS.BURN:
                animator.SetTrigger("burn");
                break;
            case CombatStatus.STATUS.BLOCK:
                animator.SetTrigger("block");
                break;
            case CombatStatus.STATUS.PARRY:
                animator.SetTrigger("parry");
                break;
        }
    }
    public void ResetSpecific(CombatStatus.STATUS status, float duration = 0.2f)
    {
        switch (status)
        {
            case CombatStatus.STATUS.POISON:
                poisonedAnimator.SetBool("poisoned", false);
                //animator.SetTrigger("poisoned");
                break;
            case CombatStatus.STATUS.REGEN:
                animator.ResetTrigger("regen");
                break;
            case CombatStatus.STATUS.BURN:
                animator.ResetTrigger("burn");
                break;
            case CombatStatus.STATUS.BLOCK:
                animator.ResetTrigger("block");
                break;
            case CombatStatus.STATUS.PARRY:
                animator.ResetTrigger("parry");
                break;
        }
    }


    public void ResetAll()
{
        
        animator.ResetTrigger("regen");
        animator.ResetTrigger("burn");
        animator.ResetTrigger("block");
        animator.ResetTrigger("parry");

    }



}
