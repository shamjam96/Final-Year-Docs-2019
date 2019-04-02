using UnityEngine;

namespace Assets.Scripts
{
    public class TapToPlaceParent2 : MonoBehaviour
    {

        // Called by GazeGestureManager when the user performs a Select gesture
        void OnSelect()
        {
            //this.SendMessageUpwards("OnPlacement", SendMessageOptions.DontRequireReceiver);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}