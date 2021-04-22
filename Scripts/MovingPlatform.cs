using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    Vector2 lastPosition = Vector2.zero;

    GameObject ghost;
    Rigidbody2D ghostRigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

        ghost = Instantiate(gameObject);
        Destroy(ghost.GetComponent<Renderer>());
        Destroy(ghost.GetComponent<Animator>());
        Destroy(ghost.GetComponent<MovingPlatform>());
        ghostRigidbody2D = ghost.AddComponent<Rigidbody2D>();
        ghostRigidbody2D.gravityScale = 0;
        ghostRigidbody2D.mass = 1000000f;

        ghost.transform.position = transform.position;

        foreach(Collider2D col in GetComponentsInChildren<Collider2D>()){
            Destroy(col);
        }

        Animator animator = GetComponent<Animator>();
        if (animator != null) {
            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        ghostRigidbody2D.MovePosition(transform.position);
        ghostRigidbody2D.MoveRotation(transform.rotation.eulerAngles.z);
    }
}
