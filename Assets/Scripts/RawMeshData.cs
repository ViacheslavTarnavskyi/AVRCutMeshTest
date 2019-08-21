using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RawMeshData
{
    public LinkedList<Vector3> Verts { get; private set; }
    public LinkedList<int> VertsIndexes { get; private set; }
    public LinkedList<Triangle> Tris  { get; private set; }
    public LinkedList<Vector2> Uvs  { get; private set; }
    public LinkedList<Vector3> Normals  { get; private set; }
    public LinkedList<Color> Colors  { get; private set; }
    public Mesh Prototype { get; private set; }
    public Transform MeshTransform { get; private set; }

    public Vector3 GetWorldCenterPoint => MeshTransform.TransformPoint(Utils.ComputeCenter(Verts.ToList()));

    public RawMeshData()
    {
         Verts = new LinkedList<Vector3>();
         VertsIndexes = new LinkedList<int>();
         Tris = new LinkedList<Triangle>();
         Uvs = new LinkedList<Vector2>();
         Normals = new LinkedList<Vector3>();
         Colors = new LinkedList<Color>();
    }
    
    public RawMeshData(Mesh prototype,Transform meshTransform): this()
    { 
        Prototype = prototype;
        MeshTransform = meshTransform;
    }

    /// <summary>
    /// Converrts RawMeshData into a Mesh object
    /// </summary>
    /// <returns></returns>
    public Mesh ToMesh()
    {
        Mesh res = new Mesh();
        res.vertices = Verts.ToArray();
        res.triangles = Utils.ToTris(Tris);
        res.RecalculateNormals();
        return res;
    }
}