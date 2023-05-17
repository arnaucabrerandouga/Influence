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
    public partial class Form2 : Form
    {
        Socket server;
        int nForm;
        int numPartida;
        string Jugador;

        public Form2(int nForm, string Jugador, int numPartida, Socket server)
        {
            InitializeComponent();
            this.Jugador=Jugador;
            this.nForm = nForm;
            this.numPartida = numPartida;
            this.server = server;
            //this.FormClosing += new FormClosingEventHandler(Form2_FormClosing);
        }
       
        private void Form2_Load(object sender, EventArgs e)
        {
        }
        
        public void TomaRespuesta11(string escritor, string mensaje)
        {
            //MessageBox.Show(escritor + ": " + mensaje);
            listBoxChat.Items.Add(escritor + ": " + mensaje);
        }
        
        private void buttonChat_Click(object sender, EventArgs e)
        {
            string mensaje = "11/" + numPartida + "/" + Jugador + "/" + textBoxChat.Text;
            // Enviamos al servidor el usuario y la contraseña
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            textBoxChat.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1x13.BackColor = Color.Red;
        }
    }
}

//------------------------------------------------------------------------------------

//public void TomaRespuesta4(string mensaje)
        //{
            //if (mensaje == "SI")
                //MessageBox.Show("Si existe");
            //else if (mensaje == "NO")
                //MessageBox.Show("NO existe");
        //}

//public void TomaRespuesta3(string mensaje)
        //{
            //MessageBox.Show("Existen " + mensaje + " jugadores registrados");
        //}

//private void buttonConsulta1_Click(object sender, EventArgs e)
        //{
            //string mensaje = "3/" + nForm;
            // Enviamos al servidor la consulta
            //byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            //server.Send(msg);
        //}
//private void buttonConsulta2_Click(object sender, EventArgs e)
        //{
            //if (String.IsNullOrEmpty(textBoxJugador.Text))
            //{
                //MessageBox.Show("Escribe un nombre");
            //}
            //else
            //{
                //string mensaje = "4/" + nForm + "/" + textBoxJugador.Text;
                // Enviamos al servidor la consulta
                //byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                //server.Send(msg);
            //}
        //}