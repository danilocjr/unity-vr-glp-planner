using System;

namespace eWolf.PipeBuilder.Data
{
    [Serializable]
    public class FlangeDetails
    {
        public bool Flange = false;
        public float Length = 0.15f;
        public float Interval = 1;
        public float Size = 0.15f;
    }
}