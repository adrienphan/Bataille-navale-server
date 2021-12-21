using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using ConsoleApplication1;
using System.IO;

namespace Bataille_navale
{
    internal class Server
    {
        public static async void OpenServer()
        {
            TcpListener server = null;
            try
            {
                // Création des variables contenant le port et l'adresse IP que le server doit écouter.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                // Création d'un objet TcpListener avec en paramétre le port et l'adresse IP
                server = new TcpListener(localAddr, port);
                // Ouverture du serveur.
                server.Start();
                // Création d'une variable bytes pour récupérer les données transmise et d'une variable string pour récupérer les données transcrit en string 
                Byte[] bytes = new Byte[256];
                String data = null;

                // Reception de la requête demande client envoyé..
                TcpClient client = server.AcceptTcpClient();
                data = null;
                // Lecture du contenu envoyé
                NetworkStream stream = client.GetStream();
                int i = 0;

                bytes = new Byte[256];
                data = "";
                i = 0;

                // Lancement de la boucle d'écoute 
                // Boucle sur de jeu. Commence par la lecture d'une attaque par le client
                while (Program.playerHealth > 0)
                {
                    try
                    {
                        // Reception de la requête demande client envoyé..
                        client = server.AcceptTcpClient();
                        data = null;
                        // Lecture du contenu envoyé
                        NetworkStream writeStream = client.GetStream();
                        stream = client.GetStream();
                        byte[] buffer = new byte[1024];
                        stream.Read(buffer, 0, buffer.Length);
                        int recv = 0;
                        data = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        stream.Flush();

                        // Parser l'attaque en coordonnées tableau
                        Program.playerInput = data;
                        Console.WriteLine("Received: " + Program.playerInput);
                        // Convertit l'attaque en coordonnées tableau, modifie le tableau et renvoie le message du résultat de l'attaque ennemi à envoyer au client
                        string message = Program.InputParser();
                        // 2 - Envoyer le message du résultat de l'attaque ennemi au client
                        // Envoi du message au client
                        Console.WriteLine("Sent: " + message);

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
                        Console.WriteLine("Sent: " + message);
                        Program.playerInput = "";
                    }
                    catch
                    {

                    }
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
