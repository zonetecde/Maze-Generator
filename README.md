# Maze Generator

Ce logiciel permet de générer des labyrinthes en suivant l'algorithme de Wilson.

Aperçu du logiciel :

![image](https://user-images.githubusercontent.com/56195432/189527862-0971cde2-78bf-4f8e-bb13-1da99b1d61be.png)

Le slider à gauche permet de régler la taille des bordures, celui de droite la vitesse de la génération 
Le checkbox "More Random" permet d'éviter d'avoir deux fois deux suites la même direction,
Le checkbox "Instant" permet de skip l'aperçu de la génération (pour une génération très rapide)

Aperçu d'une génération en cours :

![image](https://user-images.githubusercontent.com/56195432/189527870-d3d6a2e5-3365-49bf-a4a6-deedd81f6694.png)

Le labyrinthe final généré : 

![image](https://user-images.githubusercontent.com/56195432/189527878-c34b09cb-0948-466b-8342-1036e94c9180.png)

Un labyrinthe 100x100 (généré en 2 secondes) :

![image](https://user-images.githubusercontent.com/56195432/189527905-42d789ff-d2a1-4fbc-bc8e-53f96aeb30e4.png)

![Labyrinthe 11 09 2022 14 41 57](https://user-images.githubusercontent.com/56195432/189528131-053bf65f-1c27-40c9-99e5-9e6337237fe0.png)

Génération de masse

![image](https://user-images.githubusercontent.com/56195432/189528062-9ed631f8-c6a7-487c-a7b0-fec116df2e1d.png)

Example de sortie json

```
[[[{"Borders":[1,1,1,0]},{"Borders":[1,1,1,0]},{"Borders":[1,1,1,0]},{"Borders":[1,1,1,0]}],[{"Borders":[1,0,1,0]},{"Borders":[1,0,1,0]},{"Borders":[1,0,0,0]},{"Borders":[0,0,1,0]}],[{"Borders":[1,0,1,0]},{"Borders":[1,0,1,0]},{"Borders":[1,0,1,0]},{"Borders":[1,0,1,1]}],[{"Borders":[1,0,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,1,1,1]}]],[[{"Borders":[1,1,0,1]},{"Borders":[0,1,0,0]},{"Borders":[0,1,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,1,0,0]},{"Borders":[0,0,1,1]},{"Borders":[1,1,0,0]},{"Borders":[0,0,1,0]}],[{"Borders":[1,0,1,1]},{"Borders":[1,1,0,1]},{"Borders":[0,0,1,0]},{"Borders":[1,0,1,0]}],[{"Borders":[1,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,1]},{"Borders":[1,0,1,1]}]],[[{"Borders":[1,1,0,1]},{"Borders":[0,1,0,0]},{"Borders":[0,1,1,0]},{"Borders":[1,1,1,0]}],[{"Borders":[1,1,0,0]},{"Borders":[0,0,1,1]},{"Borders":[1,0,0,0]},{"Borders":[0,0,1,1]}],[{"Borders":[1,0,1,0]},{"Borders":[1,1,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,0,1,1]},{"Borders":[1,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,1]}]],[[{"Borders":[1,1,1,0]},{"Borders":[1,1,0,0]},{"Borders":[0,1,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,0,0,0]},{"Borders":[0,0,1,1]},{"Borders":[1,1,0,0]},{"Borders":[0,0,1,1]}],[{"Borders":[1,0,1,0]},{"Borders":[1,1,0,0]},{"Borders":[0,0,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,0,1,1]},{"Borders":[1,0,1,1]},{"Borders":[1,1,0,1]},{"Borders":[0,0,1,1]}]],[[{"Borders":[1,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,1,1,0]},{"Borders":[1,1,1,0]}],[{"Borders":[1,1,0,0]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,0]},{"Borders":[1,0,1,0]}],[{"Borders":[1,0,1,0]},{"Borders":[1,1,0,1]},{"Borders":[0,0,0,0]},{"Borders":[0,0,1,1]}],[{"Borders":[1,0,1,1]},{"Borders":[1,1,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,1,1,1]}]],[[{"Borders":[1,1,1,0]},{"Borders":[1,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,0,0,0]},{"Borders":[0,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,0]}],[{"Borders":[1,0,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,1,1,1]},{"Borders":[1,0,1,0]}],[{"Borders":[1,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,1]}]],[[{"Borders":[1,1,1,0]},{"Borders":[1,1,0,0]},{"Borders":[0,1,1,0]},{"Borders":[1,1,1,0]}],[{"Borders":[1,0,1,0]},{"Borders":[1,0,1,1]},{"Borders":[1,0,1,0]},{"Borders":[1,0,1,0]}],[{"Borders":[1,0,1,0]},{"Borders":[1,1,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,0,1,0]}],[{"Borders":[1,0,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,1]}]],[[{"Borders":[1,1,0,0]},{"Borders":[0,1,1,0]},{"Borders":[1,1,0,0]},{"Borders":[0,1,1,0]}],[{"Borders":[1,0,1,0]},{"Borders":[1,0,1,1]},{"Borders":[1,0,1,0]},{"Borders":[1,0,1,1]}],[{"Borders":[1,0,0,0]},{"Borders":[0,1,0,1]},{"Borders":[0,0,0,0]},{"Borders":[0,1,1,1]}],[{"Borders":[1,0,1,1]},{"Borders":[1,1,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,1,1,1]}]],[[{"Borders":[1,1,1,0]},{"Borders":[1,1,0,0]},{"Borders":[0,1,0,0]},{"Borders":[0,1,1,1]}],[{"Borders":[1,0,0,1]},{"Borders":[0,0,1,0]},{"Borders":[1,0,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,1,0,1]},{"Borders":[0,0,0,0]},{"Borders":[0,1,1,0]},{"Borders":[1,0,1,0]}],[{"Borders":[1,1,0,1]},{"Borders":[0,0,1,1]},{"Borders":[1,0,1,1]},{"Borders":[1,0,1,1]}]],[[{"Borders":[1,1,0,0]},{"Borders":[0,1,1,1]},{"Borders":[1,1,0,0]},{"Borders":[0,1,1,1]}],[{"Borders":[1,0,1,0]},{"Borders":[1,1,0,1]},{"Borders":[0,0,0,0]},{"Borders":[0,1,1,1]}],[{"Borders":[1,0,1,0]},{"Borders":[1,1,1,0]},{"Borders":[1,0,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,0,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,1]}]]]
```
