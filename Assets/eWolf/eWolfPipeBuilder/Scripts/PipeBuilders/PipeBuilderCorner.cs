using eWolf.PipeBuilder.Builders;
using eWolf.PipeBuilder.Data;
using eWolf.PipeBuilder.Sections;
using System.Collections.Generic;
using UnityEngine;

namespace eWolf.PipeBuilder.PipeBuilders
{
    public partial class PipeBuilderCorner : PipeBuilderBase
    {
        public PipeBuilderCorner(MeshBuilder meshBuilder, PipeBase pipeBase) : base(meshBuilder, pipeBase)
        {
        }

        public PipeBuilderCorner()
        {
        }

        public float StartOffSet { get; set; }

        public void CreatePipe(List<PipeNode> pipes, PipeNode pipeBase, SectionStraightBase straightBase)
        {
            PipeNode first = pipes[0];
            PipeNode second = pipes[1];

            PipeDefines firstPipe = new PipeDefines(first, pipeBase, StartOffSet);
            PipeDefines secondPipe = new PipeDefines(second, pipeBase, StartOffSet);

            Vector3 pos = pipeBase.transform.position;

            Vector3 mid = Vector3.Lerp(pos - firstPipe.CornerStartOffSet, pos - secondPipe.CornerStartOffSet, 0.50f);
            Vector3 midOffSet = pos - mid;
            midOffSet.Normalize();

            Vector3 CornerPoint = mid - midOffSet * 0.75f;
            Plane endOfPipePlane = new Plane(firstPipe.Direction, pos - firstPipe.CornerStartOffSet);

            Vector3 dirt = CornerPoint - pos;
            dirt.Normalize();

            Ray ray = new Ray(pos, dirt);
            float fl = 0;
            bool rayHit = endOfPipePlane.Raycast(ray, out fl);
            Vector3 mark = new Vector3();
            if (rayHit)
            {
                mark = pos + (dirt * fl);
            }
            else
            {
                AddStrightPipe(firstPipe.CornerStartOffSet, secondPipe.CornerStartOffSet, straightBase);
                return;
            }

            float length = (mark - (pos - firstPipe.CornerStartOffSet)).magnitude;

            Vector3 firstAngle = mark - (pos - firstPipe.CornerStartOffSet);
            firstAngle.Normalize();

            Vector3 secondAngle = mark - (pos - secondPipe.CornerStartOffSet);
            secondAngle.Normalize();

            Vector3 sideDir = (pos - firstPipe.CornerStartOffSet) - (pos - secondPipe.CornerStartOffSet);
            sideDir.Normalize();

            float angleSteps = 100 / (float)_pipeBase.PipeSettings.CornersDetail.Steps;
            float steps = _pipeBase.PipeSettings.CornersDetail.Steps;

            Vector3[,] rings = new Vector3[(int)(steps * 2) + 1, _pipeBase.PipeSettings.Sides + 1];

            UVSet uVSet = new UVSet(UVPoint, 1);
            float partUVStart = UVPoint;
            UVPoint += 1;

            int index = 0;

            float pipeSides = _pipeBase.PipeSettings.Sides;
            for (float i = 0; i < 100 + angleSteps; i += angleSteps)
            {
                Vector3 midAngle = Vector3.Lerp(firstAngle, secondAngle, i / 100);
                midAngle.Normalize();

                Vector3 midAngleTrist = Vector3.Lerp(firstAngle, secondAngle, i / 100);
                midAngleTrist.Normalize();

                Vector3 Up = Vector3.Cross(midAngleTrist, sideDir);
                Up.Normalize();

                Vector3 endPos = mark - (midAngle * length);

                float angle = midAngleTrist.y;

                float full = Mathf.PI * 2;
                float angleDiffPerSide = full / pipeSides;

                if (_pipeBase.PipeSettings.CornersDetail.Larger
                    && i == 0)
                {
                    CreateBasicRing(rings, index, pipeSides, midAngleTrist, Up, endPos, angle);
                    index++;
                }

                for (int j = 0; j < pipeSides + 1; j++)
                {
                    float x = Mathf.Sin(angle) * _pipeBase.PipeSettings.JoinRadius;
                    float y = -Mathf.Cos(angle) * _pipeBase.PipeSettings.JoinRadius;

                    Vector3 temp = endPos + ((midAngleTrist * x)) + ((Up * y));
                    angle -= angleDiffPerSide;

                    rings[index, j] = temp;
                }
                index++;

                if (_pipeBase.PipeSettings.CornersDetail.Larger
                  && i > 100 - angleSteps)
                {
                    CreateBasicRing(rings, index, pipeSides, midAngleTrist, Up, endPos, angle);
                    index++;
                }
            }

            CreateFromRings(pos, angleSteps, rings, uVSet, partUVStart, index, pipeSides);
        }

