using UnityEngine;

namespace info.shibuya24
{
    /// <summary>
    /// List management data to exclude from the search targets used in the SearchingInAssetsEditorWindow
    /// </summary>
    [CreateAssetMenu(fileName = "SearchStringInAssetsSetting", menuName = "Shibuya24/SearchStringInAssetsSetting")]
    public class SearchStringInAssetsSetting : ScriptableObject
    {
        /// <summary>
        /// Search Exclusion Class Array
        /// </summary>
        public string[] ignoreTypes;

        /// <summary>
        /// Search Pattern Array
        /// </summary>
        public string[] searchPatterns;
    }
}