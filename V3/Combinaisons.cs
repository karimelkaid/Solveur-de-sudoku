using System;
using System.Collections.Generic;
using System.IO;

class algo_ResolutionBackTracking
{
    public struct Region
    {
        public double ligne_debut;
        public double ligne_fin;
        public double colonne_debut;
        public double colonne_fin;
    }

    public struct Case
    {
        public int abscisse;
        public int ordonnee;
    }

    static void Main()
    {
        string nom_fichier_grille = "../simple.txt";
        int[,] grille = remplir_grille(nom_fichier_grille);
        
        // Appel de la procédure se chargeant de résoudre la grille
        ResousSudoku(grille);
    }

    /*
        remplir_grille : fonc :
            Remplit la grille à partir d'un fichier
        Paramètre :
            file_name : string : Nom du fichier à traiter
        Retour :
            res : int[,]
    */
    public static int[,] remplir_grille(string file_name)
    {
        int[,] res;
        
        StreamReader sr = File.OpenText(file_name);

        string[] nombres_sudoku = sr.ReadLine().Split(' ');
        
        // Récupération de la taille du sudoku pour instancier le tableau 2D
        double taille = Math.Sqrt(nombres_sudoku.Length);
        res = new int[(int)taille, (int)taille];
        
        // Ajout des nombres dans la grille :
        
        int pos_nombre = 0;     // La position du nombre dans la liste
        
        // Parcourt de la grille
        for (int i = 0; i < res.GetLength(0); i++)
        {
            for (int j = 0; j < res.GetLength(1); j++)
            {
                res[i, j] = int.Parse(nombres_sudoku[pos_nombre]);
                pos_nombre++;   // Nous passons au nombre suivant de la liste
            }
        }
        
        return res;
    }


    /*
        genere_grille_dans_html : proc :
            Génère dans le fichier HTML le code transformant la grille en version HTML, au bon endroit
        Paramètres :
            grille : int[,] : Grille de sudoku
            emplacement : string : Endroit du code HTML où nous voulons insérer la grille
            Xnum_grille : ref int : Numéro de la grille à créer (1 = Grille vide; 2 = Résolution partielle; 3 = Résolution complète)
    */
    public static void genere_grille_dans_html(int[,] grille, string emplacement, ref int Xnum_grille)
    {
        // Chemin du fichier HTML
        string fichier_html = "index.html";

        // Récupération de tout le code HTML dans une chaîne de caractères
        string code_html = File.ReadAllText(fichier_html);

        // Génération de la grille de sudoku en code HTML
        string grille_html = "<table class=\"g"+Xnum_grille+"\">\n";        // Chaîne où nous mettrons tout le code HTML représentant la grille
        grille_html += "\t<tbody>\n";

        double racine_taille_sudoku = Math.Sqrt(grille.GetLength(0));
        
        for (int i = 0; i < grille.GetLength(0); i++)
        {
            
            string class_tr = "";
            // Si nous sommes à la dernière ligne de la région et nous ne sommes pas à la dernière ligne --> nous mettons une bordure (en lui attribuant une classe) en bas de la case afin de séparer les régions
            if ((i+1) % racine_taille_sudoku == 0 && (i+1) != grille.GetLength(0))
            {
                class_tr = " class=\"ligne_fin_region\"";
            }

            grille_html += "\t\t<tr" + class_tr + ">\n";
            for (int j = 0; j < grille.GetLength(1); j++)
            {
                int case_actuelle = grille[i, j];
                
                string case_html;
                if (case_actuelle == 0)     // Si la case est vide, nous la représenterons avec un espace (une case vide)
                {
                    case_html = " ";
                }
                else
                {
                    case_html = case_actuelle.ToString();
                }

                string class_td = "";
                if ((j+1) % racine_taille_sudoku == 0 && (j+1) != grille.GetLength(0))
                {
                    class_td = " class=\"colonne_fin_region\"";
                }

                grille_html += "\t\t\t<td" + class_td + ">" + case_html + "</td>\n";
            }
            grille_html += "\t\t</tr>\n";
        }
        grille_html += "\t</tbody>\n";      // Fermeture du body du tableau
        grille_html += "</table class=\"g"+Xnum_grille+"\">\n";         // Fermeture du tableau


        code_html = efface_grille_si_elle_existe(code_html, Xnum_grille);       // Si la grille existe déjà, nous l'effaçons
        
        // Recherche de l'emplacement où nous voulons insérer le code html représentant la grille
        int index = code_html.IndexOf(emplacement);     // Récupération de la position du 1er caractère de là où nous voulons insérer la grille

        // Nous insérons le code de la grille au bon endroit dans le code HTML
        code_html = code_html.Insert(index, grille_html);

        // Nous remplaçons tout le contenu du fichier HTML par le nouveau code (qui contient la grille en version HTML)
        File.WriteAllText(fichier_html, code_html);

        Xnum_grille++;  // Pour mettre la bonne classe à la prochaine grille

    }
    
    
    /*
        efface_grille_si_elle_existe : fonc :
            Renvoie une chaîne représentant le code HTML sans les grilles si celles-ci sont présentent dans le code
        Paramètres :
            code_html : string : Code HTML
            num_grille_a_effacer : int : Numéro de la grille à effacer dans le code HTML
        Retour :
            res : string
    */
    public static string efface_grille_si_elle_existe(string code_html, int num_grille_a_effacer)
    {
        string res = code_html;
        
        // Recherche de l'emplacement l'ancienne grille afin de pouvoir l'écraser par la nouvelle :
        string deb = "<table class=\"g" + num_grille_a_effacer + "\">";
        int premiere_pos = code_html.IndexOf(deb);      // A partir de où nous voulons effacer

        string fin = "</table class=\"g" + num_grille_a_effacer + "\">";
        int derniere_pos = code_html.IndexOf(fin) + fin.Length;

        // Supprimez la grille de Sudoku précédemment insérée

        if (premiere_pos != -1)       // Si la grille existe dans le code html (car la fonction IndexOf renvoie -1 s'il ne trouve pas la chaîne demandée n'est pas trouvée)
        {
            int longueur_a_effacer = derniere_pos - premiere_pos;
            res = res.Remove(premiere_pos, longueur_a_effacer);
        }

        return res;

    }

