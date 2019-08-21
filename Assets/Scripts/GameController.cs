using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
   [SerializeField] private GameObject _slicePrefab;
   
   private List<SlicedObject> _objectsOnScene;

   private void OnEnable()
   {
      CutHandler.OnCutPlaneCreate += ImplementCut;
   }

   private void OnDisable()
   {
      CutHandler.OnCutPlaneCreate -= ImplementCut;
   }

   private void ImplementCut(Plane cutPlane)
   {
      List<RawMeshData> res = new List<RawMeshData>();
      foreach (var obj in _objectsOnScene)
      {
         res = MeshSlicer.SliceMesh(cutPlane, obj);
         foreach (var slice in res)
         {
            InstantiateSlice(slice);
         }
      }
   }

   private void InstantiateSlice(RawMeshData data)
   {
      SlicedObject slice = Instantiate(_slicePrefab).GetComponent<SlicedObject>();
      slice.Init(data.MeshTransform,data.ToMesh());
   }

   public void SpawnAPrimitive()
   {
      
   }
}