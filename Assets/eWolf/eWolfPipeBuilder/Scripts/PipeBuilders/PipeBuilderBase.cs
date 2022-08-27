using eWolf.PipeBuilder.Builders;
using eWolf.PipeBuilder.Data;
using System.Collections.Generic;
using UnityEngine;

namespace eWolf.PipeBuilder.PipeBuilders
{
    public class PipeBuilderBase
    {
        protected readonly MeshBuilder _meshBuilder;

        protected readonly PipeBase _pipeBase;

        private float uVPoint = 0;

        public PipeBuilderBase()
        {
        }

        public PipeBuilderBase(PipeBuilderBase pipeBase)
        {
            _meshBuilder = pipeBase._meshBuilder;
            _pipeBase = pipeBase._pipeBase;
        }

        public PipeBuilderBase(MeshBuilder meshBuilder, PipeBase pipeBase)
        {
            _meshBuilder = meshBuilder;
            _pipeBase = pipeBase;
        }

        public float UVPoint
        {
            get
            {
                return uVPoint;
            }

            set
            {
                uVPoint = value;
            }
        }

        public void ApplyCollision(GameObject gameObject)
        {
            _meshBuilder.ApplyCollision(gameObject);
        }

        public void ApplyMeshDetails(GameObject gameObject, Material material, bool applyOffSet, LightingOptions lightingOptions, List<Material> allMaterials)
        {
            _meshBuilder.ApplyMeshDetails(gameObject, material, applyOffSet, lightingOptions, allMaterials);
        }
    }
}