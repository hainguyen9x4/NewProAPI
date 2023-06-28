namespace CreateMail1s
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Take_Click(object sender, EventArgs e)
        {
            var number = 0;
            var mailHost = "@mail1s.edu.vn";
            var prefix = "h";
            if (int.TryParse(txtNumber.Text, out number))
            {
                txtMail.Text = prefix + number.ToString("D3") + mailHost;
                Clipboard.SetText(txtMail.Text);
                number++;
            }

        }
    }
}