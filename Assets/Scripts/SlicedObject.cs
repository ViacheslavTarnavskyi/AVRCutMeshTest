using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlicedObject : MonoBehaviour
{
    private const float cutForce = 80;
    
    [SerializeField]private MeshRenderer _renderer;
    [SerializeField]private MeshFilter _filter;
    [SerializeField]private MeshCollider _collider;
    [SerializeField]private Rigidbody _rigidBody;

    public MeshRenderer Graphics => _renderer;
    public MeshFilter Filter => _filter;

    public void Init(Transform prototype, Mesh mesh)
    {
        var transformCashed = transform;
        transformCashed.position = prototype.position;
        transformCashed.rotation = prototype.rotation;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        Filter.mesh = mesh;
        _collider.sharedMesh = mesh;
       _rigidBody.isKinematic = false;
       _rigidBody.AddForce(new Vector3(Random.Range(0,cutForce),Random.Range(0,cutForce),Random.Range(0,cutForce)));
    }
}
