# PlatformerController
## Incomplete Refactor of Sebastian Lague's Controller2D
I love Sebastian Lague's [2D Platformer Controller](https://www.youtube.com/watch?v=MbWK8bCAU2w), but I find the code very difficult to reason about, so I refactored it into something I found more manageable. I switched from using inheritance to composition (Lague's controller _is_ a raycaster, mine _has_ a raycaster), and added a whole bunch of helper functions to aid readability. 

Only covers Episodes 1-5 so far, will add more soon.
