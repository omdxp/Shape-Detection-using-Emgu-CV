using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace _26_Shape_Detection_using_Contours
{
    public partial class Form1 : Form
    {
        Image<Bgr, byte> imgInput;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog()==DialogResult.OK)
                {
                    imgInput = new Image<Bgr, byte>(ofd.FileName);
                    pictureBox1.Image = imgInput.Bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void shapeDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (imgInput == null)
                {
                    throw new Exception("Please select an image");
                }

                var tmp = imgInput.SmoothGaussian(5).Convert<Gray, byte>()
                    .ThresholdBinaryInv(new Gray(230), new Gray(255)); // Inv because the background of the image is white, so remove it

                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                Mat hier = new Mat();

                CvInvoke.FindContours(tmp, contours,
                    hier,
                    Emgu.CV.CvEnum.RetrType.External,
                    Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

                for (int i = 0; i < contours.Size; i++)
                {
                    double parameter = CvInvoke.ArcLength(contours[i], true);
                    VectorOfPoint approx = new VectorOfPoint();

                    CvInvoke.ApproxPolyDP(contours[i], approx, .04 * parameter, true);

                    CvInvoke.DrawContours(imgInput, contours, i, new MCvScalar(255, 0, 255), 2);

                    // Moments : concept for finding shaeps, area, measures...
                    // Here we want the center of the shape
                    var moments = CvInvoke.Moments(contours[i]);
                    int x = (int) (moments.M10 / moments.M00);
                    int y = (int) (moments.M01 / moments.M00);

                    if (approx.Size == 3) // Triangle
                    {
                        CvInvoke.PutText(imgInput, "Triangle", 
                            new Point(x, y), 
                            Emgu.CV.CvEnum.FontFace.HersheySimplex, 
                            .5,
                            new MCvScalar(255, 0, 0), 
                            2);
                    }

                    if (approx.Size == 4) // Rectangle
                    {
                        // Check for Square
                        Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                        double ar = (double) rect.Width / rect.Height;
                        if (ar >= .95 || ar <= 1.05)
                        {
                            CvInvoke.PutText(imgInput, "Square",
                            new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex,
                            .5,
                            new MCvScalar(255, 0, 0),
                            2);
                        }
                        else
                        {
                            CvInvoke.PutText(imgInput, "Rectangle",
                            new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex,
                            .5,
                            new MCvScalar(255, 0, 0),
                            2);
                        }
                    }

                    if (approx.Size == 5) // Pentagon
                    {
                        CvInvoke.PutText(imgInput, "Pentagon",
                            new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex,
                            .5,
                            new MCvScalar(255, 0, 0),
                            2);
                    }

                    if (approx.Size == 6) // Hexagon
                    {
                        CvInvoke.PutText(imgInput, "Hexagon",
                            new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex,
                            .5,
                            new MCvScalar(255, 0, 0),
                            2);
                    }

                    if (approx.Size > 6) // Circle
                    {
                        CvInvoke.PutText(imgInput, "Circle",
                            new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex,
                            .5,
                            new MCvScalar(255, 0, 0),
                            2);
                    }

                    pictureBox2.Image = imgInput.Bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
