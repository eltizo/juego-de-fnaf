using UnityEngine;

public class InteraccionPuerta : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public GameObject primeraEscena;
    public GameObject segundaEscena;

    [Header("Posición de Destino")]
    // Fijamos Z en 0 para mantener al jugador en el plano 2D correcto
    public Vector3 destino = new Vector3(-15.36101f, 17.68384f, 0f);

    private bool jugadorCerca = false;
    private GameObject jugador;

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            CambiarEscenaYTeletransportar();
        }
    }

    void CambiarEscenaYTeletransportar()
    {
        if (primeraEscena != null) primeraEscena.SetActive(false);
        if (segundaEscena != null) segundaEscena.SetActive(true);

        if (jugador != null)
        {
            Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Detiene el movimiento acumulado y posiciona el cuerpo físico
                rb.velocity = Vector2.zero;
                rb.position = destino;
            }
            
            jugador.transform.position = destino;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
            jugador = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}