    //--------------------------------------------------------------------------------------------------------------------------------
    /*
        ResousSudoku : proc :
            Fait toutes les vérfications nécessaires, puis résout le sudoku
        Paramètre :
            Xgrille : int[,] : Grille de sudoku
    */
    public static void ResousSudoku(int[,] Xgrille)
    {
        affiche_grille(Xgrille);

        // Si la grille n'a pas une forme carré, alors ce n'est pas une grille de sudoku
        if (!grille_carre(Xgrille))
        {
            Console.WriteLine("Cette grille n'est pas une grille de sudoku car elle n'est pas carrée.");
        }

        // Si la grille possède plusieurs fois le même nombre, alors la grille n'est pas solvable
        else if (repetition_nombre(Xgrille))
        {
            Console.WriteLine("Des nombres se répètent dans la grille donc ce sudoku n'est pas solvable.");
        }
        else if (nombre_non_autorise(Xgrille))
        {
            // Nous voulons trouver toutes les cases qui contiennent un nombre non autorisé sur cette grille, puis l'afficher à l'utilisateur (afin qu'il puisse résoudre le problème plus rapidement)
            List<string> coordonnees = trouve_cases_nombre_non_autorise(Xgrille);

            // Affichage de la 1ère case ayant un nombre non autorisé
            Console.Write("Nombre non autorisé dans la/les case(s) : ");
            Console.Write(coordonnees[0]);

            // S'il n'y a qu'une seule case possédant un tel nombre, si nous ne mettons pas cette condition le programme plantera, car vu que nous parcourons la liste à partir de la 2ème valeur, si elle n'existe pas, le programme plantera.
            if (coordonnees.Count > 1)
            {
                for (int i = 1; i < coordonnees.Count; i++) // i = 1 car nous avons déjà affiché la 1ère case.
                {
                    Console.Write("; " + coordonnees[i]);
                }
            }

            Console.WriteLine(); // Saut de ligne

        }
        else
        {
            bool technique_unicite = ResolutionUnicite(Xgrille);

            if (technique_unicite)
            {
                affiche_grille(Xgrille);
                Console.WriteLine("Grille complétée grâce à la technique de l'unicité.");
            }
            else if (ResolutionExclusion(Xgrille))
            {
                affiche_grille(Xgrille);
                Console.WriteLine("Grille complétée grâce à la technique de l'exclusion.");
            }
            else // Si la technique de l'unicité et de l'exclusion ne suffisent pas, on passe au backtracking (teste de toutes les possibilitées)
            {
                bool resolution = ResolutionBackTracking(Xgrille);

                if (resolution)
                {
                    affiche_grille(Xgrille);
                    Console.WriteLine("Grille complétée grâce à la technique du back tracking.");
                }
                else // Si la grille n'est pas résolu par la fonction, alors celle-ci n'est pas solvable
                {
                    Console.WriteLine("Cette grille de sudoku n'est pas solvable.");
                }

            }
        }

    }
    //--------------------------------------------------------------------------------------------------------------------------------

    
    
    //--------------------------------------------------------------------------------------------------------------------
    /*
    affiche_grille : proc :
        Affiche la grille passée en paramètre
    Paramètre :
        Xgrille : int[,] : Grille à afficher
    */
    public static void affiche_grille(int[,] Xgrille)
    {
        Console.WriteLine(
            "---------------------- Grille -------------------------------------------------------------");

        // Parcourt des cases de la grille
        for (int i = 0; i < Xgrille.GetLength(0); i++)
        {
            for (int j = 0; j < Xgrille.GetLength(1); j++)
            {
                Console.Write("\t" + Xgrille[i, j]); // Affichage de la case
            }

            // Saut de ligne une fois toute la ligne terminé
            Console.WriteLine();
        }

        Console.WriteLine(
            "-------------------------------------------------------------------------------------------");

    }
    //--------------------------------------------------------------------------------------------------------------------

    

    //--------------------------------------------------------------------------------------------------------------------
    /*
        grille_carre : fonc :
            Vérifie si la grille de sudoku fournit est un carrée
        paramètre :
            Xgrille : int[,] : grille de sudoku dont on souhaite vérifier si c'est un carré
        retour :
            res : bool
    */
    public static bool grille_carre(int[,] Xgrille)
    {
        bool res = false;

        int nb_lignes = Xgrille.GetLength(0);
        int nb_colonnes = Xgrille.GetLength(1);

        if (nb_lignes == nb_colonnes)
        {
            res = true;
        }

        return res;
    }
    //--------------------------------------------------------------------------------------------------------------------

    
    
    //--------------------------------------------------------------------------------------------------------------------
    /*
        repetition_nombre : fonc :
            Vérifie si la grille donnée contient une répétition de valeur
        Paramètre :
            Xgrille : int[,] : La grille dont on veut vérifier s'il y a répétition de valeurs
        Retour :
            res : bool
    */
    public static bool repetition_nombre(int[,] Xgrille)
    {
        bool res = false;

        bool
            trouve = false; // Dès que l'on trouvera une répétition d'un nombre, ce booléen nous permettra de casser la boucle

        // Parcourt de chaque case de la grille
        for (int i = 0; i < Xgrille.GetLength(0); i++)
        {
            for (int j = 0; j < Xgrille.GetLength(1) && trouve == false; j++)
            {
                // Nous vérifions la répétition d'un nombre seulement si la case n'est pas vide
                if (Xgrille[i, j] != 0)
                {
                    bool ligne_verifie = verifie_ligne(Xgrille, i, j, Xgrille[i, j]);
                    bool colonne_verifie = verifie_colonne(Xgrille, i, j, Xgrille[i, j]);
                    bool region_verifie = verifie_region(Xgrille, i, j, Xgrille[i, j]);

                    if (!(ligne_verifie && colonne_verifie && region_verifie)) // Si le nombre est répété sur la ligne ou la colonne ou la région
                    {
                        trouve = true;
                        res = true; // La grille n'est pas résolvable car il y a répétition d'un nombre
                    }
                }

            }
        }

        return res;

    }
    //--------------------------------------------------------------------------------------------------------------------

    

