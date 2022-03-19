# RPG Factory Project Overview
The goal of this project is to create an environment for developing RPGs in Unity. RPGs have a huge number of dynamic events and highly diverse interactions between objects, which can quickly turn Unity's component and behaviour based interface into a cumbersome mess. The purpose of this tool is to allow developers to encapsulate behaviour logic into 'Events'. These Events can be assigned to specific callbacks on any given component, allowing each callback to have an unlimited number of possible behaviours without having to add any logic to the component script itself.

## GScript
GScript is the heart of this project. It is a white-space sensitive, strictly-typed scripting language which compiles into C# (and then is further compiled by Unity). Scripts written in this language are referred to as 'Events' or 'Event Scripts'. Events are stored as ScriptableObjects in your Unity project, and are loaded in to the Scriptable Object Database (SODB).
* Each event is compiled into it's own equivalent C# function. These functions are invoked during runtime by the desired components.
* Events can invoke other Events.
* GScript can directly reference and invoke C# functions that are exposed to it.
* GScript can reference any object exposed to it in the SODB (see Scriptable Object Pipeline (SOP) section).
* There is an 'Event Editor' window in Unity which allows you to easily save, load, edit, and validate Event Scripts in your project.

## Scriptable Object Pipeline (SOP) & Scriptable Object Database (SODB)
The SOP is a system that will load desired SO data into memory at runtime. These objects are stored in the SODB as dictionaries that can be easily referenced from anywhere in the project, and allow the stored SO data to be retrieved as raw data or even as an instantiated GameObject. Instantiated GameObjects possess deep copies of the data stored in the dictionaries and can be modified without affecting the original values.

The data in the Scriptable Object Database are exposed as global variables in the GScript langauge.
