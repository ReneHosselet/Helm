using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarScript : MonoBehaviour
{
    private GameObject fogOfWarPlane;
    private Transform playerTransform;
    public LayerMask fogLayer;
    public float radius =5f;
    private float radiusSqr { get { return radius * radius; } }
    private bool isInitialized;
    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;
    // Start is called before the first frame update
    void Start()
    {
        isInitialized = false;
        //fogOfWarPlane = GameObject.FindGameObjectWithTag("Fog");
        //playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        if (playerTransform != null)
        {
            Initialized();
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if (fogOfWarPlane == null)
        {
            fogOfWarPlane = GameObject.FindGameObjectWithTag("Fog");
        }
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        else if (!isInitialized && playerTransform != null)
        {
            Debug.Log("test");
            Initialized();
        }
        else
        {
            Ray r = new Ray(transform.position, playerTransform.position - transform.position);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000, fogLayer, QueryTriggerInteraction.Collide))
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 v = fogOfWarPlane.transform.TransformPoint(vertices[i]);
                    float dist = Vector3.SqrMagnitude(v - hit.point);
                    if (dist < radiusSqr)
                    {
                        float alpha = Mathf.Min(colors[i].a, dist / radiusSqr);
                        colors[i].a = alpha;
                    }
                }
                UpdateColor();
            }
        }       
    }
    private void Initialized()
    {
        isInitialized = true;
        mesh = fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.black;
        }
        UpdateColor();
    }
    void UpdateColor()
    {
        mesh.colors = colors;
    }
}
