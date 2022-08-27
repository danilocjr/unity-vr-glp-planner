using eWolf.PipeBuilder.PipeBuilders;
using UnityEngine;

namespace eWolf.PipeBuilder.Sections
{
    public abstract class SectionStraightBase
    {
        public GameObject GameObject { get; set; }

        public PipeBase PipeBase { get; set; }

        public PipeNode PipeNode { get; set; }

        public string ConbindedName { get; set; }

        public PipeBuilderBase PipeBuilderBase { get; set; }

        public abstract float Build(float uvPosition);
    }
}