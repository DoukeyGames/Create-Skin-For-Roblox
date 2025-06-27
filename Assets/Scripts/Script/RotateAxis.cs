using PaintIn3D;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateAxis: MonoBehaviour, IDragHandler
{
    public Transform Target;
    public CwPaintDecal TargetDecal;
    public float rotationSpeed = 5f; // Speed of rotation
   
    public float minScale = 0.5f; // Speed of rotation
    public float maxScale = 1f; // Speed of rotation

    private bool isDragging = false;

    public void OnPointerDown()
    {
        isDragging = true;
    }

    public void OnPointerUp()
    {
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            RotateObject(eventData);
            ScaleObject(eventData);
        }
    }

    private void RotateObject(PointerEventData eventData)
    {
        // Get horizontal delta movement
        float deltaX = eventData.delta.x;

        // Apply rotation based on movement
        float rotationAmount = deltaX * rotationSpeed * Time.deltaTime;
        Target.transform.Rotate(0f, 0f, rotationAmount);
    }
    private void ScaleObject(PointerEventData eventData)
    {
        // Get vertical delta movement
        float deltaY = eventData.delta.y;
    
        // Calculate scale change based on movement
        float scaleChange = -deltaY * 0.01f; // Adjust scaling sensitivity as needed
    
        // Get current scale and apply change
        Vector3 currentScale = Target.transform.localScale;
        Vector3 newScale = currentScale + new Vector3(scaleChange, scaleChange, 1f);
    
        // Clamp the scale to prevent it from becoming too small or too large
        newScale = new Vector3(
            Mathf.Clamp(newScale.x, minScale, maxScale), // X-axis scaling
            Mathf.Clamp(newScale.y, minScale, maxScale), // Y-axis scaling
            1f // Z-axis scaling
        );
    
        // Apply the new scale
        Target.transform.localScale = newScale;
        TargetDecal.scale = new Vector3(newScale.x,newScale.y,5f);
    }

}
