using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movimiento : MonoBehaviour
{ 
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private Vector2 direccion;
    [SerializeField] private Animator animator;
    private Rigidbody2D rb2D;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    { 
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        direccion = new Vector2(moveX, moveY).normalized;
    }

    private void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + direccion * velocidadMovimiento * Time.fixedDeltaTime);
    }
}