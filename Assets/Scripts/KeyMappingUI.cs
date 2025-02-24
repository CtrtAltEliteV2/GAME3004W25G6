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
    public Button useItemButton;
    public Button pauseGameButton;

    private string currentAction;

    void Start()
    {
        if (moveForwardButton == null || moveBackwardButton == null || moveLeftButton == null || moveRightButton == null ||
            jumpButton == null || useItemButton == null || pauseGameButton == null || openInventoryButton == null)
        {
            Debug.LogError("One or more buttons are not assigned, check the Inspector");
            return;
        }

        Debug.Log("All buttons are assigned");

        UpdateButtonLabels();

        moveForwardButton.onClick.AddListener(() => StartKeyMapping("MoveForward"));
        moveBackwardButton.onClick.AddListener(() => StartKeyMapping("MoveBackward"));
        moveLeftButton.onClick.AddListener(() => StartKeyMapping("MoveLeft"));
        moveRightButton.onClick.AddListener(() => StartKeyMapping("MoveRight"));
        jumpButton.onClick.AddListener(() => StartKeyMapping("Jump"));
        useItemButton.onClick.AddListener(() => StartKeyMapping("UseItem"));
        openInventoryButton.onClick.AddListener(() => StartKeyMapping("Inventory"));
        pauseGameButton.onClick.AddListener(() => StartKeyMapping("PauseGame"));
    }

    void UpdateButtonLabels()
    {
        moveForwardButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("MoveForward").ToString();
        moveBackwardButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("MoveBackward").ToString();
        moveLeftButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("MoveLeft").ToString();
        moveRightButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("MoveRight").ToString();
        jumpButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("Jump").ToString();
        useItemButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("UseItem").ToString();
        openInventoryButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("Inventory").ToString();
        pauseGameButton.GetComponentInChildren<Text>().text = KeyMapperManager.Instance.GetKeyMapping("PauseGame").ToString();
    }

    void StartKeyMapping(string action)
    {
        currentAction = action;
        Debug.Log("Started key mapping for: " + action);
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