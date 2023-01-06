# Introduction
Project Speedrun is a 2D platformer game, made with Unity.

Speedrunning is the act of playing a video game, or section of a video game, with the goal of completing it as fast as possible. People usually speedrun games that were not made to be speedran (ex: Minecraft, Hitman, Jump King, etc...). An online leaderboard exists for each game where players can submit their speedrun and compare themselves to other speedrunners (https://www.speedrun.com/).

Although most games that are speedran were not initially created for that purpose, this will be the goal of Project Speedrun. It will aim to create the perfect environment for speedrunning. 

# Inspiration
This game is mainly inspired by two games:
- Ori & The Blind Forest
- Celeste

## Ori & The Blind Forest
Ori isn't too popular for speedrunning. To speedrun the game from start to finish would take a lot of time. The parts of the game which I am personally interested in are the levels where the player must escape from a certain situation, having something follow them (which would lead to Ori dying if it catches up).

In the game, there are three levels where this *speedrunning* concept is present: https://www.youtube.com/watch?v=GPB4Tkn60Oo
- Ginso Tree Escape
- Forlorn Ruins Escape
- Mount Horu Escape

# Core Concepts
I'd like to list the core concepts that are found in these games, which I wish to implement in Project Speedrun:

- **Repeatability**: If a player dies, they should be able to quickly repeat the level. Instant respawn the current solution for this point. No keys should be pressed to respawn.
- **Continuity**: This idea mainly comes from Ori, the fact that music persists even after death. This creates a sense of continuity and pushes the player to not easily give up.
- **Certainty**: The opposite of chaos. Project Speedrun should not introduce randomness like Minecraft does. The same exact inputs will lead to the exact same outputs.
- **Complexity**: The game mechanics should be complex. Unlike Geometry Dash or Jump King, Project Speedrun should not only have one mechanic (Jumping). Players must have enough tools in their belt, allowing them to develop different approaches for the same problem. Some of these tools must be difficult to use, requiring a certain skill. How the map is built will factor into this point as well.
- **Intensity**: The game should feel intense. The music will play a big role, but most importantly, the fact that something is chasing the player is crucial. They should never have the leisure to stand in their place and not move. They should be punished for small mistakes. But not too much, so that it doesn't end up becoming just like Geometry Dash.

# Maps
The initial goal is to build one good map. From there, two new maps would be introduced.

Maps need to be built in a way that allows different players to have different approaches. There will always be a clear main path, but game mechanics should always be taken into consideration.

The game mechanics will introduce variability to the maps: if a map existed, and the player could only jump, the most optimal path would be clear in most cases. However, if new mechanics are introduced, the player could combine them to stray off of the main path, and get a faster time.
