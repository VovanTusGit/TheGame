using UnityEngine;

public class KrutilkaSystem : MonoBehaviour
{
    [Header("Настройки")]
    public float rotationSpeed = 5f;
    public float minAngle = -360f;
    public float maxAngle = 360f;
    public float minFrequency = 87.5f;
    public float maxFrequency = 108.0f;
    
    private bool isDragging = false;
    private Vector2 initialMousePosition;
    private float accumulatedRotation; // Общий угол поворота (может быть за пределами 0-360)
    private float currentFrequency;

    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                initialMousePosition = GetMousePositionRelativeToKnob();
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 currentMousePosition = GetMousePositionRelativeToKnob();
            
            // Вычисляем разницу в угле мыши
            float deltaAngle = Vector2.SignedAngle(initialMousePosition, currentMousePosition);
            
            // Добавляем к общему повороту
            accumulatedRotation += deltaAngle * rotationSpeed;

            // Ограничиваем общий поворот
            accumulatedRotation = Mathf.Clamp(accumulatedRotation, minAngle, maxAngle);
            
            // Применяем вращение (Unity понимает, как отобразить большие углы)
            transform.localEulerAngles = new Vector3(accumulatedRotation, -90f, -90f);
            
            UpdateFrequency(accumulatedRotation);

            // Обновляем начальную позицию, чтобы избежать "рывков"
            initialMousePosition = currentMousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    Vector2 GetMousePositionRelativeToKnob()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 mousePos = Input.mousePosition;
        
        return (mousePos - new Vector2(screenPos.x, screenPos.y)).normalized;
    }

    void UpdateFrequency(float rotation)
    {
        float t = (rotation - minAngle) / (maxAngle - minAngle);
        currentFrequency = Mathf.Lerp(minFrequency, maxFrequency, t);
        
        Debug.Log($"Частота: {currentFrequency:F1} MHz");
    }
}
