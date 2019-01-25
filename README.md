LiNGS - Lightweight Networked Game System
=====

### Framework for networked multiplayer mobile games

LiNGS is a framework that provides the capability of network multiplayer to mobile games and is designed to work on Unity 4+ as a standalone plugin.

It acts as a middleware that provided game state synchronization and message passing between a Game Server and Clients.


Instalation
-----------
Download and import the project into Visual Studio 2013 or other similar editor. Build the projects and copy the .dll files to your UnityProject/Assets/plugins folder. After that you can use this system to create your multiplayer games.


Architecture and Usage
----------------------
The system is design to work on a client-server type architecture. Both components can be executed on a Unity game (even on mobile) or you can create a .NET based dedicated server and use a Unity client. 

#### Basic Client Architecture
![alt text](https://github.com/valterc/lings/raw/master/lings_client_arch.png "LiNGS Client Architecture Overview")

#### Basic Server Architecture
![alt text](https://github.com/valterc/lings/raw/master/lings_server_arch.png "LiNGS Server Architecture Overview")

#### Integration with Unity Game Objects
Integration with games is seemless and requires only minor changes in your logic code:
![alt text](https://github.com/valterc/lings/raw/master/lings_api_object.png "LiNGS Integration example")

The LINGS system is designed to be very simple to use and the API is documented. Check the provided 'Striker' implementation example.


Class Diagrams
----------------------

#### Common Classes
![alt text](https://github.com/valterc/lings/raw/master/lings_common_class_diagram.png "LiNGS Common Class Diagram")

#### Client Classes
![alt text](https://github.com/valterc/lings/raw/master/lings_client_class_diagram.png "LiNGS Client Class Diagram")

#### Server Classes
![alt text](https://github.com/valterc/lings/raw/master/lings_server_class_diagram.png "LiNGS Server Class Diagram")


