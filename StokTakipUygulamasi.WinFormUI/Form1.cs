using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StokTakipUygulamasi.Entity;
using StokTakipUygulamasi.ORM;
using System.Data.SqlClient;
using System.IO;

namespace StokTakipUygulamasi.WinFormUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region ProductControls
        private void tabPageProducts_Enter(object sender, EventArgs e)
        {
            try
            {
                ComboBoxCategoryFill(cbCategories);
                cbCategories.SelectedIndex = 0;
                ListViewProductFill(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void contextMenuStripProduct_Opening(object sender, CancelEventArgs e)
        {
            if (lstwProducts.SelectedItems.Count <= 0)
            {
                e.Cancel = true;
                return;
            }
        }
        private void cbCategories_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ListViewProductFill(Convert.ToInt32(cbCategories.SelectedValue));
            txtSearch.Text = "";
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ListViewProductFill(Convert.ToInt32(cbCategories.SelectedValue), txtSearch.Text);
        }
        private void tabPageProductAdd_Enter(object sender, EventArgs e)
        {
            try
            {
                ComboBoxCategoryFill(cbAddCategories);
                cbAddCategories.SelectedIndex = cbCategories.SelectedIndex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            try
            {
                Products prod = new Products
                {
                    ProductName = txtProductName.Text,
                    CategoryID = (int)cbAddCategories.SelectedValue,
                    QuantityPerUnit = txtProductUnit.Text,
                    UnitPrice = Convert.ToDecimal(txtProductPrice.Text),
                    UnitsInStock = (int)nUdUnitsInStock.Value
                };
                ProductORM prodORM = new ProductORM();
                bool result = prodORM.Insert(prod);
                if (result)
                {
                    MessageBox.Show("Ürün Ekleme İşleminiz Başarıyla Gerçekleştirildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    ListViewProductFill(Convert.ToInt32(cbAddCategories.SelectedValue));
                    cbCategories.SelectedIndex = cbAddCategories.SelectedIndex;
                    tabControlProducts.SelectedIndex = 0;
                    ProductControlsClear(true);
                }
                else
                {
                    MessageBox.Show("İşlem Sırasında Bir Hata Meydana Geldi !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch
            {
                MessageBox.Show("Fiyat Girişi Yapılırken Bir Sorunla Karşılaşıldı");
                txtProductPrice.Clear();
                txtProductPrice.Focus();
            }
        }
        bool updateProductTabPage = false;
        private void tabPageProductUpdate_Enter(object sender, EventArgs e)
        {
            if (!updateProductTabPage)
            {
                MessageBox.Show("Bu Sekmeye Ürünler Listesinden İlgili Ürüne Sağ Tıklayarak Ulaşabilirsiniz !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControlProducts.SelectedIndex = 0;
                return;
            }
            updateProductTabPage = false;
        }
        Products prod;
        private void productUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                updateProductTabPage = true;
                prod = new Products();
                prod.ProductID = Convert.ToInt32(lstwProducts.SelectedItems[0].Tag);
                prod.ProductName = lstwProducts.SelectedItems[0].Text;
                prod.UnitPrice = Convert.ToDecimal(lstwProducts.SelectedItems[0].SubItems[1].Text);
                prod.QuantityPerUnit = lstwProducts.SelectedItems[0].SubItems[2].Text;
                prod.UnitsInStock = int.Parse(lstwProducts.SelectedItems[0].SubItems[3].Text);
                prod.CategoryID = (int)cbCategories.SelectedValue;
                //---------------------------------------------------------
                ComboBoxCategoryFill(cbUpdateCategories);
                cbUpdateCategories.SelectedIndex = cbCategories.SelectedIndex;
                //---------------------------------------------------------
                txtUpdateProductName.Text = prod.ProductName;
                txtUpdateUnit.Text = prod.QuantityPerUnit;
                txtUpdateProductPrice.Text = prod.UnitPrice.ToString();
                nUdUpdateUnitsInStock.Value = prod.UnitsInStock;
                tabControlProducts.SelectedIndex = 2;
            }
            catch (Exception)
            {
                MessageBox.Show("Geçerli Bir Seçim Yapınız");
            }
        }
        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            try
            {
                prod.ProductName = txtUpdateProductName.Text;
                prod.QuantityPerUnit = txtUpdateUnit.Text;
                prod.UnitPrice = Convert.ToDecimal((txtUpdateProductPrice.Text));
                prod.UnitsInStock = (int)nUdUpdateUnitsInStock.Value;
                prod.CategoryID = (int)cbUpdateCategories.SelectedValue;

                ProductORM prodORM = new ProductORM();
                bool result = prodORM.Update(prod);
                if (result)
                {
                    MessageBox.Show("Ürün Güncelleme İşleminiz Başarıyla Gerçekleştirildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    ListViewProductFill(Convert.ToInt32(cbUpdateCategories.SelectedValue));
                    cbCategories.SelectedIndex = cbUpdateCategories.SelectedIndex;
                    tabControlProducts.SelectedIndex = 0;
                    ProductControlsClear(false);
                }
                else
                {
                    MessageBox.Show("İşlem Sırasında Bir Hata Meydana Geldi !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch
            {
                MessageBox.Show("Fiyat Girişi Yapılırken Bir Sorunla Karşılaşıldı");
                txtUpdateProductPrice.Clear();
                txtUpdateProductPrice.Focus();
            }
        }
        private void productDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string productName = lstwProducts.SelectedItems[0].Text;
                int productID = Convert.ToInt32(lstwProducts.SelectedItems[0].Tag);
                DialogResult dialogResult = MessageBox.Show("Seçili Ürünü Silmek İstediğinize Emin Misiniz ?", productName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    ProductORM prodORM = new ProductORM();
                    bool result = prodORM.Delete(productID);
                    if (result)
                    {
                        MessageBox.Show("Ürün Silme İşleminiz Başarıyla Gerçekleştirildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        ListViewProductFill(Convert.ToInt32(cbCategories.SelectedValue));
                    }
                    else
                    {
                        MessageBox.Show("İşlem Sırasında Bir Hata Meydana Geldi !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void sellProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string productName = lstwProducts.SelectedItems[0].Text;
                int productID = Convert.ToInt32(lstwProducts.SelectedItems[0].Tag);
                DialogResult dialogResult = MessageBox.Show("Seçili Ürünün Stok Miktarını " + toolStripTextBoxQuantity.Text + " Adet Eksiltmek İstediğinize Emin Misiniz ?", productName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    ProductORM prodORM = new ProductORM();
                    bool result = prodORM.StockDecrease(productID, int.Parse(toolStripTextBoxQuantity.Text));
                    if (result)
                    {
                        MessageBox.Show("Stok Azaltma İşleminiz Başarıyla Gerçekleştirildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        ListViewProductFill(Convert.ToInt32(cbCategories.SelectedValue));
                    }
                    else
                    {
                        MessageBox.Show("İşlem Sırasında Bir Hata Meydana Geldi !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void ListViewProductFill(int categoryID)
        {
            try
            {
                lstwProducts.Items.Clear();
                ProductORM prodORM = new ProductORM();
                DataTable dt = prodORM.ProductInCategory(categoryID);
                foreach (DataRow item in dt.Rows)
                {
                    ListViewItem li = new ListViewItem();
                    li.Tag = item["ProductID"].ToString();
                    li.Text = item["ProductName"].ToString();
                    li.SubItems.Add(item["UnitPrice"].ToString());
                    li.SubItems.Add(item["QuantityPerUnit"].ToString());
                    if (Convert.ToInt32(item["UnitsInStock"]) < 15)
                    {
                        li.SubItems.Add(item["UnitsInStock"].ToString());
                        li.BackColor = Color.Red;
                    }
                    else
                    {
                        li.SubItems.Add(item["UnitsInStock"].ToString());
                    }
                    li.SubItems.Add(item["CategoryName"].ToString());
                    lstwProducts.Items.Add(li);

                }
                lstwProducts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Ürünleri Listeleme İşleminde Bir Hata İle Karşılaşıldı");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void ListViewProductFill(int categoryID, string searchText)
        {
            try
            {
                lstwProducts.Items.Clear();
                ProductORM prodORM = new ProductORM();
                DataTable dt = prodORM.ProductSearch(categoryID, searchText);
                foreach (DataRow item in dt.Rows)
                {
                    ListViewItem li = new ListViewItem();
                    li.Tag = item["ProductID"].ToString();
                    li.Text = item["ProductName"].ToString();
                    li.SubItems.Add(item["UnitPrice"].ToString());
                    li.SubItems.Add(item["QuantityPerUnit"].ToString());
                    if (Convert.ToInt32(item["UnitsInStock"]) < 15)
                    {
                        li.SubItems.Add(item["UnitsInStock"].ToString());
                        li.BackColor = Color.Red;
                    }
                    else
                    {
                        li.SubItems.Add(item["UnitsInStock"].ToString());
                    }
                    li.SubItems.Add(item["CategoryName"].ToString());
                    lstwProducts.Items.Add(li);

                }
                lstwProducts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Ürünleri Listeleme İşleminde Bir Hata İle Karşılaşıldı");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void ComboBoxCategoryFill(ComboBox comboBox)
        {
            try
            {
                CategoryORM catORM = new CategoryORM();
                comboBox.DataSource = catORM.List();
                comboBox.DisplayMember = "CategoryName";
                comboBox.ValueMember = "CategoryID";
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Kategorileri Listeleme İşleminde Bir Hata İle Karşılaşıldı");
            }
        }
        void ProductControlsClear(bool addOrUpdate)
        {
            //true ise Ekleme sayfasındaki controlleri temizliyor.
            //false ise Güncelleme sayfasındaki controlleri temizliyor.
            if (addOrUpdate)
            {
                txtProductUnit.Clear();
                txtProductPrice.Clear();
                txtProductName.Clear();
                nUdUnitsInStock.Value = 0;
            }
            else
            {
                txtUpdateProductName.Clear();
                txtUpdateProductPrice.Clear();
                txtUpdateUnit.Clear();
                nUdUpdateUnitsInStock.Value = 0;
            }

        }
        #endregion

        #region CategoryControls
        private void tabPageCategories_Enter(object sender, EventArgs e)
        {
            try
            {
                CategoryORM catORM = new CategoryORM();
                dGvCategories.DataSource = catORM.List();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void dGvCategories_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int satir = dGvCategories.HitTest(e.X, e.Y).RowIndex;
                if (satir > -1)
                {
                    dGvCategories.Rows[satir].Selected = true;
                    contextMenuStripCategory.Show(Cursor.Position);
                }
            }
        }
        string imagePath = "";
        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog imageFile = new OpenFileDialog();
            imageFile.Filter = "Jpeg Dosyası(.jpg) |*.jpg|Png Dosyası(.png) |*.png";
            imageFile.Title = "Fotoğraf Dosyası Seçiniz..";
            if (imageFile.ShowDialog() == DialogResult.OK)
            {
                pbPreview.Load(imageFile.FileName);
                imagePath = imageFile.FileName;
            }
        }
        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            try
            {
                Categories cat = new Categories
                {
                    CategoryName = txtCategoryName.Text,
                    Description = rtxtCategoryDescription.Text,
                    Picture = ImageConvertToByte(imagePath)
                };
                imagePath = "";
                CategoryORM catORM = new CategoryORM();
                bool result = catORM.Insert(cat);
                if (result)
                {
                    MessageBox.Show("Kategori Ekleme İşleminiz Başarıyla Gerçekleştirildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    dGvCategories.DataSource = catORM.List();
                    tabControlCategories.SelectedIndex = 0;
                    CategoryControlsClear(true);
                }
                else
                {
                    MessageBox.Show("İşlem Sırasında Bir Hata Meydana Geldi !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        bool updateCategoryTabPage = false;
        private void tabPageCategoryUpdate_Enter(object sender, EventArgs e)
        {
            //kategori seçimi yapılmadan güncelleme sekmesine ulaşılmasını engelleyen kod.
            //bu methodun üstünde oluşturulan bool değişken ile kontrol ediliyor.
            if (!updateCategoryTabPage)
            {
                MessageBox.Show("Bu Sekmeye Kategoriler Listesinden İlgili Kategoriye Sağ Tıklayarak Ulaşabilirsiniz !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControlCategories.SelectedIndex = 0;
                return;
            }
            updateCategoryTabPage = false;
        }
        Categories cat;
        private void categoryUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                updateCategoryTabPage = true;
                pbUpdatePreview.Image = null;
                cat = new Categories();
                cat.CategoryID = (int)dGvCategories.SelectedRows[0].Cells[0].Value;
                cat.CategoryName = dGvCategories.SelectedRows[0].Cells[1].Value.ToString();
                cat.Description = dGvCategories.SelectedRows[0].Cells[2].Value.ToString();
                string categoryPictureNullControl = dGvCategories.SelectedRows[0].Cells[3].Value.ToString();
                if (categoryPictureNullControl != "")
                {
                    cat.Picture = (byte[])dGvCategories.SelectedRows[0].Cells[3].Value;
                    pbUpdatePreview.Image = ByteConvertToImage(cat.Picture);
                }
                txtUpdateCategoryName.Text = cat.CategoryName;
                rtxtUpdateCategoryDescription.Text = cat.Description;
                tabControlCategories.SelectedIndex = 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnUpdateChooseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog imageFile = new OpenFileDialog();
            imageFile.Filter = "Jpeg Dosyası(.jpg) |*.jpg|Png Dosyası(.png) |*.png";
            imageFile.Title = "Fotoğraf Dosyası Seçiniz..";
            if (imageFile.ShowDialog() == DialogResult.OK)
            {
                pbUpdatePreview.Load(imageFile.FileName);
                imagePath = imageFile.FileName;
            }
        }
        private void btnUpdateCategory_Click(object sender, EventArgs e)
        {
            try
            {
                cat.CategoryName = txtUpdateCategoryName.Text;
                cat.Description = rtxtUpdateCategoryDescription.Text;
                cat.Picture = ImageConvertToByte(imagePath);
                imagePath = "";

                CategoryORM catORM = new CategoryORM();
                bool result = catORM.Update(cat);
                if (result)
                {
                    MessageBox.Show("Kategori Güncelleme İşleminiz Başarıyla Gerçekleştirildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    dGvCategories.DataSource = catORM.List();
                    tabControlCategories.SelectedIndex = 0;
                    CategoryControlsClear(false);
                }
                else
                {
                    MessageBox.Show("İşlem Sırasında Bir Hata Meydana Geldi !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void categoryDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string categoryName = dGvCategories.SelectedRows[0].Cells[1].Value.ToString();
                int categoryID = (int)dGvCategories.SelectedRows[0].Cells[0].Value;
                DialogResult dialogResult = MessageBox.Show("Seçili Kategoriyi Silmek İstediğinize Emin Misiniz ?", categoryName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    CategoryORM catORM = new CategoryORM();
                    bool result = catORM.Delete(categoryID);
                    if (result)
                    {
                        MessageBox.Show("Kategori Silme İşleminiz Başarıyla Gerçekleştirildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        dGvCategories.DataSource = catORM.List();
                    }
                    else
                    {
                        MessageBox.Show("Kategoriye Ait Ürünler Bulunmaktadır. Lütfen İlk Önce Silmek İstediğiniz Kategoriye Ait Ürünleri Siliniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void productsOfCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                string categoryName = dGvCategories.SelectedRows[0].Cells[1].Value.ToString();
                int categoryID = (int)dGvCategories.SelectedRows[0].Cells[0].Value;
                DialogResult dialogResult = MessageBox.Show("Seçili Kategoriye Ait Tüm Ürünleri Silmek İstediğinize Emin Misiniz ?", categoryName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    CategoryORM catORM = new CategoryORM();
                    bool result = catORM.ProductsOfCategoryDelete(categoryID);
                    if (result)
                    {
                        MessageBox.Show("Kategoriye Ait Tüm Ürünler Başarıyla Silindi", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        dGvCategories.DataSource = catORM.List();
                    }
                    else
                    {
                        MessageBox.Show("İşlem Sırasında Bir Hata İle Karşılaşıldı", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        Byte[] ImageConvertToByte(string imagePath)
        {
            byte[] imageByte = null;
            if (imagePath != "")
            {
                FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageByte = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
                imagePath = "";
            }
            return imageByte;
        }
        Image ByteConvertToImage(byte[] imageByte)
        {
            byte[] data = imageByte;
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            Image img = Image.FromStream(ms);
            return img;
        }
        void CategoryControlsClear(bool addOrUpdate)
        {
            //true ise Ekleme sayfasındaki controlleri temizliyor.
            //false ise Güncelleme sayfasındaki controlleri temizliyor.
            if (addOrUpdate)
            {
                txtCategoryName.Clear();
                rtxtCategoryDescription.Clear();
                pbPreview.Image = null;
            }
            else
            {
                txtUpdateCategoryName.Clear();
                rtxtUpdateCategoryDescription.Clear();
                pbUpdatePreview.Image = null;
            }
        }
        #endregion




    }
}
