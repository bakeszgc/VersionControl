using IRF_06v2_EHMF1V.Entities;
using IRF_06v2_EHMF1V.MnbServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace IRF_06v2_EHMF1V
{
    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();
        //BindingList<string> Currencies = new BindingList<string>();
        private string result;

        public Form1()
        {
            InitializeComponent();
            dataGridView1.DataSource = Rates;
            //comboBox1.DataSource = Currencies;
            CurrenciesLekerdez();
            RefreshData();
        }

        private void ExchangeRates()
        {
            var mnbService = new MNBArfolyamServiceSoapClient();
            var request = new GetExchangeRatesRequestBody()
            {
                currencyNames=comboBox1.SelectedItem.ToString(),
                startDate=dateTimePicker1.Value.ToString(),
                endDate=dateTimePicker2.Value.ToString()
            };
            var response = mnbService.GetExchangeRates(request);
            result = response.GetExchangeRatesResult;
        }

        private void Feldolgozas()
        {
            var xml = new XmlDocument();
            xml.LoadXml(result);

            foreach (XmlElement element in xml.DocumentElement)
            {
                var rate = new RateData();
                Rates.Add(rate);

                rate.Date = DateTime.Parse(element.GetAttribute("date"));

                var childElement = (XmlElement)element.ChildNodes[0];
                rate.Currency = childElement.GetAttribute("curr");

                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0) rate.Value = value / unit;
            }
        }

        private void DiagramMegjelenit()
        {
            chartRateData.DataSource = Rates;

            var series = chartRateData.Series[0];
            series.ChartType = SeriesChartType.Line;
            series.XValueMember = "Date";
            series.YValueMembers = "Value";
            series.BorderWidth = 2;

            var legend = chartRateData.Legends[0];
            legend.Enabled = false;

            var chartArea = chartRateData.ChartAreas[0];
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisY.IsStartedFromZero = false;
        }

        private void RefreshData()
        {
            Rates.Clear();
            ExchangeRates();
            Feldolgozas();
            DiagramMegjelenit();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void CurrenciesLekerdez()
        {
            var mnbServiceCurrencies = new MNBArfolyamServiceSoapClient();
            var requestCurrencies = new GetCurrenciesRequest();
            var resultCurrencies = requestCurrencies.GetCurrencies;

            Console.WriteLine(resultCurrencies);
        }
    }
}
