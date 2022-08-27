using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SCRIPT QUE FICA NO ÍCONE PARA MANIPULAR O LINE RENDERER E CONECTAR A OUTRO ÍCONE
public class LineCreator : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform lineDestination;
    public List<Transform> lineDestinations = new List<Transform>();
    public bool canDraw;
    void Update()
    {
        if (lineDestinations.Count > 0  && canDraw)
        {
            GetComponent<LineRenderer>().enabled = true;
            Draw();
        }
        else if (lineDestination!=null)
        {
            if (lineDestination.gameObject.activeSelf)
            {
                GetComponent<LineRenderer>().enabled = true;
                Draw();
            }
            else
            {
                GetComponent<LineRenderer>().enabled = false;
            }
        }       
        else
        {
            GetComponent<LineRenderer>().enabled = false;
        }

    }

    public void Draw()
    {

        if (lineDestinations.Count > 0)
        {
            GetComponent<LineRenderer>().positionCount = lineDestinations.Count;
            
            GetComponent<LineRenderer>().SetPosition(0, transform.position);

            int lineIndex = 0;
            for (int x = 1; x < lineDestinations.Count; x++)
            {
                if (lineDestinations[x - 1] != null)
                {
                    GetComponent<LineRenderer>().SetPosition(lineIndex, lineDestinations[x - 1].transform.position);
                    lineIndex++;

                }
            }
            if (GetComponent<LineRenderer>().GetPosition(GetComponent<LineRenderer>().positionCount - 1) == new Vector3(0, 0, 0))
            {
                GetComponent<LineRenderer>().SetPosition(GetComponent<LineRenderer>().positionCount - 1, GetComponent<LineRenderer>().GetPosition(GetComponent<LineRenderer>().positionCount - 2));
            }
        }
        else
        {
            GetComponent<LineRenderer>().SetPosition(0, transform.position);
            GetComponent<LineRenderer>().SetPosition(1, lineDestination.position);
        }
    }
}
