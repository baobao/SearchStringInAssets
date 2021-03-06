# SearchStringInAssets

[![Releases](https://img.shields.io/github/release/baobao/SearchStringInAssets.svg)](https://github.com/baobao/SearchStringInAssets/releases)
[![MIT License](http://img.shields.io/badge/license-MIT-blue.svg?style=flat)](LICENSE)

**※Only macOS.**

SearchStringInAssets is an Editor extension that allows you to search the UnityEditor for strings stored in a scene, Prefab, or ScriptableObject.

![demo](https://user-images.githubusercontent.com/144386/101299365-4da63780-3875-11eb-9b56-b2fe56d95663.gif)

※Cannot be used when Spotlight is off because it uses mdfind.



## How to Use

Open SearchStringInAssets from `Tools > Shibuya24 > SearchStringInAssets`.

### If Use Custom Setting

Create setting(ScriptableObject) from `Create > Shibuya24 > SearchStringInAssets` .  
Set it in the SearchSetting of SearchStringInAssets, press the Save button to save the settings to the Editor.


## How to Install

### UPM Package Install via git URL

After Unity 2019.3.4f1, Unity 2020.1a21, that support path query parameter of git package. You can add `https://github.com/baobao/SearchStringInAssets.git?path=Assets/SearchStringInAssets` to Package Manager

![image](https://user-images.githubusercontent.com/144386/87669945-d11d9a00-c7a9-11ea-8a21-aff2cb8117f8.png)


<img src="https://user-images.githubusercontent.com/144386/101301741-20f61e00-387d-11eb-8b6e-a937f6f5814d.png" width=316 />


or add `"info.shibuya24.search-string-in-assets":"https://github.com/baobao/SearchStringInAssets.git?path=Assets/SearchStringInAssets"` to `Packages/manifest.json`.


### or import unitypackage

Download and import unitypackage from the following page.  
https://github.com/baobao/SearchStringInAssets/releases/


## License

This library is under the MIT License.
