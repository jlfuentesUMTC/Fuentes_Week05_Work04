using System;
using System.Globalization;
using System.Windows.Forms;

namespace Fuentes_Week05_Work04
{
    public partial class Form1 : Form
    {
        private readonly ErrorProvider errorProvider;
        private readonly initialData productData = new initialData();
        private bool updatingPrices;
        private bool loadingProduct;
        private string displayedBarcode;

        public Form1()
        {
            InitializeComponent();
            if (components == null)
                components = new System.ComponentModel.Container();
            errorProvider = new ErrorProvider(components)
            {
                ContainerControl = this
            };

            txtPrice.ReadOnly = true;
            cbActive.Checked = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new object[]
            {
                "Electronics",
                "Food",
                "Clothing",
                "Household",
                "Furniture",
                "Books and Stationery",
                "Personal Care",
                "Sports and Outdoors",
                "Other"
            });
            lblStatus.Text = "Status: Active";
        }

        private void txtCost_TextChanged(object sender, EventArgs e)
        {
            UpdatePriceFromCostAndMarkup();
        }

        private void txtMarkup_TextChanged(object sender, EventArgs e)
        {
            UpdatePriceFromCostAndMarkup();
        }

        private void txtPrice_TextChanged(object sender, EventArgs e)
        {
            // Price is calculated from cost and markup in this starter form.
        }

        private void UpdatePriceFromCostAndMarkup()
        {
            if (updatingPrices)
                return;

            decimal cost;
            decimal markup;
            if (!TryParseAmount(txtCost.Text, out cost) ||
                !TryParseAmount(txtMarkup.Text, out markup))
            {
                return;
            }

            updatingPrices = true;
            try
            {
                decimal price = cost * (1 + markup / 100m);
                txtPrice.Text = price.ToString("0.00", CultureInfo.CurrentCulture);
            }
            catch (OverflowException)
            {
                errorProvider.SetError(txtCost, "The calculated price is too large.");
            }
            finally
            {
                updatingPrices = false;
            }
        }

        private bool TryParseAmount(string input, out decimal amount)
        {
            return decimal.TryParse(
                input,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out amount);
        }

        private bool ValidateForm()
        {
            errorProvider.Clear();
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                errorProvider.SetError(txtBarcode, "Barcode is required.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errorProvider.SetError(txtName, "Product name is required.");
                isValid = false;
            }

            if (comboBox1.SelectedIndex < 0)
            {
                errorProvider.SetError(comboBox1, "Select a category.");
                isValid = false;
            }

            decimal cost;
            if (!TryParseAmount(txtCost.Text, out cost) || cost <= 0)
            {
                errorProvider.SetError(txtCost, "Enter a cost greater than zero.");
                isValid = false;
            }

            decimal markup;
            if (!TryParseAmount(txtMarkup.Text, out markup) || markup < 0)
            {
                errorProvider.SetError(txtMarkup, "Enter a markup of zero or more.");
                isValid = false;
            }

            if (!rbLocal.Checked && !rbAbroad.Checked)
            {
                errorProvider.SetError(gbManufactured, "Select Local or Abroad.");
                isValid = false;
            }

            return isValid;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            decimal cost;
            decimal markup;
            TryParseAmount(txtCost.Text, out cost);
            TryParseAmount(txtMarkup.Text, out markup);

            ProductInfo product = new ProductInfo
            {
                Barcode = txtBarcode.Text.Trim(),
                Name = txtName.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                Category = comboBox1.SelectedItem.ToString(),
                Origin = rbLocal.Checked ? "Local" : "Abroad",
                IsActive = cbActive.Checked,
                Cost = cost,
                Markup = markup,
                Price = cost * (1 + markup / 100m)
            };

            productData.SaveProduct(product);
            displayedBarcode = product.Barcode;
            MessageBox.Show(
                "Product saved for this session. Enter its barcode to find it again.",
                "Product Form",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            displayedBarcode = null;
            txtBarcode.Clear();
            txtName.Clear();
            txtDescription.Clear();
            comboBox1.SelectedIndex = -1;
            rbLocal.Checked = false;
            rbAbroad.Checked = false;
            cbActive.Checked = true;
            txtCost.Clear();
            txtMarkup.Clear();
            txtPrice.Clear();
            errorProvider.Clear();
            txtBarcode.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbActive_CheckedChanged(object sender, EventArgs e)
        {
            lblStatus.Text = cbActive.Checked ? "Status: Active" : "Status: Inactive";
        }

        private void rbLocal_CheckedChanged(object sender, EventArgs e) { }
        private void rbAbroad_CheckedChanged(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            if (loadingProduct)
                return;

            string barcode = txtBarcode.Text.Trim();
            ProductInfo product = productData.FindByBarcode(barcode);
            if (product != null)
            {
                ShowProduct(product);
                return;
            }

            if (!string.IsNullOrEmpty(displayedBarcode) &&
                !string.Equals(displayedBarcode, barcode, StringComparison.OrdinalIgnoreCase))
            {
                displayedBarcode = null;
                ClearProductFields();
            }
        }

        private void ShowProduct(ProductInfo product)
        {
            loadingProduct = true;
            try
            {
                txtBarcode.Text = product.Barcode;
                txtName.Text = product.Name;
                txtDescription.Text = product.Description;
                comboBox1.SelectedItem = product.Category;
                rbLocal.Checked = string.Equals(product.Origin, "Local", StringComparison.OrdinalIgnoreCase);
                rbAbroad.Checked = string.Equals(product.Origin, "Abroad", StringComparison.OrdinalIgnoreCase);
                cbActive.Checked = product.IsActive;
                txtCost.Text = product.Cost.ToString(CultureInfo.CurrentCulture);
                txtMarkup.Text = product.Markup.ToString(CultureInfo.CurrentCulture);
                txtPrice.Text = product.Price.ToString("0.00", CultureInfo.CurrentCulture);
                lblStatus.Text = product.IsActive ? "Status: Active" : "Status: Inactive";
                errorProvider.Clear();
                displayedBarcode = product.Barcode;
            }
            finally
            {
                loadingProduct = false;
            }
        }

        private void ClearProductFields()
        {
            loadingProduct = true;
            try
            {
                txtName.Clear();
                txtDescription.Clear();
                comboBox1.SelectedIndex = -1;
                rbLocal.Checked = false;
                rbAbroad.Checked = false;
                cbActive.Checked = true;
                txtCost.Clear();
                txtMarkup.Clear();
                txtPrice.Clear();
                errorProvider.Clear();
            }
            finally
            {
                loadingProduct = false;
            }
        }
        private void txtName_TextChanged(object sender, EventArgs e) { }
        private void txtDescription_TextChanged(object sender, EventArgs e) { }
        private void lblBarcode_Click(object sender, EventArgs e) { }
        private void lblName_Click(object sender, EventArgs e) { }
        private void txtDescription_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void gbManufactured_Enter(object sender, EventArgs e) { }
        private void lblStatus_Click(object sender, EventArgs e) { }
        private void lblCost_Click(object sender, EventArgs e) { }
        private void lblPrice_Click(object sender, EventArgs e) { }
        private void lblMarkup_Click(object sender, EventArgs e) { }
    }
}
