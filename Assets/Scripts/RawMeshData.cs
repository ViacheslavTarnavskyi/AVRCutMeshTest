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
    }
    
    public RawMeshData(Mesh prototype,Transform meshTransform): this()
    { 
        Prototype = prototype;
        MeshTransform = meshTransform;
    }

    public void CopyMeshParams(int index)
    {
        Normals.AddLast(Prototype.normals[index]);
        Uvs.AddLast(Prototype.uv[index]);
        Colors.AddLast(Prototype.colors[index]);
    }

    public void AddMeshParams(Vector3 normal,Vector2 uv,Color color)
    {
        Normals.AddLast(normal);
        Uvs.AddLast(uv);
        Colors.AddLast(color);
    }

    public Mesh ToMesh()
    {
        Mesh res = new Mesh();
        res.vertices = Verts.ToArray();
        res.triangles = Utils.ToTris(Tris);
        res.uv = Uvs.ToArray();
        res.colors = Colors.ToArray();
        return res;
    }
}