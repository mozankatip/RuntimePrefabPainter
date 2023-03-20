using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class Painter : MonoBehaviour
    {
        public LayerMask paintTargetLayer;
        public List<GameObject> prefabList;// a list of GameObjects that will be used as the paint prefabs
        [Range(1, 5)]
        public float paintRadius = 1f;
        [Range(0.01f, 1f)]
        public float brushDensity = 0.1f;
        [Range(100, 1000)]
        public int maxInstanceInBatch = 1000; // each batch shouldn't exceed limit of 1023

        private int maxBatchCount = 100; //DrawMeshInstanced has total 100.000 instances limit
        private int startHeight = 1000;

        // A dictionary to store the batches of each prefab
        private Dictionary<GameObject, List<List<Matrix4x4>>> prefabBatches = new Dictionary<GameObject, List<List<Matrix4x4>>>(); 
        private int instances;
        private Camera mainCamera;
        private GameObject selectedPrefab; // add a new variable to store the selected prefab

        void Start()
        {
            mainCamera = Camera.main;

            // Initialize the prefabBatches dictionary with empty lists for each prefab
            foreach (GameObject prefab in prefabList)
            {
                prefabBatches[prefab] = new List<List<Matrix4x4>>() { new List<Matrix4x4>() };
            }

            // Choose a random prefab to start with
            int prefabIndex = Random.Range(0, prefabList.Count);
            selectedPrefab = prefabList[prefabIndex];
        }
        void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0) && prefabBatches[selectedPrefab].Count < maxBatchCount)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue, paintTargetLayer);

                foreach (RaycastHit hit in hits)
                {
                    Vector3 origin = hit.point;
                    origin.y = startHeight;

                    // Add some randomness to the paint position
                    origin.x += Random.Range(-paintRadius, paintRadius);
                    origin.z += Random.Range(-paintRadius, paintRadius);

                    Ray grassRay = new Ray(origin, Vector3.down);
                    RaycastHit grassHit;

                    if (Physics.Raycast(grassRay, out grassHit))
                    {
                        // If the current batch is full, create a new one
                        if (prefabBatches[selectedPrefab][prefabBatches[selectedPrefab].Count - 1] == null || instances >= maxInstanceInBatch)
                        {
                            CreateNewBatch();
                        }

                        var target = grassHit.collider;

                        if (target)
                        {
                            // Get the bounds of the target
                            Bounds bounds = target.bounds;
                            CreateInstance(grassHit.point, Quaternion.identity, Vector3.one, brushDensity, grassHit.normal, bounds);
                        }
                    }
                }

        
            }
            RenderBatches();
        }
        private void CreateInstance(Vector3 pos, Quaternion rot, Vector3 scale, float density, Vector3 normal, Bounds bounds)
        {
            for (int i = 0; i < density; i++)
            {
                ++instances;
                var x = Random.Range(-paintRadius, paintRadius);
                var z = Random.Range(-paintRadius, paintRadius);

                // Limit the random position of the instance within the bounds of the object
                x = Mathf.Clamp(x, bounds.min.x - pos.x, bounds.max.x - pos.x);
                z = Mathf.Clamp(z, bounds.min.z - pos.z, bounds.max.z - pos.z);

                var instancePos = new Vector3(pos.x + x, pos.y, pos.z + z);
                // Choose a random prefab from the list
                int prefabIndex = Random.Range(0, prefabList.Count);
                selectedPrefab = prefabList[prefabIndex];

                Quaternion instanceRot = Quaternion.FromToRotation(Vector3.up, normal);// Calculate the rotation of the instance based on the surface normal

                prefabBatches[selectedPrefab][prefabBatches[selectedPrefab].Count - 1].Add(Matrix4x4.TRS(instancePos, instanceRot, scale));
            }
        }

        private void CreateNewBatch()
        {
            prefabBatches[selectedPrefab].Add(new List<Matrix4x4>());
            instances = 0;

            // Choose the next prefab in the list
            int prefabIndex = prefabList.IndexOf(selectedPrefab);
            if (prefabIndex == prefabList.Count - 1)
            {
                selectedPrefab = prefabList[0]; // start over from the beginning of the list
            }
            else
            {
                selectedPrefab = prefabList[prefabIndex + 1]; // choose the next prefab in the list
            }
        }

        private void RenderBatches()
        {
            foreach (GameObject prefab in prefabList)
            {
                var batchList = prefabBatches[prefab];
                for (int i = 0; i < batchList.Count; i++)
                {
                    var batch = batchList[i];

                    if (batch.Count > 0)
                    {
                        Graphics.DrawMeshInstanced(prefab.GetComponent<MeshFilter>().sharedMesh, 0, prefab.GetComponent<MeshRenderer>().sharedMaterial, batch.ToArray());
                    }
                }
            }
        }
    }
}