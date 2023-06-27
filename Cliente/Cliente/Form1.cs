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
        int port = 50009; //9070 50009
        string direccion = "147.83.117.22"; //192.168.56.102    147.83.117.22
        Thread atender;
        bool hayAlguienConectado = false;
        List<Form2> formularios = new List<Form2>();
        string jugador;
        int conectados = 0;
        List<int> listnumPartida = new List<int>();

        public Form1()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            buttonInvitacionAJugar.Visible = false;
            checkedListBox1.Visible = false;
            buttonInvitar.Visible = false;
            textBoxContraseña.UseSystemPasswordChar = true;
            buttonEliminarCuenta.Visible = false;
            buttonEliminar.Visible = false;
            buttonCancelar.Visible = false;
            labelConfirmarContraseña.Visible = false;
            textBoxConfirmarContraseña.Visible = false;
        }

        private void AtenderServidor()
        {
            while (true)
            {
                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje;
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

                            this.Invoke(new Action(() =>
                            {
                                this.BackColor = Color.Gray;
                            }));

                            // Nos desconectamos
                            server.Shutdown(SocketShutdown.Both);
                            server.Close();
                            atender.Abort();
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
                        //Mensaje de desconexión
                        mensaje = "0/";
                        msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                        this.Invoke(new Action(() =>
                        {
                            this.BackColor = Color.Gray;
                        }));
                        // Nos desconectamos
                        server.Shutdown(SocketShutdown.Both);
                        server.Close();
                        atender.Abort();
                        break;

                    case 3: //No utilizado

                        break;

                    case 4: //No utilizado
                        {
                            mensaje = trozos[1].Split('\0')[0];
                            MessageBox.Show(mensaje);

                            if (mensaje == "ELIMINADO")
                            {
                                mensaje = "6/" + jugador;
                                // Enviamos al servidor la consulta
                                msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                                server.Send(msg);
                                estoyConectado = false;
                                this.Invoke(new Action(() =>
                                {
                                    buttonEliminar.Visible = false;
                                    buttonCancelar.Visible = false;
                                    labelConfirmarContraseña.Visible = false;
                                    textBoxConfirmarContraseña.Visible = false;
                                    dataGridView1.Visible = false;
                                    buttonInvitacionAJugar.Visible = false;
                                    checkedListBox1.Visible = false;
                                    buttonInvitar.Visible = false;
                                    buttonEliminarCuenta.Visible = false;
                                    buttonRegistrarte.Visible = true;
                                }));

                                //Mensaje de desconexión
                                mensaje = "0/";
                                msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                                server.Send(msg);

                                // Nos desconectamos
                                server.Shutdown(SocketShutdown.Both);
                                server.Close();
                                this.Invoke(new Action(() =>
                                {
                                    this.BackColor = Color.Gray;
                                }));
                                atender.Abort();
                            }
                        }
                        break;
                    case 7: //Notificacion con la lista de jugadores conectados
                        {
                            this.Invoke(new Action(() =>
                            {
                                checkedListBox1.Items.Clear();
                                conectados = Convert.ToInt32(trozos[1].Split('\0')[0]);
                                dataGridView1.ColumnCount = 1;
                                dataGridView1.RowCount = conectados;
                                dataGridView1.ColumnHeadersVisible = true;
                                dataGridView1.RowHeadersVisible = false;
                                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                                dataGridView1.Columns[0].Name = "Estan conectados:";
                            }));
                            int i = 0;
                            while (i < conectados)
                            {
                                mensaje = trozos[i + 2].Split('\0')[0];
                                dataGridView1.Rows[i].Cells[0].Value = mensaje;
                                if (mensaje != jugador)
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        checkedListBox1.Items.Add(mensaje);
                                    }));
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
                                this.Invoke(new Action(() =>
                                {
                                    dataGridView1.Visible = true;
                                    buttonInvitacionAJugar.Visible = true;
                                    buttonRegistrarte.Visible = false;
                                    buttonEliminarCuenta.Visible = true;
                                }));
                            }
                            break;
                        }
                    case 9: //Notificacion que te dice que te han invitado a jugar
                        {
                            int numPartida = Convert.ToInt32(trozos[1].Split('\0')[0]); //el mensaje que recibo es mi numero de partida devo guardarlo de alguna forma
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
                                ThreadStart ts = delegate { PonerEnMarchaFormulario(jugador, numPartida, server); }; //añadir la variable partida
                                Thread T = new Thread(ts);
                                T.Start();
                            }
                            else if (mensaje =="NO")
                            {
                                MessageBox.Show("Alguien ha rechazado la partida");
                            }
                            break;
                        }
                    case 11: //Chat:  11/numpartida/escritor/mensaje
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
                    case 12: //Jugadores Partida: 12/numpartida/numjugadores/miNumeroJugador/jugador1/jugador2/...
                        {
                            int numPartida = Convert.ToInt32(trozos[1].Split('\0')[0]);
                            int numJugadores = Convert.ToInt32(trozos[2].Split('\0')[0]);
                            int miNumeroJugador = Convert.ToInt32(trozos[3].Split('\0')[0]);
                            string[] jugadores = new string[numJugadores];
                            int n = 0;
                            while (n < numJugadores)
                            {
                                string jugador = trozos[n+4].Split('\0')[0];
                                jugadores[n] = jugador;
                                n++;
                            }
                            int i = 0;
                            bool encontrado = false;
                            while (i < listnumPartida.Count() && !encontrado)
                            {
                                if (numPartida == listnumPartida[i])
                                {
                                    encontrado = true;
                                    formularios[i].TomaRespuesta12(numJugadores, jugadores, miNumeroJugador);
                                }
                                i++;
                            }
                            break;
                        }
                    case 13: // 13/numPartida/jugadorque ha pulsado/xboton/yboton/quien tira/labelsque cambian/xlabel/ylabel/xlabel/ylabel
                        {
                            int numPartida = Convert.ToInt32(trozos[1].Split('\0')[0]);
                            int numJugadorAnt = Convert.ToInt32(trozos[2].Split('\0')[0]);
                            int xBoton = Convert.ToInt32(trozos[3].Split('\0')[0]);
                            int yBoton = Convert.ToInt32(trozos[4].Split('\0')[0]);
                            int numJugadorPost = Convert.ToInt32(trozos[5].Split('\0')[0]);
                            int numlabels = Convert.ToInt32(trozos[6].Split('\0')[0]);
                            int[] labels = new int[numlabels*2];
                            if (numlabels != 0)
                            {
                                labels[0] = Convert.ToInt32(trozos[7].Split('\0')[0]);
                                labels[1] = Convert.ToInt32(trozos[8].Split('\0')[0]);
                            }
                            if (numlabels == 2)
                            {
                                labels[2] = Convert.ToInt32(trozos[9].Split('\0')[0]);
                                labels[3] = Convert.ToInt32(trozos[10].Split('\0')[0]);
                            }
                            int i = 0;
                            bool encontrado = false;
                            while (i < listnumPartida.Count() && !encontrado)
                            {
                                if (numPartida == listnumPartida[i])
                                {
                                    encontrado = true;
                                    formularios[i].TomaRespuesta13(numJugadorAnt, numJugadorPost, xBoton, yBoton, numlabels, labels);
                                }
                                i++;
                            }
                            break;
                        }
                }
            }
        }
        private void PonerEnMarchaFormulario(string Jugador, int numPartida, Socket server)
        {
            int cont = formularios.Count;
            listnumPartida.Add(numPartida);
            Form2 f = new Form2(cont, Jugador, numPartida, server);
            formularios.Add(f);
            f.ShowDialog();
        }
        private void RellenaChekList(string mensaje)
        {
            checkedListBox1.Items.Add(mensaje);
        }
        private void RellenaGridView(string mensaje, int i)
        {
            dataGridView1.Rows[i].Cells[0].Value = mensaje;
        }
        private void HacerVisibleGridView()
        {
            dataGridView1.Visible = true;
        }
        private void HacerVisibleInvitarAJugar()
        {
            buttonInvitacionAJugar.Visible = true;
        }
        


        private void buttonIniciarSesion_Click(object sender, EventArgs e)
        {
            if (!estoyConectado)
            {
                
                if (String.IsNullOrEmpty(textBoxUsuario.Text) || String.IsNullOrEmpty(textBoxContraseña.Text)) // nome she ficat nom o contra  avisame
                {
                    MessageBox.Show("Escribe un nombre y una contraseña");
                }
                else
                {
                    try
                    {
                        // Creamos un IPEndPoint con el ip del servidor y puerto del sevidor al que conectamos
                        IPAddress direc = IPAddress.Parse(direccion);
                        IPEndPoint ipep = new IPEndPoint(direc, port);

                        //Creamos el socket 
                        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        server.Connect(ipep);//Intentamos conectar el socket
                        this.BackColor = Color.Green;

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
                this.BackColor = Color.Green;

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
        }

        private void buttonCerrarSesion_Click(object sender, EventArgs e)
        {
            if (estoyConectado)
            {
                estoyConectado = false;
                this.Invoke(new Action(() =>
                {
                    dataGridView1.Visible = false;
                    buttonInvitacionAJugar.Visible = false;
                    checkedListBox1.Visible = false;
                    buttonInvitar.Visible = false;
                    buttonEliminarCuenta.Visible = false;
                    buttonRegistrarte.Visible = true;
                }));

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

                this.BackColor = Color.Gray;

            }
        }

        private void buttonInvitacionAJugar_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action (() => 
            {
                checkedListBox1.Visible = true;
                buttonInvitar.Visible = true;
            }));
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

                // Enviamos al servidor la consulta
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                this.Invoke(new Action(() =>
                {
                    checkedListBox1.Visible = false;
                    buttonInvitar.Visible = false;
                }));
            }
            else
                MessageBox.Show("Elige un maximo de 3 personas:");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (! textBoxContraseña.UseSystemPasswordChar)
                textBoxContraseña.UseSystemPasswordChar = true;
            else
                textBoxContraseña.UseSystemPasswordChar = false;
        }

        private void buttonEliminar_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxConfirmarContraseña.Text))
            {
                MessageBox.Show("Escribe una contraseña");
            }
            else
            {
                string mensaje = "4/" + jugador + "/" + textBoxConfirmarContraseña.Text;
                // Enviamos al servidor el usuario y la contraseña
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            buttonEliminar.Visible = false;
            buttonCancelar.Visible = false;
            labelConfirmarContraseña.Visible = false;
            textBoxConfirmarContraseña.Visible = false;
        }

        private void buttonEliminarCuenta_Click(object sender, EventArgs e)
        {
            buttonEliminar.Visible = true;
            buttonCancelar.Visible = true;
            labelConfirmarContraseña.Visible = true;
            textBoxConfirmarContraseña.Visible = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (estoyConectado)
            {
                estoyConectado = false;
                this.Invoke(new Action(() =>
                {
                    dataGridView1.Visible = false;
                    buttonInvitacionAJugar.Visible = false;
                    checkedListBox1.Visible = false;
                    buttonInvitar.Visible = false;
                    buttonEliminarCuenta.Visible = false;
                    buttonRegistrarte.Visible = true;
                }));

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

                this.BackColor = Color.Gray;

            }
        }
    } // cierro: public partial class Form1 : Form
}