using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParasiteTestScript : MonoBehaviour
{
    [SerializeField] Animator animator;
    string currentAnim;

    void Start()
    {
        ChangeAnimation("Parasite_Idle");
    }

    void Update()
    {
        
    }

   public void ChangeAnimation(string targetAnim, float fade = 0.2f)
    {
        currentAnim = targetAnim;
        animator.CrossFade(targetAnim, fade);
    }
}
