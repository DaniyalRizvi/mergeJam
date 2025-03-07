using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VoxelBusters.AdsKit;

public class InputManager : Singelton<InputManager>
{
    
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    private Bus _selectedBus; 
    private GameInputActions _inputActions;
    private InputAction _positionAction;

    protected override void Awake()
    {
        _inputActions = new GameInputActions();
        _positionAction = _inputActions.FindAction("Position");
        base.Awake();

    }

    public InputAction GetPositionAction()
    {
        return _positionAction;
    }

    public GameInputActions GetInputActions()
    {
        return _inputActions;
    }

    void OnEnable()
    {
        _inputActions.Player.Click.performed += OnClickPerformed; 
        _inputActions.Enable(); 
    }

    void OnDisable()
    {
        _inputActions.Player.Click.performed -= OnClickPerformed; 
        _inputActions.Disable(); 
    }

    void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (TutorialManager.Instance)
        {
            if (TutorialManager.Instance.isInAnimation)
                return;
            if (PlayerPrefs.GetInt("LevelTutorialCompleted") == 0)
            {
                switch (TutorialManager.Instance.tutorialCase)
                {
                    case 0:
                    {
                        TutorialManager.Instance.tutorialCase++;
                        TutorialManager.Instance.HidePanel();
                        TutorialManager.Instance.MoveToPassengers();
                        return;
                    }
                    case 1:
                    {
                        TutorialManager.Instance.tutorialCase++;
                        TutorialManager.Instance.HidePanel();
                        TutorialManager.Instance.MoveToBusses();
                        return;
                    }
                    case 2:
                    {
                        TutorialManager.Instance.tutorialCase++;
                        TutorialManager.Instance.HidePanel();
                        TutorialManager.Instance.InitFirstBus();
                        return;
                    }
                    case 3:
                    {
                        TutorialManager.Instance.tutorialCase++;
                        TutorialManager.Instance.HidePanel();
                        TutorialManager.Instance.hand.SetActive(true);
                        return;
                    }
                    case 4:
                    {
                        TutorialManager.Instance.tutorialCase++;

                        break;
                    }
                    case 6:
                    {
                        TutorialManager.Instance.HidePanel();
                        TutorialManager.Instance.hand.SetActive(true);
                        break;
                    }
                    case 7:
                    {
                        TutorialManager.Instance.tutorialCase++;
                        TutorialManager.Instance.MoveToFull();
                        break;
                    }
                    default:
                        break;
                }
            }

            else if (PlayerPrefs.GetInt("TrashTutorial") == 0 && PlayerPrefs.GetInt("ActualCurrentLevel")==25)
            {
                    switch (TutorialManager.Instance.customTrashIterator)
                    {
                        case 1:
                            TutorialManager.Instance.customTrashIterator++;
                            break;
                        case 2:
                        {
                            TutorialManager.Instance.customTrashIterator++;
                            TutorialManager.Instance.HidePanel();
                            //TutorialManager.Instance.InitTrashItems();
                            TutorialManager.Instance.InitFirstTrashItems();
                            break;
                        }
                        case 3:
                        {
                            //First Trash Done
                            //TutorialManager.Instance.customTrashIterator++;
                            TutorialManager.Instance.HidePanel();
                            TutorialManager.Instance.hand.SetActive(true);
                            break;
                        }
                    }
            }

            else if (PlayerPrefs.GetInt("FanTutorial") == 0 && PlayerPrefs.GetInt("ActualCurrentLevel")==35)
            {
                switch (TutorialManager.Instance.customFanIterator)
                {
                    case 1:
                        Debug.LogError($"Case 1 FanTutorial: {TutorialManager.Instance.customFanIterator}");
                        TutorialManager.Instance.customFanIterator++;
                        break;
                    case 2:

                        //Fand Button Active and Show hand On It
                        Debug.LogError($"Case 2 FanTutorial: {TutorialManager.Instance.customFanIterator}");
                        // TutorialManager.Instance.customFanIterator++;
                        TutorialManager.Instance.HidePanel();
                        TutorialManager.Instance.InitFan();

                        break;
                    case 3:
                        {
                            Debug.LogError($"Case 3 FanTutorial: {TutorialManager.Instance.customFanIterator}");
                            TutorialManager.Instance.HidePanel();
                            TutorialManager.Instance.hand.SetActive(true);
                            PlayerPrefs.SetInt("FanTutorial", 1);
                            PlayerPrefs.SetInt("FanTutorialPlayed", 1);
                            PlayerPrefs.SetInt("CurrentLevel", 35);
                            PlayerPrefs.SetInt("ActualCurrentLevel", 0);
                            TutorialManager.Instance.TutorialCompleted();
                            break;
                        }

                }
            }

            else if (PlayerPrefs.GetInt("RocketTutorial") == 0 && PlayerPrefs.GetInt("ActualCurrentLevel")==27)
            {
                switch (TutorialManager.Instance.customRocketIterator)
                {
                    case 1:
                        Debug.LogError($"Case 1 RocketTutorial: {TutorialManager.Instance.customRocketIterator}");
                        TutorialManager.Instance.customRocketIterator++;
                        break;
                    case 2:

                        //Fand Button Active and Show hand On It
                        Debug.LogError($"Case 2 RocketTutorial: {TutorialManager.Instance.customRocketIterator}");
                        // TutorialManager.Instance.customFanIterator++;
                        TutorialManager.Instance.HidePanel();
                        TutorialManager.Instance.InitRocket();
                        //TutorialManager.Instance.InitFan();

                        break;
                    case 3:
                        {
                            Debug.LogError($"Case 3 RocketTutorial: {TutorialManager.Instance.customRocketIterator}");
                            TutorialManager.Instance.HidePanel();
                            TutorialManager.Instance.hand.SetActive(true);
                            PlayerPrefs.SetInt("RocketTutorial", 1);
                            PlayerPrefs.SetInt("RocketTutorialPlayed", 1);
                            PlayerPrefs.SetInt("CurrentLevel", 27);
                            PlayerPrefs.SetInt("ActualCurrentLevel", 0);
                            TutorialManager.Instance.TutorialCompleted();
                            break;
                        }

                }
            }
            
            else if (PlayerPrefs.GetInt("JumpTutorial") == 0 && PlayerPrefs.GetInt("ActualCurrentLevel")==13)
            {
                switch (TutorialManager.Instance.customJumpIterator)
                {
                    case 1:
                        Debug.LogError($"Case 1 JumpTutorial: {TutorialManager.Instance.customJumpIterator}");
                        TutorialManager.Instance.customJumpIterator++;
                        break;
                    case 2:
                        {
                            Debug.LogError($"Case 2 JumpTutorial: {TutorialManager.Instance.customJumpIterator}");
                            TutorialManager.Instance.customJumpIterator++;
                            TutorialManager.Instance.HidePanel();
                            TutorialManager.Instance.InitFirstBus();
                            return;
                        }
                    case 3:
                        {
                            Debug.LogError($"Case 3 JumpTutorial: {TutorialManager.Instance.customJumpIterator}");
                            TutorialManager.Instance.customJumpIterator++;
                            TutorialManager.Instance.HidePanel();
                            TutorialManager.Instance.hand.SetActive(true);
                            return;
                        }
                    case 4:
                        Debug.LogError($"Case 4 JumpTutorial: {TutorialManager.Instance.customJumpIterator}");
                        TutorialManager.Instance.HidePanel();
                        TutorialManager.Instance.hand.SetActive(false);
                        TutorialManager.Instance.InitJump();
                        break;
                    case 5:
                        {
                            Debug.LogError($"Case 5 JumpTutorial: {TutorialManager.Instance.customJumpIterator}");
                            TutorialManager.Instance.HidePanel();
                            TutorialManager.Instance.hand.SetActive(true);
                            PlayerPrefs.SetInt("JumpTutorial", 1);
                            PlayerPrefs.SetInt("JumpTutorialPlayed", 1);
                            PlayerPrefs.SetInt("CurrentLevel", 13);
                            PlayerPrefs.SetInt("ActualCurrentLevel", 0);
                            TutorialManager.Instance.TutorialCompleted();
                            break;
                        }

                }
            }

        }