        internal void DrawDebug(List<PipeNode> pipes, PipeNode pipeBase)
        {
            return;
            PipeNode first = pipes[0];
            PipeNode second = pipes[1];

            PipeDefines firstPipe = new PipeDefines(first, pipeBase, StartOffSet);
            PipeDefines secondPipe = new PipeDefines(second, pipeBase, StartOffSet);

            Gizmos.color = Color.yellow;

            Vector3 pos = pipeBase.transform.position;

            Gizmos.DrawLine((pos - firstPipe.CornerStartOffSet) - firstPipe.Direction * firstPipe.Length, pos - firstPipe.CornerStartOffSet);
            Gizmos.DrawLine((pos - secondPipe.CornerStartOffSet) - secondPipe.Direction * secondPipe.Length, pos - secondPipe.CornerStartOffSet);

            Vector3 mid = Vector3.Lerp(pos - firstPipe.CornerStartOffSet, pos - secondPipe.CornerStartOffSet, 0.50f);
            Vector3 midOffSet = pos - mid;
            midOffSet.Normalize();

            Vector3 CornerPoint = mid - midOffSet * 0.75f;

            Plane pa = new Plane(firstPipe.Direction, pos - firstPipe.CornerStartOffSet);
            Gizmos.color = Color.red;

            Vector3 dirt = CornerPoint - pos;
            dirt.Normalize();

            Ray ray = new Ray(pos, dirt);
            float fl = 0;
            bool r = pa.Raycast(ray, out fl);
            Vector3 mark = new Vector3(); ;
            if (r)
            {
                mark = pos + (dirt * fl);
            }

            GameObject.Find("MarkerA").transform.position = mark;

            float length = (mark - (pos - firstPipe.CornerStartOffSet)).magnitude;

            Vector3 firstAngle = mark - (pos - firstPipe.CornerStartOffSet);
            firstAngle.Normalize();

            Vector3 secondAngle = mark - (pos - secondPipe.CornerStartOffSet);
            secondAngle.Normalize();

            Gizmos.color = Color.white;
            Gizmos.DrawLine(mark, mark - (firstAngle * length));
            Gizmos.DrawLine(mark, mark - (secondAngle * length));

            Vector3 sideDir = (pos - firstPipe.CornerStartOffSet) - (pos - secondPipe.CornerStartOffSet);
            sideDir.Normalize();

            for (float i = 0; i < 100; i += 10)
            {
                Vector3 midAngle = Vector3.Lerp(firstAngle, secondAngle, i / 100);
                midAngle.Normalize();

                Vector3 Up = Vector3.Cross(midAngle, sideDir);
                Up.Normalize();

                Gizmos.color = Color.red;
                Vector3 endPos = mark - (midAngle * length);
                Gizmos.DrawLine(mark, endPos);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(mark, endPos);

                float angle = 0;
                int pipeSides = 10;

                float full = Mathf.PI * 2;
                float angleDiffPerSide = full / pipeSides;

                Gizmos.color = Color.yellow;
                for (int j = 0; j < pipeSides + 1; j++)
                {
                    float x = Mathf.Sin(angle) * _pipeBase.PipeSettings.Radius;
                    float y = -Mathf.Cos(angle) * _pipeBase.PipeSettings.Radius;

                    Vector3 temp = endPos + ((midAngle * x)) + ((Up * y));
                    angle += angleDiffPerSide;

                    Gizmos.DrawLine(endPos, temp);
                }
            }
        }

        private void AddStrightPipe(Vector3 startPos, Vector3 endPos, SectionStraightBase straightBase)
        {
            PipeBuilderStraight pbs = new PipeBuilderStraight(this)
            {
                StartPosition = startPos,
                EndPosition = endPos,
                StartOffSet = 0,
                EndOffSet = 0
            };

            SectionStraight ss = new SectionStraight
            {
                ConbindedName = straightBase.ConbindedName,
                GameObject = straightBase.GameObject,
                PipeBuilderBase = pbs,
                PipeBase = straightBase.PipeBase
            };
            pbs.UVPoint = UVPoint;

            pbs.CreatePipe(startPos, ss);
            pbs.ApplyMeshDetails(ss.GameObject, ss.PipeBase.Material, false, new LightingOptions(), null);

            UVPoint = pbs.UVPoint;
        }

        private void CreateBasicRing(Vector3[,] rings, int index, float pipeSides, Vector3 midAngleTrist, Vector3 Up, Vector3 endPos, float angle)
        {
            for (int j = 0; j < pipeSides + 1; j++)
            {
                float x = Mathf.Sin(angle) * _pipeBase.PipeSettings.Radius;
                float y = -Mathf.Cos(angle) * _pipeBase.PipeSettings.Radius;

                Vector3 temp = endPos + ((midAngleTrist * x)) + ((Up * y));
                rings[index, j] = temp;
            }
        }

        private void CreateFromRings(Vector3 pos, float angleSteps, Vector3[,] rings, UVSet uVSet, float partUVStart, int index, float pipeSides)
        {
            float partUVGap = (3 / angleSteps) * _pipeBase.PipeSettings.UVModifier;
            float uvSlice = 1 / pipeSides;

            for (int i = 0; i < index - 1; i++)
            {
                int outterNext = i + 1;
                if (outterNext > index - 1)
                    outterNext = 0;

                uVSet.SliceVertical(partUVStart, -partUVGap);
                partUVStart -= partUVGap;

                float uvStart = 0;

                if (_pipeBase.PipeSettings.CornersDetail.Larger
                    && (i == 0 || i == index - 3))
                {
                    partUVStart -= 0.01f;
                }

                for (int j = 1; j < pipeSides + 1; j++)
                {
                    int next = j - 1;
                    if (next > pipeSides)
                        next = 1;

                    uVSet.Slice(uvStart, uvSlice);
                    uvStart += uvSlice;

                    if (!_pipeBase.PipeSettings.InsidePipe)
                    {
                        _meshBuilder.BuildQuad(
                            rings[i, j] - pos,
                            rings[i, next] - pos,
                            rings[outterNext, j] - pos,
                            rings[outterNext, next] - pos,
                            uVSet);
                    }
                    else
                    {
                        _meshBuilder.BuildQuad(
                            rings[outterNext, j] - pos,
                            rings[outterNext, next] - pos,
                            rings[i, j] - pos,
                            rings[i, next] - pos,
                            uVSet);
                    }
                }
            }
        }
    }
}