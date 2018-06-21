# UnityGrapplingHookComponent
Simple force based grappling hook componet for Unity3d.



## How to use

Add the component to your assets folder, then add the component to the player's GameObject. Under the component in inspector, add the Camera object to the camera_camera option. The component is ready to go.

## How it works

The component checks whether the player has clicked on a grappling node. Holding down left mouse keeps the tether connected and it is disconnected when the player releases left mouse. 



The player can only tether to an object with the tag "grappling_node" and the tether point is at the grappling node's transform.position point. Add some objects with tag "grappling_node" and start swinging!



This component has three modes:

* Ratcheting sets the tether length shorter when the player moves closer to the node. (This mode is the most fluid and fun mode in my opinion)

* Loose only applies the tension force when the player's distance to the grappling node is at the tether length. The tether length is set when the node is first hooked. Think a ball swinging on a rope.

* Rigid keeps the player at a set distance from the grappling node. The tether length is set when the node is first hooked. Think a ball swinging on a metal rod.



The draw_dev_line draws a line from the player's transform.position to the hooked grappling nodes transform.position is selected.

The break_tether_velo option sets a velocity where if a player exceeds, the tether breaks. Set this option to "Infinity" if you do not want this behaviour.

## How it actually works (the physics)

Depending on what mode is selected, a tension force is applied at certain conditions. 
