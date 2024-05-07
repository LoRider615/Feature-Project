/* CONTROLS
 * Movement - WASD
 * Camera - Mouse
 * Teleport to shadows - hold right mouse button to aim, release to attempt teleport
 * 
 * INFO
 * 1. Guard Pathing - Guards follow a set patrol path, this can be easily altered, when placing a guard prefab, under the patrol waypoints object there will be a path object. 
 *  That first path will act as the starting point for the guard, duplicate it and move the new path object to a seperate location to create a new point in the guards path 
 *  that they will move to. This process can be repeated as many times as desired, but keep in mind that this patrol is circular in nature, meaning the guard will path to the very 
 *  first point once the list of paths has been cycled through, and it will start again, going indefinitey.
 * 
 * 2. Shadow mechanic - Being in the shadow is determined by raycasting in the opposite rotation as the directional light and seeing if terrain is hit. This is not 
 *  perfect, however I believe this is close enough to simulating the player being in a shadow. While you are in a shadow, guards will take longer to spot you, but only about 0.5-0.8 seconds 
 *  or so, but this is a good amount of extra time that may be needed to tp out
 * 
 * 3. Guard detection - the detection angle of the guards is based off of the angle and radius of their spotlight components. It takes a few seconds to be spotted, a little longer if player 
 *  is in shadow, after a few moments the guard will turn to look at the player to make it more dangerous and incentivize using the teleport to escape the guards' view
 * 
 * 4. Shadow Teleport - Holding down Right Mouse Button will bring up a little indicator pointing to where you are aiming, up to a distance. If that location is in a shadow (using the same 
 *  logic as mentioned before), releasing the button will teleport to the location. The teleport can be done while the player is or is not in a shadow, but can only teleport to a shadow
 * */