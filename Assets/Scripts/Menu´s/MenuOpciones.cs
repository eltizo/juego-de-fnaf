using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections.Generic;

public class MenuOpciones : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject panelOpcionesPrincipal;
    public GameObject panelConfirmacionCartel;
    
    [Header("Paneles de Secciones")]
    public GameObject panelGeneral;
    public GameObject panelGraficos;
    public GameObject panelSonido;
    public GameObject panelControles;

    [Header("UI Componentes")]
    public TMP_Dropdown dropdownResolucion;
    public TMP_Dropdown dropdownModoPantalla;
    public TMP_Dropdown dropdownFPS;
    public Slider sliderVolGeneral;
    public Slider sliderVolMusica;
    public Slider sliderVolSFX;
    public CanvasScaler canvasScaler;

    [Header("Sonido (Audio Mixer)")]
    public AudioMixer audioMixer; // Ya no crasheará si está vacío

    private int[] anchos = { 800, 1280, 1366, 1920, 3840 };
    private int[] altos = { 600, 720, 768, 1080, 2160 };

    private bool hayCambiosPendientes = false;
    private int resGuardada, modoPantallaGuardado, fpsGuardado;
    private float volGeneralGuardado, volMusicaGuardado, volSFXGuardado;

    // OnEnable se ejecuta CADA VEZ que el panel de opciones se activa
    void OnEnable() 
    {
        // 1. Limpiar el estado visual (esconder cartel por si acaso)
        if (panelConfirmacionCartel != null) panelConfirmacionCartel.SetActive(false);
        hayCambiosPendientes = false;

        // 2. Configurar Dropdown
        if (dropdownResolucion != null && dropdownResolucion.options.Count == 0)
        {
            List<string> opcionesResolucion = new List<string> { "800 x 600", "1280 x 720", "1366 x 768", "1920 x 1080", "3840 x 2160" };
            dropdownResolucion.AddOptions(opcionesResolucion);
        }

        // 3. Cargar datos guardados
        resGuardada = PlayerPrefs.GetInt("ResIndice", 2); 
        modoPantallaGuardado = PlayerPrefs.GetInt("ModoPantallaIndice", 0);
        fpsGuardado = PlayerPrefs.GetInt("FpsIndice", 1); 
        volGeneralGuardado = PlayerPrefs.GetFloat("VolMaster", 0.75f);
        volMusicaGuardado = PlayerPrefs.GetFloat("VolMusica", 0.75f);
        volSFXGuardado = PlayerPrefs.GetFloat("VolSFX", 0.75f);

        // 4. Actualizar UI
        if (dropdownResolucion != null) { dropdownResolucion.value = resGuardada; dropdownResolucion.RefreshShownValue(); }
        if (dropdownModoPantalla != null) { dropdownModoPantalla.value = modoPantallaGuardado; dropdownModoPantalla.RefreshShownValue(); }
        if (dropdownFPS != null) { dropdownFPS.value = fpsGuardado; dropdownFPS.RefreshShownValue(); }
        if (sliderVolGeneral != null) sliderVolGeneral.value = volGeneralGuardado;
        if (sliderVolMusica != null) sliderVolMusica.value = volMusicaGuardado;
        if (sliderVolSFX != null) sliderVolSFX.value = volSFXGuardado;

        AplicarCambiosEnSistema();
        
        // Al cargar todo, forzamos a que no haya cambios pendientes
        hayCambiosPendientes = false; 
    }

    // --- LÓGICA DE INTERCEPCIÓN ---

    public void IntentarSalirDelMenu()
    {
        if (hayCambiosPendientes && panelConfirmacionCartel != null)
        {
            panelConfirmacionCartel.SetActive(true); // Abre el cartel
        }
        else
        {
            CerrarMenuOpcionesTotalmente(); // Sale directo si no tocaste nada
        }
    }

    public void ClickAplicarCambios()
    {
        GuardarEstadoActualEnDisco();
    }

    public void BotonCartelConfirmarSi()
    {
        GuardarEstadoActualEnDisco();
        panelConfirmacionCartel.SetActive(false);
        CerrarMenuOpcionesTotalmente();
    }

    public void BotonCartelConfirmarNo()
    {
        RevertirCambiosAlEstadoAnterior();
        panelConfirmacionCartel.SetActive(false);
        CerrarMenuOpcionesTotalmente();
    }

    private void GuardarEstadoActualEnDisco()
    {
        if (dropdownResolucion != null) resGuardada = dropdownResolucion.value;
        if (dropdownModoPantalla != null) modoPantallaGuardado = dropdownModoPantalla.value;
        if (dropdownFPS != null) fpsGuardado = dropdownFPS.value;
        if (sliderVolGeneral != null) volGeneralGuardado = sliderVolGeneral.value;
        if (sliderVolMusica != null) volMusicaGuardado = sliderVolMusica.value;
        if (sliderVolSFX != null) volSFXGuardado = sliderVolSFX.value;

        PlayerPrefs.SetInt("ResIndice", resGuardada);
        PlayerPrefs.SetInt("ModoPantallaIndice", modoPantallaGuardado);
        PlayerPrefs.SetInt("FpsIndice", fpsGuardado);
        PlayerPrefs.SetFloat("VolMaster", volGeneralGuardado);
        PlayerPrefs.SetFloat("VolMusica", volMusicaGuardado);
        PlayerPrefs.SetFloat("VolSFX", volSFXGuardado);
        PlayerPrefs.Save();

        hayCambiosPendientes = false;
        Debug.Log("Cambios Guardados");
    }

    private void RevertirCambiosAlEstadoAnterior()
    {
        if (dropdownResolucion != null) { dropdownResolucion.value = resGuardada; dropdownResolucion.RefreshShownValue(); }
        if (dropdownModoPantalla != null) { dropdownModoPantalla.value = modoPantallaGuardado; dropdownModoPantalla.RefreshShownValue(); }
        if (dropdownFPS != null) { dropdownFPS.value = fpsGuardado; dropdownFPS.RefreshShownValue(); }
        if (sliderVolGeneral != null) sliderVolGeneral.value = volGeneralGuardado;
        if (sliderVolMusica != null) sliderVolMusica.value = volMusicaGuardado;
        if (sliderVolSFX != null) sliderVolSFX.value = volSFXGuardado;

        AplicarCambiosEnSistema();
        hayCambiosPendientes = false;
    }

    private void AplicarCambiosEnSistema()
    {
        if (dropdownResolucion != null)
        {
            int ancho = anchos[dropdownResolucion.value];
            int alto = altos[dropdownResolucion.value];
            Screen.SetResolution(ancho, alto, Screen.fullScreenMode);
        }

        if (dropdownModoPantalla != null)
        {
            switch (dropdownModoPantalla.value)
            {
                case 0: Screen.fullScreenMode = FullScreenMode.Windowed; break;
                case 1: Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;
                case 2: Screen.fullScreenMode = FullScreenMode.MaximizedWindow; break;
            }
        }

        if (dropdownFPS != null)
        {
            QualitySettings.vSyncCount = 0;
            switch (dropdownFPS.value)
            {
                case 0: Application.targetFrameRate = 30; break;
                case 1: Application.targetFrameRate = 60; break;
                case 2: Application.targetFrameRate = 120; break;
                case 3: Application.targetFrameRate = 144; break;
                case 4: Application.targetFrameRate = -1; break;
            }
        }

        // Blindaje del Audio Mixer
        if (audioMixer != null)
        {
            if (sliderVolGeneral != null) audioMixer.SetFloat("VolMaster", Mathf.Log10(Mathf.Max(sliderVolGeneral.value, 0.0001f)) * 20);
            if (sliderVolMusica != null) audioMixer.SetFloat("VolMusica", Mathf.Log10(Mathf.Max(sliderVolMusica.value, 0.0001f)) * 20);
            if (sliderVolSFX != null) audioMixer.SetFloat("VolSFX", Mathf.Log10(Mathf.Max(sliderVolSFX.value, 0.0001f)) * 20);
        }
    }

    private void CerrarMenuOpcionesTotalmente()
    {
        if (panelOpcionesPrincipal != null) panelOpcionesPrincipal.SetActive(false);
    }

    // --- INTERRUPTORES DE UI ---
    public void CambiarResolucion(int indice) { hayCambiosPendientes = true; AplicarCambiosEnSistema(); }
    public void CambiarModoPantalla(int indice) { hayCambiosPendientes = true; AplicarCambiosEnSistema(); }
    public void CambiarFPS(int indice) { hayCambiosPendientes = true; AplicarCambiosEnSistema(); }
    public void CambiarVolumenGeneral(float valor) { hayCambiosPendientes = true; AplicarCambiosEnSistema(); }
    public void CambiarVolumenMusica(float valor) { hayCambiosPendientes = true; AplicarCambiosEnSistema(); }
    public void CambiarVolumenSFX(float valor) { hayCambiosPendientes = true; AplicarCambiosEnSistema(); }

    public void AbrirPanelGeneral() => ActivarPanel(panelGeneral);
    public void AbrirPanelGraficos() => ActivarPanel(panelGraficos);
    public void AbrirPanelSonido() => ActivarPanel(panelSonido);
    public void AbrirPanelControles() => ActivarPanel(panelControles);

    private void ActivarPanel(GameObject panelActivo)
    {
        if (panelGeneral) panelGeneral.SetActive(false); 
        if (panelGraficos) panelGraficos.SetActive(false); 
        if (panelSonido) panelSonido.SetActive(false); 
        if (panelControles) panelControles.SetActive(false);
        if (panelActivo) panelActivo.SetActive(true);
    }
}