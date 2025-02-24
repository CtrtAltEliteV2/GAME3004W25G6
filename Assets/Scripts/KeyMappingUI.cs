using UnityEngine;
using UnityEngine.UI;

public class KeyMappingUI : MonoBehaviour
{
    public Button moveForwardButton;
    public Button moveBackwardButton;
    public Button moveLeftButton;
    public Button moveRightButton;
    public Button jumpButton;
    public Button openInventoryButton;
    public Button pauseGameButton;

    private string currentAction;
    private Button currentButton;

    void Start()
    {
        if (moveForwardButton == null || moveBackwardButton == null || moveLeftButton == null || moveRightButton == null ||
            jumpButton == null || pauseGameButton == null || openInventoryButton == null)
        {
            Debug.LogError("One or more buttons are not assigned, check the Inspector");
            return;
        }

        Debug.Log("All buttons are assigned");

        UpdateButtonLabels();

        moveForwardButton.onClick.AddListener(() => StartKeyMapping("MoveForward", moveForwardButton));
        moveBackwardButton.onClick.AddListener(() => StartKeyMapping("MoveBackward", moveBackwardButton));
        moveLeftButton.onClick.AddListener(() => StartKeyMapping("MoveLeft", moveLeftButton));
        moveRightButton.onClick.AddListener(() => StartKeyMapping("MoveRight", moveRightButton));
        jumpButton.onClick.AddListener(() => StartKeyMapping("Jump", jumpButton));
        openInventoryButton.onClick.AddListener(() => StartKeyMapping("Inventory", openInventoryButton));
        pauseGameButton.onClick.AddListener(() => StartKeyMapping("PauseGame", pauseGameButton));
    }

    void UpdateButtonLabels()
    {
        moveForwardButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("MoveForward").ToString();
        moveBackwardButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("MoveBackward").ToString();
        moveLeftButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("MoveLeft").ToString();
        moveRightButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("MoveRight").ToString();
        jumpButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("Jump").ToString();
        openInventoryButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("Inventory").ToString();
        pauseGameButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("PauseGame").ToString();
    }

    void StartKeyMapping(string action, Button button)
    {
        currentAction = action;
        currentButton = button;
        Debug.Log("Started key mapping for: " + action);
        button.GetComponentInChildren<Text>().text = "Press any key...";
        StartCoroutine(WaitForKeyPress());
    }

    System.Collections.IEnumerator WaitForKeyPress()
    {
        Debug.Log("Waiting for key press...");
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                Debug.Log("Key pressed: " + keyCode);
                KeyMapperManager.Instance.SetKeyMapping(currentAction, keyCode);
                UpdateButtonLabels();
                break;
            }
        }
    }
}