    //--------------------------------------------------------------------------------------------------------------------
    /*
        nombre_non_autorise : fonc :
            Vérifie s'il y a une case de la grille qui n'est pas dans l'interval des nombres autorisés.
        Paramètre :
            Xgrille : int[,] : Grille de sudoku
        Retour :
            res : bool
    */
    public static bool nombre_non_autorise(int[,] Xgrille)
    {
        bool res = false;

        bool
            trouve = false; // Dès que l'on trouvera un nombre dépassant le nombre maximum autorisé, ce booléen nous permettra de casser la boucle

        int nb_lignes = Xgrille.GetLength(0);
        // Parcourt de chaque case de la grille
        for (int i = 0; i < Xgrille.GetLength(0); i++)
        {
            for (int j = 0; j < Xgrille.GetLength(1) && trouve == false; j++)
            {
                // Nous vérifions la valeur d'un nombre seulement si la case n'est pas vide
                if (Xgrille[i, j] != 0)
                {
                    // Si la valeur de la case n'est pas compris dans le bon interval des nombres autorisés
                    if (!(Xgrille[i, j] >= 1 && Xgrille[i, j] <= nb_lignes))
                    {
                        res = true;
                    }
                }

            }
        }

        return res;
    }
    //--------------------------------------------------------------------------------------------------------------------



    //--------------------------------------------------------------------------------------------------------------------
    /*
        trouve_cases_nombre_non_autorise : fonc :
            Trouve toutes les cases où le nombre à l'intérieur n'est pas autorisé --> interval =  [1;nombre de lignes]
        paramètre :
            Xgrille : int[,] : Grille de sudoku
        retour :
            res : List<string>
    */
    public static List<string> trouve_cases_nombre_non_autorise(int[,] Xgrille)
    {
        List<string> res = new List<string>();

        int taille_sudoku = Xgrille.GetLength(0);
        // Parcourt de chaque case de la grille
        for (int i = 0; i < Xgrille.GetLength(0); i++)
        {
            for (int j = 0; j < Xgrille.GetLength(1); j++)
            {
                // Nous vérifions la valeur d'une case seulement si la case n'est pas vide
                if (Xgrille[i, j] != 0)
                {
                    // Si la valeur de la case n'est pas compris dans le bon interval
                    if (!(Xgrille[i, j] >= 1 && Xgrille[i, j] <= taille_sudoku))
                    {
                        string coordonnees_case = i + "," + j;
                        res.Add(coordonnees_case);
                    }
                }

            }
        }

        return res;
    }
    //--------------------------------------------------------------------------------------------------------------------



    //--------------------------------------------------------------------------------------------------------------------
    /*
        Resous_grille_technique_unicite : fonc
            Complète la grille de sudoku grâce à la méthode de l'unicité de candidat : si nous pouvons insérer qu'une seule valeur dans une case, alors cette valeur est forcément la bonne
        Paramètre :
            Xgrille : int[,] : La grille de sudoku à résoudre
        Retour :
            res : bool
    */
    public static bool ResolutionUnicite(int[,] Xgrille)
    {
        bool res;

        List<int> candidats = new List<int>(); // Liste où seront stockés tous les candidats de la case à examiner

        int nb_lignes = Xgrille.GetLength(0); // Le nombre de lignes du sudoku

        int i = 0;
        int j = 0;
        bool resolu = false;
        bool
            blocage = false; // La technique de l'unicité de candidat ne fonctionne pas sur certaines grilles de niveau plus avancées, il est donc possible que nous soyons bloqués à un moment donné

        // Parcourt de chaque case vide (nous mettons une boucle while car nous allons incrémenter/décrémenter de manière différente suivant les cas de figure)
        while (i < Xgrille.GetLength(0) && resolu == false && blocage == false) // Parcourt de chaque lignes
        {
            while (j < Xgrille.GetLength(1) && resolu == false && blocage == false) // Parcourt de chaque colonne
            {
                int case_actuelle = Xgrille[i, j];

                candidats.Clear(); // Vidage de la liste de candidats car nous n'avons pas encore examiné les candidats pour cette case

                if (case_actuelle == 0) // Si la case est vide
                {
                    for (int nombre = 1;
                         nombre <= nb_lignes;
                         nombre++) // Nous testons chaque nombre possible sur cette grille
                    {
                        // Un nombre est un candidat seulement s'il n'est pas présent sur la ligne, la colonne ou la région

                        bool ligne_valide = verifie_ligne(Xgrille, i, j, nombre); // Vérification sur la ligne
                        bool colonne_valide = verifie_colonne(Xgrille, i, j, nombre); // Vérification sur la colonne
                        bool region_valide = verifie_region(Xgrille, i, j, nombre); // Vérification sur la région

                        if (ligne_valide && colonne_valide && region_valide) // Si le nombre est un candidat
                        {
                            candidats.Add(nombre); // Ajout du nombre dans la liste de candidats
                        }
                    }

                    // S'il n'y a qu'un seul candidat pour cette case, alors ce candidat est forcément le bon
                    if (candidats.Count == 1)
                    {
                        Xgrille[i, j] =
                            candidats
                                [0]; // S'il n'y a qu'une valeur, alors celle-ci se trouve forcément à la 1ère position de la liste


                        // Nous vérifions si la grille est complète
                        if (grille_complete(Xgrille))
                        {
                            resolu = true; // Nous avons résolu le sudoku
                        }

                        // Nous devons revenir à la 1ère case de la grille car cet ajout a probablement créé d'autres solutions uniques
                        i = 0;
                        j = 0;
                    }
                    // S'il y a plusieurs possibilité, alors nous allons regarder la prochaine case vide
                    else
                    {
                        if (!derniere_case_vide(Xgrille, i, j))
                        {
                            prochaine_case(Xgrille, ref i, ref j);
                        }
                        else // Si nous sommes à la dernière case vide de la grille
                        {
                            if (!grille_complete(
                                    Xgrille)) // Si nous sommes à la dernière case vide de la grille DE BASE ET il y a encore des cases vides --> alors la technique de l'unicité ne suffit pas (nous sommes donc bloqués)
                            {
                                blocage = true;
                            }
                        }
                    }

                }
                else // Sinon si la case est déjà renseignée, nous passons à la prochaine case
                {
                    prochaine_case(Xgrille, ref i, ref j);
                }
            }
        }

        if (blocage) // Après avoir tenté de compléter la grille, si nous sommes bloqués, alors la grille cette technique de résolution ne sufiit pas
        {
            res = false;
        }
        else // Sinon nous n'avons pas été bloqués, alors la grille est résolue.
        {
            res = true;
        }

        return res; // Retour du résultat
    }
    //--------------------------------------------------------------------------------------------------------------------

    

