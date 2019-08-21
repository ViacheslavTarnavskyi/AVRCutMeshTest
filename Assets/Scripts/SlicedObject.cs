﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicedObject : MonoBehaviour
{
    [SerializeField]private MeshRenderer _renderer;
    [SerializeField]private MeshFilter _filter;
    [SerializeField]private MeshCollider _collider;
    [SerializeField]private Rigidbody _rigidBody;
    
    public MeshRenderer Graphics { get; }
    public MeshFilter Filter { get; }

    public void Init(Transform prototype, Mesh mesh)
    {
        var transformCashed = transform;
        transformCashed.position = prototype.position;
        transformCashed.rotation = prototype.rotation;
        Filter.mesh = mesh;
    }
}