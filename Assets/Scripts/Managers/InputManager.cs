using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Bus _selectedBus; 
    private GameInputActions _inputActions; 

    void Awake()
    {
        _inputActions = new GameInputActions(); 
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
        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.collider != null)
            {
                Bus clickedBus = hit.collider.GetComponent<Bus>();
                if (clickedBus != null)
                {
                    SelectBus(clickedBus);
                }
                Slot clickedSlot = hit.collider.GetComponent<Slot>();
                if (clickedSlot != null)
                {

                    if (_selectedBus != null)
                    {
                        TryMoveBusToSlot(clickedSlot);
                    }
                }
            }
            else
            {
                _selectedBus = null;
            }
        }
        else
        {
            _selectedBus = null;
        }
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
    }

    void TryMoveBusToSlot(Slot clickedSlot)
    {
        
        bool placedInSlot = GameManager.Instance.PlaceBusInSlot(_selectedBus,clickedSlot);

        if (placedInSlot)
        {
            _selectedBus = null;
            Debug.Log("Bus successfully moved to a slot.");
        }
        else
        {
            Debug.Log("No available slots for this bus.");
        }

        
        _selectedBus = null;
    }
}
