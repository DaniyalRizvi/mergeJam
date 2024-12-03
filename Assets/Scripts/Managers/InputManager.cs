using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
        if(!context.performed || IsPointerOverUI())
            return;
        Vector2 mousePosition = _positionAction.ReadValue<Vector2>();
        Ray ray = Camera.main!.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hit) && hit.collider != null)
        {
            Bus clickedBus = hit.collider.GetComponent<Bus>();
            Slot clickedSlot = hit.collider.GetComponent<Slot>();
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
