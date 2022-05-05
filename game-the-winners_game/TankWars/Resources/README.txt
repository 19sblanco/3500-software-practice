This project was made using Model View Controller (MVC)

Model:
	inside this file we just have dictionaries containing each class and their attributes
	We also have the World.CS which is the container for all the objects and their states
	the world is modified by the controller and read by the view

View:
	This file displays the form in which the user can interact with the game. We made it so
	that the display panel is nested inside the view class. This is for easy access by the view. 
	Inside the display panel we have an onPaint() method that is invoked for each frame and it loops
	over each object in the world to be drawn and draws it. We made the design desision for drawing animations
	by drawing the same picture but at varying sizes based on the frame count. The frame count is stored 
	as memeber variables in the drawing panel. When inputs are received we simply tell the controller. 
	In the TankDrawer() method we decided to either draw the tank or the explosion with an if statement, 
	because we want to draw one or the other, not both. We decided to draw explosions inside of the tank class.
	We stored all the images so that they don't have to load each frame. This increases the speed by alot.

Controller:
	This file recieves information from the server, parses it, updates the model then tells the view. When
	inputs are recieved we set bool values up, down, left, right to true or false based on the input. Then before 
	sending the control command we check these bool values, update the control command then serialize and send it.
	Remove_beam() was put in here because the view will tell us when to remove the beam
	from the model, but cannot do so itself. If you press Q or Esc you will exit the program. 
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

PS9

Model:
	We keep track of the number of powerups in a variable. We put the logic of updating the world in each frame in the
	update method. After the update method is called we call the cleandead method which cleans up the dead objects in
	the world. We also decide to deserilize each object use an over written toString method. For spawning tanks and powerups
	we pick random locations until it doesn't intercept with the wall. We also do collision detection for tanks and walls 
	inside these tanks & walls classes.

View:
	We print when a server can receive clients, when it does receive clients, and when a client disconnects.

Controller:
	We detect if a client is disconnected and inform the model. We serialize everything in the world and send it to each 
	client.