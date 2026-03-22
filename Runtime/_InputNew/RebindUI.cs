using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class RebindUI : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference action;
    public int bindingIndex;
    public string bindingGroup; // "Keyboard&Mouse" / "Gamepad"

    [Header("UI")]
    public TextMeshProUGUI label;
    public TextMeshProUGUI status;
    public GameObject overlay;
    public Image icon;
    public Sprite keyboardIcon;
    public Sprite gamepadIcon;

    [Header("Optional")]
    public PlayerInput playerInput;
    public string gameplayMap = "Player";
    public string uiMap = "UI";

    private InputActionRebindingExtensions.RebindingOperation rebindingOp;

    // 🔹 Start
    public void StartRebind()
    {
        overlay.SetActive(true);
        status.text = "Press any key... (ESC to cancel)";

        if (playerInput != null)
            playerInput.SwitchCurrentActionMap(uiMap);

        action.action.Disable();

        rebindingOp = action.action.PerformInteractiveRebinding(bindingIndex)
            .WithBindingGroup(bindingGroup)
            .WithCancelingThrough("<Keyboard>/escape")
            .WithControlsExcluding("Mouse/position")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                if (IsDuplicate(op.selectedControl.path))
                {
                    action.action.RemoveBindingOverride(bindingIndex);
                    status.text = "Duplicate!";
                }
                else
                {
                    status.text = "";
                }

                Finish();
            })
            .OnCancel(op =>
            {
                status.text = "Canceled";
                Finish();
            });

        rebindingOp.Start();

        StartCoroutine(Timeout(5f));
    }

    // 🔹 Timeout
    IEnumerator Timeout(float t)
    {
        yield return new WaitForSeconds(t);

        if (rebindingOp != null)
            rebindingOp.Cancel();
    }

    // 🔹 Finish
    void Finish()
    {
        overlay.SetActive(false);

        rebindingOp?.Dispose();
        rebindingOp = null;

        action.action.Enable();

        if (playerInput != null)
            playerInput.SwitchCurrentActionMap(gameplayMap);

        UpdateLabel();
        UpdateIcon();
    }

    // 🔹 UI update
    public void UpdateLabel()
    {
        label.text = action.action.GetBindingDisplayString(bindingIndex);
    }

    void UpdateIcon()
    {
        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
            icon.sprite = gamepadIcon;
        else
            icon.sprite = keyboardIcon;
    }

    // 🔹 Duplicate check
    bool IsDuplicate(string newPath)
    {
        foreach (var map in action.action.actionMap.actions)
        {
            foreach (var b in map.bindings)
            {
                if (b.effectivePath == newPath)
                    return true;
            }
        }
        return false;
    }



    /*
    // Save
    PlayerPrefs.SetString("rebinds",
    playerInput.actions.SaveBindingOverridesAsJson());

    // Load (la start)
    playerInput.actions.LoadBindingOverridesFromJson(
    PlayerPrefs.GetString("rebinds", ""));
    */
}