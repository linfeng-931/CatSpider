using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
//https://www.youtube.com/watch?v=NZBAr_V7r0M

public class PlayerInputController : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject Scanner;
    public GameObject Menu;

    private PlayerInput playerInput;
    private string rePlayerInput;
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

    public void SwitchToScanner(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Scanner.SetActive(true);
            playerInput.SwitchCurrentActionMap("Scanner");
        }
    }
    public void SwitchToPlayer(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Scanner.SetActive(false);
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public void OpenMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Menu.SetActive(true);
            Time.timeScale = 0;
            Menu.GetComponent<Menu>().ShowDetail();
            rePlayerInput = playerInput.currentActionMap.name;
            playerInput.SwitchCurrentActionMap("UI");
        }
    }
    public void CloseMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Menu.SetActive(false);
            Time.timeScale = 1;
            playerInput.SwitchCurrentActionMap(rePlayerInput);
        }
    }
    public void CloseMenu()
    {
        Menu.SetActive(false);
        Time.timeScale = 1;
        playerInput.SwitchCurrentActionMap(rePlayerInput);
    }
}
