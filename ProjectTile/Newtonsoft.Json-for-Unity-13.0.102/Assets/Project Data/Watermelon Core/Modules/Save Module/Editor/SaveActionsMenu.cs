using UnityEngine;
using Watermelon;
using UnityEditor;
using YG;

namespace Watermelon
{
    public static class SaveActionsMenu
    {
        [MenuItem("Actions/Remove Save", priority = 1)]
        private static void RemoveSave()
        {
            SavesYG.DeleteAll();
            SaveController.DeleteSaveFile();
        }

        [MenuItem("Actions/Remove Save", true)]
        private static bool RemoveSaveValidation()
        {
            return !Application.isPlaying;
        }
    }
}