#if UNITY_EDITOR_OSX
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

namespace info.shibuya24
{
    /// <summary>
    /// Editor extension to search for embedded strings in a Prefab scene
    /// </summary>
    public class SearchStringInAssetsEditorWindow : EditorWindow
    {
        [MenuItem("Tools/Shibuya24/SearchStringInAssets")]
        static void Init()
        {
            var w = GetWindow<SearchStringInAssetsEditorWindow>();
            w.titleContent = new GUIContent("SearchStringInAssets");
            w.Show();
        }

        private const string Title = "SearchStringInAssets";
        private const string AssetsKeyWord = "Assets";
        private const string MetaExtension = ".meta";
        private const string SaveKey = "info.shibuya24.SearchStringInAssets";
        private static readonly int MetaExtensionLength = MetaExtension.Length;

        /// <summary>
        /// Search string variables
        /// </summary>
        private string _searchString = "";

        private Vector2 _scrollPos;

        private SearchStringInAssetsSetting _searchStringInAssetsSetting;

        /// <summary>
        /// A list containing the search results
        /// </summary>
        private readonly HashSet<Object> _searchResultObjectHashSet = new HashSet<Object>();

        private static int _dataPathLength;

        private bool _isIgnoreSearchPattern;

        private void OnEnable()
        {
            // Load Saved Setting
            if (_searchStringInAssetsSetting != null) return;
            var guid = EditorPrefs.GetString(SaveKey, null);
            if (string.IsNullOrEmpty(guid) == false)
            {
#if ENABLE_SEARCH_STRING_IN_ASSETS_LOG
                Debug.Log($"Load Settings GUID: {guid}");
#endif
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<SearchStringInAssetsSetting>(path);
                if (obj != null)
                {
                    _searchStringInAssetsSetting = obj;
                }
            }
        }

