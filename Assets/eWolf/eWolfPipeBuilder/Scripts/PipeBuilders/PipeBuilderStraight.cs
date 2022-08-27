using eWolf.PipeBuilder.Builders;
using eWolf.PipeBuilder.Data;
using eWolf.PipeBuilder.Sections;
using UnityEngine;

namespace eWolf.PipeBuilder.PipeBuilders
{
    public class PipeBuilderStraight : PipeBuilderBase
    {
        private readonly float _angleStep;
        private readonly float _pipeSides;
        private Vector3 _direction;
        private Vector3 _left;
        private Vector3 _up;

        public PipeBuilderStraight(PipeBuilderCorner pipeBuilderCorner) : base(pipeBuilderCorner)
        {
            float fullRadius = Mathf.PI * 2;

            _pipeSides = _pipeBase.PipeSettings.Sides;
            _angleStep = fullRadius / _pipeSides;
        }

        public PipeBuilderStraight(MeshBuilder meshBuilder, PipeBase pipeBase) : base(meshBuilder, pipeBase)
        {
            float fullRadius = Mathf.PI * 2;

            _pipeSides = _pipeBase.PipeSettings.Sides;
            _angleStep = fullRadius / _pipeSides;
        }

        public float EndOffSet { get; set; }
        public Vector3 EndPosition { get; set; }
        public float StartOffSet { get; set; }
        public Vector3 StartPosition { get; set; }

        public void CreatePipe(Vector3 position, SectionStraight sectionStraight)
        {
            if (EndPosition.z == StartPosition.z
                && EndPosition.x == StartPosition.x)
            {
                StartPosition = new Vector3(StartPosition.x, StartPosition.y, StartPosition.z - 0.05f);
            }

            _direction = (EndPosition - StartPosition);

            float length = _direction.magnitude;
            _direction.Normalize();

            _left = Vector3.Cross(_direction, Vector3.up);
            _up = Vector3.Cross(_direction, _left.normalized);
            _left = Vector3.Cross(_direction, _up);
            _up = Vector3.Cross(_direction, _left.normalized);

            Vector3 startOffSet = _direction * StartOffSet;
            Vector3 sectionLength = _direction * (length - EndOffSet);
            Vector3 pipeSectionStart = startOffSet + position;

            bool withFlange = _pipeBase.PipeSettings.FlangeDetail.Flange;
            if (_pipeBase.PipeSettings.FlangeDetail.Interval > length)
            {
                withFlange = false;
            }

            if (withFlange)
            {
                Vector3 flangeLength = _direction * (_pipeBase.PipeSettings.FlangeDetail.Length);

                float pipeFlangeLength = _pipeBase.PipeSettings.FlangeDetail.Length;
                float pipeSectionLength = _pipeBase.PipeSettings.FlangeDetail.Interval - _pipeBase.PipeSettings.FlangeDetail.Length;

                pipeSectionStart = CreateFlange(pipeSectionStart, pipeFlangeLength, _pipeBase.PipeSettings);

                startOffSet += flangeLength;

                float endSectionLength = 0;

                int steps = 1;
                if (length > _pipeBase.PipeSettings.FlangeDetail.Interval)
                {
                    steps = (int)((length - EndOffSet - StartOffSet) / _pipeBase.PipeSettings.FlangeDetail.Interval);

                    sectionLength = _direction * _pipeBase.PipeSettings.FlangeDetail.Interval;

                    endSectionLength = (length - EndOffSet - StartOffSet) - (steps * _pipeBase.PipeSettings.FlangeDetail.Interval);
                }

                float temp = _pipeBase.PipeSettings.Radius * 2;
                while (steps-- > 0)
                {
                    pipeSectionStart = CreateStrightPipe(pipeSectionStart, pipeSectionLength, _pipeBase.PipeSettings.Radius);

                    pipeSectionStart = CreateFlange(pipeSectionStart, pipeFlangeLength, _pipeBase.PipeSettings);
                }

                if (endSectionLength > 0)
                {
                    endSectionLength -= _pipeBase.PipeSettings.FlangeDetail.Length;

                    if (endSectionLength > _pipeBase.PipeSettings.FlangeDetail.Length)
                    {
                        endSectionLength -= _pipeBase.PipeSettings.FlangeDetail.Length;
                        pipeSectionStart = CreateStrightPipe(pipeSectionStart, endSectionLength, _pipeBase.PipeSettings.Radius);
                        pipeSectionStart = CreateFlange(pipeSectionStart, pipeFlangeLength, _pipeBase.PipeSettings);
                    }
                    else
                    {
                        pipeSectionStart = CreateStrightPipe(pipeSectionStart, endSectionLength, _pipeBase.PipeSettings.Radius);
                    }
                }
            }
            else
            {
                length -= (StartOffSet + EndOffSet);
                pipeSectionStart = CreateStrightPipe(pipeSectionStart, length, _pipeBase.PipeSettings.Radius);
            }
        }

