using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    [SelectionBase]
    class TectonicPlate : MonoBehaviour
    {
        private GameObject plateObject;
        public List<VoronoiTile> tiles;
        public Vector3 middle;
        private List<Vector3> allVerts;
        public bool isLand = false;
        public bool isDesert = false;
        public float baseHumidity = 0;

        public void Initialize(ref List<VoronoiTile> list)
        {
            tiles = list;
            foreach (var voronoiTile in tiles)
            {
                voronoiTile.transform.parent = transform;
            }
            allVerts = new List<Vector3>();
            foreach (var tile in tiles)
            {
                foreach (var vertex in tile.vertices)
                {
                    allVerts.Add(vertex);
                }
            }
            FindMiddle();
            baseHumidity = (Random.Range(0.0f, 3.0f));
            SetHumidity(baseHumidity);
        }

        private void FindMiddle()
        {
            float x = 0, y = 0, z = 0;
            for (int i = 0; i < allVerts.Count; ++i)
            {
                x += allVerts[i].x;
                y += allVerts[i].y;
                z += allVerts[i].z;
            }
            x /= allVerts.Count;
            y /= allVerts.Count;
            z /= allVerts.Count;
            middle = new Vector3(x, y, z);
        }

        public void PushOutLand(float amount)
        {
            foreach (var tile in tiles)
            {
                Vector3[] verts = tile.tileMesh.vertices;
                for (int i = 0; i < verts.Length; ++i)
                {
                    Vector3 height = verts[i] - Vector3.zero;
                    verts[i] += height*amount; //*0.0075f; //0.015f
                }
                tile.tileMesh.vertices = verts;
                tile.tileMesh.RecalculateNormals();
                tile.altitude += amount;
            }
        }

        public void SetTemp(float temp)
        {
            foreach (var tile in tiles)
            {
                tile.temperature = temp;
            }
        }

        public void SetHumidity(float humidity)
        {
            foreach (var tile in tiles)
            {
                tile.humidity = humidity;
            }
        }
    }
}
