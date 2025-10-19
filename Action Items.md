Musts:


Wants:

* animations when player ship is destroyed
* animations when alien gets destroyed
* what to do if the ball's trajectory is barely vertical and all horizontal (solution: if the ball hits a wall 4 times without hitting an alien or the player or respawning, then it will despawn and then respawn on the player after the respawn time for the ball)



Bugs:

* Some objects were not cleaned up when closing the scene. (Did you spawn new GameObjects from OnDestroy?) The following scene GameObjects were found: PowerUp_Pierce(Clone) PowerUp_Shield(Clone)
