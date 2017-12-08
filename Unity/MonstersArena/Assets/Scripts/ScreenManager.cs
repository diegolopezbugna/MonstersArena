using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class ScreenManager : MonoBehaviour {

    public Animator screenPlayMode;
    public Animator screenSelectMonster;
    public Animator screenWaitingOtherPlayers;
    public GameObject loading;
    public GameObject monsterSelectGridContainer;
    public GameObject monsterButtonPrefab;
    public Text creditsText;
    public Text playersText;
    public Text countDownText;

    private Coroutine countDownCoroutine;

    //Currently Open Screen
    private Animator m_Open;

    //The GameObject Selected before we opened the current Screen.
    //Used when closing a Screen, so we can go back to the button that opened it.
    private GameObject m_PreviouslySelected;

    void Start()
    {
        OpenPanel(screenPlayMode);
        
        if (monsterSelectGridContainer != null)
        {
            foreach (var m in GameManager.Instance.monsters)
            {
                var mb = Instantiate(monsterButtonPrefab, monsterSelectGridContainer.transform);
                mb.name = "mb_" + m.Code;
                var texts = mb.GetComponentsInChildren<Text>();
                texts[0].text = m.CreditsCost.ToString();
                texts[1].text = m.Name;
                mb.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        GameManager.Instance.OnSelectMonster(m);
//                        loading.SetActive(true);
//                        CloseCurrent();
//                        StartCoroutine(DelayedStart(1f, () => GameManager.Instance.OnSelectMonster(m)));
                    });
            }
        }
    }

    IEnumerator DelayedStart(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    public void OnSelectOnePlayer()
    {
        OpenPanel(screenSelectMonster);
    }

    public void OnSelectMultiplayer()
    {
        OpenPanel(screenSelectMonster);
    }

    public void OpenPlayMode()
    {
        OpenPanel(screenSelectMonster);
    }

    public void OpenWaitingOtherPlayers()
    {
        OpenPanel(screenWaitingOtherPlayers);
    }

    public void ShowLoading()
    {
        loading.SetActive(true);
        CloseCurrent();
    }

    public IEnumerator RefreshCredits(int credits, Action onFinished)
    {
        var currentCredits = int.Parse(creditsText.text);
        var creditsStep = (currentCredits - credits) / 10;

        for (int i = 0; i < 10; i++)
        {
            currentCredits -= creditsStep;
            creditsText.text = currentCredits.ToString();
            yield return new WaitForSecondsRealtime(0.01f);
        }

        currentCredits = credits + 5;

        for (int i = 0; i < 5; i++)
        {
            currentCredits -= 1;
            creditsText.text = currentCredits.ToString();
            yield return new WaitForSecondsRealtime(0.01f);
        }

        creditsText.text = credits.ToString();
        yield return new WaitForSecondsRealtime(0.5f);

        onFinished();
    }

    public void RefreshPlayers(int players)
    {
        playersText.text = players.ToString();
    }

    public void StartCountDown(int countDownSeconds)
    {
        if (countDownCoroutine != null)
            StopCoroutine(countDownCoroutine);
        countDownCoroutine = StartCoroutine(CountDown(countDownSeconds));
    }

    IEnumerator CountDown(int countDownSeconds)
    {
        for (int i = countDownSeconds; i >= 0; i--)
        {
            countDownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
    }







    //Closes the currently open panel and opens the provided one.
    //It also takes care of handling the navigation, setting the new Selected element.
    public void OpenPanel (Animator anim)
    {
        if (m_Open == anim)
            return;

        //Activate the new Screen hierarchy so we can animate it.
        anim.gameObject.SetActive(true);
        //Save the currently selected button that was used to open this Screen. (CloseCurrent will modify it)
        var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;
        //Move the Screen to front.
        anim.transform.SetAsLastSibling();

        CloseCurrent();

        m_PreviouslySelected = newPreviouslySelected;

        //Set the new Screen as then open one.
        m_Open = anim;
        //Start the open animation
        m_Open.SetBool("Open", true);

        //Set an element in the new screen as the new Selected one.
        GameObject go = FindFirstEnabledSelectable(anim.gameObject);
        SetSelected(go);
    }

    //Finds the first Selectable element in the providade hierarchy.
    static GameObject FindFirstEnabledSelectable (GameObject gameObject)
    {
        GameObject go = null;
        var selectables = gameObject.GetComponentsInChildren<Selectable> (true);
        foreach (var selectable in selectables) {
            if (selectable.IsActive () && selectable.IsInteractable ()) {
                go = selectable.gameObject;
                break;
            }
        }
        return go;
    }

    //Closes the currently open Screen
    //It also takes care of navigation.
    //Reverting selection to the Selectable used before opening the current screen.
    public void CloseCurrent()
    {
        if (m_Open == null)
            return;

        //Start the close animation.
        m_Open.SetBool("Open", false);

        //Reverting selection to the Selectable used before opening the current screen.
        SetSelected(m_PreviouslySelected);
        //Start Coroutine to disable the hierarchy when closing animation finishes.
        StartCoroutine(DisablePanelDeleyed(m_Open));
        //No screen open.
        m_Open = null;
    }

    //Coroutine that will detect when the Closing animation is finished and it will deactivate the
    //hierarchy.
    IEnumerator DisablePanelDeleyed(Animator anim)
    {
        bool closedStateReached = false;
        bool wantToClose = true;
        while (!closedStateReached && wantToClose)
        {
            if (!anim.IsInTransition(0))
                closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName("Closed");

            wantToClose = !anim.GetBool("Open");

            yield return new WaitForEndOfFrame();
        }

        if (wantToClose)
            anim.gameObject.SetActive(false);
    }

    //Make the provided GameObject selected
    //When using the mouse/touch we actually want to set it as the previously selected and 
    //set nothing as selected for now.
    private void SetSelected(GameObject go)
    {
        //Select the GameObject.
        EventSystem.current.SetSelectedGameObject(go);

        //If we are using the keyboard right now, that's all we need to do.
        var standaloneInputModule = EventSystem.current.currentInputModule as StandaloneInputModule;
        if (standaloneInputModule != null) // && standaloneInputModule.inputMode == StandaloneInputModule.InputMode.Buttons)
            return;

        //Since we are using a pointer device, we don't want anything selected. 
        //But if the user switches to the keyboard, we want to start the navigation from the provided game object.
        //So here we set the current Selected to null, so the provided gameObject becomes the Last Selected in the EventSystem.
        EventSystem.current.SetSelectedGameObject(null);
    }
}