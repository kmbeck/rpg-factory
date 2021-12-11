# RPG Factory Project
The goal of this project is to create an environment for developing RPGs in Unity.

## Scriptable Object Pipeline (SOP)
The SOP is a system that will load desired SO data into memory at runtime. These objects are stored in singleton dictionaries that can be easily referenced from anywhere in the project, and allow the stored SO data to be retrieved as raw data or even as an instantiated GameObject. Instantiated GameObjects possess deep copies of the data stored in the dictionaries and can be modified without affecting the original values (still needs to be tested???).

## Enum-Based (For Now..)
Many of the core components of the RPG system are defined using enums. This includes things like what Character Stats and Attributes exist in the game, what equipment slots are available in the game, what types of items and equipment there are, etc. Modifying these basic enums will change what options are available to you when creating more complex objects like equipment, spells, etc.

## 'Universal' UI Kit
Eventually there should be a set of prefabs and scripts that provide a foundation for an UI system that has the following features:
1. Simple bordered windows that can be resized and recolored during runtime.
2. Basic UI and Text animations.
3. Dynamically generate certain UIs based on SO data or other game data...

## More...