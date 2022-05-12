# Abstract

The subject of my thesis is creating a vehicle controller component for the Unity development environment. The completed component can drive on its own in a virtual city, much like a self-driving car would. It finds the shortest path to its destination, and travels there safely, according to the rule of the road. It stops at red lights, lets pedestrians go on the crosswalk, gives way when necessary, overtakes others carefully.

I first describe Unity and some other available game development environments on pricing, support, popularity, ease of use, their use outside of game development, and other considerations. I outline the structure of Unity projects and the process of developing in Unity to the point it is necessary for understanding my work. I give examples of artificial intelligence-based tools that can be used in the realization of a vehicle control system in Unity, both ones provided by the Unity environment, and ones available from Unity's store or external sources.

Since the project was done with the help of a fellow student, I first present his work in order to understand the starting point of mine. Then I talk about extending the simulation to multi-lane roads, which even though wasn’t realized was a pivotal point in the design.

I summarize the design criteria and implementation of the intersection system. I discuss the general model and the abstract solution on which the actual intersections are based. Deriving from this model, I show the realization of each type of intersection completed, a three-way and a four-way peer intersection, and a four-way intersection with traffic lights. I describe the implementation of the coloured lights in some detail.

I present the challenges of a dynamic overtaking system as well as my own solution to these, including the physical calculations involved. I describe the conditions necessary for overtaking, the way of executing it, which, unlike with intersections, takes place without central control. I talk about the difficulties of designing emergent systems comprised of smaller moving parts.

Finally, I talk about testing the system, in more detail pertaining to testing intersections and overtaking. I summarize the project, the lessons learned, and give some perspective on the field and possible extensions of this project.
