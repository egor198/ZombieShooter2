using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidKitController : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private Renderer renderer;
    [SerializeField] private Color highlightColor = Color.red * 0.5f;
    [SerializeField] private bool isTNT;

    private Material[] originalMaterials;
    private Material[] highlightedMaterials;
    private bool isHighlighted = false;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        // Сохраняем оригинальные материалы
        originalMaterials = renderer.materials;

        health = Random.Range(50, 300);

        // Создаем материалы для подсветки
        highlightedMaterials = new Material[originalMaterials.Length];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            highlightedMaterials[i] = new Material(originalMaterials[i]);
            highlightedMaterials[i].EnableKeyword("_EMISSION");
            highlightedMaterials[i].SetColor("_EmissionColor", highlightColor);
        }
    }

    public void AddHp()
    {
        if (!isTNT)
        {
            Destroy(gameObject);
            if(GameManager.currentHP + health >= gameManager.maxHp)
            {
                GameManager.currentHP = gameManager.maxHp;
            }
            else
            {
                GameManager.currentHP += health;
            }
        }
    }

    public void Highlight(bool enable)
    {
        if (renderer == null || isHighlighted == enable)
            return;

        isHighlighted = enable;
        renderer.materials = enable ? highlightedMaterials : originalMaterials;
    }
}
