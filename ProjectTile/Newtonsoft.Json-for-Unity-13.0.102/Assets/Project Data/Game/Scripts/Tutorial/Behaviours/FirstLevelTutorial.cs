using I2.Loc;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Watermelon
{
    public class FirstLevelTutorial : BaseTutorial, ITutorial
    {
        private const int STEP_1_PICK_APPLES = 0;
        private const int STEP_2_PICK_CHEESE = 1;
        private const int STEP_3_DEPTH = 2;
        private const int STEP_4_PRE_HINT_DELAY = 3;
        private const int STEP_5_HINT = 4;
        private const int STEP_6_SHUFFLE = 5;
        private const int STEP_7_PICK_ELEMENT = 6;
        private const int STEP_8_UNDO = 7;
        private const int STEP_9_FINISH = 8;

        private static FirstLevelTutorial tutorialController;

        [SerializeField] BackgroundData backgroundData;
        [SerializeField] Color tileDisableColor;

        [Header("Step I")]
        [SerializeField] LevelData firstLevelData;
        [SerializeField] PreloadedLevelData firstPreloadedLevelData;
        string firstStepTitle
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return firstStepTitleRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return firstStepTitleEn;
                else
                    return firstStepTitleEn;
            }
        }
        string firstStepMessage
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return firstStepMessageRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return firstStepMessageEn;
                else
                    return firstStepMessageEn;
            }
        }
        [SerializeField] string firstStepTitleRu = "Welcome";
        [SerializeField] string firstStepMessageRu = "Clear the game field";

        [SerializeField] string firstStepTitleEn = "Welcome";
        [SerializeField] string firstStepMessageEn = "Clear the game field";

        
        string secondStepTitle
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return secondStepTitleRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return secondStepTitleEn;
                else
                    return secondStepTitleEn;
            }
        }
        string secondStepMessage
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return secondStepMessageRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return secondStepMessageEn;
                else
                    return secondStepMessageEn;
            }
        }
        [Header("Step II")]
        [SerializeField] string secondStepTitleRu = "Great";
        [SerializeField] string secondStepMessageRu = "Let's merge 3 other tiles";

        [SerializeField] string secondStepTitleEn = "Great";
        [SerializeField] string secondStepMessageEn = "Let's merge 3 other tiles";

        [Header("Step III")]
        [SerializeField] LevelData thirdLevelData;
        [SerializeField] PreloadedLevelData thirdPreloadedLevelData;
        string thirdStepTitle
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return thirdStepTitleRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return thirdStepTitleEn;
                else
                    return thirdStepTitleEn;
            }
        }
        string thirdStepMessage
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return thirdStepMessageRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return thirdStepMessageEn;
                else
                    return thirdStepMessageEn;
            }
        }

        [SerializeField] string thirdStepTitleRu = "Unlock tile";
        [SerializeField] string thirdStepMessageRu = "On the lower layer";

        [SerializeField] string thirdStepTitleEn = "Unlock tile";
        [SerializeField] string thirdStepMessageEn = "On the lower layer";

        
        string fourthStepTitle
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return fourthStepTitleRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return fourthStepTitleEn;
                else
                    return fourthStepTitleEn;
            }
        }
        string fourthStepMessage = "";
        [Header("Step IV")]
        [SerializeField] string fourthStepTitleRu = "Great";

        [SerializeField] string fourthStepTitleEn = "Great";

        
        string fifthStepTitle
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return fifthStepTitleRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return fifthStepTitleEn;
                else
                    return fifthStepTitleEn;
            }
        }
        string fifthStepMessage
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return fifthStepMessageRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return fifthStepMessageEn;
                else
                    return fifthStepMessageEn;
            }
        }
        [Header("Step V")]
        [SerializeField] string fifthStepTitleRu = "Use a hint";
        [SerializeField] string fifthStepMessageRu = "If you get stuck";

        [SerializeField] string fifthStepTitleEn = "Use a hint";
        [SerializeField] string fifthStepMessageEn = "If you get stuck";

        
        string sixthStepTitle
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return sixthStepTitleRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return sixthStepTitleEn;
                else
                    return sixthStepTitleEn;
            }
        }
        string sixthStepMessage
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return sixthStepMessageRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return sixthStepMessageEn;
                else
                    return sixthStepMessageEn;
            }
        }
        [Header("Step VI")]
        [SerializeField] string sixthStepTitleRu = "Shuffle";
        [SerializeField] string sixthStepMessageRu = "If it's impossible to make more moves";

        [SerializeField] string sixthStepTitleEn = "Shuffle";
        [SerializeField] string sixthStepMessageEn = "If it's impossible to make more moves";

        
        string seventhStepTitle
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return seventhStepTitleRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return seventhStepTitleEn;
                else
                    return seventhStepTitleEn;
            }
        }
        string seventhStepMessage
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return seventhStepMessageRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return seventhStepMessageEn;
                else
                    return seventhStepMessageEn;
            }
        }
        [Header("Step VII")]
        [SerializeField] string seventhStepTitleRu = "Great!";
        [SerializeField] string seventhStepMessageRu = "Let's finish the level!";

        [SerializeField] string seventhStepTitleEn = "Great!";
        [SerializeField] string seventhStepMessageEn = "Let's finish the level!";

        
        string eighthStepTitle
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return eighthStepTitleRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return eighthStepTitleEn;
                else
                    return eighthStepTitleEn;
            }
        }
        string eighthStepMessage
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return eighthStepMessageRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return eighthStepMessageEn;
                else
                    return eighthStepMessageEn;
            }
        }
        [Header("Step IIX")]
        [SerializeField] string eighthStepTitleRu = "Undo";
        [SerializeField] string eighthStepMessageRu = "If you did the wrong move";

        [SerializeField] string eighthStepTitleEn = "Undo";
        [SerializeField] string eighthStepMessageEn = "If you did the wrong move";

        
        string ninthStepTitle
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return ninthStepTitleRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return ninthStepTitleEn;
                else
                    return ninthStepTitleEn;
            }
        }
        string ninthStepMessage
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return ninthStepMessageRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return ninthStepMessageEn;
                else
                    return ninthStepMessageEn;
            }
        }
        [Header("Step IX")]
        [SerializeField] string ninthStepTitleRu = "Great!";
        [SerializeField] string ninthStepMessageRu = "Complete the level to continue";

        [SerializeField] string ninthStepTitleEn = "Great!";
        [SerializeField] string ninthStepMessageEn = "Complete the level to continue";

        
        string finishTitle
        {
            get
            {
                if (LocalizationManager.CurrentLanguage == "Russian")
                    return finishTitleRu;
                else if (LocalizationManager.CurrentLanguage == "English")
                    return finishTitleEn;
                else
                    return finishTitleEn;
            }
        }
        [Header("Finish")]
        [SerializeField] string finishTitleRu = "Good job!";

        [SerializeField] string finishTitleEn = "Good job!";

        private bool isActive;
        public override bool IsActive => isActive;

        private int progress;
        public override int Progress => progress;

        public override bool IsFinished => saveData.isFinished;

        private TutorialBaseSave saveData;

        private UIGame gameUI;

        private List<TileBehavior> cheeseTiles;
        private List<TileBehavior> appleTiles;

        private TileBehavior pointerTile;
        private TileBehavior pyramidTile;
        private TileBehavior undoTile;
        private List<TileBehavior> undoClickableTiles;

        public override void Initialise()
        {
            tutorialController = this;

            saveData = SaverManagerMy.GetSaveObject<TutorialBaseSave>(string.Format(ITutorial.SAVE_IDENTIFIER, TutorialID.ToString()));

            gameUI = UIController.GetPage<UIGame>();
        }

        public override void StartTutorial()
        {
            if (isActive) return;

            isActive = true;
            progress = 0;

            EnableStep(0);

            DockBehavior.MatchCombined += OnMatchCombined;
            DockBehavior.ElementAdded += OnElementAddedToDock;
            PUController.OnPowerUpUsed += OnPUUsed;

            AdsManager.DisableBanner();
        }

        private void OnPUUsed(PUType powerUpType)
        {
            if(progress == STEP_5_HINT)
            {
                EnableStep(STEP_6_SHUFFLE);
            }
            else if (progress == STEP_6_SHUFFLE)
            {
                EnableStep(STEP_7_PICK_ELEMENT);
            }
            else if(progress == STEP_8_UNDO)
            {
                EnableStep(STEP_9_FINISH);
            }
        }

        private void EnableStep(int stepIndex)
        {
            if (stepIndex == STEP_1_PICK_APPLES)
            {
                gameUI.SetTutorialText(firstStepTitle, firstStepMessage);

                GameController.LoadCustomLevel(tutorialController.firstLevelData, tutorialController.firstPreloadedLevelData, tutorialController.backgroundData, true, () =>
                {
                    // Get cheese tiles
                    cheeseTiles = new List<TileBehavior>();
                    cheeseTiles.Add(LevelController.GetTile(new ElementPosition(0, 0, 1)));
                    cheeseTiles.Add(LevelController.GetTile(new ElementPosition(1, 0, 1)));
                    cheeseTiles.Add(LevelController.GetTile(new ElementPosition(2, 0, 1)));

                    foreach (var cheese in cheeseTiles)
                    {
                        cheese.SetBlockState(true);
                        cheese.SetColor(tileDisableColor, true);
                    }

                    // Get apple tiles
                    appleTiles = new List<TileBehavior>();
                    appleTiles.Add(LevelController.GetTile(new ElementPosition(0, 1, 1)));
                    appleTiles.Add(LevelController.GetTile(new ElementPosition(1, 1, 1)));
                    appleTiles.Add(LevelController.GetTile(new ElementPosition(2, 1, 1)));

                    foreach (var apple in appleTiles)
                    {
                        apple.SetBlockState(false);
                    }

                    ActivateTilePointer(appleTiles[0]);
                });
            }
            else if (stepIndex == STEP_2_PICK_CHEESE)
            {
                gameUI.SetTutorialText(secondStepTitle, secondStepMessage);

                foreach (var cheese in cheeseTiles)
                {
                    cheese.SetBlockState(false);
                    cheese.SetState(true, true);
                }

                ActivateTilePointer(cheeseTiles[0]);
            }
            else if (stepIndex == STEP_3_DEPTH)
            {
                gameUI.SetTutorialText(thirdStepTitle, thirdStepMessage);

                GameController.LoadCustomLevel(tutorialController.thirdLevelData, tutorialController.thirdPreloadedLevelData, tutorialController.backgroundData, false, () =>
                {
                    pyramidTile = LevelController.GetTile(new ElementPosition(1, 1, 0));

                    ActivateTilePointer(pyramidTile);

                    TileBehavior dockTile = LevelController.SpawnDockTile(0);

                    Vector3 tileSize = dockTile.transform.localScale;
                    dockTile.transform.localScale = Vector3.zero;
                    dockTile.transform.DOScale(tileSize, 0.5f).SetEasing(Ease.Type.BackOut);
                });
            }
            else if (stepIndex == STEP_4_PRE_HINT_DELAY)
            {
                gameUI.SetTutorialText(fourthStepTitle, fourthStepMessage);

                RaycastController.Disable();

                Tween.DelayedCall(0.6f, () =>
                {
                    EnableStep(STEP_5_HINT);
                });
            }
            else if (stepIndex == STEP_5_HINT)
            {
                gameUI.SetTutorialText(fifthStepTitle, fifthStepMessage);

                PUUIBehavior hintPanel = PUController.PowerUpsUIController.GetPanel(PUType.Hint);
                hintPanel.gameObject.SetActive(true);
                hintPanel.Settings.Save.Amount = 1;
                hintPanel.Redraw();

                Tween.NextFrame(() =>
                {
                    TutorialCanvasController.ActivatePointer(hintPanel.transform.position, TutorialCanvasController.POINTER_DEFAULT);
                });
            }
            else if (stepIndex == STEP_6_SHUFFLE)
            {
                TutorialCanvasController.ResetPointer();

                gameUI.SetTutorialText(sixthStepTitle, sixthStepMessage);

                PUController.PowerUpsUIController.HidePanel(PUType.Hint);

                PUUIBehavior shufflePanel = PUController.PowerUpsUIController.GetPanel(PUType.Shuffle);
                shufflePanel.gameObject.SetActive(true);
                shufflePanel.Settings.Save.Amount = 1;
                shufflePanel.Redraw();

                Tween.NextFrame(() =>
                {
                    TutorialCanvasController.ActivatePointer(shufflePanel.transform.position, TutorialCanvasController.POINTER_DEFAULT);
                }, 2);
            }
            else if (stepIndex == STEP_7_PICK_ELEMENT)
            {
                gameUI.SetTutorialText(seventhStepTitle, seventhStepMessage);

                TutorialCanvasController.ResetPointer();

                undoClickableTiles = new List<TileBehavior>(LevelController.LevelRepresentation.Tiles);
                for(int i = 0; i < undoClickableTiles.Count; i++)
                {
                    if (undoTile == null && undoClickableTiles[i].IsClickable)
                    {
                        undoTile = undoClickableTiles[i];

                        undoClickableTiles.RemoveAt(i);

                        break;
                    }
                }

                foreach (TileBehavior tile in undoClickableTiles)
                {
                    tile.SetBlockState(true);
                    tile.SetColor(tileDisableColor, true);
                }

                Tween.DelayedCall(0.3f, () =>
                {
                    ActivateTilePointer(undoTile);
                });

                PUController.PowerUpsUIController.HidePanel(PUType.Shuffle);
            }
            else if (stepIndex == STEP_8_UNDO)
            {
                gameUI.SetTutorialText(eighthStepTitle, eighthStepMessage);

                PUUIBehavior undoPanel = PUController.PowerUpsUIController.GetPanel(PUType.Undo);
                undoPanel.gameObject.SetActive(true);
                undoPanel.Settings.Save.Amount = 1;
                undoPanel.Redraw();

                Tween.NextFrame(() =>
                {
                    TutorialCanvasController.ActivatePointer(undoPanel.transform.position, TutorialCanvasController.POINTER_DEFAULT);
                });
            }
            else if (stepIndex == STEP_9_FINISH)
            {
                TutorialCanvasController.ResetPointer();

                foreach (TileBehavior tile in undoClickableTiles)
                {
                    tile.SetBlockState(false);
                    tile.SetState(LevelController.LevelRepresentation.IsTileUnconcealed(tile), true);
                }

                gameUI.SetTutorialText(ninthStepTitle, ninthStepMessage);

                PUController.PowerUpsUIController.HidePanel(PUType.Undo);
            }

            progress = stepIndex;
        }

        private void ActivateTilePointer(TileBehavior tileBehavior)
        {
            if(tileBehavior != null)
            {
                TutorialCanvasController.ActivatePointer(tileBehavior.transform.position, TutorialCanvasController.POINTER_DEFAULT);

                pointerTile = tileBehavior;
            }
        }

        private void DisableTilePointer()
        {
            TutorialCanvasController.ResetPointer();

            pointerTile = null;
        }

        private void OnElementAddedToDock(ISlotable tile)
        {
            TileBehavior pickedTile = (TileBehavior)tile;
            if(pickedTile != null)
            {
                if (pickedTile == pointerTile)
                    DisableTilePointer();

                if(progress == STEP_1_PICK_APPLES)
                {
                    appleTiles.Remove(pickedTile);

                    if(appleTiles.Count > 0)
                    {
                        ActivateTilePointer(appleTiles[0]);
                    }
                    else
                    {
                        EnableStep(STEP_2_PICK_CHEESE);
                    }
                }
                else if(progress == STEP_2_PICK_CHEESE)
                {
                    cheeseTiles.Remove(pickedTile);

                    if (cheeseTiles.Count > 0)
                    {
                        ActivateTilePointer(cheeseTiles[0]);
                    }
                }
                else if(progress == STEP_3_DEPTH)
                {
                    if(pickedTile == pyramidTile)
                    {
                        EnableStep(STEP_4_PRE_HINT_DELAY);
                    }
                }
                else if (progress == STEP_7_PICK_ELEMENT)
                {
                    if (pickedTile == undoTile)
                    {
                        EnableStep(STEP_8_UNDO);
                    }
                }
            }
        }

        private void OnMatchCombined(List<ISlotable> tiles)
        {
            if (progress == STEP_2_PICK_CHEESE)
            {
                if (cheeseTiles.IsNullOrEmpty())
                {
                    EnableStep(STEP_3_DEPTH);
                }
            }
            else if(progress == STEP_9_FINISH)
            {
                if (LevelController.LevelRepresentation.Tiles.IsNullOrEmpty())
                {
                    gameUI.SetTutorialText(finishTitle, "");

                    Tween.DelayedCall(1.0f, () =>
                    {
                        CompleteTutorial();
                    });
                }
            }
        }

        public void CompleteTutorial()
        {
            FinishTutorial();

            gameUI.DisableTutorial();

            DockBehavior.MatchCombined -= OnMatchCombined;
            DockBehavior.ElementAdded -= OnElementAddedToDock;
            PUController.OnPowerUpUsed -= OnPUUsed;

            AdsManager.EnableBanner();

            LevelController.CompleteCustomLevel();

            GameController.LoadLevel(0, () =>
            {
                gameUI.PowerUpsUIController.ShowPanels();
            });
        }

        public override void FinishTutorial()
        {
            TutorialCanvasController.ResetPointer();

            PUBehavior[] powerUps = PUController.ActivePowerUps;
            foreach(var powerUp in powerUps)
            {
                powerUp.Settings.Save.Amount = powerUp.Settings.DefaultAmount;
            }

            saveData.isFinished = true;

            isActive = false;
        }

        public override void Unload()
        {

        }

        public void OnSkipButtonClicked()
        {
            if(isActive && !saveData.isFinished)
            {
                CompleteTutorial();
            }
        }
    }
}
