# PlatformerController
## Incomplete Refactor of Sebastian Lague's Controller2D
I love Sebastian Lague's [2D Platformer Controller](https://www.youtube.com/watch?v=MbWK8bCAU2w), but I find the code very difficult to reason about, so I refactored it into something I found more manageable. I switched from using inheritance to composition (Lague's controller _is_ a raycaster, mine _has_ a raycaster), and added a whole bunch of helper functions to aid readability. 

Only covers Episodes 1-5 so far, will add more soon.

## Project Settings Note
To use this controller in modern versions of Unity, you need to either go to __Project Settings > Physics2D__ and enable __Auto Sync Transforms__ or put an explicit call to __Physics2D.SyncTransforms()__ at the end of the controller's __Move()__ function.
