using UnityEngine;
using UnityEngine.InputSystem;
//https://www.youtube.com/watch?v=NZBAr_V7r0M

public class PlayerInputController : MonoBehaviour
{
    public PlayerController playerController;
    private PlayerInput playerInput;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        //playerController.;
        playerInput.actions["SwitchMap"].performed += SwitchActionMap;
    }
    private void OnDisable()
    {
        playerInput.actions["SwitchMap"].performed -= SwitchActionMap;
    }
    private void SwitchActionMap(InputAction.CallbackContext context)
    {
        playerInput.actions.FindActionMap("Scanner").Enable();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
