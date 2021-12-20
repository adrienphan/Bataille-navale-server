using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Bataille_navale
{
    class Client
    {
        static void Connect(String server, String message)
        {
            try
            {
                // Création des variables contenant le port et l'adresse IP sur lequel envoyer.
                Int32 port = 13000;
                TcpClient client = new TcpClient(server, port);
                // Transcription du message à envoyer en bytes.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                // Ouverture de la connection avec le server.
                NetworkStream stream = client.GetStream();
                // Envoie du message en bytes.
                stream.Write(data, 0, data.Length);
                //Affichage du message envoyé.
                Console.WriteLine("Sent: {0}", message);
                // Receive the TcpServer.response.

                data = new Byte[256];
                // String to store the response ASCII representation.
                String responseData = String.Empty;
                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
    }
}
