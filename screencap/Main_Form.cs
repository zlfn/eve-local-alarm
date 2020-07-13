﻿using AForge.Imaging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Windows.Forms;

namespace screencap
{
    public partial class Main_Form : Form
    {
        public Process proc;
        public int isareaset = 0, text_xstart = 0, text_ystart = 0, text_xend = 0, text_yend = 0, status = 0;

        SoundPlayer wav = new SoundPlayer(Properties.Resources.Siren);

        Bitmap img_bad = new Bitmap(Properties.Resources._06);
        Bitmap img_neut = new Bitmap(Properties.Resources._10);
        Bitmap img_terrible = new Bitmap(Properties.Resources._12);
        Bitmap img_war = new Bitmap(Properties.Resources._16);

        private void label1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/dryoo/eve-local-alarm");
        }

        private void Main_Form_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 툰 고르는 화면 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            new process_select(this).ShowDialog();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            DoRequest();
        }

        public Main_Form()
        {
            InitializeComponent();
        }


        // 영역 설정 버튼 
        private void button2_Click(object sender, EventArgs e)
        {
            var Capture = new CaptureLib();
            Bitmap bmp = Capture.CaptureWindow(proc.MainWindowHandle);
            new image_cord(this, bmp).ShowDialog();
            bmp.Dispose();  // 좋음
        }

        private void DoRequest()
        {
            int num = 0;
            var Capture = new CaptureLib();

            Bitmap bmp = Capture.CaptureWindow(proc.MainWindowHandle);
            if (bmp.Height < 700 || bmp.Width < 1000)
            {
                label_status.Text = "";
                if (label_status.ForeColor == Color.Red)
                    label_status.ForeColor = Color.Black;
                else
                    label_status.ForeColor = Color.Red;
            }
            else
            {
                label_status.Text = "";
                if (isareaset == 1)
                    bmp = bmp.Clone(new Rectangle(text_xstart, text_ystart, text_xend - text_xstart, text_yend - text_ystart), bmp.PixelFormat);
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose();
                }
                pictureBox1.Image = bmp;
                //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                Bitmap img_original = bmp;

                Bitmap image = new Bitmap(img_original.Width, img_original.Height, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage((System.Drawing.Image)image))
                    graphics.DrawImage((System.Drawing.Image)img_original, new Rectangle(0, 0, image.Width, image.Height));

                Bitmap template = new Bitmap(img_bad.Width, img_bad.Height, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage((System.Drawing.Image)template))
                    graphics.DrawImage((System.Drawing.Image)img_bad, new Rectangle(0, 0, template.Width, template.Height));
                TemplateMatch[] templateMatchArray = new ExhaustiveTemplateMatching(0.921f).ProcessImage(image, template);
                BitmapData bitmapData = img_original.LockBits(new Rectangle(0, 0, img_original.Width, img_original.Height), ImageLockMode.ReadWrite, img_bad.PixelFormat);
                foreach (TemplateMatch templateMatch in templateMatchArray)
                {
                    ++num;
                }
                img_original.UnlockBits(bitmapData);

                template = new Bitmap(img_neut.Width, img_neut.Height, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage((System.Drawing.Image)template))
                    graphics.DrawImage((System.Drawing.Image)img_neut, new Rectangle(0, 0, template.Width, template.Height));
                templateMatchArray = new ExhaustiveTemplateMatching(0.921f).ProcessImage(image, template);
                bitmapData = img_original.LockBits(new Rectangle(0, 0, img_original.Width, img_original.Height), ImageLockMode.ReadWrite, img_neut.PixelFormat);
                foreach (TemplateMatch templateMatch in templateMatchArray)
                {
                    ++num;
                }
                img_original.UnlockBits(bitmapData);

                // 
                template = new Bitmap(img_terrible.Width, img_terrible.Height, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage((System.Drawing.Image)template))
                    graphics.DrawImage((System.Drawing.Image)img_terrible, new Rectangle(0, 0, template.Width, template.Height));
                templateMatchArray = new ExhaustiveTemplateMatching(0.921f).ProcessImage(image, template);
                bitmapData = img_original.LockBits(new Rectangle(0, 0, img_original.Width, img_original.Height), ImageLockMode.ReadWrite, img_terrible.PixelFormat);
                foreach (TemplateMatch templateMatch in templateMatchArray)
                {
                    ++num;
                }
                img_original.UnlockBits(bitmapData);

                template = new Bitmap(img_war.Width, img_war.Height, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage((System.Drawing.Image)template))
                    graphics.DrawImage((System.Drawing.Image)img_war, new Rectangle(0, 0, template.Width, template.Height));
                templateMatchArray = new ExhaustiveTemplateMatching(0.921f).ProcessImage(image, template);
                bitmapData = img_original.LockBits(new Rectangle(0, 0, img_original.Width, img_original.Height), ImageLockMode.ReadWrite, img_war.PixelFormat);
                foreach (TemplateMatch templateMatch in templateMatchArray)
                {
                    //AForge.Imaging.Drawing.Rectangle(bitmapData, templateMatch.Rectangle, Color.White);
                    ++num;
                }
                img_original.UnlockBits(bitmapData);

                if (num <= 0)
                    return;

                image.Dispose();
                template.Dispose();
                templateMatchArray = null;
                bitmapData = null;

                wav.Play();
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

    
 
    }
}
