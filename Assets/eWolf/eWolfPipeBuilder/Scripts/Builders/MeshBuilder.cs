using eWolf.PipeBuilder.Data;
using System.Collections.Generic;
using System.Linq;

#if DEBUG

using UnityEditor;

#endif

using UnityEngine;

namespace eWolf.PipeBuilder.Builders
{
    public class MeshBuilder
    {
        private List<Vector3> _meshVertices = new List<Vector3>();
        private List<Vector2> _meshUVs = new List<Vector2>();
        private Dictionary<string, List<int>> _meshMaterialsTriangles = new Dictionary<string, List<int>>();

        public MeshBuilder()
        {
            CreateMaterialsArray();
        }

        public List<Vector3> MeshVertices
        {
            get
            {
                return _meshVertices;
            }
        }

        internal void Clear()
        {
            _meshVertices = new List<Vector3>();
            _meshUVs = new List<Vector2>();
            _meshMaterialsTriangles = new Dictionary<string, List<int>>();
            CreateMaterialsArray();
        }

        public List<Vector2> MeshUVs
        {
            get
            {
                return _meshUVs;
            }
        }

        public object Clone()
        {
            MeshBuilder mb = new MeshBuilder
            {
                _meshUVs = new List<Vector2>(_meshUVs),
                _meshMaterialsTriangles = _meshMaterialsTriangles,
                _meshVertices = new List<Vector3>(_meshVertices)
            };

            return mb;
        }

        public List<int> GetTriangles()
        {
            return GetTriangles("Base");
        }

        public List<int> GetTriangles(string materialName)
        {
            if (string.IsNullOrEmpty(materialName))
                materialName = "Base";

            if (!_meshMaterialsTriangles.ContainsKey(materialName))
                _meshMaterialsTriangles.Add(materialName, new List<int>());

            return _meshMaterialsTriangles[materialName];
        }

        public void CreateMaterialsArray()
        {
            _meshMaterialsTriangles.Add("Base", new List<int>());
        }

        internal void ApplyMaterial(string materialName, Material material)
        {
            if (_meshMaterialsTriangles.ContainsKey(materialName))
                return;

            _meshMaterialsTriangles.Add(materialName, new List<int>());
        }

        public void ApplyMeshDetails(GameObject baseobject, Material material, bool applyOffSet, LightingOptions lights, List<Material> AllMaterials)
        {
#if DEBUG
            Mesh mesh = new Mesh
            {
                name = "Building" + baseobject.name
            };
            baseobject.GetComponent<MeshFilter>().mesh = mesh;

            if (applyOffSet)
                ApplyObjectOffSet(baseobject.transform.parent.transform.position);

            mesh.vertices = MeshVertices.ToArray();
            mesh.uv = MeshUVs.ToArray();

            // Create the material and assign triangles
            Renderer r = baseobject.GetComponent<Renderer>();
            List<Material> materials = new List<Material>();
            int count = 0;
            mesh.subMeshCount = _meshMaterialsTriangles.Count;

            materials.Add(material);

            foreach (KeyValuePair<string, List<int>> meshTris in _meshMaterialsTriangles)
            {
                if (meshTris.Value.Count == 0)
                {
                    continue;
                }

                if (meshTris.Key == material.name)
                {
                    List<int> tris = mesh.GetTriangles(0).ToList();
                    tris.AddRange(meshTris.Value);
                    mesh.SetTriangles(tris.ToArray(), 0);
                    continue;
                }

                if (meshTris.Key != "Base")
                {
                    if (!materials.Any(x => x.name == meshTris.Key))
                    {
                        Material mat = AllMaterials.Find((m) => m.name == meshTris.Key);
                        materials.Add(mat);
                    }
                }
                mesh.SetTriangles(meshTris.Value.ToArray(), count++);
            }

            mesh.subMeshCount = count; // just in case we didn't add all of them

            r.materials = materials.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            if (lights.BakedLighting)
            {
                UnwrapParam up = new UnwrapParam
                {
                    hardAngle = lights.HardAngle,
                    packMargin = lights.PackMargin,
                    angleError = lights.AngleError,
                    areaError = lights.AngleError
                };

                Unwrapping.GenerateSecondaryUVSet(mesh, up);
            }
#endif
        }

        public void ApplyCollision(GameObject baseobject)
        {
            MeshCollider meshCollider = baseobject.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = baseobject.AddComponent<MeshCollider>();
            }

            meshCollider.convex = false;
            meshCollider.sharedMesh = baseobject.GetComponent<MeshFilter>().sharedMesh;
        }

        public void BuildTri(Vector3 a, Vector3 b, Vector3 c, Vector2 uvc, Vector2 uva, Vector2 uvb, string materialName = null)
        {
            int indexA = AddVectorUVSets(a, uva);
            int indexB = AddVectorUVSets(b, uvb);
            int indexC = AddVectorUVSets(c, uvc);

            GetTriangles(materialName).AddRange(new int[] { indexA, indexB, indexC });
        }

        public void BuildQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, UVSet uvset, string materialName = null)
        {
            int indexA = AddVectorUVSets(a, uvset.BR);
            int indexB = AddVectorUVSets(b, uvset.TR);
            int indexC = AddVectorUVSets(c, uvset.BL);
            int indexD = AddVectorUVSets(d, uvset.TL);

            GetTriangles(materialName).AddRange(new int[] { indexA, indexB, indexC });
            GetTriangles(materialName).AddRange(new int[] { indexD, indexC, indexB });
        }

        public void BuildQuadFlipped(Vector3 a, Vector3 b, Vector3 c, Vector3 d, UVSet uvset)
        {
            int indexA = AddVectorUVSets(a, uvset.BR);
            int indexB = AddVectorUVSets(b, uvset.TR);
            int indexC = AddVectorUVSets(c, uvset.BL);
            int indexD = AddVectorUVSets(d, uvset.TL);

            GetTriangles().AddRange(new int[] { indexC, indexB, indexA });
            GetTriangles().AddRange(new int[] { indexB, indexC, indexD });
        }

        private int AddVectorUVSets(Vector3 points, Vector2 uvs)
        {
            for (int i = 0; i < MeshVertices.Count; i++)
            {
                Vector3 vec = MeshVertices[i];
                Vector2 v2 = MeshUVs[i];

                if (vec == points && v2 == uvs)
                    return i;
            }

            MeshVertices.Add(points);
            MeshUVs.Add(uvs);
            return MeshVertices.Count - 1;
        }

        private void ApplyObjectOffSet(Vector3 offSet)
        {
            for (int i = 0; i < MeshVertices.Count; i++)
            {
                MeshVertices[i] -= offSet;
            }
        }
    }
}