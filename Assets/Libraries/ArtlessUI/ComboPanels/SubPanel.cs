using ArtlessUI.Messaging;
using UnityEngine;

namespace ArtlessUI.Ui.SubPanels
{
    public abstract class SubPanel : MonoBehaviour
    {
        protected PanelNavigation navigation { get; private set; }
        protected Messenger messenger { get; private set; }

        public void Initialize(PanelNavigation navigation, Messenger messenger)
        {
            this.navigation = navigation;
            this.messenger = messenger;
            OnInitialization();
        }

        public virtual void OnActivation() { }
        public virtual void OnInactivation() { }
        public virtual void OnInitialization() { }
        public virtual void OnDeinitialization() { }
    }
}