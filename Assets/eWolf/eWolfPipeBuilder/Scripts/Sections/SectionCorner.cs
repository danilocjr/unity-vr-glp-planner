using eWolf.PipeBuilder.Data;
using eWolf.PipeBuilder.PipeBuilders;
using UnityEngine;

namespace eWolf.PipeBuilder.Sections
{
    public class SectionCorner : SectionStraightBase
    {
        public override float Build(float uvPosition)
        {
            PipeBuilderCorner pbs = (PipeBuilderCorner)PipeBuilderBase;
            pbs.UVPoint = uvPosition;

            pbs.CreatePipe(PipeNode._pipes, PipeNode, this);

            pbs.ApplyMeshDetails(GameObject, PipeBase.Material, false, new LightingOptions(), null);

            if (PipeBase.PipeSettings.CollisionDetails.Style == CollisionStyles.HighRes)
            {
                pbs.ApplyCollision(GameObject);
            }

            return pbs.UVPoint;
        }
    }
}