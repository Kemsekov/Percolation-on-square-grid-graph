# Graph percolation project
This repository contains c# code to create a square graph percolation.

1. Generate square grid of nodes, where each node occupies unique position with some probability.
2. Generate edges between down and right nodes, forming a connected grid, where each unique edge is create with some probability.
3. Compute connected components using union find data structure.
4. Do visualization with recolored components.

Example with N=100, node_probability=0.6, edge_probability=0.8
![example](https://github.com/user-attachments/assets/01abce21-b8c7-4611-be7e-b72a5eef4d71)

Example with N=400, node_probability=0.95, edge_probability=0.4
![example](https://github.com/user-attachments/assets/1474b0af-3d7a-46e7-a27b-af39d5c3110b)
