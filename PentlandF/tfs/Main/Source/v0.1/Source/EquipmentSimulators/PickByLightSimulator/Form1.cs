using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;

namespace PickByLightSimulator
{
    public partial class Form1 : Form
    {
        private const int Rows = 3;
        private const int Columns = 3;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<string> ButtonClicked; 

        public Form1()
        {
            InitializeComponent();
            InitializeShelfLayout();
        }

        private void InitializeShelfLayout()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Size = new Size(Rows * 300, Columns * 200);
            
            var tableLayout = new TableLayoutPanel();
            this.Controls.Add(tableLayout);
            tableLayout.Name = "layoutContainer";
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayout.RowCount = Rows;
            tableLayout.ColumnCount = Columns;

            const int width = 100/Columns;
            const int height = 100/Rows;
            for (var row = 0; row < Rows; row++)
            {
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, height));
                for (var col = 0; col < Columns; col++)
                {
                    tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, width));
                    var btn = new Button();
                    btn.Name = "btn" + (row + 1) + "_" + (col + 1);
                    btn.Text = (row + 1) + ":" + (col + 1);
                    btn.Font = new Font(FontFamily.GenericSansSerif, 14f, FontStyle.Bold);
                    btn.Dock = DockStyle.Fill;
                    btn.Click += btn_Click;
                    tableLayout.Controls.Add(btn, col, row);
                }
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if(btn == null) return;
            btn.BackColor = SystemColors.Control;
            if (ButtonClicked != null)
                ButtonClicked(btn.Text);
        }
    }
}
