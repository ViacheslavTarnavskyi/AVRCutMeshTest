using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
   [SerializeField] private GameObject _slicePrefab;
   [SerializeField] private Transform _spawnPosition;
   [SerializeField] private AudioSource _swipeEffectSource;

   private List<SlicedObject> _objectsOnScene = new List<SlicedObject>();
   
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
      SwipeEffect();
      List<RawMeshData> res = new List<RawMeshData>();
      for(int i = _objectsOnScene.Count-1; i >= 0 ; i--)
      {
         res = MeshSlicer.SliceMesh(cutPlane, _objectsOnScene[i]);
         if (res.Count > 0)
         {
            Destroy(_objectsOnScene[i].gameObject);
            _objectsOnScene.RemoveAt(i);
         }
         foreach (var slice in res)
         {
            InstantiateSlice(slice);
         }
      }
   }

   private void InstantiateSlice(RawMeshData data)
   {
      SlicedObject slice = Instantiate(_slicePrefab,transform).GetComponent<SlicedObject>();
      slice.Init(data.MeshTransform,data.ToMesh());
      _objectsOnScene.Add(slice);
   }

   public void SpawnAPrimitive()
   {
      SlicedObject newSlice = Instantiate(_slicePrefab).GetComponent<SlicedObject>();
      newSlice.transform.position = _spawnPosition.position;
      _objectsOnScene.Add(newSlice);
   }

   private void SwipeEffect()
   {
      _swipeEffectSource.Play();
   }
}