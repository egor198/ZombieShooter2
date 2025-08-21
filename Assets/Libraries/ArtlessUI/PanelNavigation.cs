using System.Collections.Generic;
using System.Linq;

namespace ArtlessUI.Ui
{
    public class PanelNavigation
    {
        private Dictionary<string, PanelView> panels = new Dictionary<string, PanelView>();
        private Stack<PanelView> history = new Stack<PanelView>();
        private PanelView currentPanel;
        private PanelView CurrentPanel
        {
            get { return currentPanel; }
            set
            {
                if (currentPanel == value)
                    return;

                if (currentPanel != null && value != null)
                    currentPanel.Deactivate();

                currentPanel = value;
                if (currentPanel != null)
                    currentPanel.Activate();
            }
        }

        public void Register(PanelView menu)
        {
            if (panels.ContainsKey(menu.Name))
                return;

            panels.Add(menu.Name, menu);
            menu.SetVisible(false);
        }

        public void InvokeMenu(Router router)
        {
            InvokeMenu(router.PageName, router.ClearHistory);
        }

        public void InvokeMenu(string name, bool clearHistory = false)
        {
            if (CurrentPanel != null)
                history.Push(CurrentPanel);

            if (clearHistory)
                history.Clear();

            CurrentPanel = panels[name];
        }

        public void HideCurrentMenu()
        {
            CurrentPanel = null;
        }

        public void ReturnMenu()
        {
            if (history.Count > 0)
            {
                CurrentPanel = history.Pop();
            }
            else
            {
                var firstPanel = panels.Values.FirstOrDefault(x => x.IsFirst);
                if (firstPanel == null)
                    firstPanel = panels.Values.First();

                CurrentPanel = firstPanel;
            }
        }
        
        public void Unregister(PanelView menu)
        {
            if (!panels.ContainsKey(menu.Name))
                return;

            panels.Remove(menu.Name);
            
            if (CurrentPanel == menu)
                CurrentPanel = null;
        }
    }
}