    //--------------------------------------------------------------------------------------------------------------------

    /*
    derniere_case_vide : fonc :
        Renvoie un booléen disant si les coordonnées fournis en paramètre représentent la dernière case vide de la grille de sudoku
    paramètres :
        grille : int[,] : Grille de sudoku
        ligne : int : Numéro de la ligne de la case à étudier
        colonne : int : Numéro de la colonne de la case à étudier
    retour :
        res : bool
*/
    public static bool derniere_case_vide(int[,] grille, int ligne, int colonne)
    {
        bool res = false;


        // Il faut d'abord récupérer les coordonnées de la dernière case vide, puis les comparer avec les coordonnées fournis en paramètre


        // -----------------------------------------------------------

        // Récupération des coordonnées de la dernière case vide :
        int ligne_derniere_case_vide = 0;
        int colonne_derniere_case_vide = 0;

        for (int i = 0; i < grille.GetLength(0); i++) // Parcourt de chaque lignes
        {
            for (int j = 0; j < grille.GetLength(1); j++) // Parcourt de chaque colonne
            {
                if (grille[i, j] == 0) // Nous récupérons les coordonnées seulement si la case est vide
                {
                    ligne_derniere_case_vide = i;
                    colonne_derniere_case_vide = j;
                }
            }
        }
        // -----------------------------------------------------------


        // -----------------------------------------------------------

        // Comparaison avec les coordonnées fournis en paramètre :

        if (ligne == ligne_derniere_case_vide && colonne == colonne_derniere_case_vide)
        {
            res = true;
        }

        // -----------------------------------------------------------

        return res;
    }
    //--------------------------------------------------------------------------------------------------------------------

    

    //------------------------------------------------------------------------------------------------------------------
    /*
        ResousExclusion : proc :
            Tente (ne suffira peut-être pas) de compléter la grille de sudoku avec la méthode de l'exclusion : Pour chaque région, nous regardons pour chaque nombre si celui-ci est insérable qu'à UNE SEULE case de la région --> si oui, nous l'insérons car c'est forcément la bonne case, sinon nous regardons le prochain nombre
        Paramètre :
            Xgrille : int[,] : Grille de sudoku à résoudre
    */
    public static bool ResolutionExclusion(int[,] Xgrille)
    {
        bool res;
        
        bool resolu = false;
        bool blocage = false;
        
        List<int[,]> regions = recup_regions(Xgrille);      // Nous récupérons toutes les régions de la grille dans une liste afin de les parcourir

        int nb_max_possible = Xgrille.GetLength(0);
        
        bool remplissage;

        while ( resolu == false && blocage == false )   // Nous continuons cette méthode de tant que la grille n'a pas été résolu et on est pas bloqué
        {
            remplissage = false;    // Il faut le remettre à false car à chaque fois que nous insérons un nombre il se met à true
            
            for ( int i=0; i<regions.Count; i++ )       // Parcourt de toutes les régions
            {
                int[,] region = regions[i];
                
                for (int nb_a_tester = 1; nb_a_tester <= nb_max_possible; nb_a_tester++)        // Teste de tous les nombres possibles
                {
                    if (!nombre_present_dans_la_region(region , nb_a_tester))       // Si le nombre n'est pas dans la région, alors nous effectuons le blocage (nous comptons le nombre de case de la région où il est possible d'insérer le nombre)
                    {
                        int num_region = i;
                        bool[,] tab_cases_bloquees = bloque(Xgrille, region, num_region,  nb_a_tester);

                        List<Case> cases_non_bloquees = trouve_cases_non_bloquees(tab_cases_bloquees);

                        // S'il n'y a qu'une seule case où il est possible d'insérer le nombre, nous l'ajoutons dedans
                        if ( cases_non_bloquees.Count == 1 )
                        {
                            // Récupération des coordonnées (DAND LA RÉGION) de la case où ajouter le nombre
                            int ligne_case = cases_non_bloquees[0].abscisse;
                            int colonne_case = cases_non_bloquees[0].ordonnee;

                            // Ajout du nombre au bon endroit de la grille
                            ajoute_nombre_dans_grille(Xgrille, num_region, ligne_case, colonne_case, nb_a_tester);
                            region[ligne_case, colonne_case] = nb_a_tester;     // Il faut également mettre à jour la liste des régions pour les prochains passages

                            // Nous venons d'ajouter un nombre dans la grille donc nous le mettons à true
                            remplissage = true;
                        }
                    }
                }
            }
            
            // Une fois que toutes les régions ont été parcourues, nous regardons s'il y a eu un changement : si oui, nous continuons cette méthode, sinon nous arrêtons ici
            // Si la grille est complète, 
            if (grille_complete(Xgrille))
            {
                resolu = true;
            }
            else if ( !grille_complete(Xgrille) && remplissage== false )
            {
                blocage = true;
            }
        }


        if (resolu)
        {
            res = true;
        }
        else
        {
            res = false;
        }

        return res;
    }

    //------------------------------------------------------------------------------------------------------------------

    
    
