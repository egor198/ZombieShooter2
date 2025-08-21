namespace ArtlessUI.Ui.SubPanels
{
    public class ComboPanel : PanelView
    {
        private SubPanel[] subPanels;
        
        protected override void OnInitialization()
        {
            subPanels = GetComponents<SubPanel>();
            foreach (var subPanel in subPanels)
                subPanel.Initialize(navigation, messenger);
        }

        protected override void OnActivation() 
        {
            foreach (var subPanel in subPanels)
                subPanel.OnActivation();
        }

        protected override void OnInactivation() 
        {
            foreach (var subPanel in subPanels)
                subPanel.OnInactivation();
        }

        protected override void OnDeinitialization() 
        {
            foreach (var subPanel in subPanels)
                subPanel.OnDeinitialization();
        }
    }
}