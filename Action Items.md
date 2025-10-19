Musts:

* sound effect for the ball bouncing off the wall



Wants:

* animations when player ship is destroyed
* animations when alien gets destroyed
* what to do if the ball's trajectory is barely vertical and all horizontal (solution: if the ball hits a wall 4 times without hitting an alien or the player or respawning, then it will despawn and then respawn on the player after the respawn time for the ball)



Bugs:
* When a level clears but some power-ups exist, they are still there. Should wipe out any falling power-ups and disable all power-ups if a level is cleared
* Some objects were not cleaned up when closing the scene. (Did you spawn new GameObjects from OnDestroy?) The following scene GameObjects were found: PowerUp_Pierce(Clone) PowerUp_Shield(Clone)

* after player-bounce sound was added, it makes the sound repeatedly when it should not make it at all
* when pierce power up is activated, aliens being destroyed do not do anything.