using ArtlessUI.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArtlessUI.Ui
{
    public class AllGroupPanels : MonoBehaviour
    {
        private readonly PanelNavigation navigation = new PanelNavigation();
        private readonly Messenger messenger = new Messenger();

        private void Start()
        {
            var panels = GetChildrensComponents<PanelView>(transform);
            foreach (var panel in panels)
                panel.Initialize(navigation, messenger);

            var firstPanel = panels.FirstOrDefault(x => x.IsFirst);
            if (firstPanel == null)
                throw new Exception("Could not found first panel");

            navigation.InvokeMenu(firstPanel.Name);
        }

        private List<T> GetChildrensComponents<T>(Transform transform) where T : MonoBehaviour
        {
            var components = new List<T>();
            var childrens = GetChildrens(transform);
            foreach (var child in childrens)
                if (child.gameObject.TryGetComponent<T>(out T component))
                    components.Add(component);

            return components;
        }

        private List<GameObject> GetChildrens(Transform transform)
        {
            var childrens = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                childrens.Add(transform.GetChild(i).gameObject);
                childrens.AddRange(GetChildrens(transform.GetChild(i).transform));
            }
            return childrens;
        }
    }
}