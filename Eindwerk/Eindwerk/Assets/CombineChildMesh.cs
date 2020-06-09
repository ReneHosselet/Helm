﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineChildMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CombineMeshes();
    }

    // Update is called once per frame
    void Update()
    {
        CombineMeshes();
    }
    private void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }
}