        void OnGUI()
        {
            if (_dataPathLength <= 0)
            {
                SetDataPathLength();
            }

            GUI.skin.label.fontSize = 24;
            GUILayout.Label(Title, GUILayout.Height(40f));
            GUI.skin.label.fontSize = 0;

            _searchString = EditorGUILayout.TextField("Search keywords", _searchString);
            if (string.IsNullOrEmpty(_searchString) == false && ValidateInputText(_searchString) == false)
            {
                _searchString = "";
            }

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            {
                _searchStringInAssetsSetting =
                    (SearchStringInAssetsSetting) EditorGUILayout.ObjectField("Search Setting",
                        _searchStringInAssetsSetting, typeof(SearchStringInAssetsSetting), false);

                EditorGUI.BeginDisabledGroup(_searchStringInAssetsSetting == null);
                {
                    if (GUILayout.Button("SAVE", GUILayout.Width(60f), GUILayout.Height(20f)))
                    {
                        SaveSetting();
                    }

                    if (GUILayout.Button("Edit", GUILayout.Width(60f), GUILayout.Height(20f)))
                    {
                        Selection.activeObject = _searchStringInAssetsSetting;
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();


            _isIgnoreSearchPattern = EditorGUILayout.ToggleLeft(
                new GUIContent("Ignore Search Pattern",
                    "If enable, search pattern in search setting is disabled.Use for full text search."),
                _isIgnoreSearchPattern);

            EditorGUILayout.Space();

            if (GUILayout.Button("Search", GUILayout.MaxWidth(100f), GUILayout.Height(24f)))
            {
                if (string.IsNullOrEmpty(_searchString))
                {
                    return;
                }

                _searchResultObjectHashSet.Clear();

                if (_isIgnoreSearchPattern == false)
                {
                    foreach (var pattern in _searchStringInAssetsSetting.searchPatterns)
                    {
                        Search(pattern, _searchResultObjectHashSet);
                    }
                }
                else
                {
                    Search(IgnoredSearchPattern, _searchResultObjectHashSet);
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField($"「{_searchString}」Search Results : {_searchResultObjectHashSet.Count}");

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            {
                if (_searchResultObjectHashSet.Count > 0)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    {
                        foreach (var asset in _searchResultObjectHashSet)
                        {
                            EditorGUILayout.ObjectField(asset.name, asset, asset.GetType(), false);
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Not found.");
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        static readonly Encoding Enc = Encoding.GetEncoding("utf-8");
        /// <summary>
        /// Search with Pattern
        /// </summary>
        private void Search(string pattern, HashSet<Object> searchObjectResultHashSet)
        {
            var seachResultPathList = new List<string>();
            var searchKey = string.Format(pattern, _searchString);
            GetSearchFilePathList(searchKey, seachResultPathList);

            foreach (var fileFullPath in seachResultPathList)
            {
                // FullPath to DataPath
                var dataPath = FullPathToDataPath(fileFullPath);
#if ENABLE_SEARCH_STRING_IN_ASSETS_LOG
                Debug.Log($"{dataPath} | {fileFullPath}");
#endif
                // Convert to the entity file path if it was a meta file
                dataPath = ConvertMetaFileIfNeed(dataPath);

                var asset = AssetDatabase.LoadAssetAtPath(dataPath, typeof(Object));
                // Add to the list once it passes the ignore list
                if (asset != null && IsIgnoreType(asset) == false)
                {
                    searchObjectResultHashSet.Add(asset);
                }
            }
        }

        /// <summary>
        /// Full-width and half-width checks
        /// </summary>
        private bool ValidateInputText(string str)
        {
            int num = Enc.GetByteCount(str);
            return num == str.Length;
        }

        private void SaveSetting()
        {
#if ENABLE_SEARCH_STRING_IN_ASSETS_LOG
            Debug.Log("SaveSetting : " + _searchStringInAssetsSetting);
#endif
            if (_searchStringInAssetsSetting != null)
            {
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_searchStringInAssetsSetting,
                    out string guid,
                    out long localId))
                {
                    EditorPrefs.SetString(SaveKey, guid);
                }
            }
        }

        private string ConvertMetaFileIfNeed(string inputPath)
        {
            if (inputPath.Contains(MetaExtension))
            {
                return inputPath.Remove(inputPath.Length - MetaExtensionLength);
            }

            return inputPath;
        }

        private string FullPathToDataPath(string fileFullPath)
        {
            return $"{AssetsKeyWord}{fileFullPath.Substring(_dataPathLength)}";
        }

        static void SetDataPathLength()
        {
            _dataPathLength = Application.dataPath.Length;
        }

        /// <summary>
        /// Return the file name of the asset containing the specified string
        /// </summary>
        private void GetSearchFilePathList(string searchString, List<string> result)
        {
            Assert.IsNotNull(result, "Invalid result is null.");

            var arg = $"-onlyin . {searchString}";
#if ENABLE_SEARCH_STRING_IN_ASSETS_LOG
            Debug.Log($"arg : {arg}");
#endif
            var processStartInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                FileName = "mdfind",
                Arguments = arg,
                WorkingDirectory = Application.dataPath
            };

            using (var process = Process.Start(processStartInfo))
            {
                process.StandardInput.Close();
                var standardOutput = process.StandardOutput;
                var standardError = process.StandardError;
                var err = standardError.ReadToEnd();

                if (string.IsNullOrEmpty(err) == false)
                {
                    Debug.LogError(err);
                }

                while (true)
                {
                    var str = standardOutput.ReadLine();
                    if (string.IsNullOrEmpty(str))
                    {
                        break;
                    }

                    result.Add(str);
                }

                standardOutput.Close();
                standardError.Close();
                process.WaitForExit();
            }
        }

        /// <summary>
        /// Ignore the search return flag
        /// </summary>
        private bool IsIgnoreType(Object obj)
        {
            if (_searchStringInAssetsSetting == null)
            {
                return false;
            }

            var typeName = obj.GetType().Name;
            foreach (var ignoreTypeString in _searchStringInAssetsSetting.ignoreTypes)
            {
                if (typeName == ignoreTypeString)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
#endif