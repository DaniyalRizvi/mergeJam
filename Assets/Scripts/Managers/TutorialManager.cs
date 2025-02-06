using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialManager : Singelton<TutorialManager>
{
    public GameObject panel;
    public GameObject handIcon;
    public GameObject hand;
    public GameObject imagePanel;
    public GameObject passengerPanel;
    public GameObject vehiclePanel;
    public TMP_Text panelText;
    public int tutorialCase = 0;
    public Transform mainCamera;
    public Transform passengerTransform;
    public Transform busTransform;
    public Transform homeTransform;
    public Transform fan;
    public Transform rocket;
    public Transform jump;
    public List<Outline> passengerOutlines;
    public List<Outline> busOutlines;
    public bool isInAnimation = false;
    internal List<GameObject> Busses = new List<GameObject>();

    public void HidePanel()
    {
        panel.SetActive(false);
        imagePanel.SetActive(false);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("TutorialCase"))
            tutorialCase = PlayerPrefs.GetInt("TutorialCase");

        if (PlayerPrefs.GetInt("TrashTutorial") == 1 && PlayerPrefs.GetInt("TrashTutorialPlayed") == 0)
        {
            StartCoroutine(PlayTrashTutorial());
        }
    }

    public IEnumerator PlayTrashTutorial()
    {
        //HidePanel();
        yield return new WaitForSeconds(0.5f);
        InitFirstTrashItems();
        yield return new WaitForSeconds(2f);
        InitSecondTrashItems();
        yield return new WaitForSeconds(2f);

        PlayerPrefs.SetInt("TrashTutorialPlayed", 1);
        //TutorialCompleted();
    }

    public void InitPanel(string text)
    {
        handIcon.SetActive(true);
        hand.SetActive(false);
        panel.SetActive(true);
        panelText.SetText(text);
    }

    public void MoveToPassengers()
    {
        StartCoroutine(MoveToPassengersCoroutine());
    }

    private IEnumerator MoveToPassengersCoroutine()
    {
        isInAnimation = true;
        float time = 0;
        const float duration = 1f;
        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetPosition = passengerTransform.position;
        
        var startRotation = mainCamera.transform.rotation;
        var targetRotation = passengerTransform.transform.rotation;
        while (time < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        transform.rotation = targetRotation;
        isInAnimation = false;
        passengerOutlines.ForEach(x => x.enabled = true);

        yield return new WaitForSeconds(0.5f);
        
        passengerPanel.SetActive(true);
        vehiclePanel.SetActive(false);
        imagePanel.SetActive(true);
        
        InitPanel("These are the passengers you need to accommodate." + "\nMake sure to match the colors! Passengers only board vehicles of the same color.");
    }
    
    public void MoveToBusses()
    {
        passengerOutlines.ForEach(x => x.enabled = false);
        StartCoroutine(MoveToBussesCoroutine());
    }

    private IEnumerator MoveToBussesCoroutine()
    {
        isInAnimation = true;
        float time = 0;
        const float duration = 1f;
        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetPosition = busTransform.position;
        
        var startRotation = mainCamera.transform.rotation;
        var targetRotation = busTransform.transform.rotation;
        while (time < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        transform.rotation = targetRotation;
        isInAnimation = false;
        busOutlines.ForEach(x => x.enabled = true);
        yield return new WaitForSeconds(0.5f);
        
        imagePanel.SetActive(true);
        vehiclePanel.SetActive(true);
        passengerPanel.SetActive(false);
        
        InitPanel("These are the vehicles." + 
        "\nTap a vehicle to move it onto the vehicle slot." + 
        "\nBut be careful! You might not need every vehicle.");
    }

    public void MoveToFull()
    {
        foreach (var x in passengerOutlines.Where(x => x != null))
        {
            x.enabled = false;
        }

        PlayerPrefs.SetInt("LevelTutorialCompleted",1);
        Debug.Log("LevelTutorialCompleted: "+PlayerPrefs.GetInt("LevelTutorialCompleted"));
        HidePanel();
        StartCoroutine(MoveToFullCoroutine());
        
    }

    private IEnumerator MoveToFullCoroutine()
    {
        isInAnimation = true;
        float time = 0;
        const float duration = 1f;
        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetPosition = homeTransform.position;
        
        var startRotation = mainCamera.transform.rotation;
        var targetRotation = homeTransform.transform.rotation;
        while (time < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        transform.rotation = targetRotation;
        isInAnimation = false;
        InitPanel("Great! You're ready to start the game.");
        TutorialCompleted();
    }

    public void InitFirstBus()
    {
        InitPanel("Tap First vehicle to move it to the vehicle slot.");
        handIcon.SetActive(false);
        hand.SetActive(false);//ZZ
        busOutlines.ForEach(x => x.enabled = false);
        var outline = busOutlines.FirstOrDefault(x => x.GetComponent<Bus>().busColor == Colors.Blue && x.GetComponent<Bus>().capacity == 1);
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.green;
            outline.OutlineWidth = 5f;
            var pos = mainCamera.GetComponent<Camera>().WorldToScreenPoint(outline.transform.position);
            hand.transform.position = pos + new Vector3(25, 0, -25);
            Busses.Clear();
            Busses.Add(outline.gameObject);
        }
    }

    public void InitSecondBus()
    {
        InitPanel("Tap this vehicle to move it to the next slot.");
        handIcon.SetActive(false);
        hand.SetActive(false);//ZZ
        foreach (var x in busOutlines.Where(x => x != null))
        {
            x.enabled = false;
        }

        var outline = busOutlines.FirstOrDefault(x => x!= null && x.GetComponent<Bus>().busColor == Colors.Blue && x.GetComponent<Bus>().capacity == 1 && x.GetComponent<Bus>().currentSize >= 1);
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.green;
            outline.OutlineWidth = 5f;
            var pos = mainCamera.GetComponent<Camera>().WorldToScreenPoint(outline.transform.position);
            hand.transform.position = pos + new Vector3(25, 0, -25);
            Busses.Clear();
            Busses.Add(outline.gameObject);
        }
    }

    public void InitTrashItems()
    {
        
        foreach (var x in busOutlines.Where(x => x != null))
        {
            x.enabled = false;
        }

        Busses.Clear();
        var outlineOne = busOutlines.FirstOrDefault(x => x != null && x.GetComponent<Bus>().busColor == Colors.Pink);
        var outlineTwo = busOutlines.FirstOrDefault(x =>
            x != null && x.GetComponent<Bus>().busColor == Colors.Pink && x != outlineOne);
        if (outlineOne != null)
        {
            outlineOne.enabled = true;
            outlineOne.OutlineColor = Color.red;
            outlineOne.OutlineWidth = 5f;
            Busses.Add(outlineOne.gameObject);
        }

        if (outlineTwo != null)
        {
            outlineTwo.enabled = true;
            outlineTwo.OutlineColor = Color.red;
            outlineTwo.OutlineWidth = 5f;
            Busses.Add(outlineTwo.gameObject);
        }
        InitPanel("Some vehicles are unwanted vehicles. These take up space in the vehicle slots." + 
        "\nTo get rid of trash items, merge two trash items together to free up space.");
    }
    public bool IsFirstTrashDone;
    Outline outlineOneRef;
    public void InitFirstTrashItems()
    {
        outlineOneRef = new Outline();
       // InitPanel("Tap this Trash vehicle to move it to the vehicle slot.");
        handIcon.SetActive(false);
        hand.SetActive(false);//ZZ
        foreach (var x in busOutlines.Where(x => x != null))
        {
            x.enabled = false;
        }
        Outline outlineOne = busOutlines.FirstOrDefault(x => x!=null &&  x.GetComponent<Bus>().busColor == Colors.Pink && x.GetComponent<Bus>().capacity == 1);
        if (outlineOne != null)
        {
            this.outlineOneRef = outlineOne;
            outlineOne.enabled = true;
            outlineOne.OutlineColor = Color.green;
            outlineOne.OutlineWidth = 5f;
            var pos = mainCamera.GetComponent<Camera>().WorldToScreenPoint(outlineOne.transform.position);
            hand.transform.position = pos + new Vector3(25, 0, -25);
            Busses.Clear();
            Busses.Add(outlineOne.gameObject);
        }
        #region TRASH
        //foreach (var x in busOutlines.Where(x => x != null))
        //{
        //    x.enabled = false;
        //}

        //Busses.Clear();
        //var outlineOne = busOutlines.FirstOrDefault(x => x != null && x.GetComponent<Bus>().busColor == Colors.Pink);
       
        //if (outlineOne != null)
        //{
        //    outlineOne.enabled = true;
        //    outlineOne.OutlineColor = Color.red;
        //    outlineOne.OutlineWidth = 5f;

        //    var pos = mainCamera.GetComponent<Camera>().WorldToScreenPoint(outlineOne.transform.position);
        //    hand.transform.position = pos + new Vector3(25, 0, -25);

        //    Busses.Add(outlineOne.gameObject);
        //}
        #endregion
        InitPanel("Some vehicles are unwanted vehicles. These take up space in the vehicle slots." +
        "\nTo get rid of trash items, merge two trash items together to free up space.");

        IsFirstTrashDone = true;
        HidePanel();
    }
    public void InitSecondTrashItems()
    {
        handIcon.SetActive(false);
        hand.SetActive(false);//ZZ
        foreach (var x in busOutlines.Where(x => x != null))
        {
            x.enabled = false;
        }
        //Outline outline = busOutlines.FirstOrDefault(x => x.GetComponent<Bus>().busColor == Colors.Pink && x.GetComponent<Bus>().capacity == 1);
        Outline outline = busOutlines.FirstOrDefault(x =>
           x != null && x.GetComponent<Bus>().busColor == Colors.Pink && x != outlineOneRef);
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.green;
            outline.OutlineWidth = 5f;
            var pos = mainCamera.GetComponent<Camera>().WorldToScreenPoint(outline.transform.position);
            hand.transform.position = pos + new Vector3(25, 0, -25);
            Busses.Clear();
            Busses.Add(outline.gameObject);
        }
        InitPanel("Tap To The Second unwanted vehicle to move it to the vehicle slot.");
        IsFirstTrashDone = false;
    }

    public void InitFan()
    {
        StartCoroutine(InitFanCoroutine());
    }
    public void InitFanPanel()
    {
        InitPanel("The Fan rearranges the vehicle pile, making vehicles at the bottom easier to access.");
    }
    private IEnumerator InitFanCoroutine()
    {
        yield return null;
        //InitPanel("The Fan rearranges the vehicle pile, making vehicles at the bottom easier to access.");
        isInAnimation = true;
        float time = 0;
        const float duration = 1f;
        Vector3 startPosition = fan.localPosition;
        Vector3 targetPosition = new Vector3(250, -800);
        fan.GetComponent<Button>().interactable = false;
        fan.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        var startScale = fan.transform.localScale;
        var targetScale = Vector3.one;
        while (time < duration)
        {
            fan.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            fan.transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        fan.transform.localPosition = targetPosition;
        fan.transform.localScale = targetScale;
        Vector3 HandTargetPos = targetPosition+new Vector3(100, 0, -25);
        hand.transform.localPosition = HandTargetPos;
        hand.SetActive(true);
        fan.GetComponent<Button>().interactable = true;
        isInAnimation = false;
        tutorialCase++;
        HidePanel();
    }

    public void InitRocket()
    {
        StartCoroutine(InitRocketCoroutine());
    }
    public void InitRocketPanel()
    {
        StartCoroutine(InitRocketPanelCoroutine());
    }
    private IEnumerator InitRocketPanelCoroutine()
    {
        isInAnimation = true;
        yield return new WaitForSeconds(2f);
        isInAnimation = false;
        HidePanel();
        InitPanel("Rockets destroy a trash item, clearing space for you to easily select required items.");
        TutorialManager.Instance.tutorialCase++;
    }

    private IEnumerator InitRocketCoroutine()
    {
        HidePanel();
        rocket.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(0.1f);
        //yield return new WaitForSeconds(5f);
        //yield return null;
        //InitPanel("Rockets destroy a trash item, clearing space for you to easily select required items.");
        isInAnimation = true;
        float time = 0;
        const float duration = 1f;
        Vector3 startPosition = rocket.localPosition;
        Vector3 targetPosition = new Vector3(0, -800);
        rocket.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        var startScale = rocket.transform.localScale;
        var targetScale = Vector3.one;
        while (time < duration)
        {
            rocket.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            rocket.transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        rocket.transform.localPosition = targetPosition;
        rocket.transform.localScale = targetScale;
        isInAnimation = false;

        yield return new WaitForSeconds(.5f);
        Vector3 HandTargetPos = targetPosition + new Vector3(100, 0, -25);
        hand.transform.localPosition = HandTargetPos;
        hand.SetActive(true);
        rocket.GetComponent<Button>().interactable = true;
        HidePanel();
    }

    public void InitJump()
    {
        StartCoroutine(InitJumpCoroutine());
    }
    public void InitJumpPanel()
    {
        StartCoroutine(InitJumpPanelCoroutine());
    }

    public IEnumerator InitJumpCoroutine()
    {
        HidePanel();
        jump.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(0.1f);
        isInAnimation = true;
        float time = 0;
        const float duration = 1f;
        Vector3 startPosition = jump.localPosition;
        Vector3 targetPosition = new Vector3(-250, -800);
        jump.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        var startScale = jump.transform.localScale;
        var targetScale = Vector3.one;
        while (time < duration)
        {
            jump.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            jump.transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        jump.transform.localPosition = targetPosition;
        jump.transform.localScale = targetScale;
        isInAnimation = false;

        yield return new WaitForSeconds(.5f);
        Vector3 HandTargetPos = targetPosition + new Vector3(100, 0, -25);
        hand.transform.localPosition = HandTargetPos;
        hand.SetActive(true);
        jump.GetComponent<Button>().interactable = true;
        HidePanel();
    }

    private IEnumerator InitJumpPanelCoroutine()
    {
        isInAnimation = true;
        yield return new WaitForSeconds(2f);
        isInAnimation = false;
        HidePanel();
        InitPanel("Jump bounces the car back to the ground from the slot.");
        // tutorialCase++;
    }

    public void TutorialCompleted()
    {
        Invoke(nameof(InitPanelAsync),2f);
    }

    private void InitPanelAsync()
    {
        InitPanel("Congratulations on completing the tutorial");
        Invoke(nameof(LoadGameScene), 2f);
    }

    private void LoadGameScene()
    {
        //SceneManager.LoadScene(sceneBuildIndex: 1);
        //PlayerPrefs.SetInt("TutorialCompleted",1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
