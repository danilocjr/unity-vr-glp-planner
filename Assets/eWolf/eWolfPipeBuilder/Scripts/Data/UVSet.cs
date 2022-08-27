using System;
using UnityEngine;

namespace eWolf.PipeBuilder.Data
{
    public class UVSet : ICloneable
    {
        public Vector2 BL;

        public Vector2 BR;

        public Vector2 TL;

        public Vector2 TR;

        public UVSet(Vector2 topLeft, Vector2 topRight, Vector2 botLeft, Vector3 botRight)
        {
            TL = topLeft;
            TR = topRight;
            BL = botLeft;
            BR = botRight;
        }

        public UVSet()
        {
            TL = new Vector3(0, 0);
            TR = new Vector3(0, 1);

            BL = new Vector3(1, 0);
            BR = new Vector3(1, 1);
        }

        public UVSet(float start, float length)
        {
            TL = new Vector3(start, 0);
            TR = new Vector3(start + length, 0);
            BL = new Vector3(start, 1);
            BR = new Vector3(start + length, 1);
        }

        public object Clone()
        {
            return new UVSet(TL, TR, BL, BR);
        }

        public void Flipvertical()
        {
            Vector3 store = TL;
            TL.y = BL.y;
            BL.y = store.y;

            store = TR;
            TR.y = BR.y;
            BR.y = store.y;
        }

        public void Slice(float start, float length)
        {
            TL.y = start;
            TR.y = start;
            BL.y = start + length;
            BR.y = start + length;
        }

        public void SliceVertical(float start, float length)
        {
            TL.x = start + length;
            TR.x = start;
            BL.x = start + length;
            BR.x = start;
        }
    }
}
