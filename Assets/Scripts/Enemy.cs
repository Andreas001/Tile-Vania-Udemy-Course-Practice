using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;

    void Start()
    {
        
    }

    void Update()
    {
        if(IsFacingRight()) {
            GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, 0f);
        }
        else {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, 0f);
        }
    }

    private bool IsFacingRight() {
        return transform.localScale.x > 0;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        transform.localScale = new Vector2(-(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x)), 1f);
    }
}
