# Projet Sudoku SAE 12 - IUT R. Schuman

Ce projet a été réalisé en collaboration avec Ali MHAIDIR dans le cadre de la SAE 12 à l'IUT R. Schuman. L'objectif était de développer plusieurs programmes capables de compléter une grille de sudoku en utilisant différentes approches algorithmiques. Le projet se divise en trois versions principales, chacune adoptant une stratégie différente pour la résolution de sudoku.

## Prérequis

Pour compiler et exécuter les programmes développés dans le cadre de ce projet, vous aurez besoin de Mono. Mono est un environnement d'exécution open source qui permet de faire tourner des programmes C# sur différentes plateformes.

## Approches de Résolution

### V1 : BackTracking

La première version utilise l'approche du BackTracking pour parcourir les différentes possibilités jusqu'à trouver une solution valide pour la grille de sudoku.

### V2 : Approche de l'Exclusion et de l'Unicité

La seconde version améliore le processus de résolution en intégrant des techniques d'exclusion et d'unicité. Ces techniques résolvent partiellement la grille, permettant ainsi de réduire le nombre de choix possibles avant d'appliquer le BackTracking.

### V3 : Combinaison des Méthodes

La version bonus, V3, combine toutes les méthodes précédentes (BackTracking, Unicité, et Exclusion) pour optimiser la résolution. Le BackTracking est utilisé uniquement lorsque cela est strictement nécessaire, ce qui rend l'approche plus efficace sur des grilles de sudoku complexes.

## Représentation des Grilles

Les grilles de sudoku sont représentées par des fichiers texte, nommés par exemple `simple.txt`, avec une structure spécifique où chaque chiffre est séparé par un espace, et les zéros représentent les cases vides

## Comment Utiliser

Pour compiler et exécuter un programme de résolution de sudoku, suivez ces étapes :

1. Placez-vous dans le répertoire contenant le programme à tester.
2. Compilez le code source en utilisant Mono. Par exemple, pour compiler le fichier `BackTracking.cs`, utilisez la commande suivante : mcs BackTracking.cs
3. Une fois la compilation réussie, exécutez le programme compilé avec la commande : mono BackTracking.exe

Répétez ces étapes pour tester les autres programmes en remplaçant `BackTracking` par le nom approprié du fichier source ou du programme compilé.

## Licence

Ce projet est placé dans le domaine public et est disponible sous la licence 0BSD, ce qui signifie que vous pouvez l'utiliser, le modifier et le distribuer sans aucune restriction et sans obligation de créditer l'auteur original.
---
Pour toute question ou suggestion, n'hésitez pas à ouvrir une issue ou à me contacter directement.
