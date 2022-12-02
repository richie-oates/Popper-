# Popper-
A 2D bubble popping game designed for Android
\nPlay the web version here ... https://richie-oates.github.io/Popper-/
\nGet the android version here ... https://play.google.com/store/apps/details?id=com.DrahcirGames.Popper&hl=en&gl=US

	A recreation of a game I played on a Windows PDA with a stylus back in around 2007/8.
	I don't remember the title of that game and all my search attempts have drawn a blank so 
	unfortunately I can't assign proper credit for the original idea.

The Premise
-----------
	Bubbles fall from the top of the screen, you score points if you can pop them by tapping (mobile) or mouse clicking (PC)
	If you allow too many of them to fall off the bottom of screen then it brings the end of the game
	Also if you tap/click and miss too often it can also bring about the end

	Other objects will also appear the longer you last in the game, and they all have different effects:
		Clouds - burst into smaller clouds
		Balloons - burst and pop all objects within a specified radius
		Clocks - freeze all objects for a limited time so you can pop them easier
		Birds - double the points you score from popping bubbles for a limited time

The Structure of the Game
-------------------------
	The game has 2 unity scenes, a loading scene, and the main scene. The loading scene was necessary because on Android the 
	main scene was just taking too long to load and left a black screen while doing so.

	The GameManager script; 
		- this inherits from a Singleton class so there cam only be one of them and it can be called from all other scripts.
		- it loads the main scene and fades out the loading screen once it's loaded
		- it handles the various game states (Pregame, running, pause, frozen, endgame, quit), 
		  and the two game modes (Arcade, casual)
		- broadcasts events to let other areas of the code know when state has changed
		- handles pausing and quitting the game

	PREGAME:
		- Once the main scene has loaded we're presented with the start menu in the pregame state
		- Each menu has an IndividualMenuController script attached which uses tweens to show or hide each menu
		- The UIManager script displays different menu screens depending on game state
	
		Background:
			- The background is consistent throughout the game
			- Clouds scroll across at different speeds using a simple Scrolling Background script
				- A duplicate of the sprite is created as a child and positioned to the right of the original
				- Once the first sprite has moved a distance of its width across the screen it is then put back to the original position
			- A blur panel is activated when menus are shown to take focus away from the background

		Settings Menu:
			- Change the track for in game or menu music
			- Tracks are stored in an array in the sound settings object/script
			- Volume of music and sound effects are controlled via the audiomixer
			- Simple vibration toggle for Android
			- Settings are saved in PlayerPrefs

		RecordsMenu:
			- Displays High Score and Highest Combo
			- These are found in PlayerPrefs

		TutorialMenu:
			- Several slides describing how to play the game
			- Right and left buttons scroll through the slides by keeping track of which number we're on and deactivating all slides then activating the one we're on

		StartButton
			- Hides menu and switches to RUNNING GameState

	RUNNING:
		- The in-game HUD shows the score, a combo meter, danger meter, and a pause button
		- Objects are pooled in the ObjectPooler and spawned by the ObjectSpawner script

		ObjectSpawner:
			- I think this currently does too much and will need refactoring in the future
			- It spawns objects at intervals based on the rarity of the object and the current wave(level) that we're on
			- The interval decreases in time but also if we click and miss or let bubbles fall off the screeen
			- Sets the random range of speed for bubbles as well as size of bubbles and clouds
			- Listens for events from other scripts to adjust difficulty or trigger game over
	
		Poppable Objects:
			- Prefabs created for each type of object
			- Each has its own script inherited from the base ObjectOnClick script
				- This checks to make sure the game is not paused and the object has not already been hit
				- Plays a sound and broadcasts an event to say an object of this type has been hit
				- Performs different function specific to the object type
			- OutOfBounds script deactivates if it goes off the screen, and broadcasts an event if it was a bubble
			- Each has its own movement script as they all move in different ways
