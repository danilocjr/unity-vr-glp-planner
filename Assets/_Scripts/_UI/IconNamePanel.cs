using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IconNamePanel : MonoBehaviour
{
    public bool isOnMap;
    public bool isBeingPointed;
    public Animator namePanelAnim;

    Vector3 globalScale = new Vector3(-0.00015f, -0.00015f, -0.00015f);

    private void Start()
    {
        globalScale = new Vector3(-0.00015f, -0.00015f, -0.00015f);
    }
    void OnRaycastEnter(GameObject sender)
    {
        isBeingPointed = true;
    }

    // These methods will be called by RaycastExample
    void OnRaycastExit(GameObject sender)
    {
        isBeingPointed = false;
    }

    void Update()
    {
        if (namePanelAnim == null)
            return;

        namePanelAnim.transform.localScale = Vector3.one;

        if (namePanelAnim.transform.localScale != new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity))
            namePanelAnim.transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);

        SetName();
    }

    private void LateUpdate()
    {
        if (namePanelAnim == null)
            return;

        if (isBeingPointed && isOnMap)
        {
            namePanelAnim.SetBool("show", true);
        }
        else
        {
            namePanelAnim.SetBool("show", false);
        }
    }

    void SetName()
    {
        if (namePanelAnim.GetComponentInChildren<Text>() == null)
        {
            Debug.Log("Componente Text NULL");
            return;
        }   

        if (GetComponent<UsinaStorage>() != null)
        {
           namePanelAnim.GetComponentInChildren<Text>() .text = GetComponent<UsinaStorage>().usina.nome;
        }
        else if (GetComponent<FSUStorage>() != null)
        {
            namePanelAnim.GetComponentInChildren<Text>().text = GetComponent<FSUStorage>().fsu.nome;
        }
        else if (GetComponent<FSRUStorage>() != null)
        {
            namePanelAnim.GetComponentInChildren<Text>().text = GetComponent<FSRUStorage>().fsru.nome;
        }
        else if (GetComponent<SubestacaoStorage>() != null)
        {
            namePanelAnim.GetComponentInChildren<Text>().text = GetComponent<SubestacaoStorage>().subestacao.nome;
        }
        else if (GetComponent<PEStorage>() != null)
        {
            namePanelAnim.GetComponentInChildren<Text>().text = GetComponent<PEStorage>().pe.nome;
        }
        else if (GetComponent<GNLStorage>() != null)
        {
            namePanelAnim.GetComponentInChildren<Text>().text = GetComponent<GNLStorage>().gnl.nome;
        }
    }
}
