using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSorting : MonoBehaviour
{
    void Start()
    {
        // Set the sorting layer of the particle system.
        var sorting = this.GetComponent<ParticleSystem>().GetComponent<Renderer>();
        sorting.sortingLayerName = "Default";
        sorting.sortingOrder = 3;
    }
}
