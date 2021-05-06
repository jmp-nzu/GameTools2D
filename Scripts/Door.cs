using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();    
    }

    public void Open()
    {
        animator.SetBool("open", true);
    }

    public void Close()
    {
        animator.SetBool("open", false);
    }
}
