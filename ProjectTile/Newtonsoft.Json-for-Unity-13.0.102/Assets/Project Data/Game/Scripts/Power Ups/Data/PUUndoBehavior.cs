using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class PUUndoBehavior : PUBehavior
    {
        private PUUndoSettings customSettings;

        public override void Initialise()
        {
            customSettings = (PUUndoSettings)settings;
        }

        public override bool Activate()
        {
            var baseSettings = (PUCustomSettings)settings;
            if (LevelController.MaxReachedLevelIndex < baseSettings.MinRequiredLevel)
            {
                return false;
            }

            if (!LevelController.IsBusy)
            {
                IsBusy = true;

                RaycastController.Disable();

                LevelController.SetBusyState(true);

                return LevelController.ReturnTiles(customSettings.RevertElementsCount, () =>
                {
                    IsBusy = false;

                    RaycastController.Enable();

                    LevelController.SetBusyState(false);
                });
            }

            return false;
        }

        public override void ResetBehavior()
        {
            IsBusy = false;
        }
    }
}
