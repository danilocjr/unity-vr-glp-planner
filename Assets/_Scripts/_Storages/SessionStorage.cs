using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionStorage : MonoBehaviour
{
    public Session session;
    SessionManager sessionManager;
    public bool isOnResources;
    private void OnEnable()
    {
        sessionManager = GameObject.Find("SessionManager").GetComponent<SessionManager>();
    }

    public void CallLoadSession()
    {
        sessionManager.CheckIfWantToSaveChangesBeforeLoad(this.gameObject);
    }
}
