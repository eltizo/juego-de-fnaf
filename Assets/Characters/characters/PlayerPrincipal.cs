using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrincipal : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;

    [Header("Dash / Slice")]
    [SerializeField] private float velocidadDash = 15f;
    [SerializeField] private float cooldownDash = 4f;

    [Header("Opciones Visuales")]
    [Tooltip("Marca esto solo si la animación del Slice fue dibujada apuntando al lado opuesto del Walk")]
    [SerializeField] private bool invertirSpriteEnSlice = false;

    [Header("UI Dash")]
    [SerializeField] private Slider sliderCooldown;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;

    private float gravedadInicial;
    private bool puedeHacerDash = true;
    private bool sePuedeMover = true;
    private bool estaHaciendoDash = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        gravedadInicial = rb.gravityScale;

        if (sliderCooldown != null)
        {
            sliderCooldown.maxValue = cooldownDash;
            sliderCooldown.value = 0f;
        }
    }

    void Update()
    {
        if (sePuedeMover)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // Actualizar animación de caminar
            anim.SetBool("IsWalking", movement != Vector2.zero);

            // Girar personaje
            GirarPersonaje(movement.x);
        }

        // Dash con Q
        if (Input.GetKeyDown(KeyCode.Q) && puedeHacerDash && !estaHaciendoDash)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (sePuedeMover)
        {
            rb.velocity = movement.normalized * speed;
        }
    }

    private void GirarPersonaje(float direccionX)
    {
        // Escala original: -4 para derecha, 4 para izquierda
        if (direccionX > 0)
        {
            transform.localScale = new Vector3(-5, 5, 5);
        }
        else if (direccionX < 0)
        {
            transform.localScale = new Vector3(5, 5, 5);
        }
    }

    private IEnumerator Dash()
    {
        puedeHacerDash = false;
        sePuedeMover = false;
        estaHaciendoDash = true;

        rb.gravityScale = 0f;

        Vector2 direccionDash = movement;

        // Si no hay teclas presionadas, dash en la dirección a la que está mirando
        if (direccionDash == Vector2.zero)
        {
            bool mirandoDerecha = transform.localScale.x < 0;
            direccionDash = mirandoDerecha ? Vector2.right : Vector2.left;
        }
        else
        {
            GirarPersonaje(direccionDash.x);
        }

        // Si el dibujo del Slice estaba invertido respecto al Walk
        if (invertirSpriteEnSlice)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        rb.velocity = direccionDash.normalized * velocidadDash;

        // Activar animación de Dash / Slice
        anim.SetBool("IsDashing", true);

        // Esperar un frame para que el Animator actualice el estado
        yield return null;

        // Obtener la duración exacta de los 56 frames (~0.93s)
        float duracionAnimacion = anim.GetCurrentAnimatorStateInfo(0).length;

        if (duracionAnimacion <= 0)
        {
            duracionAnimacion = 56f / 60f;
        }

        yield return new WaitForSeconds(duracionAnimacion);

        rb.velocity = Vector2.zero;
        rb.gravityScale = gravedadInicial;

        // Termina Dash
        anim.SetBool("IsDashing", false);

        estaHaciendoDash = false;
        sePuedeMover = true;

        // Leer movimiento al terminar el dash
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Restaurar orientación de movimiento
        if (movement.x != 0)
        {
            GirarPersonaje(movement.x);
        }

        anim.SetBool("IsWalking", movement != Vector2.zero);

        // Empieza el cooldown
        StartCoroutine(CooldownDash());
    }

    private IEnumerator CooldownDash()
    {
        float tiempoRestante = cooldownDash;

        while (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;

            if (sliderCooldown != null)
            {
                sliderCooldown.value = tiempoRestante;
            }

            yield return null;
        }

        if (sliderCooldown != null)
        {
            sliderCooldown.value = 0f;
        }

        puedeHacerDash = true;
    }
}