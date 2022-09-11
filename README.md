# Maze Generator

Ce logiciel permet de générer des labyrinthes en suivant l'algorithme de Wilson.

Aperçu du logiciel :

![image](https://user-images.githubusercontent.com/56195432/189495376-b27eeff2-39d8-49d1-b75a-dc26b30413b8.png)

Le slider à gauche permet de régler la taille des bordures, celui de droite la vitesse de la génération 
Le checkbox "More Random" permet d'éviter d'avoir deux fois deux suites la même direction,
Le checkbox "Instant" permet de skip l'aperçu de la génération (pour une génération très rapide)

Aperçu d'une génération en cours :

![image](https://user-images.githubusercontent.com/56195432/189516468-006d33c1-c1c2-48ee-8d77-45b533b24a2d.png)

Le labyrinthe final généré : 

![image](https://user-images.githubusercontent.com/56195432/189516471-79f7f068-6ec3-437f-9a80-a74678338f81.png)

Un labyrinthe 100x100 (généré en 2 secondes) :

![image](https://user-images.githubusercontent.com/56195432/189495417-a4cb4f7b-f6e7-4f35-86b6-1802e52e2333.png)

![Labyrinthe 10 09 2022 19 43 48](https://user-images.githubusercontent.com/56195432/189495436-806d79d5-487a-47e1-a726-dedcdeafc3eb.png)

Génération de masse

![image](https://user-images.githubusercontent.com/56195432/189495456-00fab476-3f8e-4644-9608-61907507c33c.png)

Example de sortie json

```
[[[{"Borders":[1,1,1,0]},{"Borders":[1,1,1,0]},{"Borders":[1,1,1,0]},{"Borders":[1,1,1,0]}],[{"Borders":[1,0,1,0]},{"Borders":[1,0,1,0]},{"Borders":[1,0,0,0]},{"Borders":[0,0,1,0]}],[{"Borders":[1,0,1,0]},{"Borders":[1,0,1,0]},{"Borders":[1,0,1,0]},{"Borders":[1,0,1,1]}],[{"Borders":[1,0,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,1,1,1]}]],[[{"Borders":[1,1,0,1]},{"Borders":[0,1,0,0]},{"Borders":[0,1,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,1,0,0]},{"Borders":[0,0,1,1]},{"Borders":[1,1,0,0]},{"Borders":[0,0,1,0]}],[{"Borders":[1,0,1,1]},{"Borders":[1,1,0,1]},{"Borders":[0,0,1,0]},{"Borders":[1,0,1,0]}],[{"Borders":[1,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,1]},{"Borders":[1,0,1,1]}]],[[{"Borders":[1,1,0,1]},{"Borders":[0,1,0,0]},{"Borders":[0,1,1,0]},{"Borders":[1,1,1,0]}],[{"Borders":[1,1,0,0]},{"Borders":[0,0,1,1]},{"Borders":[1,0,0,0]},{"Borders":[0,0,1,1]}],[{"Borders":[1,0,1,0]},{"Borders":[1,1,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,0,1,1]},{"Borders":[1,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,1]}]],[[{"Borders":[1,1,1,0]},{"Borders":[1,1,0,0]},{"Borders":[0,1,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,0,0,0]},{"Borders":[0,0,1,1]},{"Borders":[1,1,0,0]},{"Borders":[0,0,1,1]}],[{"Borders":[1,0,1,0]},{"Borders":[1,1,0,0]},{"Borders":[0,0,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,0,1,1]},{"Borders":[1,0,1,1]},{"Borders":[1,1,0,1]},{"Borders":[0,0,1,1]}]],[[{"Borders":[1,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,1,1,0]},{"Borders":[1,1,1,0]}],[{"Borders":[1,1,0,0]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,0]},{"Borders":[1,0,1,0]}],[{"Borders":[1,0,1,0]},{"Borders":[1,1,0,1]},{"Borders":[0,0,0,0]},{"Borders":[0,0,1,1]}],[{"Borders":[1,0,1,1]},{"Borders":[1,1,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,1,1,1]}]],[[{"Borders":[1,1,1,0]},{"Borders":[1,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,0,0,0]},{"Borders":[0,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,0]}],[{"Borders":[1,0,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,1,1,1]},{"Borders":[1,0,1,0]}],[{"Borders":[1,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,1]}]],[[{"Borders":[1,1,1,0]},{"Borders":[1,1,0,0]},{"Borders":[0,1,1,0]},{"Borders":[1,1,1,0]}],[{"Borders":[1,0,1,0]},{"Borders":[1,0,1,1]},{"Borders":[1,0,1,0]},{"Borders":[1,0,1,0]}],[{"Borders":[1,0,1,0]},{"Borders":[1,1,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,0,1,0]}],[{"Borders":[1,0,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,1]}]],[[{"Borders":[1,1,0,0]},{"Borders":[0,1,1,0]},{"Borders":[1,1,0,0]},{"Borders":[0,1,1,0]}],[{"Borders":[1,0,1,0]},{"Borders":[1,0,1,1]},{"Borders":[1,0,1,0]},{"Borders":[1,0,1,1]}],[{"Borders":[1,0,0,0]},{"Borders":[0,1,0,1]},{"Borders":[0,0,0,0]},{"Borders":[0,1,1,1]}],[{"Borders":[1,0,1,1]},{"Borders":[1,1,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,1,1,1]}]],[[{"Borders":[1,1,1,0]},{"Borders":[1,1,0,0]},{"Borders":[0,1,0,0]},{"Borders":[0,1,1,1]}],[{"Borders":[1,0,0,1]},{"Borders":[0,0,1,0]},{"Borders":[1,0,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,1,0,1]},{"Borders":[0,0,0,0]},{"Borders":[0,1,1,0]},{"Borders":[1,0,1,0]}],[{"Borders":[1,1,0,1]},{"Borders":[0,0,1,1]},{"Borders":[1,0,1,1]},{"Borders":[1,0,1,1]}]],[[{"Borders":[1,1,0,0]},{"Borders":[0,1,1,1]},{"Borders":[1,1,0,0]},{"Borders":[0,1,1,1]}],[{"Borders":[1,0,1,0]},{"Borders":[1,1,0,1]},{"Borders":[0,0,0,0]},{"Borders":[0,1,1,1]}],[{"Borders":[1,0,1,0]},{"Borders":[1,1,1,0]},{"Borders":[1,0,0,1]},{"Borders":[0,1,1,0]}],[{"Borders":[1,0,0,1]},{"Borders":[0,0,0,1]},{"Borders":[0,1,0,1]},{"Borders":[0,0,1,1]}]]]
```
