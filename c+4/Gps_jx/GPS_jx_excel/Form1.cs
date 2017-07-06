using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPS_jx_excel
{
	public partial class Form1 : Form
	{

		public Form1()
		{
			InitializeComponent();
			OpenGPS.Click += OpenGPS_Click;
		}

		private void OpenGPS_Click(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
