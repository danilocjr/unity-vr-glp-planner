using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UsinaButtonBehaviour : MonoBehaviour
{
    public Text usinaLabel;
    public GameObject usinaObj;
    public GameObject frotaObj;
    public double lng, lat;
    ConnectionCreator connCreator;
    FSUDestinationBehaviour fsuManager;
    List<OnlineMapsMarker3D> usinasWithoutTerm = new List<OnlineMapsMarker3D>();

    public void SetForFSU(GameObject _usinaObj, GameObject _frotaObj, ConnectionCreator _connCreator, FSUDestinationBehaviour _fsuManager = null, List<OnlineMapsMarker3D> _usinasWithoutTerm = null)
    {
        if (_fsuManager != null)
            fsuManager = _fsuManager;
        if (_usinasWithoutTerm != null)
            usinasWithoutTerm = _usinasWithoutTerm;

        usinaObj = _usinaObj;
        frotaObj = _frotaObj;
        connCreator = _connCreator;
        UsinaStorage usinaStorage = usinaObj.GetComponent<UsinaStorage>();
        usinaLabel.text = usinaStorage.usina.nome;
        lat = usinaStorage.usina.coordenadas.lat;
        lng = usinaStorage.usina.coordenadas.lon;
        GetComponent<Toggle>().onValueChanged.AddListener(ConnectUsina);
    }

    public void SetForFSRU(GameObject _usinaObj)
    {
        usinaObj = _usinaObj;
        UsinaStorage usinaStorage = usinaObj.GetComponent<UsinaStorage>();
        usinaLabel.text = usinaStorage.usina.nome;
        lat = usinaStorage.usina.coordenadas.lat;
        lng = usinaStorage.usina.coordenadas.lon;
        GetComponent<Button>().onClick.AddListener(CentralizeUsina);
    }

    public void ConnectUsina(bool connect)
    {
        TextMesh msg = connCreator.markerDisplacer.msg;
        if (connect)
        {
            if (usinaObj != null && connCreator != null)
            {
                OnlineMapsMarker3D usinaMarker = usinaObj.GetComponent<OnlineMapsMarker3DInstance>().marker as OnlineMapsMarker3D;
                if (usinasWithoutTerm.Count>0 && usinasWithoutTerm.Contains(usinaMarker))
                {
                    if(fsuManager!=null)
                    {
                        fsuManager.frotaObj = frotaObj;
                        fsuManager.BuildInUsina(usinaMarker);
                    }
                }
                else
                {
                    connCreator.ConectarFrotaComTermRegas(frotaObj, usinaObj.GetComponent<UsinaStorage>().termRegasMarker.instance);
                }
            }
        }
        else
        {
            Frota frotaToRemove = null;
            foreach (Frota f in frotaObj.GetComponent<FrotaStorage>().frotas)
            {
                if(f.terminalRegasDestino == usinaObj.GetComponent<UsinaStorage>().termRegasMarker.instance.GetComponent<TerminalRegasStorage>().terminal.nome)
                {
                    frotaToRemove = f;
                    UsinaStorage usinaStorage = usinaObj.GetComponent<UsinaStorage>();
                    TerminalRegasStorage termMarker = usinaStorage.termRegasMarker.instance.GetComponent<TerminalRegasStorage>();
                    usinaStorage.usina.fonteDoGas = "gasoduto";
                    usinaStorage.SetState();
                    connCreator.sessionManager.session.frotasColocadas.Remove(f);
                    OnlineMapsDrawingElementManager.RemoveItem(usinaStorage.frotaLine);
                    OnlineMapsDrawingElementManager.RemoveItem(usinaStorage.frotaLine2);
                    connCreator.sessionManager.UpdateSession();
                }
            }

            if (frotaToRemove != null)
            {
                frotaObj.GetComponent<FrotaStorage>().frotas.Remove(frotaToRemove);
            }
            
        }
    }


    public void CentralizeUsina()
    {
        OnlineMaps.instance.SetPosition(lng, lat);
    }
}
