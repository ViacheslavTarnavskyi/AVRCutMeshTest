using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
        public int P0 { get; }
        public int P1 { get; }
        public int P2 { get; }

        public Triangle(int p0, int p1, int p2)
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
        }
      
        public TriangleCutTypeOrigin DetectTriangleCutType(RawMeshData origin)
        {
            if (origin.VertsIndexes.Contains(P1))
            {
                return TriangleCutTypeOrigin.P01;
            }
            else if(origin.VertsIndexes.Contains(P2))
            {
                return TriangleCutTypeOrigin.P20;
            }
            else
            {
                return TriangleCutTypeOrigin.P0;
            }
        }
      
        public void DetectTriangleOriginSide(RawMeshData posVerts, RawMeshData negVerts,out RawMeshData origin,out RawMeshData alien)
        {
            if (posVerts.VertsIndexes.Contains(P0))
            {
                origin = posVerts;
                alien = negVerts;
            }
            else
            {
                origin = negVerts;
                alien = posVerts;
            }
        }
}
