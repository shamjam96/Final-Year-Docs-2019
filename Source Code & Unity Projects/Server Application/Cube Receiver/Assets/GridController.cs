using System;
using UnityEngine;

namespace Assets
{
    public class GridController : Singleton<GridController>
    {
        private GameObject[] cubes = new GameObject[9];

        private Material _endMaterial;
        private Material _startMaterial;
        private Material _normMaterial;
        // Start is called before the first frame update
        void Start()
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "cube")
                {
                    int cubeNumber = Int32.Parse(child.name.Remove(0, 4));
                    cubes[cubeNumber] = child.gameObject;
                }
            }
            _endMaterial = Resources.Load<Material>("End");
            _startMaterial = Resources.Load<Material>("Start");
            _normMaterial = Resources.Load<Material>("Normal");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetGrid(int start, int end)
        {
            ResetCubes();
            cubes[start].GetComponent<Renderer>().material = _startMaterial;
            cubes[end].GetComponent<Renderer>().material = _endMaterial;
        }

        public void ResetCubes()
        {
            foreach (var cube in cubes)
            {
                cube.GetComponent<Renderer>().material = _normMaterial;
            }
        }
    }
}
