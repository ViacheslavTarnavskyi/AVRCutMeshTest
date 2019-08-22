using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cut
{
   private Mesh _originalMesh;
   private RawMeshData _posSubMesh;
   private RawMeshData _negSubMesh;
   private Plane _cutterPlane;

   public List<RawMeshData> Meshes { get; private set; } 
   
   /// <summary>
   /// Constructor
   /// </summary>
   /// <param name="originalMesh"></param>
   /// <param name="posSubMesh"></param>
   /// <param name="negSubMesh"></param>
   /// <param name="cutterPlane"></param>
   public Cut(Mesh originalMesh, RawMeshData posSubMesh, RawMeshData negSubMesh, Plane cutterPlane)
   {
      _originalMesh = originalMesh;
      _posSubMesh = posSubMesh;
      _negSubMesh = negSubMesh;
      _cutterPlane = cutterPlane;

      Meshes = new List<RawMeshData>();
      CutMesh();
   }

   /// <summary>
   /// Cut algorithm
   /// </summary>
   private void CutMesh()
   {
      List<Vector3> cutEdge = new List<Vector3>();
      List<Triangle> originalTriangles = _originalMesh.GetMeshTriangles();
      foreach (var triangle in originalTriangles)
      {
         if (triangle.Belongs(_posSubMesh))
         {
            CopyMeshValuesByTri(triangle, _posSubMesh);
         }
         else if (triangle.Belongs(_negSubMesh))
         {
            CopyMeshValuesByTri(triangle, _negSubMesh);
         }
         else
         {
            CutTriangle(triangle, out List<Vector3> cut);
            cutEdge.AddRange(cut);
         }
      }
      
      AddCap(_negSubMesh,cutEdge,true);
      AddCap(_posSubMesh,cutEdge,false);
      
      Meshes.Add(_posSubMesh);
      Meshes.Add(_negSubMesh);
   }

/// <summary>
/// Ads a cap for the hull data
/// </summary>
/// <param name="data"></param>
/// <param name="edge"></param>
/// <param name="inverted"></param>
   private void AddCap(RawMeshData data,List<Vector3> edge, bool inverted)
   {
      Vector3 capCenter = Utils.ComputeCenter(edge);
      edge.Add(capCenter);
      int VertsCount = data.Verts.Count;
      data.Verts.AddRange(edge);

      if (inverted)
      {
         for (int i = VertsCount; i < data.Verts.Count - 2; i += 2)
         {
            data.Tris.AddLast(new Triangle(i, i + 1, data.Verts.Count - 1));
         }
      }
      else
      {
         for (int i = VertsCount; i < data.Verts.Count - 2; i += 2)
         {
            data.Tris.AddLast(new Triangle(i + 1, i, data.Verts.Count - 1));
         }
      }
   }
   
   /// <summary>
   /// Copies verts from data prototype by a triangle
   /// </summary>
   /// <param name="triangle"></param>
   /// <param name="data"></param>
   private void CopyMeshValuesByTri(Triangle triangle, RawMeshData data)
   {
      data.Verts.AddRange(data.Prototype.GetVerts(triangle));
      data.Tris.AddLast(new Triangle(data.Verts.Count - 3, data.Verts.Count - 2, data.Verts.Count - 1));
   }

   /// <summary>
   /// Cuts a triangle
   /// </summary>
   /// <param name="triangle"></param>
   /// <param name="cutEdge"></param>
   private void CutTriangle(Triangle triangle, out List<Vector3> cutEdge)
   {
      cutEdge = new List<Vector3>();

      triangle.DetectTriangleOriginSide(_posSubMesh, _negSubMesh, out RawMeshData originSide, out RawMeshData alienSide);
      TriangleCutTypeOrigin cutType = triangle.DetectTriangleCutType(originSide);
      
      //different cut logic due to a triangle points location
      switch (cutType)
      {
         case TriangleCutTypeOrigin.P0:
            cutEdge.AddRange(CreateP0Cut(triangle, originSide, alienSide));
            break;
         case TriangleCutTypeOrigin.P01:
            cutEdge.AddRange(CreateP01Cut(triangle, originSide, alienSide));
            break;
         case TriangleCutTypeOrigin.P20:
            cutEdge.AddRange(CreateP20Cut(triangle, originSide, alienSide));
            break;
      }
   }

   /// <summary>
   /// Computes a line/plane intersection point
   /// </summary>
   /// <param name="plane"></param>
   /// <param name="l1"></param>
   /// <param name="l2"></param>
   /// <returns></returns>
   private Vector3 PlaneLineIntersection(Plane plane, Vector3 l1, Vector3 l2)
   {
      Ray ray = new Ray();
      ray.origin = l1;
      ray.direction = l2 - l1;
      plane.Raycast(ray, out var enter);
      return ray.GetPoint(enter);
   }

   /// <summary>
   /// Triangle Cut logic for triangle (0)|(1,2)
   /// </summary>
   /// <param name="triangle"></param>
   /// <param name="originSide"></param>
   /// <param name="alienSide"></param>
   /// <returns></returns>
   private List<Vector3> CreateP0Cut(Triangle triangle, RawMeshData originSide, RawMeshData alienSide)
   {
      List<Vector3> cutEdge = new List<Vector3>();
      
      //getting verts values by triangle
      Vector3 p0Val = _originalMesh.vertices[triangle.P0];
      Vector3 p1Val = _originalMesh.vertices[triangle.P1];
      Vector3 p2Val = _originalMesh.vertices[triangle.P2];
      
      //computing plane/tri intersection points
      Vector3 intersection1 = GetLocalIntersectionPoint(_cutterPlane, p0Val, p1Val,originSide);
      Vector3 intersection2 = GetLocalIntersectionPoint(_cutterPlane, p0Val, p2Val,originSide);

      //adding new vertices on one side of the plane
      originSide.Verts.AddLast(p0Val);
      originSide.Verts.AddLast(intersection1);
      originSide.Verts.AddLast(intersection2);

      //generating a triangle on these vertices
      originSide.Tris.AddLast(new Triangle(
         originSide.Verts.Count - 3, 
         originSide.Verts.Count - 2, 
         originSide.Verts.Count - 1)
      );
            
      //adding new vertices on other side of the plane
      alienSide.Verts.AddLast(intersection2);
      alienSide.Verts.AddLast(intersection1);
      alienSide.Verts.AddLast(p1Val);
      alienSide.Verts.AddLast(p2Val);

      //generating triangles on these vertices
      alienSide.Tris.AddLast(new Triangle(
         alienSide.Verts.Count - 3,
         alienSide.Verts.Count - 2,
         alienSide.Verts.Count - 4)
      );
      alienSide.Tris.AddLast(new Triangle(
         alienSide.Verts.Count - 4, 
         alienSide.Verts.Count - 2, 
         alienSide.Verts.Count - 1)
      );

      //adding intersections as edge points order dependent on origin side relatively to a plane
      if (originSide.Equals(_posSubMesh))
      {
         cutEdge.Add(intersection1);
         cutEdge.Add(intersection2);
      }
      else
      {
         cutEdge.Add(intersection2);
         cutEdge.Add(intersection1);
      }

      return cutEdge;
   }

   /// <summary>
   /// Triangle Cut logic for triangle (0,1)|(2)
   /// </summary>
   /// <param name="triangle"></param>
   /// <param name="originSide"></param>
   /// <param name="alienSide"></param>
   /// <returns></returns>
   private List<Vector3> CreateP01Cut(Triangle triangle, RawMeshData originSide, RawMeshData alienSide)
   {
      List<Vector3> cutEdge = new List<Vector3>();
           
      //getting verts values by triangle
      Vector3 p0Val = _originalMesh.vertices[triangle.P0];
      Vector3 p1Val = _originalMesh.vertices[triangle.P1];
      Vector3 p2Val = _originalMesh.vertices[triangle.P2];

      //computing plane/tri intersection points
      Vector3 intersection1 = GetLocalIntersectionPoint(_cutterPlane, p0Val, p2Val,originSide);
      Vector3 intersection2 = GetLocalIntersectionPoint(_cutterPlane, p1Val, p2Val,originSide);

      //adding new vertices on one side of the plane
      originSide.Verts.AddLast(p0Val);
      originSide.Verts.AddLast(p1Val);
      originSide.Verts.AddLast(intersection2);
      originSide.Verts.AddLast(intersection1);
            
      //generating triangles on these vertices
      originSide.Tris.AddLast(new Triangle(
         originSide.Verts.Count - 3, 
         originSide.Verts.Count - 2, 
         originSide.Verts.Count - 4)
      );
      originSide.Tris.AddLast(new Triangle(
         originSide.Verts.Count - 4,
         originSide.Verts.Count - 2,
         originSide.Verts.Count - 1)
      );

      //adding new vertices on other side of the plane
      alienSide.Verts.AddLast(intersection1);
      alienSide.Verts.AddLast(intersection2);
      alienSide.Verts.AddLast(p2Val);

      //generating triangles on these vertices
      alienSide.Tris.AddLast(new Triangle(
         alienSide.Verts.Count - 3, 
         alienSide.Verts.Count - 2,
         alienSide.Verts.Count - 1)
      );

      //adding intersections as edge points order dependent on origin side relatively to a plane

      if (originSide.Equals(_posSubMesh))
      {
         cutEdge.Add(intersection2);
         cutEdge.Add(intersection1);
      }
      else
      {
         cutEdge.Add(intersection1);
         cutEdge.Add(intersection2);
      }
      return cutEdge;
   }

   /// <summary>
   /// Triangle Cut logic for triangle (2,0)|(1) 
   /// </summary>
   /// <param name="triangle"></param>
   /// <param name="originSide"></param>
   /// <param name="alienSide"></param>
   /// <returns></returns>
   private List<Vector3> CreateP20Cut(Triangle triangle, RawMeshData originSide, RawMeshData alienSide)
   {
      List<Vector3> cutEdge = new List<Vector3>();
           
      //getting verts values by triangle
      Vector3 p0Val = _originalMesh.vertices[triangle.P0];
      Vector3 p1Val = _originalMesh.vertices[triangle.P1];
      Vector3 p2Val = _originalMesh.vertices[triangle.P2];
      
      //computing plane/tri intersection points
      Vector3 intersection1 = GetLocalIntersectionPoint(_cutterPlane, p0Val, p1Val, originSide);
      Vector3 intersection2 = GetLocalIntersectionPoint(_cutterPlane, p2Val, p1Val, originSide);
      
      //adding new vertices on one side of the plane
      originSide.Verts.AddLast(p2Val);
      originSide.Verts.AddLast(p0Val);
      originSide.Verts.AddLast(intersection1);
      originSide.Verts.AddLast(intersection2);

      //generating triangles on these vertices
      originSide.Tris.AddLast(new Triangle(
         originSide.Verts.Count - 3,
         originSide.Verts.Count - 2,
         originSide.Verts.Count - 4)
      );
      originSide.Tris.AddLast(new Triangle(
         originSide.Verts.Count - 4,
         originSide.Verts.Count - 2,
         originSide.Verts.Count - 1)
      );

      //adding new vertices on other side of the plane
      alienSide.Verts.AddLast(intersection2);
      alienSide.Verts.AddLast(intersection1);
      alienSide.Verts.AddLast(p1Val);

      //generating triangles on these vertices
      alienSide.Tris.AddLast(new Triangle(
         alienSide.Verts.Count - 3,
         alienSide.Verts.Count - 2, 
         alienSide.Verts.Count - 1)
      );

      //adding intersections as edge points order dependent on origin side relatively to a plane
      if (originSide.Equals(_posSubMesh))
      {
         cutEdge.Add(intersection1);
         cutEdge.Add(intersection2);
      }
      else
      {
         cutEdge.Add(intersection2);
         cutEdge.Add(intersection1);
      }
      return cutEdge;
   }

   /// <summary>
   /// Computes intersection point vor 2 verts and a plane in mesh local space 
   /// </summary>
   /// <param name="plane"></param>
   /// <param name="p1"></param>
   /// <param name="p2"></param>
   /// <param name="mesh"></param>
   /// <returns></returns>
   private Vector3 GetLocalIntersectionPoint(Plane plane, Vector3 p1, Vector3 p2, RawMeshData mesh)
   {
      //computing verts world position;
      Vector3 p0World = mesh.MeshTransform.TransformPoint(p1);
      Vector3 p1World = mesh.MeshTransform.TransformPoint(p2);
      
      return mesh.MeshTransform.InverseTransformPoint(PlaneLineIntersection(_cutterPlane, p0World, p1World));
   }
}