# evo
An evolutionary algorithm for sound localization.

Each agent has two inputs (left ear, right ear), and two outputs (left leg, right leg). The "ear" inputs have a magnitude governed by their distance from a "target", and the "leg" outputs control the motion and turning of the agent. Each is controlled by an algorithm that governs the relationship between the inputs and outputs. When the program is executed, a group of totally random algorithms is created. As can be seen in the below video, their behavior is random and ineffective.

https://www.youtube.com/watch?v=raLRx7Dx_Uo

However, each generation the agents that reach the target are duplicated with minor random changes, replacing the agents that failed to reach the target. Over the course of many generations, the algorithms governing the agents start to use the difference in loudness across the ears to dynamically orient themselves toward the target and approach it. Below is an example of the behavior of the agents after 1110 generations, where they quickly beeline toward the target.

https://www.youtube.com/watch?v=Z1JLjYFXUAM
