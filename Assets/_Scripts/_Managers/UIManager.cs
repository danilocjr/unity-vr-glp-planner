using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update

    public SessionManager sessionManager;
    public Animator sessionsAnim, premisesAnim, viewsAnim, sessionsBtn, premisesBtn, viewsBtn, presetsAnim;
    public GameObject sessionsCheck, premisesCheck, viewsCheck, presetsCheck;

    public GameObject oskParent;

    public Transform oskLeft, oskCenter, oskRight, oskCenter2;

    public GameObject newSessionBtn, saveSessionBtn, renameSessionBtn, deleteSessionBtn;

    private void Start()
    {
        //sessionsAnim.gameObject.SetActive(false);
        //premisesAnim.gameObject.SetActive(false);
        //viewsAnim.gameObject.SetActive(false);
        //presetsAnim.gameObject.SetActive(false);
        sessionsCheck.SetActive(false);
        premisesCheck.SetActive(false);
        viewsCheck.SetActive(false);
        presetsCheck.SetActive(false);

        EventTrigger.Entry sessionNameEntry = new EventTrigger.Entry();
        sessionNameEntry.eventID = EventTriggerType.Select;
        sessionNameEntry.callback.AddListener((eventData) => { oskParent.GetComponentInChildren<OnScreenKeyboard>().SetActiveFocus(sessionManager.sessionNameField); });
        sessionManager.sessionNameField.GetComponent<EventTrigger>().triggers.Add(sessionNameEntry);

        EventTrigger.Entry sessionRenameEntry = new EventTrigger.Entry();
        sessionRenameEntry.eventID = EventTriggerType.Select;
        sessionRenameEntry.callback.AddListener((eventData) => { oskParent.GetComponentInChildren<OnScreenKeyboard>().SetActiveFocus(sessionManager.sessionRenameField); });
        sessionManager.sessionRenameField.GetComponent<EventTrigger>().triggers.Add(sessionRenameEntry);


        foreach (InputField i in GetComponentsInChildren<InputField>())
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Select;
            entry.callback.AddListener((eventData) => { oskParent.GetComponentInChildren<OnScreenKeyboard>().SetActiveFocus(i); });
            if (i.GetComponent<EventTrigger>() != null)
            {

                i.GetComponent<EventTrigger>().triggers.Add(entry);
                print("setou " + i.gameObject.name);
            }
            else
            {
                print("NÃO setou " + i.gameObject.name);
            }
        }
    }

    //TODO qual o motivo deste update??
    private void Update()
    {
        renameSessionBtn.SetActive(!string.IsNullOrEmpty(sessionManager.session.nome));
        deleteSessionBtn.SetActive(!string.IsNullOrEmpty(sessionManager.session.nome));
    }

    public void OnClickSessions(bool response)
    {
        //sessionsAnim.gameObject.SetActive(response);
        sessionsAnim.SetBool("show", response);
        sessionsCheck.SetActive(response);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnClickPremises(bool response)
    {
        //premisesAnim.gameObject.SetActive(response);
        premisesAnim.SetBool("show", response);
        premisesCheck.SetActive(response);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnClickViews(bool response)
    {
        //viewsAnim.gameObject.SetActive(response);
        viewsAnim.SetBool("show", response);
        viewsCheck.SetActive(response);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnClickPresets(bool response)
    {
        //presetsAnim.gameObject.SetActive(response);
        presetsAnim.SetBool("show", response);
        presetsCheck.SetActive(response);
        if (!response)
        {
            presetsAnim.GetComponent<PresetsManager>().CancelEdition();
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SetLeftOSKPoint()
    {
        oskParent.transform.SetParent(oskLeft);
        oskParent.GetComponent<RectTransform>().localPosition = Vector3.zero;
        oskParent.transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (oskParent.GetComponentInChildren<OnScreenKeyboard>().panels[0].activeSelf)
        {
            oskParent.GetComponentInChildren<OnScreenKeyboard>().panels[0].transform.SetParent(oskParent.GetComponentInChildren<OnScreenKeyboard>().leftMainKeyboardPoint);
            oskParent.GetComponentInChildren<OnScreenKeyboard>().panels[0].transform.localPosition = Vector3.zero;
            oskParent.GetComponentInChildren<OnScreenKeyboard>().panels[0].transform.SetParent(oskParent.GetComponentInChildren<OnScreenKeyboard>().gameObject.transform);
        }
    }

    public void SetCenterOSKPoint()
    {
        oskParent.transform.SetParent(oskCenter);
        oskParent.GetComponent<RectTransform>().localPosition = Vector3.zero;
        oskParent.transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (oskParent.GetComponentInChildren<OnScreenKeyboard>().panels[0].activeSelf)
        {
            oskParent.GetComponentInChildren<OnScreenKeyboard>().panels[0].transform.SetParent(oskParent.GetComponentInChildren<OnScreenKeyboard>().centerMainKeyboardPoint);
            oskParent.GetComponentInChildren<OnScreenKeyboard>().panels[0].transform.localPosition = Vector3.zero;
            oskParent.GetComponentInChildren<OnScreenKeyboard>().panels[0].transform.SetParent(oskParent.GetComponentInChildren<OnScreenKeyboard>().gameObject.transform);
        }
    }

    public void SetCenterOSKPoint2()
    {
        oskParent.transform.SetParent(oskCenter2);
        oskParent.GetComponent<RectTransform>().localPosition = Vector3.zero;
        oskParent.transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (oskParent.GetComponentInChildren<OnScreenKeyboard>().panels[0].activeSelf)
        {
            oskParent.GetComponentInChildren<OnScreenKeyboard>().panels[0].transform.SetParent(oskParent.GetComponentInChildren<OnScreenKeyboard>().centerMainKeyboardPoint);
            oskParent.GetComponentInChildren<OnScreenKeyboard>().panels[0].transform.localPosition = Vector3.zero;
            oskParent.GetComponentInChildren<OnScreenKeyboard>().panels[0].transform.SetParent(oskParent.GetComponentInChildren<OnScreenKeyboard>().gameObject.transform);
        }
    }

    public void SetRightOSKPoint()
    {
        oskParent.transform.SetParent(oskRight);
        oskParent.GetComponent<RectTransform>().localPosition = Vector3.zero;
        oskParent.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
