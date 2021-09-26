using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using C1.Win.C1Tile;
namespace Image_Gallery_Demo
{
    public partial class ImageGallery : Form
    {
        DataFetcher datafetch = new DataFetcher(); // creating object of datafetch class
        List<ImageItem> imagesList; // a list of objects of class ImageItem
        int checkedItems = 0;
        public ImageGallery()
        {
            InitializeComponent();
        }
        private async void OnSearchClick(object sender, EventArgs e)
        {
            // setting status bar visiblity to truw to show progress bar
            statusStrip1.Visible = true;
            // storing the list of objects returned by GetImageData method of DataFetcher class in imageList
            imagesList = await datafetch.GetImageData(_search.Text);
            // passing imageList to AddTiles Method to display images
            AddTiles(imagesList);
            //setting status bar visiblity to false
            statusStrip1.Visible = false;
        }

        private void OnExportClick(object sender, EventArgs e)
        {
            // using try catch block to catch any exception during saving like if the
            // user closes the save dialog box 
            try
            {
                List<Image> images = new List<Image>();
                foreach (Tile tile in _imageTileControl.Groups[0].Tiles)
                {
                    if (tile.Checked)
                    {
                        images.Add(tile.Image); // adding checked tiles images to list image
                    }
                }

                ConvertToPdf(images); // converting selected images to pdf

                SaveFileDialog saveFile = new SaveFileDialog(); // showing save dialog box

                saveFile.DefaultExt = "pdf";
                saveFile.Filter = "PDF files (*.pdf)|*.pdf*";
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    c1PdfDocument1.Save(saveFile.FileName); // using c1PdfDocument control to save images to pdf file
                }
            }
            catch
            {
                Console.WriteLine("Operation Aborted");
            }
        }

        private void ConvertToPdf(List<Image> images)
        {
            // creating pdf file with image on each page of the document
            RectangleF rect = c1PdfDocument1.PageRectangle;
            bool firstPage = true;
            foreach (var selectedimg in images)
            {
                if (!firstPage)
                {
                    c1PdfDocument1.NewPage();
                }
                firstPage = false;
                rect.Inflate(-72, -72);
                c1PdfDocument1.DrawImage(selectedimg, rect);
            }
        }

        private void OnExportImagePaint(object sender, PaintEventArgs e)
        {
            // drawing rectange around export to pdf image
            Rectangle r = new Rectangle(_exportImage.Location.X, _exportImage.Location.Y, _exportImage.Width, _exportImage.Height);
            r.X -= 29;
            r.Y -= 3;
            r.Width+=10;
            r.Height--;
            Pen p = new Pen(Color.LightGray);
            e.Graphics.DrawRectangle(p, r);
            e.Graphics.DrawLine(p, new Point(0, 43), new Point(this.Width, 43));

        }

        private void OnSearchPanelPaint(object sender, PaintEventArgs e)
        {
            // drawing a rectangle around search item panel control
            Rectangle r = _searchBox.Bounds;
            r.Inflate(3, 3);
            Pen p = new Pen(Color.LightGray);
            e.Graphics.DrawRectangle(p, r);
        }

        private void OnTileChecked(object sender, C1.Win.C1Tile.TileEventArgs e)
        {
            checkedItems++; // incrementing checkedItems to control the visiblity of _exportImage Picture box 
            _exportImage.BringToFront(); // bringing _exportImage picturebox to front of c1Tile control
            _exportImage.Visible = true;
            pictureBox1.BringToFront(); // bringing  picturebox1 to front of c1Tile control
            pictureBox1.Visible = true;
        }

        private void OnTileUnchecked(object sender, C1.Win.C1Tile.TileEventArgs e)
        {
            checkedItems--;

            // condition to set visiblity to false when no image is selected
            _exportImage.Visible = checkedItems > 0;
            pictureBox1.Visible = checkedItems > 0;
        }

        private void OnTileControlPaint(object sender, PaintEventArgs e)
        {
            // drawing a line above tile control to seprate it from export image picturebox
            Pen p = new Pen(Color.LightGray);
            e.Graphics.DrawLine(p, 0, 40, 800, 40);
        }

        private void AddTiles(List<ImageItem> imageList)
        {
            // adding images to tiles 
            _imageTileControl.Groups[0].Tiles.Clear();
            foreach (var imageitem in imageList)
            {
                Tile tile = new Tile();
                tile.HorizontalSize = 2;
                tile.VerticalSize = 2;
                _imageTileControl.Groups[0].Tiles.Add(tile);
                Image img = Image.FromStream(new MemoryStream(imageitem.Base64));
                Template tl = new Template();
                ImageElement ie = new ImageElement();
                ie.ImageLayout = ForeImageLayout.Stretch;
                tl.Elements.Add(ie);
                tile.Template = tl;
                tile.Image = img;
            }
        }
        

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = new Rectangle(_exportImage.Location.X, _exportImage.Location.Y, _exportImage.Width, _exportImage.Height);
            r.X -= 29;
            r.Y -= 3;
            r.Width += 10;
            r.Height--;
            Pen p = new Pen(Color.LightGray);
            e.Graphics.DrawRectangle(p, r);
            e.Graphics.DrawLine(p, new Point(0, 43), new Point(this.Width, 43));
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to picturebox1.
            List<Image> images = new List<Image>();
            foreach (Tile tile in _imageTileControl.Groups[0].Tiles)
            {
                if (tile.Checked)
                {
                    images.Add(tile.Image); // adding checked tiles images to list image
                }
            }
            foreach (var selectedimg in images)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Jpeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
                saveFileDialog1.Title = "Save an Image File";
                saveFileDialog1.ShowDialog();

                // If the file name is not an empty string open it for saving.
                if (saveFileDialog1.FileName != "")
                {
                    // Saves the Image via a FileStream created by the OpenFile method.
                    System.IO.FileStream fs =
                        (System.IO.FileStream)saveFileDialog1.OpenFile();
                    // Saves the Image in the appropriate ImageFormat based upon the
                    // File type selected in the dialog box.
                    // NOTE that the FilterIndex property is one-based.
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            selectedimg.Save(fs,
                              System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;

                        case 2:
                            selectedimg.Save(fs,
                              System.Drawing.Imaging.ImageFormat.Bmp);
                            break;

                        case 3:
                            selectedimg.Save(fs,
                              System.Drawing.Imaging.ImageFormat.Gif);
                            break;
                    }

                    fs.Close();

                }
            }
        }
    }
}
