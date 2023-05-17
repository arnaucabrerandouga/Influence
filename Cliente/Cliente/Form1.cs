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
        int port = 9060;
        string direccion = "192.168.56.102";
        Thread atender;
        //delegate void DelegadoParaPonerTexto(string texto);
        List<Form2> formularios = new List<Form2>();
        string jugador;
        //string jugadorInvitado;
        //string[] nombresConectados = new string[50];
        int conectados = 0;
        int numPartida;
        List<int> listnumPartida = new List<int>();

        public Form1()
        {
            InitializeComponent();

            //dataGridView1.CellClick += dataGridView1_CellClick;
            //FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            CheckForIllegalCrossThreadCalls = false; //Necesario para que los elementos de los formularios puedan ser
            //accedidos desde threads a los que los crearon
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            buttonInvitacionAJugar.Visible = false;
            checkedListBox1.Visible = false;
            buttonInvitar.Visible = false;                      
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
                    case 1: //Te dice si has iniciado sesion correctamente o no
                        mensaje = trozos[1].Split('\0')[0];
                        if (mensaje == "BIEN") //ha anat be obram un form per fer peticions
                        {
                            jugador = textBoxUsuario.Text;
                            mensaje = "8/" + jugador;
                            // Enviamos al servidor la consulta
                            msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                            server.Send(msg);
                        }
                        else if (mensaje == "INCORRECTO")// nom contra incorrecte
                        {
                            MessageBox.Show("No se encuentra al usuario/contraseña incorrecta");
                            //Mensaje de desconexión
                            mensaje = "0/";
                            msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                            server.Send(msg);

                            // Nos desconectamos
                            atender.Abort();
                            server.Shutdown(SocketShutdown.Both);
                            server.Close();
                        }

                        else if (mensaje == "ERROR")// ERROR
                        {
                            MessageBox.Show("ERROR");
                            //Mensaje de desconexión
                            mensaje = "0/";
                            msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                            server.Send(msg);

                            // Nos desconectamos
                            atender.Abort();
                            server.Shutdown(SocketShutdown.Both);
                            server.Close();
                        }
                        break;

                    case 2: //Te dice si te has registrado correctamente o no 
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
                        //formularios[nform].TomaRespuesta3(mensaje);
                        break;

                    case 4: //Dime si este usuario existe
                        nform = Convert.ToInt32(trozos[1]);
                        mensaje = trozos[2].Split('\0')[0];
                        //formularios[nform].TomaRespuesta4(mensaje);
                        break;
                    case 7: //Notificacion con la lista de jugadores conectados
                        {
                            checkedListBox1.Items.Clear();
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
                                if (mensaje != jugador)
                                {
                                    checkedListBox1.Items.Add(mensaje);
                                }
                                i++;
                            }
                            break;

                        }
                    case 8: //Mensaje que te dice si ya estas conectado en otro formulario.
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
                                buttonInvitacionAJugar.Visible = true;
                            }
                            break;

                        }
                    case 9: //Notificacion que te dice que te han invitado a jugar | falta añadir un num de form 10/num/si
                        {
                            int numPartida = Convert.ToInt32(trozos[1].Split('\0')[0]); //el mensaje que recibo es mi numero de partida devo guardarlo de alguna forma
                            //string anfitrion = trozos[1].Split('\0')[0];
                            //string numPartida = trozos[2].Split('\0')[0];
                            DialogResult result = MessageBox.Show("Te han invitado a jugar, quieres aceptar?", "Confirmación", MessageBoxButtons.OKCancel);

                            if (result == DialogResult.OK)
                            {
                                mensaje = "10/" + numPartida + "/SI";
                                // Enviamos al servidor el usuario y la contraseña
                                msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                                server.Send(msg);
                            }
                            else
                            {
                                mensaje = "10/" + numPartida + "/NO";
                                // Enviamos al servidor el usuario y la contraseña
                                msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                                server.Send(msg);
                            }

                            break;
                        }
                    case 10: //Notificacion para jugar partida 10/numPartida/SI o /NO
                        {
                            int numPartida = Convert.ToInt32(trozos[1].Split('\0')[0]);
                            mensaje = trozos[2].Split('\0')[0];
                            if (mensaje == "SI")
                            {
                                ThreadStart ts = delegate { PonerEnMarchaFormulario(jugador, numPartida); }; //añadir la variable partida
                                Thread T = new Thread(ts);
                                T.Start();
                            }
                            else if (mensaje =="NO")
                            {
                                MessageBox.Show("Alguien ha rechazado la partida");
                            }
                            break;
                        }
                    case 11:
                        {
                            int numPartida = Convert.ToInt32(trozos[1].Split('\0')[0]);
                            string escritor = trozos[2].Split('\0')[0];
                            mensaje = trozos[3].Split('\0')[0];
                            int i = 0;
                            bool encontrado = false;
                            while (i < listnumPartida.Count() && !encontrado)
                            {
                                if (numPartida == listnumPartida[i])
                                {
                                    encontrado = true;
                                    formularios[i].TomaRespuesta11(escritor, mensaje);
                                }
                                i++;
                            }
                            break;
                        }

                }
            }
        }
        private void PonerEnMarchaFormulario(string Jugador, int numPartida)
        {
            int cont = formularios.Count;
            listnumPartida.Add(numPartida);
            Form2 f = new Form2(cont, Jugador, numPartida, server);
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

        private void buttonCerrarSesion_Click(object sender, EventArgs e)
        {
            estoyConectado = false;
            dataGridView1.Visible = false;
            buttonInvitacionAJugar.Visible = false;

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

        private void buttonInvitacionAJugar_Click(object sender, EventArgs e)
        {
            checkedListBox1.Visible = true;
            buttonInvitar.Visible = true;
        }

        private void buttonInvitar_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count <= 3)
            {
                string mensaje = "9/" + jugador + "/" + checkedListBox1.CheckedItems.Count.ToString();
                for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                {
                    mensaje = mensaje + "/" + checkedListBox1.CheckedItems[i].ToString();
                }

                MessageBox.Show(mensaje);
                // Enviamos al servidor la consulta
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                checkedListBox1.Visible = false;
                buttonInvitar.Visible = false;
            }
            else
                MessageBox.Show("Elige un maximo de 3 personas:");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (textBoxContraseña.UseSystemPasswordChar)
                textBoxContraseña.UseSystemPasswordChar = false;
            else
                textBoxContraseña.UseSystemPasswordChar = true;
        }
    }
}

// ------------------------------- COSAS QUE NO NECESITO -----------------------------------
// Crear un vector de strings para almacenar los elementos seleccionados
//string[] nombresConectados = new string[checkedListBox1.CheckedItems.Count];

// Recorrer los elementos seleccionados y agregarlos al vector
//for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
//{
//    elementosSeleccionados[i] = checkedListBox1.CheckedItems[i].ToString();
//}
//----------------------------------
//private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
//{

//foreach (string nombre in nombresConectados)
//{
//    checkedListBox1.Items.Add(nombre);
//}
//}
//-------------------------------------------
//private void dataGridView1_CellClick(object sender,DataGridViewCellEventArgs e)
//{
// dataGridView1.ReadOnly = false;
//  jugadorInvitado = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
//  DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
// cell.Style.BackColor = Color.Green;
// MessageBox.Show(jugadorInvitado);
//}