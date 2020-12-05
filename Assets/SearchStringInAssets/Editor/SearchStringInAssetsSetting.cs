using UnityEngine;

namespace info.shibuya24
{
    /// <summary>
    /// List management data to exclude from the search targets used in the SearchingInAssetsEditorWindow
    /// </summary>
    public class SearchStringInAssetsSetting : ScriptableObject
    {
        /// <summary>
        /// Search Exclusion Class Array
        /// </summary>
        public string[] ignoreTypes;

        public string[] searchPatterns;
    }
}