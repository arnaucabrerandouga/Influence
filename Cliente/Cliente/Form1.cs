using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        bool estoyConectado = false;
        Socket server;
        int port = 9050;
        string direccion = "192.168.56.102";
        Thread atender;
        //delegate void DelegadoParaPonerTexto(string texto);
        List<Form2> formularios = new List<Form2>();
        string jugador;

        public Form1()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            CheckForIllegalCrossThreadCalls = false; //Necesario para que los elementos de los formularios puedan ser
            //accedidos desde threads a los que los crearon
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
        }

        private void AtenderServidor()
        {
            while (true)
            {
                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                //MessageBox.Show(Encoding.ASCII.GetString(msg2)); // Para confirmar que todo va bien
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje;
                int nform;
                byte[] msg;

                switch (codigo)
                {
                    case 1:
                        mensaje = trozos[1].Split('\0')[0];
                        if (mensaje == "BIEN") //ha anat be obram un form per fer peticions
                        {
                            jugador = textBoxUsuario.Text;
                            //ThreadStart ts = delegate { PonerEnMarchaFormulario(jugador); };
                            //Thread T = new Thread(ts);
                            //T.Start();

                            mensaje = "8/" + jugador;
                            // Enviamos al servidor la consulta
                            msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                            server.Send(msg);
                        }
                        else if (mensaje == "INCORRECTO")// nom contra incorrecte
                            MessageBox.Show("No se encuentra al usuario/contraseña incorrecta");

                        else if (mensaje == "ERROR")// ERROR
                            MessageBox.Show("ERROR");
                        break;

                    case 2:
                        mensaje = trozos[1].Split('\0')[0];
                        if (mensaje == "BIEN") //ha anat be obram un form per fer peticions
                            MessageBox.Show("Se ha registrado");

                        else if (mensaje == "EXISTE") //si el usuario ya existe
                            MessageBox.Show("Este usuario ya existe");

                        else if (mensaje == "ERROR")// ERROR
                            MessageBox.Show("ERROR");
                        break;

                    case 3: //Dime cuantos usuarios hay
                        nform = Convert.ToInt32(trozos[1]);
                        mensaje = trozos[2].Split('\0')[0];
                        formularios[nform].TomaRespuesta3(mensaje);
                        break;

                    case 4: //Dime si este usuario existe
                        nform = Convert.ToInt32(trozos[1]);
                        mensaje = trozos[2].Split('\0')[0];
                        formularios[nform].TomaRespuesta4(mensaje);
                        break;
                    case 7: //Notificacion
                        {
                            int conectados;
                            conectados = Convert.ToInt32(trozos[1].Split('\0')[0]);
                            dataGridView1.ColumnCount = 1;
                            dataGridView1.RowCount = conectados;
                            dataGridView1.ColumnHeadersVisible = true;
                            dataGridView1.RowHeadersVisible = false;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                            dataGridView1.Columns[0].Name = "Estan conectados:";
                            int i = 0;
                            while (i < conectados)
                            {
                                mensaje = trozos[i + 2].Split('\0')[0];
                                dataGridView1.Rows[i].Cells[0].Value = mensaje;
                                i++;
                            }
                            break;

                        }
                    case 8:
                        {
                            mensaje = trozos[1].Split('\0')[0];
                            if (mensaje == "SI")
                            {
                                MessageBox.Show("Ya estas coenctado");
                            }
                            else if (mensaje == "NO")
                            {
                                mensaje = "5/" + jugador;
                                // Enviamos al servidor la consulta
                                msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                                server.Send(msg);
                                estoyConectado = true;
                                dataGridView1.Visible = true;
                            }
                            break;

                        }

                }
            }
        }
        private void PonerEnMarchaFormulario(string Jugador)
        {
            int cont = formularios.Count;
            Form2 f = new Form2(cont, Jugador, server);
            formularios.Add(f);
            f.ShowDialog();


        }

        private void buttonIniciarSesion_Click(object sender, EventArgs e)
        {
            if (!estoyConectado)
            {
                try
                {
                    // Creamos un IPEndPoint con el ip del servidor y puerto del sevidor al que conectamos
                    IPAddress direc = IPAddress.Parse(direccion);
                    IPEndPoint ipep = new IPEndPoint(direc, port);

                    //Creamos el socket 
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    server.Connect(ipep);//Intentamos conectar el socket
                    MessageBox.Show("Conectado");

                    //Creo un thread que realiza la funcion AtenderServidor
                    ThreadStart ts = delegate { AtenderServidor(); };
                    atender = new Thread(ts);
                    atender.Start();
                }
                catch (SocketException ex)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    return;
                }
                if (String.IsNullOrEmpty(textBoxUsuario.Text) || String.IsNullOrEmpty(textBoxContraseña.Text)) // nome she ficat nom o contra  avisame
                {
                    MessageBox.Show("Escribe un nombre y una contraseña");
                }
                else
                {
                    string mensaje = "1/" + textBoxUsuario.Text + "/" + textBoxContraseña.Text;
                    // Enviamos al servidor el usuario y la contraseña
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
            }
            else
            {
                MessageBox.Show("Ya hay un usuario conectado en este equipo");
            }
        }

        private void buttonRegistrarte_Click(object sender, EventArgs e)
        {
            string mensaje;
            byte[] msg;
            try
            {
                // Creamos un IPEndPoint con el ip del servidor y puerto del sevidor al que conectamos
                IPAddress direc = IPAddress.Parse(direccion);
                IPEndPoint ipep = new IPEndPoint(direc, port);

                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Connect(ipep);//Intentamos conectar el socket
                MessageBox.Show("Conectado");

                //Creo un thread que realiza la funcion AtenderServidor
                ThreadStart ts = delegate { AtenderServidor(); };
                atender = new Thread(ts);
                atender.Start();
            }
            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

            if (String.IsNullOrEmpty(textBoxUsuario.Text) || String.IsNullOrEmpty(textBoxContraseña.Text)) // nome she ficat nom o contra  avisame
            {
                MessageBox.Show("Escribe un nombre y una contraseña");
            }
            else
            {
                mensaje = "2/" + textBoxUsuario.Text + "/" + textBoxContraseña.Text;
                // Enviamos al servidor el usuario y la contraseña
                msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            //Mensaje de desconexión
            mensaje = "0/";
            msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            atender.Abort();
            server.Shutdown(SocketShutdown.Both);
            server.Close(); 
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            

        }

        private void buttonCerrarSesion_Click(object sender, EventArgs e)
        {
            estoyConectado = false;
            dataGridView1.Visible = false;

            string mensaje = "6/" + jugador;
            // Enviamos al servidor la consulta
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            //Mensaje de desconexión
            mensaje = "0/";
            msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            atender.Abort();
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }
    }
}
