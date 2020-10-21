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
    public partial class frmLlamadasSinValorRetornoConParametros : Form
    {
        //** Declaracion de variables GLOBALES***//
        double costoMinuto;
        double costoLlamada;
        //** Fin de la declaracion de GLOBALES **//

        public frmLlamadasSinValorRetornoConParametros()
        {
            InitializeComponent();
            tHora.Enabled = true;
        }

        private void frmLlamadasSinValorRetornoNiParametros_Load(object sender, EventArgs e)
        {
            //Mostrando la fecha actual
            lblFecha.Text = DateTime.Now.ToShortDateString();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            //Capturando los datos 
            string tipo = cboTipo.Text;
            string horario = cboHorario.Text;
            int minutos = int.Parse(txtMinutos.Text);
            
            //Determinar el costo por minuto
            asignaCostoxMinuto(tipo);

            //Determinar el costo por llamada
            asignaCostoxLlamada(horario, minutos);

            //Imprimir el registro de llamadas
            imprimirRegistro(tipo, horario, minutos);

            lvEstadisticas.Items.Clear();
        }

        private void cboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Asignar el costo por minuto 
            asignaCostoxMinuto(cboTipo.Text);
            lblCosto.Text = costoMinuto.ToString("C");
        }

        private void tHora_Tick(object sender, EventArgs e)
        {
            lblHora.Text = DateTime.Now.ToString("hh:mm:ss");
        }

        private void btnEstadisticas_Click(object sender, EventArgs e)
        {
            //Determinar el numero de llamadas entre 10 y 30
            int cLlamadas = 0;
            for (int i = 0; i < lvRegistro.Items.Count; i++)
            {
                int minutos = int.Parse(lvRegistro.Items[i].SubItems[2].Text);
                if (minutos >= 10 && minutos <= 30) cLlamadas++;
            }

            //Determinar los valores acumulador por tipo de llamada
            double aLocNac = 0,aLocInt = 0, aMovNac = 0,aMovInt = 0;
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

            //Determinar el mayor monto de llamada 
            double mayorMonto = double.Parse(lvRegistro.Items[0].SubItems[4].Text);
            int posicion = 0;
            for (int i = 0; i < lvRegistro.Items.Count; i++)
            {
              if (double.Parse(lvRegistro.Items[i].SubItems[4].Text) > mayorMonto)
              {
                 mayorMonto = double.Parse(lvRegistro.Items[i].SubItems[4].Text);
                 posicion = i;
              }
            }

            string tipoMayor = lvRegistro.Items[posicion].SubItems[0].Text;
            string horarioMayor = lvRegistro.Items[posicion].SubItems[1].Text;

            //Mostrar los resultados en la lista de estadisticas
            imprimirEstadisticas(cLlamadas, aLocNac, aLocInt, aMovNac, aMovInt, mayorMonto, tipoMayor, horarioMayor);
        }

        //Asignacion de costo por minuto segun el tipo 
        void asignaCostoxMinuto(string tipo)
        {
            switch (tipo)
            {
                case "Local Nacional": costoMinuto = 0.20; break;
                case "Local Internacional": costoMinuto = 0.50; break;
                case "Movil Nacional": costoMinuto = 1.20; break;
                case "Movil Internacional": costoMinuto = 2.20; break;
            }
        }

        //Asignar el costo por llamada segun el horario
        void asignaCostoxLlamada(string horario, int minutos)
        {
            //Calculando el importe
            double importe = costoMinuto * minutos;

            //Determinado el descuento segun el horario
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

        //Enviando informacion a la lista lvRegistro
        void imprimirRegistro(string tipo, string horario, int minutos)
        {
            ListViewItem fila = new ListViewItem(tipo);
            fila.SubItems.Add(horario);
            fila.SubItems.Add(minutos.ToString());
            fila.SubItems.Add(costoMinuto.ToString("0.00"));
            fila.SubItems.Add(costoLlamada.ToString("0.00"));
            lvRegistro.Items.Add(fila);
        }

        //Enviando la informacion a la lista lvEstadisticas
        void imprimirEstadisticas(int cLlamadas, double aLocNac, double aLocInt,
                                  double aMovNac, double aMovInt, double mayorMonto,
                                  string tipoMayor, string horarioMayor)
        {
            //Enviando los resultados
            lvEstadisticas.Items.Clear();
            string[] elementosFila = new string[2];
            ListViewItem row;

            elementosFila[0] = "Numero de llamadas entre 10 y 30 minutos";
            elementosFila[1] = cLlamadas.ToString();
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Costo acumulado por Local Nacional";
            elementosFila[1] = aLocNac.ToString("C");
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Costo acumulado por Local Internacional";
            elementosFila[1] = aLocInt.ToString("C");
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Costo acumulado por Movil Nacional";
            elementosFila[1] = aMovNac.ToString("C");
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Costo acumulado por Movil Internacional";
            elementosFila[1] = aMovInt.ToString("C");
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Mayor monto de llamada";
            elementosFila[1] = mayorMonto.ToString("C");
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
    }
}
