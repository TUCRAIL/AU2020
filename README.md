# AU2020 - Infrastructure Design Slam

## Introduction

In the Infrastructure Design Slam session TUC RAIL demonstrates 3 design challenges and how to meet them using innovative solutions:

- Parametric Offset Feature Lines:

The Platform Edge function was introduced in the 2019 version of Civil 3D, its name does not do justice to what it can do for general Civil design. In this session an example is shown how custom user parameters can be defined and used in your own design logic, even a custom transition system can be created. The resulting feature line is then used as a target in a corridor calculation, hereby integrating a new parametric source of stations and geometry in your corridor.

More information on the mathematical functions can be found here: https://github.com/ArashPartow/exprtk

- Dynamic Station values for Corridor generation

In standard corridor design station values are very static and dependent on direct user input (typing in the value or using the station selector), even the frequency settings have their limitations. By creating new dynamo nodes that expose the creation of baseline regions and configuration of frequency settings, you can now define baseline regions in a more dynamic way and better connect with other disciplines that do not use station values. With the frequency settings you can also define a custom LOD control for your corridor geometry.

- 3D solid creation using corridor geometry

While Civil 3D can create 3D solids from a corridor for each region, it is not able to create multiple singular 3D solids in one or multiple regions using corridor generated geometry. Using dynamo, we are now able to do this and create a dynamic link between the corridor geometry and the 3D solids that are created with dynamo. This means that the generated 3D solids can follow the LOD defined in the corridor calculation.

## Presenter
Wouter Bulens
Methods Coordinator / BIM Manager

wouter.bulens@tucrail.be

https://www.linkedin.com/in/wouter-bulens-11278319/

## Data

In the [data](https://github.com/TUCRAIL/AU2020/tree/master/data) folder you will find the drawings and dynamo script used in the presentation. Feel free to adjust and play with them as you like.

## Installation

Visual Studio 2019 was used to create this solution.  
The [source code](https://github.com/TUCRAIL/AU2020/tree/master/src) can be cloned to your computer for building and testing.
  
## Packages
When building the solution the dll's are copied to the packages in the [Dynamo_Packages](https://github.com/TUCRAIL/AU2020/tree/master/src/Dynamo_Packages) folder.  

Place the packages into this folder:  
%APPDATA%\Autodesk\C3D 2021\Dynamo\2.5\packages

Open Dynamo in AutoCAD Civil 3D 2021 to test the scripts.
