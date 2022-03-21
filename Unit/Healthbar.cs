using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField]
    private Image health;
    [SerializeField]
    private Image selected;

    private Vulnerable vul;

    void Start()
    {
        health = GetComponentInChildren<Image>();
        vul = GetComponentInParent<Vulnerable>();

        if (vul.IsPlayer)
        {
            selected.gameObject.SetActive(true);
            selected.GetComponent<Image>().color = new Color(0f, 250f, 0f, 150f);
        }
    }

    void OnGUI()
    {
        if (health == null || vul == null)
        {
            Destroy(gameObject);
            return;
        }

        health.fillAmount = vul.HealthAmount;
    }

    public void Highlight(bool state)
    {
        selected.gameObject.SetActive(state);
    }
}