        //--------
    /*
        recup_regions : fonc :
            Renvoie une liste contenant toutes les régions de la grille de sudoku
        Paramètre :
            grille : int[,] : Grille de sudoku à compléter
        Retour :
            res : List<int[,]>
    */
    public static List<int[,]> recup_regions(int[,] grille)
    {
        List<int[,]> res = new List<int[,]>();      // Déclaration + instanciation de la liste contenant les régions

        List<int> temp = new List<int>();       // Liste d'entiers où nous stockerons les nombres de la région que nous étudierons

        double racine_de_taille_sudoku = Math.Sqrt(grille.GetLength(0));
        
        double ligne_debut_region = 0;         // Déclaration + initialisation direct de la ligne du début, de la région à examiner
        double ligne_fin_region = racine_de_taille_sudoku;         // Déclaration + initialisation direct de la ligne de la fin, de la région à examiner

        double colonne_debut_region = 0;         // Déclaration + initialisation direct de la colonne du début, de la région à examiner
        double colonne_fin_region = racine_de_taille_sudoku;         // Déclaration + initialisation direct de la colonne de la fin, de la région à examiner

        while (ligne_fin_region <= 9)       // Boucle : Tant que nous n'avons pas parcouru toutes les lignes de la régions
        {
            while (colonne_fin_region <= 9)         // Boucle : Tant que nous n'avons pas parcouru toutes les colonnes de la régions
            {
                int[,] t = new int[(int)racine_de_taille_sudoku, (int)racine_de_taille_sudoku];       // Nous voulons un tableau 2D pour chaque nouvelle région


                // Nous récupérons tous les nombres de la région dans la liste "temp"
                for (int i = (int)ligne_debut_region; i < ligne_fin_region; i++)
                {
                    for (int j = (int)colonne_debut_region; j < colonne_fin_region; j++)
                    {
                        temp.Add(grille[i, j]);
                    }
                }

                
                // Une fois tous les nombres de la région récupérés dans la liste "temp", il faut les ajouter dans le tableau 2D (qui représente la région
                
                int a = 0;      // Va nous permettre de récupérer le bon nombre à la bonne position dans la liste "temp"
                
                // Nous mettons les nombres récupérés dans le tableau de la région
                for (int i = 0; i < racine_de_taille_sudoku; i++)
                {
                    for (int j = 0; j < racine_de_taille_sudoku; j++)
                    {
                        t[i, j] = temp[a];
                        a++;        // Nous passons à la prochaine valeur de la liste
                    }
                }
                
                res.Add(t);         // Nous ajoutons la région dans la liste des régions

                temp.Clear();       // Nous avons déjà sauvegardé la région donc nous pouvons effacer la liste.
                
                // Nous passons à la prochaine région de la même ligne
                colonne_debut_region += racine_de_taille_sudoku;
                colonne_fin_region += racine_de_taille_sudoku;
            }
            
            // Une fois arrivé ici, nous avons étudié toutes les régions de la ligne actuelle, donc nous passons à la prochaine ligne
            ligne_debut_region += racine_de_taille_sudoku;
            ligne_fin_region += racine_de_taille_sudoku;
            colonne_fin_region = racine_de_taille_sudoku;
            colonne_debut_region = 0;
        }
        
        return res;         // Retour du résultat
    }
    //-------
    
    
    
    //--------------------------
    /*
    nombre_present_dans_la_region : fonc :
        Vérifie si le nombre passé en paramètre se trouve dans la région (également passée en paramètre)
    Paramètres :
        Xregion : int[,] : La région
        Xnombre : int : Le nombre à vérifier
*/
    public static bool nombre_present_dans_la_region(int[,] Xregion, int Xnombre)
    {
        // Nous déclarons et initialisons le résultat à false car nous n'avons pas encore trouvé
        bool res = false;

        bool trouve = false;

        // Parcourt de la région
        for (int i = 0; i < Xregion.GetLength(0) && trouve == false; i++)
        {
            for (int j = 0; j < Xregion.GetLength(1) && trouve==false; j++)
            {
                if (Xregion[i, j] == Xnombre)       // Si la case de la région vaut le nombre --> nous arrêtons de chercher et nous renvoyons true
                {
                    trouve = true;

                    res = true;
                }
            }
        }

        return res;
    }

    //--------------------------
    
    
    
    //-------------------------------
        /*
        bloque : fonc :
            Renvoie un tableau de booléen disant quelles cases de la région en paramètre sont bloquées par le nombre (en pramètre également)
        Paramètres :
            Xgrille : int[,] : Grille de sudoku
            Xregion : int[,] : Région du sudoku où effectuer le blocage du nombre
            Xnombre : int : Nombre avec lequel nous allons bloquer les cases
    */
    public static bool[,] bloque(int[,] Xgrille, int[,] Xregion, int Xnum_region, int Xnombre)
    {
        int nombre_de_lignes = Xregion.GetLength(0);
        int nombre_de_colonnes = Xregion.GetLength(1);
        bool[,] res = new bool[nombre_de_lignes,nombre_de_colonnes];    // Étant donné que nous représentons chaque case de la région, alors le tableau aura les mêmes dimensions que la région

        
        // Parcourt de chaque case de la région
        for (int i = 0; i < nombre_de_lignes; i++)
        {
            for (int j = 0; j < nombre_de_colonnes; j++)
            {
                if ( Xregion[i,j] != 0 )           // De base, aucune case n'est bloqué sauf celles qui sont déjà remplies
                {
                    res[i, j] = true;
                }
                else
                {
                    res[i, j] = false;
                }
            }
        }
        
        
        // Dorénavent, nous allons effectuer les blocages
        // Parcourt de la région
        for (int i = 0; i < nombre_de_lignes; i++)
        {
            for (int j = 0; j < nombre_de_colonnes; j++)
            {
                if (res[i, j] == false)     // Nous examinons seulement les cases qui ne sont pas bloquées
                {
                    // Pour faire appel à la fonction qui vérifie si un nombre est présent sur la ligne, nous avons besoin de connaître les coordonnées de la case que nous analysons dans la GRILLE
                    
                    // Récupération des coordonnées de la case dans la grille :
                    double[] coordonnees_dans_la_grille = recup_coordonnees_dans_la_grille(Xgrille, Xnum_region, i, j);
                    int ligne_dans_la_grille = (int)coordonnees_dans_la_grille[0];
                    int colonne_dans_la_grille = (int)coordonnees_dans_la_grille[1];
                    
                    // Maintenant que nous avons les bonnes coordonnées dans la grille, nous pouvons appeler les fonctions qui vérifient si un nombre est présent sur la ligne et la colonne
                    bool verification_ligne = verifie_ligne(Xgrille, ligne_dans_la_grille, colonne_dans_la_grille, Xnombre);
                    bool verification_colonne = verifie_colonne(Xgrille, ligne_dans_la_grille, colonne_dans_la_grille, Xnombre);

                    if (!verification_ligne || !verification_colonne)       // Si dans la grille, il y a le même nombre dans la ligne ou le même nombre dans la colonne --> nous bloquons la case
                    {
                        res[i, j] = true;
                    }

                }
            }
        }

        return res;
    }
    //-------------------------------
    
    
    
