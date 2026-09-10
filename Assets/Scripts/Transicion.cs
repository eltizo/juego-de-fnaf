using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transicion : MonoBehaviour
{
   private Animator animator; 
   [SerializeField] private AnimationClip animacionFinal; // Asignar la animación de transición en el Inspector

    private void Start()

   {
       animator = GetComponent<Animator>();
   }

   private void Update()
   {
       if (Input.GetKeyDown(KeyCode.E))
       {
           StartCoroutine(CambiarEscena());
       }
        IEnumerator CambiarEscena()
        {
       
           animator.SetTrigger("Iniciar");
           yield return new WaitForSeconds(animacionFinal.length);
           SceneManager.LoadScene("1");
        
        }
    }
}