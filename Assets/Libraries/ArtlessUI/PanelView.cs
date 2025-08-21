using ArtlessUI.Messaging;
using UnityEngine;

namespace ArtlessUI.Ui
{
    public abstract class PanelView : MonoBehaviour
    {
        public string Name { get { return _name; }}
        public bool IsFirst { get { return _isFirst; }}

        [Header("Panel options")]
        [SerializeField] private string _name;
        [SerializeField] private bool _isFirst;

        protected PanelNavigation navigation { get; private set; }
        protected Messenger messenger { get; private set; }

        private void OnEnable()
        {
            //if (navigation != null && messenger != null)

        }

        private void OnDisable()
        {
            //if (navigation != null && messenger != null)

        }

        private void OnDestroy()
        {
            this.navigation.Unregister(this);
            OnDeinitialization();
        }

        public void SetVisible(bool value)
        {
            if (value)
                Show();
            else
                Hide();
        }

        public void Initialize(PanelNavigation navigation, Messenger messenger)
        {
            this.navigation = navigation;
            this.messenger = messenger;
            this.navigation.Register(this);
            OnInitialization();
        }

        public virtual void Activate()
        {
            Show();
            OnActivation();
        }

        public virtual void Deactivate()
        {
            Hide();
            OnInactivation();
        }

        protected virtual void OnActivation() { }
        protected virtual void OnInactivation() { }
        protected virtual void OnInitialization() { }
        protected virtual void OnDeinitialization() { }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}