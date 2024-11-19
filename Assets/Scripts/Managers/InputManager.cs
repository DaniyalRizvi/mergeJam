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
        if (Physics.Raycast(ray, out var hit) && hit.collider != null)
        {
            Bus clickedBus = hit.collider.GetComponent<Bus>();
            Slot clickedSlot = hit.collider.GetComponent<Slot>();
            if (clickedBus != null)
            {
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
    }

    void TryMoveBusToSlot(Slot clickedSlot)
    {
        bool placedInSlot = GameManager.Instance.PlaceBusInSlot(_selectedBus, clickedSlot);

        if (placedInSlot)
        {
            DeselectBus();
            Debug.Log("Bus successfully moved to a slot.");
        }
        else
        {
            Debug.Log("No available slots for this bus.");
        }
    }

    void DeselectBus()
    {
        if (_selectedBus)
        {
            Destroy(_selectedBus.GetComponent<Outline>());
            _selectedBus = null;
        }
    }
}