    //----------------------------------------
    /*
    trouve_cases_non_bloquees : fonc :
        Renvoie une liste des cases non bloquées de la région
    Paramètre :
        Xtab_cases_bloquees : bool[,] : Région contenant les cases bloquées et non bloquées
    Retour :
        res : List<string>
*/
    public static List<Case> trouve_cases_non_bloquees(bool[,] Xtab_cases_bloquees)
    {
        // Déclaration + instanciation du résultat à renvoyer
        List<Case> res = new List<Case>();

        // Parcourt du tableau des cases de la région
        for (int i = 0; i < Xtab_cases_bloquees.GetLength(0); i++)
        {
            for (int j = 0; j < Xtab_cases_bloquees.GetLength(1); j++)
            {
                // Si la case n'est pas bloquée --> nous l'ajoutons dans le résultat
                if (Xtab_cases_bloquees[i, j] == false)
                {
                    Case c;
                    c.abscisse = i;
                    c.ordonnee = j;
                    
                    res.Add(c);
                }
            }
        }
        
        return res;
    }
    //----------------------------------------
    
    
    
    //---------------------------------
    /*
    ajoute_nombre_dans_grille : proc :
        À partir du numéro de la région et des coordonnées d'une case dans cette région, cette procédure ajoute le nombre au bon endroit dans la grille
    Paramètres :
        Xgrille : int[,] : Grille de sudoku
        Xnum_region : int : Numéro de la région
        Xligne_case : int : Ligne de la case dans la région (par exemple pour une grille 9x9; la ligne maximum sera 2 (0; 1 et 2)
        Xcolonne_case : int : Colonne de la case dans la région (par exemple pour une grille 9x9; la ligne maximum sera 2 (0; 1 et 2)
        Xnombre_a_ajouter : int : Le nombre à ajouter dans la case

*/
    public static void ajoute_nombre_dans_grille(int[,] Xgrille, int Xnum_region, int Xligne_case, int Xcolonne_case, int Xnombre_a_ajouter)
    {

        // Récupération des coordonnées
        double[] coordonnees_dans_la_grille = recup_coordonnees_dans_la_grille(Xgrille, Xnum_region, Xligne_case, Xcolonne_case);

        int ligne_correct = (int)coordonnees_dans_la_grille[0];
        int colonne_correct = (int)coordonnees_dans_la_grille[1];
        
        // Ajout
        Xgrille[ligne_correct, colonne_correct] = Xnombre_a_ajouter;

    }
    //-------------------------------
    
    
    
    //------------------------
        /*
        recup_coordonnees_dans_la_grille : fonc :
            À partir d'une case (ligne, colonne) de la région passée en paramètre, cette fonction trouve la case exact dans la grille de sudoku
        Paramètres :
            Xgrille : int[,] : Grille de sudoku
            Xnum_region : int : Numéro de la région (compté de la gauche vers la droite)
            Xligne_case : int : Ligne de la case dans la région (par exemple pour une grille 9x9; la ligne maximum sera 2 (0; 1 et 2)
            Xcolonne_case : int : Colonne de la case dans la région (par exemple pour une grille 9x9; la ligne maximum sera 2 (0; 1 et 2)
    */
    public static double[] recup_coordonnees_dans_la_grille(int[,] Xgrille, int Xnum_region, int Xligne_case, int Xcolonne_case)
    {
        // Déclaration + instanciation du résultat
        double[] res = new double[2];
        
        
        /* Pour récupérer les coordonnées dans la grille, il faut récupérer les coordonnées de la 1ère case de la région
         où ajouter le nombre, puis additionner les lignes ensemble et les colonnes ensemble pour trouver la bonne case.*/

        int nb_lignes = Xgrille.GetLength(0);
        double racine_de_nb_lignes = Math.Sqrt(nb_lignes);
        
        // -------------------------------------------------------------------------------
        // Récupération des coordonnées de la 1ère case de la région où ajouter le nombre :

        int num_region_actuelle = 0;
        
        double ligne_debut_region = 0;
        double ligne_fin_region = racine_de_nb_lignes;

        double colonne_debut_region = 0;
        double colonne_fin_region = racine_de_nb_lignes;

        bool trouve = false;

        // Parcourt de la 1ère case de toutes les régions (de la gauche vers la droite en partant du haut) jusqu'à trouver la région concernée (passé en paramètre)
        while (ligne_fin_region <= nb_lignes && trouve == false)
        {
            while (colonne_fin_region <= nb_lignes && trouve == false)
            {
                //Console.WriteLine("Num région : "+num_region_actuelle); // POUR TESTER

                if (num_region_actuelle == Xnum_region)
                {
                    trouve = true;
                }
                else
                {
                    colonne_debut_region += racine_de_nb_lignes;
                    colonne_fin_region += racine_de_nb_lignes;
                    
                    num_region_actuelle++;
                }
                
            }

            if (trouve == false)    // Nous changeons de ligne seulement si nous n'avons pas encore trouvé la bonne région
            {
                
                ligne_debut_region += racine_de_nb_lignes;
                ligne_fin_region += racine_de_nb_lignes;

                colonne_fin_region = racine_de_nb_lignes;
                colonne_debut_region = 0;
            }
        }
        // -------------------------------------------------------------------------------

        
        // -------------------------------------------------------------------------------
        // Coordonnées exacts de la où ajouter la valeur :

        double ligne_correct = ligne_debut_region + Xligne_case;
        double colonne_correct = colonne_debut_region + Xcolonne_case;

        res[0] = ligne_correct;
        res[1] = colonne_correct;

        // -------------------------------------------------------------------------------
        
        return res;
    }
    //-------------------------



