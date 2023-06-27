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
        int Puntos1 = 0;
        int Puntos2 = 0;
        int Puntos3 = 0;
        int Puntos4 = 0;
        int PuntosTotales = 0;
        int miNumero;
        int esMiTurno = 0;

        public Form2(int nForm, string Jugador, int numPartida, Socket server)
        {
            InitializeComponent();
            //CheckForIllegalCrossThreadCalls = false;
            this.Jugador=Jugador;
            this.nForm = nForm;
            this.numPartida = numPartida;
            this.server = server;
            //this.FormClosing += new FormClosingEventHandler(Form2_FormClosing);
        }
       
        private void Form2_Load(object sender, EventArgs e)
        {
            string mensaje = "12/" + numPartida + "/" + Jugador; //Pedimos informacion de la partida
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            labelJugador1.ForeColor = Color.Blue;
            labelJugador2.ForeColor = Color.Red;
            labelJugador3.ForeColor = Color.Green;
            labelJugador4.ForeColor = Color.Yellow;
            labelPuntos1.ForeColor = Color.Blue;
            labelPuntos2.ForeColor = Color.Red;
            labelPuntos3.ForeColor = Color.Green;
            labelPuntos4.ForeColor = Color.Yellow;
        }

        public void TomaRespuesta11(string escritor, string mensaje) //Chat
        {
            this.Invoke(new Action(() =>
            {
                listBoxChat.Items.Add(escritor + ": " + mensaje);
            }));
        }
  
        private void buttonChat_Click(object sender, EventArgs e)
        {
            string mensaje = "11/" + numPartida + "/" + Jugador + "/" + textBoxChat.Text;
            // Enviamos al servidor el usuario y la contraseña
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            textBoxChat.Clear();
        }

        public void TomaRespuesta12(int numJugadores, string[] jugadores, int miNumeroJugador)
        {
            miNumero = miNumeroJugador;
            //MessageBox.Show(miNumero.ToString());
            this.Invoke(new Action(() =>
            {
                labelJugador1.Text = jugadores[0];
                labelJugador2.Text = jugadores[1];
                labelPuntos1.Text = Puntos1.ToString();
                labelPuntos2.Text = Puntos2.ToString();
            }));
            
            if (numJugadores == 2)
            {
                this.Invoke(new Action(() =>
                {
                    labelJugador3.Visible = false;
                    labelJugador4.Visible = false;
                    labelPuntos3.Visible = false;
                    labelPuntos4.Visible = false;
                })); 
                
            }
            else if (numJugadores == 3)
            {
                this.Invoke(new Action(() =>
                {
                    labelJugador4.Visible = false;
                    labelPuntos4.Visible = false;
                    labelJugador3.Text = jugadores[2];
                    labelPuntos3.Text = Puntos3.ToString();
                })); 
                
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    labelJugador3.Text = jugadores[2];
                    labelJugador4.Text = jugadores[3];
                    labelPuntos3.Text = Puntos3.ToString();
                    labelPuntos4.Text = Puntos4.ToString();
                }));
                
            }
            if (miNumero == 0)
            {
                esMiTurno = 1;
            }
        }
        public void TomaRespuesta13(int numJugadorAnt, int numJugadorPost, int xBoton, int yBoton, int numlabels, int[] labels)
        {
            string nombreLabelPuntos = "labelPuntos" + (numJugadorAnt + 1);
            if (numJugadorPost == miNumero)
            {
                esMiTurno = 1;
            }
            string nombreBoton = ("button" + xBoton + "x" + yBoton);
            PintarBoton(nombreBoton, numJugadorAnt);
            if (numlabels != 0)
            {
                sumarPuntos(nombreLabelPuntos, numJugadorAnt);
                string nombreLabel = ("label" + labels[0] + "x" + labels[1]);
                PintarLabel(nombreLabel, numJugadorAnt);
                if (numlabels == 2)
                {
                    sumarPuntos(nombreLabelPuntos, numJugadorAnt);
                    nombreLabel = ("label" + labels[2] + "x" + labels[3]);
                    PintarLabel(nombreLabel, numJugadorAnt);
                }
            }
            if (PuntosTotales == 49)
            {
                MessageBox.Show("Fin de la partida");
                int maximo = Math.Max(Math.Max(Puntos1, Puntos2), Math.Max(Puntos3, Puntos4)); //Para encontrar al ganador
                MessageBox.Show(Convert.ToString(maximo));
            }
        }

        private void sumarPuntos(string nombreLabelPuntos, int numJugadorAnt)
        {
            Control[] controles = Controls.Find(nombreLabelPuntos, true);
            this.Invoke(new Action(() =>
            {
                controles[0].Text = (Convert.ToInt32(controles[0].Text) + 1).ToString();
            }));
            if (numJugadorAnt == 1)
            {
                Puntos1++;
                PuntosTotales++;
            }
            if (numJugadorAnt == 2)
            {
                Puntos2++;
                PuntosTotales++;
            }
            if (numJugadorAnt == 3)
            {
                Puntos3++;
                PuntosTotales++;
            }
            if (numJugadorAnt == 4)
            {
                Puntos4++;
                PuntosTotales++;
            }
        }

        private void PintarBoton(string nombreBoton, int numJugador)
        {
            Control[] controles = Controls.Find(nombreBoton, true);
            if (numJugador == 0)
            {
                this.Invoke(new Action(() =>
                {
                    controles[0].BackColor = Color.Blue;
                }));
            }
            else if (numJugador == 1)
            {
                this.Invoke(new Action(() =>
                {
                    controles[0].BackColor = Color.Red;
                }));           
            }
            else if (numJugador == 2)
            {
                this.Invoke(new Action(() =>
                {
                    controles[0].BackColor = Color.Green;
                }));             
            }
            else if (numJugador == 3)
            {
                this.Invoke(new Action(() =>
                {
                    controles[0].BackColor = Color.Yellow;
                }));            
            }
        }

        private void PintarLabel(string nombreLabel, int numJugador)
        {
            Control[] controles = Controls.Find(nombreLabel, true);
            if (numJugador == 0)
            {
                this.Invoke(new Action(() =>
                {
                    controles[0].BackColor = Color.Blue;
                }));
            }
            else if (numJugador == 1)
            {
                this.Invoke(new Action(() =>
                {
                    controles[0].BackColor = Color.Red;
                }));
            }
            else if (numJugador == 2)
            {
                this.Invoke(new Action(() =>
                {
                    controles[0].BackColor = Color.Green;
                }));
            }
            else if (numJugador == 3)
            {
                this.Invoke(new Action(() =>
                {
                    controles[0].BackColor = Color.Yellow;
                }));
            }
        }
       
        // Enviamos al servidor un mnesaje que dice que boton apretas i 
        //  13/numPartida/Jugador/BotoX/BotoY
        private void button1x14_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/1/14";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button3x14_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/3/14";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button5x14_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/5/14";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button7x14_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/7/14";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button9x14_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/9/14";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button11x14_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/11/14";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button13x14_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/13/14";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button0x13_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/0/13";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button2x13_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/2/13";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button4x13_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/4/13";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button6x13_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/6/13";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button8x13_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/8/13";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button10x13_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/10/13";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button12x13_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/12/13";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button14x13_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/14/13";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button1x12_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/1/12";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button3x12_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/3/12";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button5x12_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/5/12";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button7x12_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/7/12";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button9x12_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/9/12";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button11x12_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/11/12";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button13x12_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/13/12";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button0x11_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/0/11";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button2x11_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/2/11";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button4x11_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/4/11";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button6x11_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/6/11";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button8x11_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/8/11";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button10x11_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/10/11";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button12x11_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/12/11";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button14x11_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/14/11";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button1x10_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/1/10";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button3x10_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/3/10";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button5x10_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/5/10";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button7x10_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/7/10";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button9x10_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/9/10";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button11x10_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/11/10";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button13x10_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/13/10";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button0x9_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/0/9";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button2x9_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/2/9";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button4x9_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/4/9";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button6x9_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/6/9";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button8x9_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/8/9";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button10x9_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/10/9";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button12x9_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/12/9";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button14x9_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/14/9";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button1x8_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/1/8";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button3x8_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/3/8";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button5x8_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/5/8";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button7x8_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/7/8";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button9x8_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/9/8";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button11x8_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/11/8";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button13x8_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/13/8";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button0x7_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/0/7";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button2x7_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/2/7";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button4x7_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/4/7";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button6x7_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/6/7";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button8x7_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/8/7";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button10x7_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/10/7";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button12x7_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/12/7";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button14x7_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/14/7";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button1x6_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/1/6";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button3x6_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/3/6";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button5x6_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/5/6";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button7x6_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/7/6";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button9x6_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/9/6";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button11x6_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/11/6";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button13x6_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/13/6";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button0x5_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/0/5";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button2x5_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/2/5";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button4x5_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/4/5";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button6x5_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/6/5";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button8x5_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/8/5";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button10x5_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/10/5";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button12x5_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/12/5";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button14x5_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/14/5";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button1x4_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/1/4";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button3x4_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/3/4";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button5x4_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/5/4";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button7x4_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/7/4";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button9x4_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/9/4";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button11x4_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/11/4";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button13x4_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/13/4";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button0x3_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/0/3";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button2x3_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/2/3";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button4x3_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/4/3";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button6x3_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/6/3";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button8x3_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/8/3";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button10x3_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/10/3";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button12x3_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/12/3";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button14x3_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/14/3";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button1x2_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/1/2";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button3x2_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/3/2";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button5x2_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/5/2";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button7x2_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/7/2";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button9x2_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/9/2";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button11x2_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/11/2";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button13x2_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/13/2";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button0x1_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/0/1";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button2x1_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/2/1";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button4x1_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/4/1";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button6x1_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/6/1";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button8x1_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/8/1";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button10x1_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/10/1";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button12x1_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/12/1";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button14x1_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/14/1";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button1x0_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/1/0";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button3x0_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/3/0";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button5x0_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/5/0";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button7x0_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/7/0";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button9x0_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/9/0";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button11x0_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/11/0";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }

        private void button13x0_Click(object sender, EventArgs e)
        {
            if (esMiTurno == 1)
            {
                string mensaje = "13/" + numPartida + "/" + miNumero + "/13/0";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                esMiTurno = 0;
            }
        }
    }
}