using UnityEngine;
using UnityEngine.UI;

public class ButtonTransparencyController : MonoBehaviour
{
    [SerializeField] private Button currentButton; // 현재 반투명한 버튼
    private Image currentButtonImage;
    private Color originalColor = Color.white;

    private void Awake()
    {
        currentButtonImage = currentButton.GetComponent<Image>();
    }
    public void ToggleTransparency(Button newButton)
    {
        if (currentButton != null)
        {
            // 현재 버튼의 색상을 원래대로 복원
            currentButtonImage.color = originalColor;
        }

        // 새 버튼의 색상을 반투명하게 설정
        currentButton = newButton;
        currentButtonImage = newButton.GetComponent<Image>();
        //originalColor = Color.white;

        Color newColor = originalColor;
        newColor.a = 0.5f; // 알파 값을 50%로 설정
        currentButtonImage.color = newColor;
    }
}
