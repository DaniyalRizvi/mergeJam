using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singelton<InputManager>
{
    private Bus _selectedBus; 
    private GameInputActions _inputActions;

    protected override void Awake()
    {
        _inputActions = new GameInputActions(); 
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
        Vector2 mousePosition = Mouse.current.position.ReadValue();
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
                        GameManager.Instance.CheckForMerging(currentSlot, out Bus finalBus);
                        GameManager.Instance.CheckForMerging(newSlot, out finalBus);
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
        if (_selectedBus != null && bus != _selectedBus)
        {
            if (bus.CanMergeWith(_selectedBus))
            {
                bus.MergeWith(_selectedBus);
            }
            return;
        }
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
}
