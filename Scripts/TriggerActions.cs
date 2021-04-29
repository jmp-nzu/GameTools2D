using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerActions : MonoBehaviour
{
    public string targetTag = "Player";
    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetTag == "" || collision.gameObject.tag == targetTag)
        {
            OnEnter?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (targetTag == "" || collision.gameObject.tag == targetTag)
        {
            OnExit?.Invoke();
        }
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
