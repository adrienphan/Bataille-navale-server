using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Bataille_navale;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        public static int longitudeShot = 0;
        public static int latitudeShot = 0;
        public static string[,] battleMap = new string[10, 10];
        public static Dictionary<string, int> boats = new Dictionary<string, int>();
        public static string message = "";
        public static string linePosition = "";
        public static string columnPosition = ";";
        public static string playerInput = "";
        public static int playerHealth;
        public static void Main(string[] args)
        {
            Init();
            // Remplace le placeur de bateaux. Sinon, à mettre en commentaires.
            DebugInit();
            Server.OpenServer();
        }
        static int lineCharacterToInt(char charToUnicode)
        {
            return (int)charToUnicode - (int)'A';
        }
        //Creation des bateaux.
        public static void Init()
        {
            boats.Add("carrier", 5);
            boats.Add("battleship", 4);
            boats.Add("cruiser", 3);
            boats.Add("submarine", 3);
            boats.Add("destroyer", 2);
            playerHealth = boats.Sum(x => x.Value);
        }
        //Positionnement des bateaux sur la carte
        public static void DebugInit()
        {
            battleMap[1, 1] = "carrier";
            battleMap[1, 2] = "carrier";
            battleMap[1, 3] = "carrier";
            battleMap[1, 4] = "carrier";
            battleMap[1, 5] = "carrier";
            battleMap[2, 7] = "battleship";
            battleMap[3, 7] = "battleship";
            battleMap[4, 7] = "battleship";
            battleMap[5, 7] = "battleship";
            battleMap[3, 2] = "cruiser";
            battleMap[4, 2] = "cruiser";
            battleMap[5, 2] = "cruiser";
            battleMap[5, 4] = "submarine";
            battleMap[6, 4] = "submarine";
            battleMap[7, 4] = "submarine";
            battleMap[8, 8] = "destroyer";
            battleMap[8, 9] = "destroyer";       
        }
        //Vérifie que la saisie de la position d'attaque soit correcte.
        public static bool InputCheck()
        {
            //Récupére la colonne et la ligne d'attaque.
            linePosition = playerInput.Substring(0, 1/*EXCLU*/);
            columnPosition = playerInput.Substring(1);
            //Création de deux expressions régulière pour vérifier la saisie. 
            Regex longitudeCheck = new Regex(@"^[A-J]+$");
            Regex latitudeCheck = new Regex(@"^[0-9]+$");          
                // Verifier la validité de la ligne
                if (!longitudeCheck.IsMatch(linePosition))
                {
                    Console.WriteLine("La lettre de la ligne n'est pas valide");
                    return false;
                }
                // Verifier la validité de la colonne
                if (!latitudeCheck.IsMatch(columnPosition)
                    || int.Parse(columnPosition) <= 0 || int.Parse(columnPosition) > 10)
                {
                    Console.WriteLine("Le chiffre de la colonne n'est pas valide");
                    return false;
                }
            return true;
        }
        //Transforme les variables de position d'attaque en entier et les envoie à la fonction CheckAtk().
        public static string InputParser() 
        {
            linePosition = playerInput.Substring(0, 1/*EXCLU*/);
            columnPosition = playerInput.Substring(1);
            longitudeShot = lineCharacterToInt(playerInput[0]);
            latitudeShot = int.Parse(columnPosition) - 1;
            message = CheckAtk(longitudeShot, latitudeShot);
            return message;
        }
        //
        public static string CheckAtk(int longitudeShot, int latitudeShot)
        {
            string message = "";           
            // Attaque de l'adversaire
            if (battleMap[longitudeShot, latitudeShot] == null)
            {
                message = "On a entendu plouf au loin. Et des rires distants.";
                return message;
                //Console.WriteLine("On a entendu plouf au loin. Et des rires distants.");
            }
            else if (battleMap[longitudeShot, latitudeShot] == "Touché")
            {
                message = $"Vous avez déjà tiré sur cette case. Achetez - vous des lunettes.";
                return message;
               // Console.WriteLine($"Vous avez déjà tiré sur cette case. Achetez-vous des lunettes.");
            }
            else if (battleMap[longitudeShot, latitudeShot] != null)
            {
                playerHealth--;
                //Console.WriteLine(battleMap[longitudeShot, latitudeShot]);
                //Console.WriteLine($"Le bateau de type {battleMap[longitudeShot, latitudeShot].ToString()} a été touché en {playerInput}");
                boats[battleMap[longitudeShot, latitudeShot]] -= 1;
                if (boats[battleMap[longitudeShot, latitudeShot]] == 0)
                {
                    battleMap[longitudeShot, latitudeShot] = "Touché";
                    message = "Touché. Coulé.";
                    message = checkHealth(message);
                    return message;
                    //Console.WriteLine("Touché. Coulé.");
                }
                else
                {
                    battleMap[longitudeShot, latitudeShot] = "Touché";
                    message = "Touché";
                    message = checkHealth(message);
                    return message;                  
                }
                
            }
            
            return null;
        }
        public static string checkHealth(string message)
        {
            if (playerHealth == 0)
            {
                message = "Game Over";
            }
            return message;
        }
         public static void GameOver()
        {

            // Game over. On peux recommencer le jeu.
            while (true)
            {
                Console.WriteLine("Fin de la partie. Voulez-vous rejouer? Ecrivez oui pour rejouer. Ecrivez non pour quitter.");
                string playerInput = Console.ReadLine();
                try
                {
                    if (playerInput == "oui")
                    {
                        Console.Clear();
                        Program.Main(null);
                    }
                    if (playerInput == "non")
                    {
                        
                        break;
                    }
                }
                catch
                {
                    Console.WriteLine("Commande invalide");
                }
            }
        }
    }
}