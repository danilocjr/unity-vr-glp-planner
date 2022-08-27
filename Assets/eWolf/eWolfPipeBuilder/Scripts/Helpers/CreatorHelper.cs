using UnityEngine;

namespace eWolf.PipeBuilder.Helpers
{
    public static class CreatorHelper
    {
        public static GameObject CreatePipe(Vector3 position, Transform parent)
        {
            
            GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("PipeNode_pf"),
                position,
                Quaternion.identity);
           
            go.transform.parent = parent;
            return go;
        }
    }
}