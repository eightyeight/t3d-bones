//-----------------------------------------------------------------------------
// Copyright (c) 2012 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Load up our main GUI which lets us see the game.
exec("./hudlessGui.gui");

//-----------------------------------------------------------------------------
// Create a datablock for the observer camera.
datablock CameraData(Observer) {
   mode = "Observer";
};

//-----------------------------------------------------------------------------
// Called when all datablocks have been transmitted.
function GameConnection::onEnterGame(%client) {
   // Create a camera for the client.
   %c = spawnObject(Camera, Observer);
   GameCleanup.add(%c);
   %c.scopeToClient(%client);
   %client.setControlObject(%c);
   // Activate HUD which allows us to see the game. This should technically be
   // a commandToClient, but since the client and server are on the same
   // machine...
   Canvas.setContent(HudlessPlayGui);
   activateDirectInput();
}

//-----------------------------------------------------------------------------
// Called when the engine has been initialised.
function onStart() {
   // Create objects in the game!
   new SimGroup(GameCleanup);
   new SimGroup(GameGroup) {
      singleton Material(BlankWhite) {
         diffuseMap[0] = "art/images/white";
         mapTo = "white";
      };
      new LevelInfo(theLevelInfo) {
         canvasClearColor = "0 0 0";
      };
      new GroundPlane(theGround) {
         Position = "0 0 0";
         Material = "BlankWhite";
      };
      new Sun(theSun) {
         azimuth = "230.396";
         elevation = "45";
         color = "0.968628 0.901961 0.901961 1";
         ambient = "0.078431 0.113725 0.156863 1";
         castShadows = "1";
      };
   };

   // Create some keybinds for the console and to exit.
   GlobalActionMap.bind(keyboard, "tilde", toggleConsole);
   GlobalActionMap.bind(keyboard, "escape", quit);
}

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onExit() {
   echo("GOODBYE.");

   // Delete the objects we created.
   GameCleanup.delete();
   GameGroup.delete();

   // Delete the connection if it's still there.
   ServerConnection.delete();
   ServerGroup.delete();

   // Delete all the connections:
   while (ClientGroup.getCount()) {
      %client = ClientGroup.getObject(0);
      %client.delete();
   }

   // Delete all the data blocks...
   deleteDataBlocks();
}