/*
 * Arquitectura: Scanner/Runtime
 * Script: ScannerTrigger
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona escaneo de elementos, datos escaneables y feedback visual del escaner.
 * Relaciones: Usa IScannable para escanear objetos sin conocer su implementacion concreta.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ScannerTrigger : MonoBehaviour
{
    [Header("Configuracion de input")]
    public InputActionReference scanAction;
    [Header("Configuracion de escaneo")]
    public GameObject PivotScan;
    public AudioSource audioScanner;
    private Animator animator;
    [Header("Interfaz de usuario")]
    public GameObject canvasName;
    public TextMeshProUGUI elementNameText;
    public GameObject objectActual;
    private void Awake()
    {
        if (PivotScan != null)
        {
        animator = PivotScan.GetComponent<Animator>();
        PivotScan.SetActive(false);

        }
        scanAction.action.performed += ctx => ToogleScanning();
    }
    private void OnEnable()
    {
        scanAction.action.Enable();
    }

    private void OnDisable()
    {
        scanAction.action.Disable();
    }

    private void ToogleScanning()
    {
        if (PivotScan != null)
        {
            if (PivotScan.activeSelf)
            {
                StopScanning();
            }
            else
            {
                StartScanning();
            }
        }
    }
    private void StartScanning()
    {
        //Debug.Log("ScannerTrigger: StartScanning called");
        if (PivotScan != null)
        {
            PivotScan.SetActive(true);
            if (AudioManager.Instance != null)
            {
            AudioManager.Instance.Play("Scannersound");
            }
            if (animator != null)
            {
                animator.SetTrigger("StartScan");
            }
            //if (audioScanner != null)
            //{
            //    audioScanner.Play();
            //}

        }
    }

    private void StopScanning()
    {
        //Debug.Log("ScannerTrigger: StopScanning called");
        if (PivotScan != null)
        {
            PivotScan.SetActive(false);
            AudioManager.Instance.Stop("Scannersound");
            if (animator != null)
            {
                animator.ResetTrigger("StartScan");
            }
            if (audioScanner != null)
            {
                audioScanner.Stop();
            }
        }
    }
    public void captureElementInfo(GameObject element)
    {
        string nameScaned = element.name;
        Debug.Log("Element scanned: " + nameScaned);
        //if (canvasName != null && elementNameText !=null)
        //{
        //    elementNameText.text = element.name;
            
        //}
    }
    public void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Element"))
        {
            objectActual = other.gameObject;
            string elementName = other.gameObject.name;
            Debug.Log("{elementName}" + elementName);
            Debug.Log(other.gameObject.name);
  
            //ScannerSystem.Instance.StartScanning();
        }
    }
    [Header("Texture Effect")]
    public float scanDuration = 2f; // Duración del escaneo en segundos
    private Renderer meshRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = PivotScan.GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PivotScan.activeSelf && meshRenderer != null)
        {
            float offset = Time.time * scanDuration;
            meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
    }
}
