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
        string Jugador;

        public Form2(int nForm, string Jugador, Socket server)
        {
            InitializeComponent();
            this.Jugador=Jugador;
            this.nForm = nForm;
            this.server = server;
            this.FormClosing += new FormClosingEventHandler(Form2_FormClosing);
        }
       
        private void Form2_Load(object sender, EventArgs e)
        {
            string mensaje = "5/" + this.Jugador;
            // Enviamos al servidor la consulta
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void buttonConsulta1_Click(object sender, EventArgs e)
        {
            string mensaje = "3/" + nForm;
            // Enviamos al servidor la consulta
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void buttonConsulta2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxJugador.Text))
            {
                MessageBox.Show("Escribe un nombre");
            }
            else
            {
                string mensaje = "4/" + nForm + "/" + textBoxJugador.Text;
                // Enviamos al servidor la consulta
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }

        }
        public void TomaRespuesta3(string mensaje)
        {
            MessageBox.Show("Existen " + mensaje + " jugadores registrados");
        }
        public void TomaRespuesta4(string mensaje)
        {
            if (mensaje == "SI")
                MessageBox.Show("Si existe");
            else if (mensaje == "NO")
                MessageBox.Show("NO existe");
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            string mensaje = "6/" + this.Jugador;
            // Enviamos al servidor la consulta
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }
    }
}
