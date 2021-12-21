using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using ConsoleApplication1;

namespace Bataille_navale
{
    internal class Server
    {
        public static void OpenServer()
        {
            TcpListener server = null;
            try
            {
                // Création des variables contenant le port et l'adresse IP que le server doit écouter.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("192.168.1.33");
                // Création d'un objet TcpListener avec en paramétre le port et l'adresse IP
                server = new TcpListener(localAddr, port);
                // Ouverture du serveur.
                server.Start();
                // Création d'une variable bytes pour récupérer les données transmise et d'une variable string pour récupérer les données transcrit en string 
                Byte[] bytes = new Byte[256];
                String data = null;
                // Lancement de la boucle d'écoute 
                while (true)
                {
                    Console.Write("Waiting for a connection... ");
                    // Reception de la requête demande client envoyé..
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    data = null;
                    // Lecture du contenu envoyé
                    NetworkStream stream = client.GetStream();
                    int i = 0;
                    // Boucle sur de jeu. Commence par la lecture d'une attaque par le client
                    while (Program.playerHealth > 0)
                    {
                        stream.Flush();
                        while(i == 0)
                        {
                            i = stream.Read(bytes, 0, bytes.Length);
                        }
                        // 1 - Recevoir l'attaque ennemie
                        // transcrit les bytes envoyé en string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        // Parser l'attaque en coordonnées tableau
                        Program.playerInput = data;
                        Console.WriteLine("Le client a envoyé le message suivant : " + Program.playerInput);
                        // Convertit l'attaque en coordonnées tableau, modifie le tableau et renvoie le message du résultat de l'attaque ennemi à envoyer au client
                        string message = Program.InputParser();
                        // 2 - Envoyer le message du résultat de l'attaque ennemi au client
                        // Envoi du message au client
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Le serveur envoie le message suivant : " + message);

                        // 3 - Saisir notre attaque
                        // Recevoir la saisie utilisateur
                        // Boucle de vérification de validité de la saisie. Sinon, on revient à l'étape précédente
                        do
                        {
                            Console.WriteLine("Indiquez les coordonées du tir.");
                            Program.playerInput = Console.ReadLine();
                        }
                        while (!Program.InputCheck());

                        // 4 - Envoyer l'attaque à l'ennemi
                        // Envoi du string au client

                        message = Program.playerInput;
                        msg = System.Text.Encoding.ASCII.GetBytes(message);
                        stream.Write(msg, 0, msg.Length);
                    }

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
                                stream.Close();
                                client.Close();
                                break;
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Commande invalide");
                        }
                    }

                    // Shutdown and end connection
                    //client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}
