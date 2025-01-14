using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class ThrowStrengthIndicator : MonoBehaviour
{
    // set strength bar from 0 to 1
    public void SetStrengthBar(double amount)
    {
        Debug.Assert(amount >= 0 && amount <= 1);
        int maxWidth = 192;
        int newWidth = (int)(amount * maxWidth);
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 size = rt.sizeDelta;
        size.x = newWidth;
        rt.sizeDelta = size;
    }

    public IEnumerator PrintNoFireMessage()
    {
        GameObject message = gameObject.transform.GetChild(0).gameObject;
        Debug.Log(message);
        TextMeshProUGUI tmp = message.GetComponent<TextMeshProUGUI>();
        Debug.Log(tmp);
        tmp.enabled = true;
        yield return new WaitForSeconds(2);
        tmp.enabled = false;
    }
}