    //--------------------------------------------------------------------------------------------------------------------
    /*
        Resolution_sudoku_backtracking : fonc
            Résous la grille de sudoku grâce à la méthode du backtracking ( = "retour sur trace" ). Cette technique de résolution consiste à tester toutes les possibilitées dans l'ordre jusqu'à que la grille soit résolue ou qu'il n'y ait plus aucune solution.
        Paramètre :
            Xgrille : int[,] : La grille de sudoku à résoudre
        Retour :
            res : bool
    */
    public static bool ResolutionBackTracking(int[,] Xgrille)
    {
        bool res = false; // Ce booléen sert à savoir si la grille a été résolu ou pas

        int
            ancienne_valeur_de_la_case =
                0; // Cette variable nous servira à stocker l'ancienne valeur qu'il y avait dans une case avant de revenir dessus, afin de tester les autres valeurs qui n'ont pas encore été testé 
        List<string>
            cases_remplies =
                new List<string>(); // Liste contenant toutes les cases (ligne,colonne,valeur de la case) où nous avons inséré une valeur

        bool resolu = false; // Ce booléen nous permettra d'arrêter si nous avons trouvé la solution
        bool
            impossible =
                false; // Ce booléen nous permettra d'arrêter si nous avons testé toutes les possibilitées et nous n'avons trouvé aucune solution

        int i = 0; // Ligne où l'on se situe dans la grille de sudoku
        int j = 0; // Colonne où l'on se situe dans la grille de sudoku

        while (i < Xgrille.GetLength(0) && resolu == false && impossible == false) // Parcourt de chaque ligne
        {
            while (j < Xgrille.GetLength(1) && resolu == false && impossible == false) // Parcourt de chaque colonne
            {
                int valeur_case = Xgrille[i, j];
                if (valeur_case == 0) // Si la case est vide
                {
                    bool candidat_existant = false;
                    bool nombre_a_inserer_trouve =
                        false; // Ce booléen nous permettra d'arrêter de chercher un candidat dès que nous en aurons trouvé un

                    for (int nombre_a_inserer = 1;
                         nombre_a_inserer <= Xgrille.GetLength(0) && nombre_a_inserer_trouve == false;
                         nombre_a_inserer++) // Parcourt de tous les nombres possibles sur cette grille
                    {
                        if (nombre_a_inserer >
                            ancienne_valeur_de_la_case) // Quand nous revenons à la case précédente car nous étions bloqués à l'étape précédente, il faut commencer à tester à partir de la valeur juste après l'ancien nombre qu'il y avait avant (par exemple, s'il y avait un 3 nous commencerons à 4)
                        {
                            bool ligne_valide = verifie_ligne(Xgrille, i, j, nombre_a_inserer);
                            bool colonne_valide = verifie_colonne(Xgrille, i, j, nombre_a_inserer);
                            bool region_valide = verifie_region(Xgrille, i, j, nombre_a_inserer);

                            if (ligne_valide && colonne_valide &&
                                region_valide) // Si le nombre est un candidat ( = s'il n'est pas présent sur la ligne, la colonne ou la région)
                            {
                                nombre_a_inserer_trouve = true;
                                candidat_existant = true;

                                Xgrille[i, j] = nombre_a_inserer; // Nous insérons le nombre dans la case

                                // Nous sauvegardons la case (ligne,colonne,valeur de la case) que nous venons de renseigner afin de revenir dessus si plus tard nous sommes bloqués
                                string infos_case = i + "," + j + "," + nombre_a_inserer;
                                cases_remplies.Add(infos_case);

                                ancienne_valeur_de_la_case =
                                    0; // Etant donné que nous venons de remplir une case, nous allons passer à la prochaine case vide et celle-ci n'ayant pas d'ancienne valeur, nous mettons cette varaible à 0 (si nous ne mettons pas cette assignation, le programme testera des nombres à partir du mauvais endroit pour la prochaine case vide, voir ligne 633)
                            }
                        }
                    }

                    if (candidat_existant) // S'il y a au moins un candidat (que nous avons forcément inséré), alors nous passons à la prochaine case
                    {
                        prochaine_case(Xgrille, ref i, ref j);
                    }
                    else // Si la case vide que nous venons d'étudier n'a aucun candidat --> 2 issues : soit nous revenons à la case précédente où nous avons émis une hypothèse (seulement si elle existe), soit nous ne pouvons pas revenir plus en arrière et la grille est donc non résolvable
                    {
                        if (cases_remplies.Count >
                            0) // S'il y a au moins une case où nous avons émis une hypothèse --> on la remet à vide
                        {
                            // Nous récupérons les coordonnées de la dernière case où nous avons émis une hypothèse
                            int derniere_pos = cases_remplies.Count - 1;
                            string[] t = cases_remplies[derniere_pos].Split(',');
                            cases_remplies
                                .RemoveAt(
                                    derniere_pos); // Il faut enlever la case car on vient de l'utiliser pour revenir dessus

                            // Une fois les coordonnées récupérées, nous revenons à cette case
                            i = int.Parse(t[0]);
                            j = int.Parse(t[1]);

                            ancienne_valeur_de_la_case = int.Parse(t[2]); // Afin de tester à partir du bon nombre

                            Xgrille[i, j] =
                                0; // Nous remettons la case à vide car l'ancienne valeur qu'il y avait dedans ne fonctionne pas
                        }
                        else // Sinon s'il n'y a aucune case sur laquelle nous pouvons revenir dessus --> alors cette grille n'est pas solvable
                        {
                            impossible = true;
                        }
                    }
                }
                else // Sinon si la case est déjà renseignée
                {
                    // Nous passons à la case suivante
                    prochaine_case(Xgrille, ref i, ref j);
                }

                // Avant de passer à la prochaine case, nous vérifions si la grille est complète
                bool sudoku_termine = grille_complete(Xgrille);
                if (sudoku_termine)
                {
                    resolu = true; // Si la grille est complète, alors nous avons résolu le sudoku
                    res = true; // Sudoku résolu
                }
            }
        }


        return res;
    }
    //--------------------------------------------------------------------------------------------------------------------

    

    //--------------------------------------------------------------------------------------------------------------------
    /*
        prochaine_case : proc
            Incrémente la ligne et la colonne de sorte à passer à la prochaine case de la grille de sudoku
        Paramètres :
            Xgrille : int[,] : Grille de sudoku
            ligne : ref int : adresse de la ligne où l'on se situe dans la grille
            colonne : ref int : adresse de la colonne où l'on se situe dans la grille
    */
    public static void prochaine_case(int[,] Xgrille, ref int ligne, ref int colonne)
    {
        if (colonne == Xgrille.GetLength(0) - 1) // Si nous sommes à la dernière colonne de la ligne
        {
            ligne++; // Nous allons à la ligne suivante
            colonne = 0; // Nous revenons à la 1ère colonne
        }
        else // Sinon si nous ne sommes pas à la dernière colonne de la ligne
        {
            colonne++; // Nous passons à la colonne suivante
        }
    }
    //--------------------------------------------------------------------------------------------------------------------



