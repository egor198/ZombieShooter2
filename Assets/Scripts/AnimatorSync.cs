using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSync : MonoBehaviour
{
    [SerializeField] private Animator sourceAnimator;
    private Animator targetAnimator;

    private List<string> boolParamNames = new List<string>();
    private List<string> triggerNames = new List<string>();
    private Dictionary<string, bool> previousTriggerStates = new Dictionary<string, bool>();

    void Start()
    {
        targetAnimator = GetComponent<Animator>();

        if (sourceAnimator == null || targetAnimator == null)
        {
            Debug.LogError("Animators not assigned!");
            enabled = false;
            return;
        }

        // Собираем имена параметров один раз при старте
        foreach (AnimatorControllerParameter param in sourceAnimator.parameters)
        {
            if (targetAnimator.HasParameter(param.name, param.type))
            {
                if (param.type == AnimatorControllerParameterType.Bool)
                {
                    boolParamNames.Add(param.name);
                }
                else if (param.type == AnimatorControllerParameterType.Trigger)
                {
                    triggerNames.Add(param.name);
                    previousTriggerStates[param.name] = false;
                }
            }
        }
    }

    void Update()
    {
        // Синхронизация булевых параметров
        for (int i = 0; i < boolParamNames.Count; i++)
        {
            string paramName = boolParamNames[i];
            bool value = sourceAnimator.GetBool(paramName);
            targetAnimator.SetBool(paramName, value);
        }

        // Синхронизация триггеров
        for (int i = 0; i < triggerNames.Count; i++)
        {
            string triggerName = triggerNames[i];
            bool currentState = sourceAnimator.GetBool(triggerName);
            bool previousState = previousTriggerStates[triggerName];

            if (currentState && !previousState)
            {
                targetAnimator.SetTrigger(triggerName);
            }

            previousTriggerStates[triggerName] = currentState;
        }
    }
}

public static class AnimatorExtensions
{
    public static bool HasParameter(this Animator animator, string paramName, AnimatorControllerParameterType type)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName && param.type == type)
                return true;
        }
        return false;
    }
}