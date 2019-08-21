using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshSlicer
{
   public static List<RawMeshData> SliceMesh(Plane cutterPlane, SlicedObject obj)
   {
      if (!IsMeshIntersectedByPlane(cutterPlane, obj.Filter.mesh, obj.Graphics.transform, out RawMeshData positive, out RawMeshData negative))
      {
         return new List<RawMeshData>();
      }

      Cut cut = new Cut(obj.Filter.mesh, positive, negative, cutterPlane);
      
      return cut.meshes;
   }

   private static bool IsMeshIntersectedByPlane( Plane cutterPlane, Mesh mesh, Transform transform ,out RawMeshData pos, out RawMeshData neg)
   {
      pos = new RawMeshData(mesh,transform);
      neg = new RawMeshData(mesh,transform);
      
      if (!mesh.bounds.IsIntersectedByPlane(cutterPlane,transform))
      {
         return false;
      }
      
      else
      {
         DivideVerticesToPosAndNegSubmeshes(mesh,cutterPlane,transform ,mesh.vertices, out pos, out neg);

         if (pos.VertsIndexes.Count == 0 || neg.VertsIndexes.Count == 0)
         {
            return false;
         }
         else
         {
            return true;
         }
      }
   }
   
   private static void DivideVerticesToPosAndNegSubmeshes(Mesh mesh, Plane cutterPlane,Transform meshTransform, Vector3[] verts, out RawMeshData pos, out RawMeshData neg)
   {
      pos = new RawMeshData(mesh, meshTransform);
      neg = new RawMeshData(mesh, meshTransform);
      
      for (int i = 0; i < verts.Length; i++)
      {
         if (cutterPlane.GetSide(meshTransform.TransformPoint(verts[i])))
         {
            pos.VertsIndexes.AddLast(i);
         }
         else
         {
            neg.VertsIndexes.AddLast(i);
         }
      }
   }
}