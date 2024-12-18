using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
            if(TutorialManager.Instance.isInAnimation)
                return;
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
                        TutorialManager.Instance.hand.SetActive(true);
                        TutorialManager.Instance.HidePanel();
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
                case 8:
                {
                    TutorialManager.Instance.tutorialCase++;
                    TutorialManager.Instance.HidePanel();
                    TutorialManager.Instance.InitTrashItems();
                    break;
                }
                case 9:
                {
                    TutorialManager.Instance.HidePanel();
                    break;
                }
                default:
                    break;
            }
            
            //TutorialManager.Instance.hand.SetActive(false);//Z
        }
        if(!context.performed || IsPointerOverUI())
            return;

         
        Vector2 mousePosition = _positionAction.ReadValue<Vector2>();
        Ray ray = Camera.main!.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hit) && hit.collider != null)
        {
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
            if (clickedBus != null)
            {
                if (_selectedBus != null)
                {
                    if (_selectedBus.AssignedSlot != null && clickedBus.AssignedSlot != null)
                    {
                        var currentSlot = clickedBus.AssignedSlot;
                        var newSlot = _selectedBus.AssignedSlot;
                        _selectedBus.AssignSlot(currentSlot);
                        newSlot.AssignBus(clickedBus);
                        clickedBus.AssignSlot(newSlot);
                        currentSlot.AssignBus(_selectedBus);
                        GameManager.Instance.TriggerCascadingMerge(currentSlot,out _);
                        DeselectBus();
                    }

                    return;
                }

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
                else if (_selectedBus != null)
                {
                    TryMoveBusToSlot(clickedSlot);
                }

                return;
            }
        }

        DeselectBus();
    }


    
    void SelectBus(Bus bus)
    {
        _selectedBus = bus;
        _selectedBus.gameObject.AddComponent<Outline>();
        if (bus.AssignedSlot == null)
            TryMoveBusToSlot();
        if (TutorialManager.Instance)
        {
            if (TutorialManager.Instance.hand.activeInHierarchy)
            {
                TutorialManager.Instance.hand.SetActive(false);
                Debug.LogError("HandOFF");
            }
        }
    }
    
    void TryMoveBusToSlot()
    { 
        GameManager.Instance.PlaceBusInSlot(_selectedBus);
    }

    void TryMoveBusToSlot(Slot clickedSlot)
    { 
        GameManager.Instance.PlaceBusInSlot(_selectedBus, clickedSlot);
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
