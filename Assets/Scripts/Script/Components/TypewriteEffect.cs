using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float typingSpeed = 0.05f;

    public string fullText;
    private Coroutine typingCoroutine;

    public UnityEvent OnCompleted;

    public void StartTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        textComponent.text = "";
        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
        OnCompleted?.Invoke();
    }
}