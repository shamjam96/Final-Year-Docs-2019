using System;
using UnityEngine.Networking;

namespace Assets.Networking
{
    public class SelectedCubeMessage : MessageBase
    {
        public int Cube1;
        public int Cube2;
        public long SentTime;
    }
}
