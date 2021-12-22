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
                IPAddress localAddr = IPAddress.Parse("192.168.1.33");
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
                while (true)
                {
                    try
                    {
                        // Reception de la requête demande client envoyé..
                        client = server.AcceptTcpClient();
                        data = null;
                        // Lecture du contenu envoyé
                        NetworkStream writeStream = client.GetStream();
                        stream = client.GetStream();
                        byte[] buffer = new byte[256];
                        stream.Read(buffer, 0, buffer.Length);
                        int recv = 0;
                        foreach (byte b in buffer)
                        {
                            if (b != 0)
                            {
                                recv++;
                            }
                        }
                       
                        data = Encoding.UTF8.GetString(buffer, 0, recv);
                        stream.Flush();

                        // Parser l'attaque en coordonnées tableau
                        Program.playerInput = data;
                        Console.WriteLine("Received: " + Program.playerInput);
                        if (Program.playerInput.Contains("Game Over"))
                        {
                            Console.WriteLine("Bravo, vous avez gagné!!");
                            Program.GameOver();
                            break;
                        }
                        // Convertit l'attaque en coordonnées tableau, modifie le tableau et renvoie le message du résultat de l'attaque ennemi à envoyer au client
                        string message = Program.InputParser();
                        // 2 - Envoyer le message du résultat de l'attaque ennemi au client
                        // Envoi du message au client
                        if (Program.playerHealth == 0)
                        {
                            Console.WriteLine("Sent: Game Over");
                            Byte[] sendGOMsg = System.Text.Encoding.ASCII.GetBytes(message);
                            stream.Write(sendGOMsg, 0, sendGOMsg.Length);
                            Console.WriteLine("Vous avez perdu!");
                            Program.GameOver();
                            break;
                        }
                        Console.WriteLine("Sent: " + message);
                        Byte[] sendMsg = System.Text.Encoding.ASCII.GetBytes(message);                       
                        stream.Write(sendMsg,0,sendMsg.Length);
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
                        Byte[] sendAtk = System.Text.Encoding.ASCII.GetBytes(message);                       
                        stream.Write(sendAtk, 0, sendAtk.Length);
                        Program.playerInput = "";
                    }
                    catch
                    {

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
