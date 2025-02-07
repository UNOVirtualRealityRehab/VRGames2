# Description
This application is a collection of games designed to help with the rehabilitation process for people who have suffered a stroke or similar brain injury by creating games that encourage repetitive movements but in an engaging way. This project is an attempt at gamifying the rehabilitation process. There are four proof-of-concept games included. 
1. A game where you pop balloons with darts.
2. A game where you throw paper airplanes.
3. A game where you climb a wall.
4. A game where you stack blocks.

To use this application, the files must be built in the Unity engine and deployed to a virtual reality headset such as the Meta Quest 2.

# Release Notes:

### 9/28/2023 (Milestone 1)

General:
* Added a guest login button to allow play if the server is offline or not set up.

Balloon Game:
  * Added multiple difficulty levels that automatically trigger upon a designated score threshold. Balloons will move faster and become harder to predict.
  * Added a win condition that triggers a "You won!" message. The game will (for now) automatically restart after this happens.
  * Added basic effect that will be applied to new "Powerup" balloons that have not yet been implemented. Currently they are applied to normal balloons as a proof of concept.
    
### 10/19/2023 (Milestone 2)

General:
* Connected VR-Games to our student server

Balloon Game:
* Refactored BalloonGameplayManager class
* Created new BalloonManager class to handle the balloons in the scene and balloon spawning
* Created a game settings class that can be used to create and specify different game settings such as:
  * Game mode
  * Spawn pattern
  * Probability of different types of balloons spawning
  * Where a balloon spawns
  * Time between each balloon spawn
* Implemented alternating and cocurrent spawn patterns
* Created Balloon stream powerup balloon
  * This balloon causes 5 extra balloons to spawn rapdily!

### 11/02/2023 (Milestone 3)

Balloon Game:
  * New Additions:
      * New Balloon - The "Onion" balloon has multiple layers that need to be popped
      * New conffetti effect on victory

 
  * Improvements:
      * Recreated balloon stream powerup to align with refactored code
      * Created a stream of balloons prefab to be spawned by the powerup
      * Simplified balloon collider
      * Refactoring
      * Added a method to delay the destruction of balloons by a given number of seconds 
      * Created a dart manager class to handle dart spawning and got rid of the manual dart spawners
      * Tracking for separate left and right points
      * Fixed scoreboard not correctly displaying the current left and right scores
      * Disabled balloon spawner once goal has been reached
      * Fixed skipping over point goal (e.g. going from 3 to 5 points with a goal of 4) not triggering win state
   
### 11/16/23 (Milestone 4)

Balloon Game:
* Implemented at the restore life balloon and the target balloon
* Simplified the spawning by changing the duration between balloon spawns to a static timer (no more random time spawns).
* Implemented another spawn pattern called "Random." This spawn pattern will be similar to picking a random balloon, that is, the balloon spawn is chosen based on a probability.
* Implemented the ability for the goal to be left hand points, right hand points, or total points.
* Added a speed modifier, and changed the update methods of the balloons to make use of said modifier (the modifier will be used by the clinician to control the speed of the balloons).
* Implemented the feature to adjust the kill zone with respect to the height of the player.
* Fixed dart getting destroyed when colliding with the kill zone.
*  Fixed the ungrabbable dart bug.

### 12/8/23 (Milestone 5)
Balloon Game:
* Added a new environment and added game sounds to the environments.
* Added a new game mode called Career mode.
* Added achievements to the game.
* General refactoring and balancing of the game.
* Added necessary elements to the game needed for the clinician to control the game settings.
