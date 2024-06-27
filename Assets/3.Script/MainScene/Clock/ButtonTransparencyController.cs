using UnityEngine;
using UnityEngine.UI;

public class ButtonTransparencyController : MonoBehaviour
{
    [SerializeField] private Button currentButton; // ���� �������� ��ư
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
            // ���� ��ư�� ������ ������� ����
            currentButtonImage.color = originalColor;
        }

        // �� ��ư�� ������ �������ϰ� ����
        currentButton = newButton;
        currentButtonImage = newButton.GetComponent<Image>();
        //originalColor = Color.white;

        Color newColor = originalColor;
        newColor.a = 0.5f; // ���� ���� 50%�� ����
        currentButtonImage.color = newColor;
    }
}
