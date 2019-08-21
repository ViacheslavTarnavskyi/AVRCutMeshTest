using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlicedObject : MonoBehaviour
{
    private const float CUT_FORCE = 30;
    private const float DRAG = 5;
    
    [SerializeField]private MeshRenderer _renderer;
    [SerializeField]private MeshFilter _filter;
    [SerializeField]private MeshCollider _collider;
    [SerializeField]private Rigidbody _rigidBody;

    public MeshRenderer Graphics => _renderer;
    public MeshFilter Filter => _filter;

    public void Init(Transform prototype, Mesh mesh)
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        var transformCashed = transform;
        transformCashed.position = prototype.position;
        transformCashed.rotation = prototype.rotation;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        Filter.mesh = mesh;
        _collider.sharedMesh = mesh;
       _rigidBody.isKinematic = false;
       _rigidBody.drag = DRAG;
       _rigidBody.AddForce( Vector3.one.Random(CUT_FORCE));
       _rigidBody.AddTorque(Vector3.one.Random(CUT_FORCE));
    }
}
