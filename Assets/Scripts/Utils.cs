using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static List<Triangle> GetMeshTriangles(this Mesh mesh)
    {
        List<Triangle> res = new List<Triangle>(); 
      
        for (int i = 0; i < mesh.triangles.Length - 2; i += 3)
        {
            res.Add(new Triangle(mesh.triangles[i],mesh.triangles[i+1],mesh.triangles[i+2]));
        }

        return res;
    }
    
    public static bool Belongs(this Triangle triangle, RawMeshData data)
    {
        return data.VertsIndexes.Contains(triangle.P0) && data.VertsIndexes.Contains(triangle.P1) && data.VertsIndexes.Contains(triangle.P2);
    }
    
    public static List<Vector3> GetVerts(this Mesh mesh, Triangle triangle)
    {
        List<Vector3> res = new List<Vector3>();
        res.Add(mesh.vertices[triangle.P0]);
        res.Add(mesh.vertices[triangle.P1]);
        res.Add(mesh.vertices[triangle.P2]);
        return res;
    }
    
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

    public static void AddRange<T>(this LinkedList<T> list, List<T> range)
    {
        foreach (var el in range)
        {
            list.AddLast(el);
        } 
    }

    public static Vector3 ComputeCenter(List<Vector3> points)
    {
        Vector3 center = Vector3.zero;

        foreach (var point in points)
        {
            center += point;
        }

        return center/ points.Count;
    }

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
    
    public static List<int> TriangulateArea(List<Vector3> position, Vector3 normal)
    {
        if (position.Count >= 3)
        {
            List<int> triangles = new List<int>();
            List<int> idx = new List<int>();
            Debug.LogError("wqwqwqwq");
            for (int i = 0; i < 4; i++)
            {
                Debug.LogError(i);

                idx.Add(0);
            }
            Vector3 cross = Vector3.zero;
            float inner = 0.0f;
            Debug.LogError("dfdfdfdf");

            for (int i = 0; i < position.Count; i += 3)
            {
                for (int k = 1; k < 4; k++)
                {
                    idx[k] = idx[0] + (k - 1);
                }

                cross = Vector3.Cross(position[idx[3]] - position[idx[1]], position[idx[2]] - position[idx[1]]);
                inner = Vector3.Dot(cross, normal);
                if (inner < 0)
                {
                    idx[1] = idx[3];
                    idx[3] = idx[0];
                }
                for (int j = 1; j < 4; j++)
                {
                    triangles.Add(idx[j]);
                }
                idx[0]++;
            }

            return triangles;
        }
        else
        {
            return null;
        }
    }

    public static List<Triangle> ToTriangles(this List<int> tris)
    {
        List<Triangle> res = new List<Triangle>();
        for (int i = 0; i < tris.Count; i+=3)
        {
            res.Add(new Triangle(tris[i],tris[i+1],tris[i+2]));
        }
        return res;
    }
}