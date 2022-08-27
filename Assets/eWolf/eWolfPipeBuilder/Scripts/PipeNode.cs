using eWolf.PipeBuilder.Builders;
using eWolf.PipeBuilder.Helpers;
using eWolf.PipeBuilder.PipeBuilders;
using eWolf.PipeBuilder.Sections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace eWolf.PipeBuilder
{
    public class PipeNode : MonoBehaviour
    {
        [SerializeField]
        public List<PipeNode> _pipes = new List<PipeNode>();

        private readonly MeshBuilder _meshBuilder = new MeshBuilder();

        private Vector3 _cachedPosition;

        private Vector3 _left;

        private Vector3 _up;

        public MeshBuilder MeshBuilder
        {
            get
            {
                return _meshBuilder;
            }
        }

        public void AddPipe(PipeNode pn)
        {
            _pipes.Add(pn);
            pn._pipes.Add(this);
        }

        public void Draw()
        {
            foreach (PipeNode pn in _pipes)
            {
                if (pn.name == "Pipe_0001DB")
                {
                    Gizmos.color = Color.red;
                    Vector3 a = transform.position;
                    Vector3 b = pn.transform.position;
                    Vector3 c = a - b;
                    c.Normalize();
                    Gizmos.DrawLine(b, b + c);
                    UpdateVectors(transform.position, pn.transform.position);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(b, b + _left / 2);
                    Gizmos.DrawLine(b, b - _left / 2);
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(b, b - _up / 2);
                }
                else
                {
                    Gizmos.DrawLine(transform.position, pn.transform.position);
                }
            }

            if (name == "Pipe_0002DB")
            {
                PipeBase pipeBase = NodeHelper.GetPipeBase(transform);
                PipeBuilderCorner pbc = new PipeBuilderCorner(null, pipeBase);
                pbc.StartOffSet = pipeBase.PipeSettings.CornersDetail.Size;
                pbc.DrawDebug(_pipes, this);
            }
        }

        public GameObject ExtendPipe()
        {
            Vector3 newPipe = transform.position;

            if (_pipes.Count == 1)
            {
                Vector3 direction = transform.position - _pipes[0].transform.position;
                direction.Normalize();
                newPipe += direction * 2;
            }
            else
            {
                newPipe.x += 2;
            }

            return AddPipe(newPipe, transform.parent);
        }

        public bool CanExtendPipes()
        {
            return (_pipes.Count != 2);
        }

        public Vector3 GetDirection(Vector3 mainPipe)
        {
            return (mainPipe - transform.position).normalized;
        }

        public float GetLength(Vector3 mainPipe)
        {
            return (mainPipe - transform.position).magnitude;
        }

        public Vector3 GetOffSetFrom(Vector3 position, float offSet)
        {
            return GetDirection(position) * offSet;
        }

        public Vector3 GetRightAngle(Vector3 position)
        {
            return Quaternion.AngleAxis(-90, Vector3.up) * GetDirection(position);
        }

        public void OnDrawGizmosSelected()
        {
            NodeHelper.DrawAll(gameObject);

            if (_cachedPosition != transform.position)
            {
                _cachedPosition = transform.position;
                PipeBase pb = NodeHelper.GetPipeBase(transform);
                if (pb.PipeSettings.AutoBuild)
                {
                    pb.BuildPipes();
                }
            }
        }

        public void RemoveNodeFromList(PipeNode remove)
        {
            _pipes.Remove(remove);
        }

        internal void CreateSections(List<SectionStraightBase> sectionBuilders, PipeBase pipeBase)
        {
            _meshBuilder.Clear();
            if (!_pipes.Any())
                return;

            if (_pipes.Count == 2)
            {
                PipeBuilderCorner pipeBuilderCorner = new PipeBuilderCorner(_meshBuilder, pipeBase)
                {
                    StartOffSet = pipeBase.PipeSettings.CornersDetail.Size
                };

                SectionCorner ss = new SectionCorner
                {
                    PipeNode = this,
                    ConbindedName = gameObject.name,
                    GameObject = gameObject,
                    PipeBuilderBase = pipeBuilderCorner,
                    PipeBase = pipeBase
                };
                sectionBuilders.Add(ss);
            }

            foreach (PipeNode pipe in _pipes)
            {
                float endOffSet = 0;
                float startOffSet = 0;

                if (!PipeEnd())
                {
                    startOffSet = pipeBase.PipeSettings.CornersDetail.Size;
                }
                if (!PipeOtherEnd(pipe))
                {
                    endOffSet = pipeBase.PipeSettings.CornersDetail.Size;
                }

                string pipeName = NodeHelper.ConbindedNames(pipe.gameObject.name, gameObject.name);
                if (!sectionBuilders.Any(x => x.ConbindedName == pipeName))
                {
                    pipe.MeshBuilder.Clear();
                    PipeBuilderStraight pipeBuilderHelper = new PipeBuilderStraight(pipe.MeshBuilder, pipeBase)
                    {
                        StartPosition = pipe.transform.position - transform.position,
                        EndPosition = new Vector3(0, 0, 0),
                        StartOffSet = endOffSet,
                        EndOffSet = startOffSet
                    };

                    SectionStraight ss = new SectionStraight
                    {
                        ConbindedName = pipeName,
                        GameObject = pipe.gameObject,
                        PipeBuilderBase = pipeBuilderHelper,
                        PipeBase = pipeBase
                    };
                    sectionBuilders.Add(ss);
                }
            }
        }

        public static PipeNode CreatePipe(Vector3 pos, Transform parent)
        {
            string newPipeName = NodeHelper.FindUniqueName(parent);

            PipeNode pn = CreatorHelper.CreatePipe(pos, parent).GetComponent<PipeNode>();
            pn.name = newPipeName;

            return pn;
        }

        private GameObject AddPipe(Vector3 pos, Transform parent)
        {
            PipeNode pn = CreatePipe(pos, parent);

            AddPipe(pn);

            return pn.gameObject;
        }

        private void OnValidate()
        {
        }

        private bool PipeEnd()
        {
            return _pipes.Count == 1;
        }

        private bool PipeOtherEnd(PipeNode farPipe)
        {
            return farPipe._pipes.Count == 1;
        }

        private bool RemoveChild(PipeNode pipeNode)
        {
            return _pipes.Remove(pipeNode);
        }

        private void UpdateVectors(Vector3 position1, Vector3 position2)
        {
            Vector3 direction = position1 - position2;
            direction.Normalize();

            _left = Vector3.Cross(direction, Vector3.up);
            _up = Vector3.Cross(direction, _left.normalized);
            _left = Vector3.Cross(direction, _up);
            _up = Vector3.Cross(direction, _left.normalized);
        }
    }
}