using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace ProcessamentoImagens
{
    public partial class frmPrincipal : Form
    {
        private Image image;
        private Bitmap imageBitmap;
        private List<Point> pontosRetangulo = new List<Point>();

        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void btnAbrirImagem_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.Filter = "Arquivos de Imagem (*.jpg;*.gif;*.bmp;*.png)|*.jpg;*.gif;*.bmp;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                image = Image.FromFile(openFileDialog.FileName);
                pictBoxImg1.Image = image;
                pictBoxImg1.SizeMode = PictureBoxSizeMode.AutoSize;
                pictBoxImg2.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            pictBoxImg1.Image = null;
            pictBoxImg2.Image = null;
        }

        //esqueletizar zhang suen
        private void btnEsqueletizarComDMA_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.EsqueletizarDMA(imageBitmap, imgDest);
            imageBitmap = new Bitmap(imgDest);
            pictBoxImg1.Image = imageBitmap; //para que o algoritmo do zhang suen já possa pegar a imagem correta
            pictBoxImg2.Image = imgDest;
        }

        //contorno countour following
        private void btnContornoComDMA_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.ContornoDMA(imageBitmap, imgDest, pontosRetangulo);
            pictBoxImg2.Image = imgDest;
        }

        //retangulos com DMA
        private void btnRetanguloComDMA_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.DesenharRetangulosDMA(imageBitmap, imgDest, pontosRetangulo);
            pictBoxImg2.Image = imgDest;
        }
    }
}
