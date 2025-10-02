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

        private void btnLuminanciaSemDMA_Click(object sender, EventArgs e)
        {
            //consigo chamar os métodos pois eles são estáticos na classe Filtros
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.convert_to_gray(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }

        private void btnLuminanciaComDMA_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.convert_to_grayDMA(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }

        private void btnNegativoSemDMA_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.negativo(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }

        private void btnNegativoComDMA_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.negativoDMA(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }

        private void btnEsqueletizarComDMA_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.EsqueletizarDMA(imageBitmap, imgDest);
            imageBitmap = new Bitmap(imgDest);
            pictBoxImg1.Image = imageBitmap;
            pictBoxImg2.Image = imgDest;
        }

        private void btnContornoComDMA_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.ContornoDMA(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }

        //espelho vertical
        private void btnEspelhoVertical_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.espelho_vertical(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }

        //espelho horizontal
        private void btnEspelhoHorizontal_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.espelho_horizontal(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }

        private void btnCanalAzul_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.canal_azul(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }
        private void btnCanalVerde_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.canal_verde(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }
        private void btnCanalVermelho_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.canal_vermelho(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }

        private void btnPretoBranco_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.preto_branco(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }

        private void btnNoventa_Click(object sender, EventArgs e)
        {
            //pego invertido para poder girar a minha imagem 90 graus
            imageBitmap = (Bitmap)image;
            int width = imageBitmap.Height;
            int height = imageBitmap.Width;
            Bitmap imgDest = new Bitmap(width, height);
            Filtros.noventa(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }

        private void btnInverteAzulVermelho_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.inverte_azul_vermelho(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }

        private void btnInverteDiagonal_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.inverte_diagonal(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }

        private void rachaQuatro_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imageBitmap = (Bitmap)image;
            Filtros.racha_quatro(imageBitmap, imgDest);
            pictBoxImg2.Image = imgDest;
        }
    }
}
