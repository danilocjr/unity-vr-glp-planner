using eWolf.PipeBuilder.Data;
using eWolf.PipeBuilder.PipeBuilders;
using UnityEngine;

namespace eWolf.PipeBuilder.Sections
{
    public class SectionStraight : SectionStraightBase
    {
        public override float Build(float uvPosition)
        {
            PipeBuilderStraight pbs = (PipeBuilderStraight)PipeBuilderBase;
            pbs.UVPoint = uvPosition;

            pbs.CreatePipe(new Vector3(), this);
            pbs.ApplyMeshDetails(GameObject, PipeBase.Material, false, new LightingOptions(), null);

            if (PipeBase.PipeSettings.CollisionDetails.Style == CollisionStyles.HighRes)
            {
                pbs.ApplyCollision(GameObject);
            }

            return pbs.UVPoint;
        }
    }
}