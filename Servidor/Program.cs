using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servidor
{
    class Servidor
    {
       
        
        //Jugador jugadorClase;
        TcpClient cliente1Clase;
        string numeroCliente = "0";
        int puntaje = 0;

        static void Main(string[] args)
        {
            TcpListener servidorSocket = new TcpListener(8080);
            TcpClient cliente = default(TcpClient);
            //Jugador jugador = new Jugador();
            int canCliente = 0;

            servidorSocket.Start();
            Console.WriteLine(" >> " + "Servidor Iniciado");

            while (true)
            {
                canCliente += 1;
                cliente = servidorSocket.AcceptTcpClient();
                Console.WriteLine("Cliente: " + canCliente + " Conectado.");
                Servidor servidor = new Servidor();
                servidor.InicioCliente(cliente,  Convert.ToString(canCliente));
            }

        }

        public void InicioCliente(TcpClient cliente,  string numeroCliente)
        {
            this.cliente1Clase = cliente;
            //this.jugadorClase = jugador;
            this.numeroCliente = numeroCliente;

            Thread hiloCliente = new Thread(juego);
            hiloCliente.Start();
        }

        public void juego()
        {
            byte[] bytesCliente = new byte[10025];
            string mensajeCliente = null;
            Byte[] bytesServidor = null;
            string mensajeServidor = null;
           

          
            while (true)
            {

                NetworkStream networkStream = cliente1Clase.GetStream();
                networkStream.Read(bytesCliente, 0, 1024);
                mensajeCliente = System.Text.Encoding.ASCII.GetString(bytesCliente);
                mensajeCliente = mensajeCliente.Substring(0, mensajeCliente.IndexOf("$"));

                string[] data = mensajeCliente.Split(',');

                if (data[0].Equals("profesor"))
                {
                    if(Preguntas.pregunta == null)
                    {
                        Preguntas.pregunta = data[1];
                        Preguntas.respuesta = data[2];

                        NetworkStream serverStream = cliente1Clase.GetStream();
                        bytesServidor = System.Text.Encoding.ASCII.GetBytes("Pregunta recibida" + "$");
                        serverStream.Write(bytesServidor, 0, bytesServidor.Length);
                        serverStream.Flush();

                        Console.WriteLine("Se recibe la pregunta");
                    }
                }
                else if (data[0].Equals("estudiante"))
                {
                    if (Preguntas.pregunta != null)
                    {
                        NetworkStream serverStream = cliente1Clase.GetStream();
                        bytesServidor = System.Text.Encoding.ASCII.GetBytes(Preguntas.pregunta + "$");
                        serverStream.Write(bytesServidor, 0, bytesServidor.Length);
                        serverStream.Flush();

                        Console.WriteLine("Se le envia la pregunta al estudiante");
                    }
                    else
                    {
                        NetworkStream serverStream = cliente1Clase.GetStream();
                        bytesServidor = System.Text.Encoding.ASCII.GetBytes("No existe pregunta" + "$");
                        serverStream.Write(bytesServidor, 0, bytesServidor.Length);
                        serverStream.Flush();

                        Console.WriteLine("Se le indica al estudiante que no existe pregunta");
                    }
                }
            }    
        }
    }

    public static class Preguntas
    {

        public static string pregunta { get; set; }

        public static string respuesta { get; set; }
    }


}
    

