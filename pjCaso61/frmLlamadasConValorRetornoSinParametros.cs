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
    public partial class frmLlamadasConValorRetornoSinParametros : Form
    {
        public frmLlamadasConValorRetornoSinParametros()
        {
            InitializeComponent();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            //Imprimir el registro de llamadas
            ListViewItem fila = new ListViewItem(getTipo());
            fila.SubItems.Add(getHorario());
            fila.SubItems.Add(getMinutos().ToString());
            fila.SubItems.Add(asignaCostoxMinuto().ToString("0.00"));
            fila.SubItems.Add(asignaCostoxLlamada().ToString("0.00"));
            lvRegistro.Items.Add(fila);

            lvEstadisticas.Items.Clear();
        }

        private void frmLlamadasConValorRetornoSinParametros_Load(object sender, EventArgs e)
        {
            //Mostrando la fecha actual
            lblFecha.Text = DateTime.Now.ToShortDateString();
        }

        private void cboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Asignar el costo por minuto 
            lblCosto.Text = asignaCostoxMinuto().ToString("C");
        }

        private void btnEstadisticas_Click(object sender, EventArgs e)
        {
            //Determinar el total acumulado del costo por llamada por tipo 
            double aLocNac = 0,aLocInt = 0,aMovNac = 0,aMovInt = 0;
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

            //Enviando los resultados
            lvEstadisticas.Items.Clear();
            string[] elementosFila = new string[2];
            ListViewItem row;

            elementosFila[0] = "Numero de llamadas entre 10 y 30 minutos";
            elementosFila[1] = numeroLlamadas().ToString();
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

            elementosFila[0] = "Mayor monto de llamada";
            elementosFila[1] = mayorMonto.ToString("C");
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Tipo de llamada con mayor monto";
            elementosFila[1] = lvRegistro.Items[posicion].SubItems[0].Text;
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);

            elementosFila[0] = "Horario con mayor monto";
            elementosFila[1] = lvRegistro.Items[posicion].SubItems[1].Text;
            row = new ListViewItem(elementosFila);
            lvEstadisticas.Items.Add(row);
        }

        private void tHora_Tick(object sender, EventArgs e)
        {
            lblHora.Text = DateTime.Now.ToString("hh:mm:ss");
        }
        
        //Capturando los valores del formulario 
        string getTipo()
        {
            return cboTipo.Text;
        }

        string getHorario()
        {
            return cboHorario.Text;
        }

        int getMinutos()
        {
            return int.Parse(txtMinutos.Text);
        }
        
        //Asignacion de costo por minuto segun el tipo 
        double asignaCostoxMinuto()
        {
            double costoMinuto=0;
            switch (cboTipo.Text)
            {
                case "Local Nacional": costoMinuto = 0.20; break;
                case "Local Internacional": costoMinuto = 0.50; break;
                case "Movil Nacional": costoMinuto = 1.20; break;
                case "Movil Internacional": costoMinuto = 2.20; break;
            }
            return costoMinuto;
        }

        //Asignar el costo por llamada segun el horario
        double asignaCostoxLlamada()
        {
            //Calculando el importe
            double importe = asignaCostoxMinuto() * getMinutos();

            //Determinado el descuento segun el horario
            double descuento = 0;
            switch (getHorario())
            {
                case "Diurno (07:00-13:00)": descuento = importe * 0.3; break;
                case "Tarde (13:00-19:00)": descuento = importe * 0.2; break;
                case "Noche (19:00-23:00)": descuento = importe * 0.1; break;
                case "Madrugada (23:00-07:00)": descuento = importe * 0.3; break;
            }
            return importe - descuento;
        }
        
        //Determinar el numero de llamadas entre 10 y 30 minutos
        int numeroLlamadas()
        {
            int cLlamadas = 0;
            for (int i = 0; i < lvRegistro.Items.Count; i++)
            {
                int minutos = int.Parse(lvRegistro.Items[i].SubItems[2].Text);
                if (minutos >= 10 && minutos <= 30) cLlamadas++;
            }
            return cLlamadas;
        }
    }
}
