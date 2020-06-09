using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private float droneSpeed = 2f;
    [SerializeField] private float aggroRadius = 4f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private GameObject droneProjectilePrefab;
    [SerializeField] private GameObject drones;
    [SerializeField] private GameObject panelBlue;
    [SerializeField] private GameObject panelRed;

    private bool flagEndGame = false;
    private float gameTime;


    public static GameSettings Instance { get; private set; }

    public static float DroneSpeed => Instance.droneSpeed;
    public static float AggroRadius => Instance.aggroRadius;
    public static float AttackRange => Instance.attackRange;
    public static GameObject DroneProjectilePrefab => Instance.droneProjectilePrefab;
    public static GameObject Drones => Instance.drones;
    public static GameObject PanelBlue => Instance.panelBlue;
    public static GameObject PanelRed => Instance.panelRed;

    private void Start()
    {
        gameTime = Time.time;
    }
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        if (flagEndGame)
        {
            return;
        }

        Drone[] lastDrones = drones.GetComponentsInChildren<Drone>();

        List<Drone> teamyBlue = lastDrones.Where(u => u.Team.Equals(Team.Blue)).ToList();
        List <Drone> teamyRed = lastDrones.Where(u => u.Team.Equals(Team.Red)).ToList();

        if (Time.time - gameTime > 60f)
        {
            int index = Random.Range(0, lastDrones.Length);

            if (lastDrones[index].Team.Equals(Team.Blue))
            {
                teamyBlue = new List<Drone>();
            }
            else
            {
                teamyRed = new List<Drone>();
            }
        }

        if (teamyBlue.Count == 0)
        {
            panelRed.SetActive(true);
            panelRed.GetComponentInChildren<Button>().onClick.AddListener(Restart);
            flagEndGame = true;
            return;
        }
        if (teamyRed.Count == 0)
        {
            panelBlue.SetActive(true);
            panelBlue.GetComponentInChildren<Button>().onClick.AddListener(Restart);
            flagEndGame = true;
            return;
        }
        
    }

    public void Restart()
    {
        if (flagEndGame)
        {
            SceneManager.LoadScene(0);
        }
        
    }
    // DrawLine with GL.LINES
    /*
    static Material lineMaterial;
    // When added to an object, draws colored rays from the
    // transform position.
    public int lineCount = 100;
    public float radius = 3.0f;

    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public void OnRenderObject() //OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        for (int i = 0; i < lineCount; ++i)
        {
            float a = i / (float)lineCount;
            float angle = a * Mathf.PI * 2;
            // Vertex colors change from red to green
            GL.Color(new Color(a, 1 - a, 0, 0.8F));
            // One vertex at transform position
            GL.Vertex3(0, 0, 0);
            // Another vertex at edge of circle
            GL.Vertex3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }
        GL.End();
        GL.PopMatrix();
    }
    */
}
