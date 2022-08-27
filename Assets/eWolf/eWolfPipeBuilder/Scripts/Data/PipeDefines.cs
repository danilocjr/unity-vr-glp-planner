using UnityEngine;

namespace eWolf.PipeBuilder.Data
{
    public class PipeDefines
    {
        private readonly PipeNode _pipeNode;
        private readonly PipeNode _pipeBase;
        public float Length;
        public Vector3 Direction;
        public Vector3 Left;
        public Vector3 Up;
        public Vector3 CornerStartOffSet;

        public PipeDefines(PipeNode pipeNode, PipeNode pipeBase, float startOffSet)
        {
            _pipeNode = pipeNode;
            _pipeBase = pipeBase;

            ProcessPipeDirections();

            CornerStartOffSet = pipeNode.GetOffSetFrom(pipeBase.transform.position, startOffSet);
        }

        public Vector3 Position()
        {
            return _pipeNode.transform.position;
        }

        private void ProcessPipeDirections()
        {
            Direction = _pipeNode.GetDirection(_pipeBase.transform.position);

            Length = _pipeNode.GetLength(_pipeBase.transform.position);

            Left = Vector3.Cross(Direction, Vector3.up);
            Up = Vector3.Cross(Direction, Left.normalized);
            Left = Vector3.Cross(Direction, Up);
            Up = Vector3.Cross(Direction, Left.normalized);
        }
    }
}