using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pjCaso61
{
    public partial class frmLlamadas : Form
    {
        //Declaracion de variables GLOBALES
        string tipo;
        string horario;
        int minutos;
        double costoMinuto;
        double costoLlamada;

        //Contadores y acumuladores
        int cLlamadas;
        double aLocNac, aLocInt, aMovNac, aMovInt;
        double mayorMonto;
        string horarioMayor;
        string tipoMayor;

        public frmLlamadas()
        {
            InitializeComponent();
            tHora.Enabled = true;
        }


        private void frmLlamadas_Load(object sender, EventArgs e)
        {
            lblFecha.Text = DateTime.Now.ToShortDateString();
        }

        private void cboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Asignar el costo por minuto 
            asignaCostoxMinuto();
            lblCosto.Text = costoMinuto.ToString("C");
        }

        private void tHora_Tick(object sender, EventArgs e)
        {
            lblHora.Text = DateTime.Now.ToString("hh:mm:ss");
        }
        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            //Capturando los datos 
            horario = cboHorario.Text;
            minutos = int.Parse(txtMinutos.Text);

            //Determinar el costo por minuto
            asignaCostoxMinuto();

            //Determinar el costo por llamada
            asignaCostoxLlamada();

            //Imprimir el registro de llamadas
            imprimirRegistro();

            lvEstadisticas.Items.Clear();
        }

        private void btnEstadisticas_Click(object sender, EventArgs e)
        {
            imprimirEstadisticas();
        }


        //Metodos sin valor de retorno sin parametros
        void asignaCostoxMinuto()
        {
            tipo = cboTipo.Text;
            switch (tipo)
            {
                case "Local Nacional": costoMinuto = 0.20; break;
                case "Local Internacional": costoMinuto = 0.50; break;
                case "Movil Nacional": costoMinuto = 1.20; break;
                case "Movil Internacional": costoMinuto = 2.20; break;
            }
        }

        void asignaCostoxLlamada()
        {
            double importe = costoMinuto * minutos;
            double descuento = 0;
            switch (horario)
            {
                case "Diurno (07:00-13:00)": descuento = importe * 0.3; break;
                case "Tarde (13:00-19:00)": descuento = importe * 0.2; break;
                case "Noche (19:00-23:00)": descuento = importe * 0.1; break;
                case "Madrugada (23:00-07:00)": descuento = importe * 0.3; break;
            }
            costoLlamada = importe - descuento;
        }

        void imprimirRegistro()
        {
            ListViewItem fila = new ListViewItem(tipo);
            fila.SubItems.Add(horario);
            fila.SubItems.Add(minutos.ToString());
            fila.SubItems.Add(costoMinuto.ToString("0.00"));
            fila.SubItems.Add(costoLlamada.ToString("0.00"));
            lvRegistro.Items.Add(fila);
        }

        void imprimirEstadisticas()
        {
            //Contar el numero de llamadas entre 10 y 30 minutos
            numeroLlamadas();

            //Monto acumulado del costo por llamada por tipo
            costoAcumuladoxtipo();

            //Mayor costo por llamada, que tipo y horario
            mayorMontoLlamada();

            //Enviando los resultados
            lvEstadisticas.Items.Clear();
            string[] elementosFila = new string[2];
            ListViewItem row;

            elementosFila[0] = "Numero de llamadas entre 10 y 30 minutos";
            elementosFila[1] = cLlamadas.ToString();
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Costo acumulado por Local Nacional";
            elementosFila[1] = aLocNac.ToString();
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Costo acumulado por Local Internacional";
            elementosFila[1] = aLocInt.ToString();
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Costo acumulado por Movil Nacional";
            elementosFila[1] = aMovNac.ToString();
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Costo acumulado por Movil Internacional";
            elementosFila[1] = aMovInt.ToString();
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Mayor monto de llamada";
            elementosFila[1] = mayorMonto.ToString();
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Tipo de llamada con mayor monto";
            elementosFila[1] = tipoMayor;
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Horario con mayor monto";
            elementosFila[1] = horarioMayor;
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);
        }
            
        //Determinar el numero de llamadas entre 10 y 30 minutos
        void numeroLlamadas()
        {
            cLlamadas = 0;
            for (int i = 0; i < lvRegistro.Items.Count; i++)
            {
                int minutos = int.Parse(lvRegistro.Items[i].SubItems[2].Text);
                if (minutos >= 10 && minutos <= 30) cLlamadas++;
            }
        }

        //Determinar el total acumulado del costo por llamada por tipo 
        void costoAcumuladoxtipo()
        {
            aLocNac = 0;aLocInt = 0;aMovNac = 0;aMovInt = 0;
            for (int i = 0; i < lvRegistro.Items.Count; i++)
            {
                //Capturando el tipo de llamada
                string t = lvRegistro.Items[i].SubItems[0].Text;
                if (t == "Local Nacional")
                    aLocNac += double.Parse(lvRegistro.Items[i].SubItems[4].Text);
                else if (t == "Local Internacional")
                    aLocInt += double.Parse(lvRegistro.Items[i].SubItems[4].Text);
                else if (t == "Movil Nacional")
                    aMovNac += double.Parse(lvRegistro.Items[i].SubItems[4].Text);
                else if (t == "Movil Internacional")
                    aMovInt += double.Parse(lvRegistro.Items[i].SubItems[4].Text);
            }
        }

        //Determinar el mayor monto de llamada 
        void mayorMontoLlamada()
        {
            int posicion = 0;
            mayorMonto = double.Parse(lvRegistro.Items[0].SubItems[4].Text);
            
            for (int i = 0; i < lvRegistro.Items.Count; i++)
            {
                if (double.Parse(lvRegistro.Items[i].SubItems[4].Text) > mayorMonto)
                {
                    mayorMonto = double.Parse(lvRegistro.Items[i].SubItems[4].Text);
                    posicion = i;
                }
            }

            tipoMayor = lvRegistro.Items[posicion].SubItems[0].Text;
            horarioMayor = lvRegistro.Items[posicion].SubItems[1].Text;
        }
    }
}

