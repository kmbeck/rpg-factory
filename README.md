# RPG Factory Project Overview
The purpose of this tool is to allow developers to encapsulate behaviour logic into 'Events'. These Events can be assigned to specific callbacks on any given Component, allowing each callback to have an unlimited number of possible behaviours without having to add any logic to the Component script itself.

## GScript
A white-space sensitive, strictly-typed scripting language which compiles into C# (and then is further compiled by Unity). Scripts written in this language are referred to as 'Events' or 'Event Scripts'. Events are stored as ScriptableObjects in your Unity project, and are loaded in to the Scriptable Object Database (SODB).
* Each event is compiled into it's own equivalent C# function. These functions are invoked during runtime by the desired components.
* Events can invoke other Events.
* Events can get/set global variables that persist between scripts called Flags.
* GScript can directly reference and invoke C# functions that are exposed to it via the Event Interface.
* GScript can reference any object exposed to it in the SODB (see Scriptable Object Pipeline (SOP) section).
* There is an 'Event Editor' window in Unity which allows you to easily save, load, edit, and validate Event Scripts in your project.

## Flags
One of the main features of GScript is the ability to get & modify Flag variables. Flag variables are created in the Unity Editor as Scriptable Objects and loaded into a library in the SODB at runtime. They can be ints, floats, bools, or strings.

## How To Use Events
Each event will be stored as a scritable object and loaded into a dictionary in the SODB. Any Component that needs to invoke an event will simply need to know the unique ID of the Event you want it to run.

## Scriptable Object Pipeline (SOP) & Scriptable Object Database (SODB)
The SOP is a system that will load desired SO data into memory at runtime. These objects are stored in the SODB in dictionaries that can be referenced from anywhere in the project, and allow the stored SO data to be retrieved as raw data or even as an instantiated GameObject. Instantiated GameObjects possess deep copies of the data stored in the dictionaries and can be modified without affecting the original values.

The data in the Scriptable Object Database are exposed as global variables in the GScript langauge.
