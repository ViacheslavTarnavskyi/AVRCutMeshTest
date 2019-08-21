using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Gets Triangle list from a mesh
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    public static List<Triangle> GetMeshTriangles(this Mesh mesh)
    {
        List<Triangle> res = new List<Triangle>(); 
      
        for (int i = 0; i < mesh.triangles.Length - 2; i += 3)
        {
            res.Add(new Triangle(mesh.triangles[i],mesh.triangles[i+1],mesh.triangles[i+2]));
        }

        return res;
    }
    
    /// <summary>
    /// tels if a triangle belongs whole to mesh data
    /// </summary>
    /// <param name="triangle"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool Belongs(this Triangle triangle, RawMeshData data)
    {
        return data.VertsIndexes.Contains(triangle.P0) && data.VertsIndexes.Contains(triangle.P1) && data.VertsIndexes.Contains(triangle.P2);
    }
    
    /// <summary>
    /// Gets verts from a mesh by a triangle
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="triangle"></param>
    /// <returns> verts list</returns>
    public static List<Vector3> GetVerts(this Mesh mesh, Triangle triangle)
    {
        List<Vector3> res = new List<Vector3>();
        res.Add(mesh.vertices[triangle.P0]);
        res.Add(mesh.vertices[triangle.P1]);
        res.Add(mesh.vertices[triangle.P2]);
        return res;
    }
    
    /// <summary>
    /// Calculate courners by Bounds
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public static Vector3[] GetBoundsCorners(this Bounds bounds)
    {
        Vector3[] boundPoints = new Vector3[8];
      
        boundPoints[0] = bounds.min;
        boundPoints[1] = bounds.max;
        boundPoints[2] = new Vector3(boundPoints[0].x, boundPoints[0].y, boundPoints[1].z);
        boundPoints[3] = new Vector3(boundPoints[0].x, boundPoints[1].y, boundPoints[0].z);
        boundPoints[4] = new Vector3(boundPoints[1].x, boundPoints[0].y, boundPoints[0].z);
        boundPoints[5] = new Vector3(boundPoints[0].x, boundPoints[1].y, boundPoints[1].z);
        boundPoints[6] = new Vector3(boundPoints[1].x, boundPoints[0].y, boundPoints[1].z);
        boundPoints[7] = new Vector3(boundPoints[1].x, boundPoints[1].y, boundPoints[0].z);

        return boundPoints;
    }
    
    /// <summary>
    /// telss whethe Bounds is intersected by a plane
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="cutterPlane"></param>
    /// <param name="boundsTransform"></param>
    /// <returns>true if does</returns>
    public static bool IsIntersectedByPlane(this Bounds bounds, Plane cutterPlane, Transform boundsTransform )
    {
        Vector3[] boundPoints = bounds.GetBoundsCorners();

        for (int i = 0; i < boundPoints.Length; i++)
        {
            boundPoints[i] = boundsTransform.TransformPoint(boundPoints[i]);
        }
        
        bool[] positivSided = new bool[8];
      
        positivSided[0] = cutterPlane.GetSide(boundPoints[0]);

        for (int i = 1; i < boundPoints.Length; i++)
        {
            positivSided[i] = cutterPlane.GetSide(boundPoints[i]);
            if (positivSided[i] != positivSided[i - 1])
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// AddRange Extension for a LinkedList
    /// </summary>
    /// <param name="list"></param>
    /// <param name="range"></param>
    /// <typeparam name="T"></typeparam>
    public static void AddRange<T>(this LinkedList<T> list, List<T> range)
    {
        foreach (var el in range)
        {
            list.AddLast(el);
        } 
    }

    /// <summary>
    /// Copmutes a center point among a point array
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static Vector3 ComputeCenter(List<Vector3> points)
    {
        Vector3 center = Vector3.zero;

        foreach (var point in points)
        {
            center += point;
        }

        return center/ points.Count;
    }

    /// <summary>
    /// Translates Triangles to int tris array
    /// </summary>
    /// <param name="tris"></param>
    /// <returns></returns>
    public static int[] ToTris(LinkedList<Triangle> tris)
    {
        int[] res = new int[tris.Count * 3];
        int index = 0;

        foreach (var tri in tris)
        {
            res[index] = tri.P0;
            index++;
            res[index] = tri.P1;
            index++;
            res[index] = tri.P2;
            index++;
        }

        return res;
    }

    /// <summary>
    /// Random range for a vector3
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static Vector3 Random(this Vector3 origin, float range)
    {
        return new Vector3(
            origin.x * UnityEngine.Random.Range(0,range),
            origin.y * UnityEngine.Random.Range(0,range),
            origin.z * UnityEngine.Random.Range(0,range)
            );
    }
}