using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Assets.Scripts
{
    public class GestureManager : MonoBehaviour
    {
        public static GestureManager Instance { get; private set; }
        GestureRecognizer recogniser;
        public GameObject FocusedObject { get; private set; }
        // Use this for initialization
        void Start()
        {
            Instance = this;
            recogniser = new GestureRecognizer();
            recogniser.Tapped += (args) =>
            {
                if (FocusedObject != null)
                {
                    if (FocusedObject.transform.parent != null && FocusedObject.transform.parent.name!= "Spatial Mapping")
                    {
                        FocusedObject.SendMessageUpwards("OnSelect", FocusedObject, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        FocusedObject.SendMessage("OnSelect", FocusedObject, SendMessageOptions.DontRequireReceiver);
                    }
                }
            };
            recogniser.StartCapturingGestures();
        }

        // Update is called once per frame
        void Update()
        {
            // Figure out which hologram is focused this frame.
            GameObject oldFocusObject = FocusedObject;

            // Do a raycast into the world based on the user's
            // head position and orientation.
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
            {
                // If the raycast hit a hologram, use that as the focused object.
                if (!hitInfo.collider.gameObject.name.StartsWith("spatial"))
                {
                    FocusedObject = hitInfo.collider.gameObject;
                }

            }
            else
            {
                // If the raycast did not hit a hologram, clear the focused object.
                FocusedObject = null;
            }

            // If the focused object changed this frame,
            // start detecting fresh gestures again.
            if (FocusedObject != oldFocusObject)
            {
                recogniser.CancelGestures();
                recogniser.StartCapturingGestures();
            }
        }
    }
}
