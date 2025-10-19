Musts:

* make walls look good

* sound effects for when ball hits alien
* sound effects for when ball hits player
* sound effects for level cleared or game over
* when power ups are active, some graphic should indicate that. (Freeze, aliens are frozen. Immunity, the player has a shield. Pierce, the ball has a unique graphic)
* when player respawns, they should be immune for a couple seconds
* when 3 seconds left in player's shield, it turns to red to signal that 3 seconds are left


Wants:

* animations when player ship is destroyed
* animations when alien gets destroyed
* what to do if the ball's trajectory is barely vertical and all horizontal (solution: if the ball hits a wall 4 times without hitting an alien or the player or respawning, then it will despawn and then respawn on the player after the respawn time for the ball)



Bugs:
* When a level clears but some power-ups exist, they are still there. Should wipe out any falling power-ups and disable all power-ups if a level is cleared
* Some objects were not cleaned up when closing the scene. (Did you spawn new GameObjects from OnDestroy?) The following scene GameObjects were found: PowerUp_Pierce(Clone) PowerUp_Shield(Clone)
