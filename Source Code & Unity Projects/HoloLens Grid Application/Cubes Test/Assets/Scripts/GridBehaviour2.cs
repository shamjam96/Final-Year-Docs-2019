using System;
using System.Collections;
using Assets.Networking;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Assets.Scripts
{
    public class GridBehaviour2 : MonoBehaviour, IInputClickHandler
    {
        //private List<Transform> _cubes = new List<Transform>();
        private GameObject _currentFirstCube;
        private GameObject _currentSecondCube;
        private GameObject _lastSetCube;
        private GameObject _sphere;
        private bool _isFirstCubeSet = false;

        private bool _isPlacementMode = false;

        // Use this for initialization
        void Start()
        {
            SpatialMapping.Instance.DrawVisualMeshes = false;
        }

        // Update is called once per frame
        void Update()
        {
            //If the user is in placing mode,
            //update the placement to match the user's gaze.

            if (_isPlacementMode)
            {
                // Do a raycast into the world that will only hit the Spatial Mapping mesh.
                var headPosition = Camera.main.transform.position;
                var gazeDirection = Camera.main.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                    30.0f, SpatialMapping.PhysicsRaycastMask))
                {
                    // Move this object's parent object to
                    // where the raycast hit the Spatial Mapping mesh.
                    this.transform.position = hitInfo.point;

                    // Rotate this object's parent object to face the user.
                    Quaternion toQuat = Camera.main.transform.localRotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    this.transform.rotation = toQuat;
                }
            }
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            var selectedObject = eventData.selectedObject;
            if (selectedObject == null)
            {
                return;
            }

            if (selectedObject.name.StartsWith("Cube"))
            {
                if (_isPlacementMode)
                {
                    OnPlacement();
                }
                else
                {
                    GameObject cube = selectedObject;

                    if (cube == _lastSetCube)
                    {
                        return;
                    }

                    if (!_isFirstCubeSet)
                    {
                        if (_currentFirstCube != null)
                        {
                            //SetGameObjectColor(_currentFirstCube, Color.white);
                            ResetColor(_currentFirstCube);
                        }

                        _currentFirstCube = cube;
                        SetGameObjectColor(cube, Color.red);
                        _isFirstCubeSet = true;
                    }
                    else
                    {
                        if (_currentSecondCube != null)
                        {
                            //SetGameObjectColor(_currentSecondCube, Color.white);
                            ResetColor(_currentSecondCube);
                        }

                        _currentSecondCube = cube;
                        SetGameObjectColor(cube, Color.yellow);
                        _isFirstCubeSet = false;
                        MoveSphere();
                        int start = Int32.Parse(_currentFirstCube.name.Remove(0, 4));
                        int end = Int32.Parse(_currentSecondCube.name.Remove(0, 4));
                        SendCubeMessage(start, end);
                    }

                    _lastSetCube = cube;
                }
            }
            else
            {
                OnPlacement();
            }
        }

        private void OnPlacement()
        {
            _isPlacementMode = !_isPlacementMode;
            if (_isPlacementMode)
            {
                SpatialMapping.Instance.DrawVisualMeshes = true;
                SpatialMapping.Instance.MappingEnabled = true;
            }
            // If the user is not in placing mode, hide the spatial mapping mesh.
            else
            {
                SpatialMapping.Instance.DrawVisualMeshes = false;
                SpatialMapping.Instance.MappingEnabled = false;
                ResetCubes();
            }

        }

        private void ResetCubes()
        {
            if (_sphere != null)
            {
                Destroy(_sphere);
            }
            ResetColor(_currentFirstCube);
            ResetColor(_currentSecondCube);
            _currentFirstCube = null;
            _currentSecondCube = null;
            _lastSetCube = null;
            _isFirstCubeSet = false;
            SendResetCubeMessage();
        }

        private void MoveSphere()
        {
            if (_sphere != null)
            {
                Destroy(_sphere);
            }
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            SetGameObjectColor(sphere, Color.green);
            sphere.transform.position = _currentFirstCube.transform.position + new Vector3(0, 0.05f, 0);
            Vector3 finalPosition = _currentSecondCube.transform.position + new Vector3(0, 0.05f, 0);
            StartCoroutine(MoveOverSeconds(sphere, finalPosition, 1));
            _sphere = sphere;
        }

        public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
        {
            float elapsedTime = 0;
            Vector3 startingPos = objectToMove.transform.position;
            while (elapsedTime < seconds)
            {
                objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            objectToMove.transform.position = end;
        }

        private void SetGameObjectColor(GameObject myObject, Color color)
        {
            if (myObject == null)
                return;
            var material = new Material(Shader.Find("Diffuse")) { color = color };
            myObject.GetComponent<Renderer>().material = material;
        }

        private void ResetColor(GameObject myObject)
        {
            if (myObject == null)
                return;
            Material material;
            if (myObject.CompareTag("CubeEven"))
            {
                material = (Material)Resources.Load("CubeEven", typeof(Material));
            }
            else if (myObject.CompareTag("CubeOdd"))
            {
                material = (Material)Resources.Load("CubeOdd", typeof(Material));
            }
            else
            {
                material = new Material(Shader.Find("Diffuse")) { color = Color.white };
            }
            myObject.GetComponent<Renderer>().material = material;
        }

        private void SendCubeMessage(int start, int end)
        {
            SelectedCubeMessage msg = new SelectedCubeMessage();
            msg.Cube1 = start;
            msg.Cube2 = end;
            long sendTime = Client.Instance.GetRTT();
            msg.SentTime = sendTime;
            Debug.Log($"Cube Message sent:{sendTime} - [{start},{end}]");
            Client.Instance.client.Send(1001, msg);
        }

        private void SendResetCubeMessage()
        {
            SendCubeMessage(-1, -1);
        }
    }
}