        if (!context.performed || IsPointerOverUI())
            return;

         
        Vector2 mousePosition = _positionAction.ReadValue<Vector2>();
        Ray ray = Camera.main!.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hit) && hit.collider != null)
        {
            DeselectBus();//Remove When Enable Swaping Bus
            Bus clickedBus = hit.collider.GetComponent<Bus>();
            Slot clickedSlot = hit.collider.GetComponent<Slot>();
            if (TutorialManager.Instance)
            {
                if (clickedBus != null)
                {
                    if (!TutorialManager.Instance.Busses.Contains(clickedBus.gameObject))
                    {
                        return;
                    }
                }
            }

            if (clickedBus != null && !GameManager.Instance.PlacingBus && !GameManager.Instance.MergingBus)
            {
                // if (_selectedBus != null)
                // {
                //     if (_selectedBus.AssignedSlot != null && clickedBus.AssignedSlot != null)
                //     {
                //        Debug.Log("HTFTFTGBDF");
                //        
                //         var currentSlot = clickedBus.AssignedSlot;
                //         var newSlot = _selectedBus.AssignedSlot;
                //         _selectedBus.AssignSlot(currentSlot);
                //         newSlot.AssignBus(clickedBus);
                //         clickedBus.AssignSlot(newSlot);
                //         currentSlot.AssignBus(_selectedBus);
                //         //GameManager.Instance.TriggerCascadingMerge(currentSlot,out _);
                //         DeselectBus();
                //        
                //     }
                //
                //     return;
                // }

                DeselectBus();
                SelectBus(clickedBus);

                return;
            }

            if (clickedSlot != null)
            {
                if (clickedSlot.isLocked)
                {
                    clickedSlot.UnlockSlot();
                    DeselectBus();
                }

                //Remove When Enable Swaping Bus
                //else if (_selectedBus != null)
                //{
                //    TryMoveBusToSlot(clickedSlot);
                //}
                //return;

            }
        }

        DeselectBus();
    }


    
    void SelectBus(Bus bus)
    {
        _selectedBus = bus;
        if(_selectedBus.gameObject.GetComponent<Outline>()==null)
            _selectedBus.gameObject.AddComponent<Outline>();
        if (bus.AssignedSlot == null)
            TryMoveBusToSlot();
        if (TutorialManager.Instance)
        {
            if (TutorialManager.Instance.hand.activeInHierarchy)
            {
                TutorialManager.Instance.hand.SetActive(false);
            }
            //for Trash Item we check this 

        }
    }
    
    void TryMoveBusToSlot()
    { 
        GameManager.Instance.PlaceBusInSlot(_selectedBus);
    }

    void TryMoveBusToSlot(Slot clickedSlot)
    { 
        //GameManager.Instance.PlaceBusInSlot(_selectedBus, clickedSlot);
    }

    public void DeselectBus()
    {
        if (_selectedBus)
        {
            Destroy(_selectedBus.GetComponent<Outline>());
            _selectedBus = null;
        }
    }
    
    private bool IsPointerOverUI()
    {
        var mousePosition = _positionAction.ReadValue<Vector2>();
        var pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = mousePosition;
        var results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);
        return results.Count > 0;
    }
}
