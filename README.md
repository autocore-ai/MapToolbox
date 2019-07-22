# Map Toolbox for Autoware.

[![GitHub license](https://img.shields.io/github/license/autocore-ai/AutowareUnityTools.svg)](https://github.com/autocore-ai/AutowareUnityTools)
[![GitHub release](https://img.shields.io/github/release/autocore-ai/AutowareUnityTools.svg)](https://github.com/autocore-ai/AutowareUnityTools/releases)
[![Unity version](https://img.shields.io/badge/unity-2019.1%2B-green.svg)](https://unity3d.com/unity/whats-new/2019.1.0)

## Description

This a unity plugin helps user create vector maps for [Autoware](https://github.com/autowarefoundation/autoware) efficiently. 

## Features

* pcd import
  * binary pcd
    * ✔️ x y z
    * ✔️ x y z color
    * ✔️ x y z intensity
    * ✔️ x y z intensity ring
* vector maps export
  * Autoware
    * ✔️ lane merge and branch
    * ✔️ lane speed
    * ✔️ white line
    * ✔️ stop line
    * ✔️ traffic signal
    * ✔️ road edge
    * ✔️ curb

## Roadmap

* export zone type for Autoware
* add support to Lanelets 

## Requirements

* [Unity 2019.1](https://store.unity.com/download?ref=personal)
* [Git](https://www.git-scm.com/download/)

## How to compile

* Create your new project in Unity 2019.1
* Add two lines below to Packages/manifest.json dependencies
  
``` json
"com.autocore.map-toolbox": "https://github.com/autocore-ai/MapToolbox.git",
"com.nition.unity-octree": "https://github.com/autocore-ai/UnityOctree.git#upm",
```
## User manual
* please check video. docs will be provided soon.

[![Watch the video](https://img.youtube.com/vi/WTRHPs8pN04/0.jpg)](https://youtu.be/WTRHPs8pN04)
