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
                Slot clickedSlot = hit.collider.GetComponent<Slot>();
                if (clickedBus != null)
                {
                    SelectBus(clickedBus);
                }
                else if (clickedSlot != null)
                {

                    if (_selectedBus != null)
                    {
                        TryMoveBusToSlot(clickedSlot);
                    }
                }
                else
                {
                    if (_selectedBus)
                    {
                        Destroy(_selectedBus.GetComponent<Outline>());
                        _selectedBus = null;
                    }
                }
            }
            else
            {
                if (_selectedBus)
                {
                    Destroy(_selectedBus.GetComponent<Outline>());
                    _selectedBus = null;
                }
            }
        }
        else
        {
            if (_selectedBus)
            {
                Destroy(_selectedBus.GetComponent<Outline>());
                _selectedBus = null;
            }
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
        _selectedBus.gameObject.AddComponent<Outline>();
    }

    void TryMoveBusToSlot(Slot clickedSlot)
    {
        
        bool placedInSlot = GameManager.Instance.PlaceBusInSlot(_selectedBus,clickedSlot);

        if (placedInSlot)
        {
            if (_selectedBus)
            {
                Destroy(_selectedBus.GetComponent<Outline>());
                _selectedBus = null;
            }
            Debug.Log("Bus successfully moved to a slot.");
        }
        else
        {
            Debug.Log("No available slots for this bus.");
        }

        
        _selectedBus = null;
    }
}
