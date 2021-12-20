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
                IPAddress localAddr = IPAddress.Parse("192.168.1.67");

                // Création d'un objet TcpListener avec en paramétre le port et l'adresse IP
                server = new TcpListener(localAddr, port);

                // Ouverture du server.
                server.Start();

                // Création d'une variable bytes pour récupérer les données transmise et d'une variable string pour récupérer les données transcrit en string 
                Byte[] bytes = new Byte[256];
                String data = null;

                // Lancement de la boucle d'écoute.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Reception de la requête demande client envoyé..
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Lecture du contenu envoyé
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Boucle sur le contenu envoyé.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // transcrit les bytes envoyé en string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        for (int msgNumber = 0; msgNumber < 2; msgNumber++) {
                            Console.WriteLine("Indiquez votre attaque");
                            data = Console.ReadLine();
                            //transcrit le string en bytes
                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                            // renvoie le message.
                            stream.Write(msg, 0, msg.Length);
                            Console.WriteLine("Sent: {0}", data);
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
