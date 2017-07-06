using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace demo
{
	public partial class Form1 : Form
	{
		
		public Form1()
		{
			
			InitializeComponent();
			this.logbutton.Click += Logbutton_Click;
		}

		private void Logbutton_Click(object sender, EventArgs e)
		{
			log4net.ILog log = log4net.LogManager.GetLogger("MyLogger");

		}

	}

	
}
