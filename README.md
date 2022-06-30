# Map Toolbox for Autoware.

[![GitHub license](https://img.shields.io/github/license/autocore-ai/AutowareUnityTools.svg)](https://github.com/autocore-ai/AutowareUnityTools)
[![GitHub release](https://img.shields.io/github/release/autocore-ai/AutowareUnityTools.svg)](https://github.com/autocore-ai/AutowareUnityTools/releases)
[![Unity version](https://img.shields.io/badge/unity-2019.3%2B-green.svg)](https://unity3d.com/unity/whats-new/2019.3.0)

## Description

This a unity plugin helps user create lanelet2 maps for [Autoware](https://github.com/autowarefoundation/autoware) efficiently.

For vector map format (which will become obsoleted), ref to [vector_map](https://github.com/autocore-ai/MapToolbox/tree/vector_map) branch.

## Requirements
* Ubuntu 20.04
* [Unity 2019.4.0](https://store.unity.com/download?ref=personal) or above
* [Git](https://www.git-scm.com/download/)

## How to use
* Create your new project in Unity 2019.4
* In unity editor menu bar, click Window -> Package Manager -> ➕ -> Add package from git URL...
* Paste this git URL in text field
```
https://github.com/autocore-ai/MapToolbox.git#lanelet2_linux
```
* If any error message appears, try restart unity editor.
* Import PCD by right click in Project Panel and Import New Asset... then drag pcd file to hierarchy.
![](https://raw.githubusercontent.com/autocore-ai/MapToolbox/doc/images/pcd.gif)
* Create new map by right click in Hierarchy, Autoware -> Lanelet2Map
* Create new lanelet by "Add Lanelet" on Lanelet2Map script, left mouse with **Crtl** to add nodes and **Ctrl + Shift** to remove nodes.
* Select Nodes to move position
![](https://raw.githubusercontent.com/autocore-ai/MapToolbox/doc/images/lanelet.gif)
* Add traffic light buttom ref line just like add lanelet nodes, then modify height of traffic light
![](https://raw.githubusercontent.com/autocore-ai/MapToolbox/doc/images/traffilight.gif)
* Select two nodes and link to a stop line then add traffic light selection with **Ctrl**, click button "add regulatory element" on "Way" script to create relation between stop line and traffic light.
![](https://raw.githubusercontent.com/autocore-ai/MapToolbox/doc/images/traffilight_element.gif)
* Click "Save" on Lanelet2Map to export osm map file.
