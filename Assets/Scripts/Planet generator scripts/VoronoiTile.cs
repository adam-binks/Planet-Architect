using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
    class VoronoiTile : MonoBehaviour
    {
        public GameObject tileObject;
        public List<Vector3> vertices;
        private Vector3 normal;
        public Vector3 centerPoint;
        private Color tileColor;
        public Mesh tileMesh;
        public Mesh dualMesh;
        public Material tileMat;
        public List<VoronoiTile> neighbors;
        public MeshCollider collider;
        public bool processed = false;
        public int plate;
        public bool sand = false;
        public bool rock = false;
        
        // PROPRETIES
        public float humidity = -1;
        public float temperature = -1;
        public float altitude = 0;
        public Biomes biome = 0;
        public Biomes baseBiome = 0;

        private bool test = false;

        public Vector3 Normal { get {return normal;} set { normal = value; } }

        public void Initialize(List<Vector3> verts, bool ocean)
        {
            // initialize
            tileObject = new GameObject("TileObject", typeof(MeshRenderer));
            tileMesh = new Mesh();
            vertices = verts;
            plate = -1;

            // find centerpoint
            centerPoint = CenterPoint(vertices);

            //centerPoint = Vector3.zero;
            tileColor = new Color(UnityEngine.Random.Range(0.0f, 1.0f), 
                UnityEngine.Random.Range(0.0f, 1.0f),
                UnityEngine.Random.Range(0.0f, 1.0f), 1);
            

            // sort points clockwise
            SortPointsClockwise(centerPoint);

            // create triangles
            if (ocean == false) CreateTriangles(tileColor);
            else CreateTrianglesOcean();
            foreach (Transform child in transform) Destroy(child.gameObject);

            // merge into one mesh
            UnifyMesh(ref tileMesh);
            Destroy(tileObject);
        }

        private void CreateTriangles(Color color)
        {
            Color[] colors = new Color[3];
            for (int c = 0; c < 3; ++c) colors[c] = tileColor;
            for (int i = 0; i < vertices.Count; ++i)
            {
                Vector3 a = vertices[i];
                Vector3 b = new Vector3();
                if (i == vertices.Count - 1) b = vertices[0];
                else b = vertices[i + 1];
                Mesh triangle = GenerateTriangle(a, b, centerPoint);

                //Mesh triangle = GenerateTriangle(a, b, c);
                GameObject triangleObject = new GameObject("Triangle", typeof(MeshFilter));
                triangleObject.GetComponent<MeshFilter>().mesh = triangle;
                triangleObject.transform.parent = this.transform;

                Mesh triangleToCenter = new Mesh();

                triangleToCenter = GenerateTriangle(b, a, Vector3.zero);
                GameObject triangleToCenterObject = new GameObject("Triangle", typeof(MeshFilter));
                triangleToCenterObject.GetComponent<MeshFilter>().mesh = triangleToCenter;
                triangleToCenterObject.transform.parent = this.transform;
            }
        }

        private void CreateTrianglesOcean()
        {
            for (int i = 0; i < vertices.Count; ++i)
            {
                Vector3 a = vertices[i];
                Vector3 b = new Vector3();
                if (i == vertices.Count - 1) b = vertices[0];
                else b = vertices[i + 1];

                Mesh triangle = new Mesh();
                triangle = GenerateTriangle(a, b, centerPoint);
                GameObject triangleObject = new GameObject("Triangle", typeof(MeshFilter));
                triangleObject.GetComponent<MeshFilter>().mesh = triangle;
                triangleObject.transform.parent = this.transform;
            }
        }

        private Mesh GenerateTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            Mesh face = new Mesh
            {
                vertices = new Vector3[] {a, b, c},
                triangles = new int[] {0, 1, 2}
            };
            face.RecalculateNormals();
            face.RecalculateBounds();
            return face;
        }

        private Vector3 CenterPoint(List<Vector3> vertices)
        {
            float x = 0, y = 0, z = 0;
            for (int i = 0; i < vertices.Count; ++i)
            {
                x += vertices[i].x;
                y += vertices[i].y;
                z += vertices[i].z;
            }
            x /= vertices.Count;
            y /= vertices.Count;
            z /= vertices.Count;
            return new Vector3(x, y, z);
        }

        private void UnifyMesh(ref Mesh mesh)
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];
            var index = 0;
            for (int i = 0; i < meshFilters.Length; ++i)
            {
                if (meshFilters[i].sharedMesh == null) continue;
                combine[index].mesh = meshFilters[i].sharedMesh;
                combine[index++].transform = meshFilters[i].transform.localToWorldMatrix;
            }
            mesh.CombineMeshes(combine);
        }

        private void SortPointsClockwise(Vector3 centerPoint)
        {
            // find coordinate system from point
            Vector3 toCross = vertices.First() - Vector3.zero;
            Vector3 toCross2 = centerPoint - Vector3.zero;
            Vector3 up = Vector3.Cross(toCross2, toCross);
            
            // create axis
            Vector3 Zaxis = Vector3.Normalize(toCross2);
            Vector3 Xaxis = Vector3.Normalize(Vector3.Cross(up, Zaxis));
            Vector3 Yaxis = Vector3.Cross(Zaxis, Xaxis);
            
            // sort points clockwise for winding with new coordinate system
            vertices.Sort((e1, e2) => Mathf.Atan2(Vector3.Dot(e1, Yaxis), 
                Vector3.Dot(e1, Xaxis)).CompareTo(Mathf.Atan2(Vector3.Dot(e2, Yaxis), Vector3.Dot(e2, Xaxis))));
        }

        public override string ToString()
        {
            return String.Format("Color: {0} Vertices: {1} Center: {2} Normal: {3}", tileColor, 
                vertices.Count, CenterPoint(vertices), normal);
        }

        public void FindNeighbors(List<VoronoiTile> otherTiles)
        {
            List<VoronoiTile> neighborTiles = new List<VoronoiTile>();
            neighbors = new List<VoronoiTile>();
            List<Vector3> tileCenters = new List<Vector3>();
            foreach (var voronoiTile in otherTiles)
            {
                tileCenters.Add(voronoiTile.centerPoint);
            }

            List<Vector3> uniqueVerts = vertices.Distinct().ToList();

            for (int i = 0; i < uniqueVerts.Count+1; ++i)
            {
                Vector3 toAdd = FindClosest(centerPoint, tileCenters);
                tileCenters.Remove(toAdd);
                tileObject.AddComponent<VoronoiTile>();
                var tileComp = tileObject.GetComponent<VoronoiTile>();
                foreach (var tile in otherTiles)
                {
                    if (tile.centerPoint == toAdd) tileComp = tile;
                }
                neighbors.Add(tileComp);
            }
            Destroy(tileObject);
        }

        private Vector3 FindClosest(Vector3 point, List<Vector3> others)
        {
            Vector3 closest = new Vector3();
            bool isFirst = true;
            float smallestDistance = 0;
            foreach (var p in others)
            {
                float distance = Vector3.Distance(p, point);
                if (isFirst || distance < smallestDistance)
                {
                    smallestDistance = distance;
                    closest = p;
                    isFirst = false;
                }
            }
            return closest;
        }

        public void Push(float value)
        {
            Vector3[] verts = tileMesh.vertices;
            for (int q = 0; q < verts.Length; ++q)
            {
                Vector3 height = verts[q] - Vector3.zero;
            
                verts[q] += height * value;
            }
            tileMesh.vertices = verts;
            tileMesh.RecalculateNormals();
            altitude += value;
        }
        
        public void DetermineBiome()
        {
            if (humidity < 0) humidity = 0;
            if (humidity > 4) humidity = 4;
            if (temperature < 0) temperature = 0;
            if (temperature > 4) temperature = 4;

            if (humidity >= 0 && humidity <= 1)
            {
                if(temperature >= 0 && temperature <= 1) biome = Biomes.Tundra; // tundra
                else if (temperature > 1 && temperature <= 3) biome = Biomes.Dirt; // dirt
                else if (temperature > 3 && temperature <= 4) biome = Biomes.Desert; // desert
            }
            else if (humidity > 1 && humidity <= 2)
            {
                if (temperature >= 0 && temperature <= 1) biome = Biomes.Snow; // snow
                else if (temperature >= 1 && temperature <= 2) biome = Biomes.Forest; // forest
                else if (temperature > 2 && temperature <= 3) biome = Biomes.Plains; // planes
                else if (temperature > 3 && temperature <= 4) biome = Biomes.Desert; // desert
            }
            else if (humidity > 2 && humidity <= 3)
            {
                if (temperature >= 0 && temperature <= 1) biome = Biomes.Snow; // snow
                else if (temperature > 1 && temperature <= 3) biome = Biomes.Plains; // planes
                else if (temperature > 3 && temperature <= 4) biome = Biomes.Jungle; // jungle
            }
            else if (humidity > 3 && humidity <= 4)
            {
                if (temperature >= 0 && temperature <= 1) biome = Biomes.Glacier; // glacier
                else if (temperature > 1 && temperature <= 4) biome = Biomes.Jungle; // sand
            }
        }

        public void DetermineBaseBiome()
        {
            if (humidity < 0) humidity = 0;
            if (humidity > 4) humidity = 4;
            if (temperature < 0) temperature = 0;
            if (temperature > 4) temperature = 4;

            if (humidity >= 0 && humidity <= 1)
            {
                if (temperature >= 0 && temperature <= 1) baseBiome = Biomes.Tundra; // tundra
                else if (temperature > 1 && temperature <= 3) baseBiome = Biomes.Dirt; // dirt
                else if (temperature > 3 && temperature <= 4) baseBiome = Biomes.Desert; // desert
            }
            else if (humidity > 1 && humidity <= 2)
            {
                if (temperature >= 0 && temperature <= 1) baseBiome = Biomes.Snow; // snow
                else if (temperature >= 1 && temperature <= 2) baseBiome = Biomes.Forest; // forest
                else if (temperature > 2 && temperature <= 3) baseBiome = Biomes.Plains; // planes
                else if (temperature > 3 && temperature <= 4) baseBiome = Biomes.Desert; // desert
            }
            else if (humidity > 2 && humidity <= 3)
            {
                if (temperature >= 0 && temperature <= 1) baseBiome = Biomes.Snow; // snow
                else if (temperature > 1 && temperature <= 3) baseBiome = Biomes.Plains; // planes
                else if (temperature > 3 && temperature <= 4) baseBiome = Biomes.Jungle; // jungle
            }
            else if (humidity > 3 && humidity <= 4)
            {
                if (temperature >= 0 && temperature <= 1) baseBiome = Biomes.Glacier; // glacier
                else if (temperature > 1 && temperature <= 4) baseBiome = Biomes.Jungle; // sand
            }

        }

        public enum Biomes
        {
            Sand,
            Glacier,
            Plains,
            Snow,
            Swamp,
            Hill,
            Mountain,
            Jungle,
            Desert,
            Dirt,
            Tundra,
            Forest
        }

        //public void OnMouseOver()
        //{
        //    if(altitude <= 0) Generate.biomeText.text = "Water";
        //    else if (altitude >= 0.06f) Generate.biomeText.text = "Mountains";
        //    else if (altitude >= 0.04f) Generate.biomeText.text = "Hills";
        //    else Generate.biomeText.text = biome.ToString();
        //    if (Input.GetMouseButtonDown(0)) Push(0.02f);
        //    else if (Input.GetMouseButtonDown(1)) Push(-0.02f);
        //}

        //public void OnMouseExit()
        //{
        //    Generate.biomeText.text = "";
        //}
    }
}