    //--------------------------------------------------------------------------------------------------------------------
    /*
        grille_complete : fonc
            Vérifie si la grille est totalement complète
        Paramètre :
            XXgrille : int[,] : grille de sudoku
        Retour :
            res : bool
    */
    public static bool grille_complete(int[,] XXgrille)
    {
        bool res = true; // De base, nous considérons que la grille est complète
        bool
            trouve = false; // Ce booléen nous servira à sortir du parcourt de la grille dès que nous trouverons UNE SEULE case vide

        // Parcourt de la grille
        for (int i = 0; i < XXgrille.GetLength(0) && trouve == false; i++)
        {
            for (int j = 0; j < XXgrille.GetLength(1) && trouve == false; j++)
            {
                if (XXgrille[i, j] ==
                    0) // Si la case est vide, alors la grille n'est pas complète et nous avons trouvé la réponse
                {
                    res = false;
                    trouve = true;
                }
            }
        }

        return res; // Retour du résultat
    }
    //--------------------------------------------------------------------------------------------------------------------


    
    //--------------------------------------------------------------------------------------------------------------------
    /*
        verifie_ligne : fonc :
            Vérifie s'il le nombre passé en paramètre n'est pas présent sur la ligne de la case passé en paramètre
        Paramètres :
            grille : int[,] : Grille de sudoku
            Xi : int : Ligne de la case où l'on veut vérifier la ligne
            Xj : int : Colonne de la case où l'on veut vérifier la ligne
            Xentier_verifier : int : L'entier dont nous voulons vérifier la non-présence sur la ligne
        Retour :
            res : bool
    */
    public static bool verifie_ligne(int[,] grille, int Xi, int Xj, int Xentier_verifier)
    {
        bool
            res = true; // De base, nous considérons que nous pouvons insérer l'entier (le nombre n'est pas présent sur la ligne)

        bool trouve = false; // Nous servira à casser la boucle

        // Parcourt de chaque valeur de la ligne où se situe la case
        for (int col = 0; col < grille.GetLength(1) && trouve == false; col++)
        {
            // Pas la peine de vérifier la case où l'on veut insérer une valeur
            if (!(col == Xj))
            {
                if (grille[Xi, col] ==
                    Xentier_verifier) // Si la case de la ligne contient le nombre dont nous voulons vérifier la validité, alors nous ne pouvons pas l'insérer
                {
                    res = false;
                    trouve = true;
                }
            }

        }

        return res;
    }
    //--------------------------------------------------------------------------------------------------------------------

    

    //--------------------------------------------------------------------------------------------------------------------
    /*
        verifie_colonne : fonc :
            Vérifie s'il le nombre passé en paramètre n'est pas présent sur la colonne de la case passé en paramètre
        Paramètres :
            grille : int[,] : Grille de sudoku
            Xi : int : Ligne de la case où l'on veut vérifier la ligne
            Xj : int : Colonne de la case où l'on veut vérifier la ligne
            Xentier_verifier : int : L'entier dont nous voulons vérifier la non-présence sur la colonne
        Retour :
            res : bool
    */

    public static bool verifie_colonne(int[,] grille, int Xi, int Xj, int Xentier_verifier)
    {
        bool
            res = true; // De base, nous considérons que nous pouvons insérer l'entier (le nombre n'est pas présent sur la colonne)

        bool trouve = false; // Nous servira à casser la boucle

        // Parcourt de chaque valeur de la ligne où se situe la case
        for (int i = 0; i < grille.GetLength(0) && trouve == false; i++)
        {
            // Pas la peine de vérifier la case où l'on veut insérer une valeur
            if (!(i == Xi))
            {
                int entier_actuel = grille[i, Xj];

                if (entier_actuel == Xentier_verifier)
                {
                    res = false;
                    trouve = true;
                }
            }
        }



        return res;
    }
    //--------------------------------------------------------------------------------------------------------------------

    

    //--------------------------------------------------------------------------------------------------------------------
    public static bool verifie_region(int[,] grille, int Xi, int Xj, int Xentier_a_verifier)
    {
        bool res = true;


        double ligne_deb_region = 0;
        double col_deb_region = 0;

        // Il faut d'abord récupérer la région où se situe la case (il faut d'abord trouvé le bon interval des lignes ENSUITE le bon interval des colonnes
        double nb_lignes = grille.GetLength(0);
        double racine_de_nb_lignes = Math.Sqrt(nb_lignes);


        // Déclaration + initialisation direct d'un entier représentant la dernière ligne d'une région (on l'initialise à la 1ère fin de ligne d'une région)
        double ligne_fin_region = 0;

        bool ligne_debut_trouve = false;
        while (ligne_debut_trouve == false)
        {
            ligne_fin_region += racine_de_nb_lignes;

            if (Xi < ligne_fin_region)
            {
                ligne_deb_region = ligne_fin_region - racine_de_nb_lignes;
                ligne_debut_trouve = true;
            }

        }
        // Là nous avons trouvé l'interval des lignes, mtn faut les colonnes

        double colonne_fin_region = 0;
        bool colonne_debut_trouve = false;
        while (colonne_debut_trouve == false)
        {
            colonne_fin_region += racine_de_nb_lignes;

            if (Xj < colonne_fin_region)
            {
                col_deb_region = colonne_fin_region - racine_de_nb_lignes;
                colonne_debut_trouve = true;
            }
        }

        Region r;
        r.ligne_debut = ligne_deb_region;
        r.ligne_fin = ligne_fin_region;
        r.colonne_debut = col_deb_region;
        r.colonne_fin = colonne_fin_region;

        /*Console.WriteLine("Ligne début région : "+ligne_deb_region);
        Console.WriteLine("Ligne fin région : "+ligne_fin_region);
        Console.WriteLine("colonne début région : "+col_deb_region);
        Console.WriteLine("colonne fin région : "+colonne_fin_region);*/

        bool trouve = false;
        // Parcours de la région
        for (double i = r.ligne_debut; i < r.ligne_fin; i++)
        {
            for (double j = r.colonne_debut; j < r.colonne_fin && trouve == false; j++)
            {
                // Pas la peine de vérifier la case que l'on veut vérifier car on sait qu'elle est vide
                if (i != Xi && j != Xj)
                {
                    int entier_actuel =
                        grille[(int)i,
                            (int)j]; // Nous devons caster car les indices des cases dans un tableau sont des entiers

                    //Console.WriteLine("Entier actuel : "+entier_actuel);

                    if (entier_actuel == Xentier_a_verifier)
                    {
                        res = false;
                        trouve = true;
                    }
                }
            }
        }


        return res;
    }

    //--------------------------------------------------------------------------------------------------------------------
}