        private Vector3 CreateFlange(Vector3 pipeSectionStart, float pipeSectionLength, PipeSettings pipeSettings)
        {
            CreateFlange(pipeSectionStart, pipeSettings.Radius, pipeSettings.FlangeRadius);

            pipeSectionStart = CreateStrightPipe(pipeSectionStart, pipeSectionLength, _pipeBase.PipeSettings.FlangeRadius);

            CreateFlange(pipeSectionStart, pipeSettings.FlangeRadius, pipeSettings.Radius);

            return pipeSectionStart;
        }

        private void CreateFlange(Vector3 startOffSet, float radius, float flangeRadius)
        {
            UVPoint += 0.01f;

            float uvSlice = 1 / _pipeSides;
            float uvStart = 0;

            float x2 = 0;
            float y2 = 0;
            float x2Outter = 0;
            float y2Outter = 0;
            float angle = 0;

            float flangeUVSize = Mathf.Abs(flangeRadius - radius);
            UVSet uVSet = new UVSet(UVPoint, flangeUVSize);

            UVPoint += flangeUVSize;

            for (int i = 0; i < _pipeSides + 1; i++)
            {
                uVSet.Slice(uvStart, uvSlice);

                float x = Mathf.Sin(angle) * radius;
                float y = -Mathf.Cos(angle) * radius;
                float xOutter = Mathf.Sin(angle) * flangeRadius;
                float yOutter = -Mathf.Cos(angle) * flangeRadius;

                Vector3 a = (_left * x + y * _up) + startOffSet;
                Vector3 b = (_left * x2 + y2 * _up) + startOffSet;
                Vector3 c = (_left * xOutter + yOutter * _up) + startOffSet;
                Vector3 d = (_left * x2Outter + y2Outter * _up) + startOffSet;
                angle += _angleStep;

                x2 = x;
                y2 = y;
                x2Outter = xOutter;
                y2Outter = yOutter;

                if (i != 0)
                {
                    if (!_pipeBase.PipeSettings.InsidePipe)
                    {
                        _meshBuilder.BuildQuad(c, d, a, b, uVSet);
                    }
                    else
                    {
                        _meshBuilder.BuildQuad(a, b, c, d, uVSet);
                    }
                    uvStart += uvSlice;
                }
            }

            UVPoint += 0.01f;
        }

        private Vector3 CreateStrightPipe(Vector3 pipeSectionStart, float length, float radius)
        {
            float uvSlice = 1 / _pipeSides;
            float uvStart = 0;

            float x2 = 0;
            float y2 = 0;
            float angle = 0;

            UVSet uVSet = new UVSet(UVPoint, length * _pipeBase.PipeSettings.UVModifier);
            UVPoint += length * _pipeBase.PipeSettings.UVModifier;

            for (int i = 0; i < _pipeSides + 1; i++)
            {
                uVSet.Slice(uvStart, uvSlice);

                float x = Mathf.Sin(angle) * radius;
                float y = -Mathf.Cos(angle) * radius;

                Vector3 a = (_left * x + y * _up) + pipeSectionStart;
                Vector3 b = (_left * x2 + y2 * _up) + pipeSectionStart;
                Vector3 c = (_left * x + y * _up) + pipeSectionStart + (_direction * length);
                Vector3 d = (_left * x2 + y2 * _up) + pipeSectionStart + (_direction * length);
                angle += _angleStep;

                x2 = x;
                y2 = y;

                if (i != 0)
                {
                    if (!_pipeBase.PipeSettings.InsidePipe)
                    {
                        _meshBuilder.BuildQuad(c, d, a, b, uVSet);
                    }
                    else
                    {
                        _meshBuilder.BuildQuad(a, b, c, d, uVSet);
                    }
                    uvStart += uvSlice;
                }
            }

            pipeSectionStart += _direction * length;
            return pipeSectionStart;
        }
    }
}
