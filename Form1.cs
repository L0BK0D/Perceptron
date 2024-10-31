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

namespace Perceptron
{
    public partial class Form1 : Form
    {
        //private int mode = 0;       //способ отрисовки
        private Point movePt;
        private Point nullPt = new Point(int.MinValue, 0);

        private SolidBrush brush = new SolidBrush(Color.White);
        private Pen pen = new Pen(Color.Black);
        
        private Point startPt;
        private int[] readedIntArray = new int[1024];
        private static int inputSize = 1024;
        private static int hiddenSize = 100;
        private static int outputSize = 4;
        Perceptron perceptron = new Perceptron(inputSize, outputSize, hiddenSize);

        public int mode { get; private set; }

        public Form1()
        {
            InitializeComponent();
            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = 1;
            numericUpDown1.Increment = 0.001M;
            numericUpDown1.Value = 0.01M;

            
            //richTextBox1.Text = output;

        }
        public static int[][] LoadTrainingData()
        {
            string directoryPath = "C:\\Users\\user\\Desktop\\Учеба\\Интеллектуальные системы и технологии\\Training";
            string[] filePaths = Directory.GetFiles(directoryPath, "*.bmp");
            const int v = 1024;
            int[][] bitmapArray = new int[filePaths.Length][];
            for (int i = 0; i < filePaths.Length; i++)
            {
                bitmapArray[i] = ConvertBmpToArray(filePaths[i]);
            }

            return bitmapArray;
        }
        public static int[][] LoadTestData()
        {
            string directoryPath = "C:\\Users\\user\\Desktop\\Учеба\\Интеллектуальные системы и технологии\\Testing";
            string[] filePaths = Directory.GetFiles(directoryPath, "*.bmp");
            const int v = 1024;
            int[][] bitmapArray = new int[filePaths.Length][];
            for (int i = 0; i < filePaths.Length; i++)
            {
                bitmapArray[i] = ConvertBmpToArray(filePaths[i]);
            }

            return bitmapArray;
        }
        public static int[] ConvertBmpToArray(string filePath)
        {
            using (Bitmap bmp = new Bitmap(filePath))
            {
                if (bmp.Width != 32 || bmp.Height != 32)
                    throw new ArgumentException("Image must be 32x32 pixels.");

                int[] array = new int[bmp.Height * bmp.Width];

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        Color pixelColor = bmp.GetPixel(x, y);
                        // Предполагаем, что изображение черно-белое
                        // Если цвет близок к черному, запишем 1, если близок к белому - 0
                        int brightness = (pixelColor.R + pixelColor.G + pixelColor.B) / 3; // Средняя яркость

                        // Определяем черный или белый
                        array[y * bmp.Width + x] = brightness < 224 ? 1 : 0; // Черный - 1, белый - 0
                    }
                }

                return array;
            }

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            int[][] trainingInputs = LoadTrainingData();
            string output = string.Join("", trainingInputs[0]);
            /*
            for (int i = 0; i < 32; i++) 
            {
                for (int j = 0;j<32; j++)
                {
                    richTextBox1.AppendText(trainingInputs[0][i*32+j].ToString());
                }
                richTextBox1.AppendText("\n");
            }
            */
            int[] trainingOutputs = new int[trainingInputs.Length];
            for (int i = 0; i < 88; i++) trainingOutputs[i] = 0;
            for (int i = 88; i < 179; i++) trainingOutputs[i] = 1;
            for (int i = 179; i<273; i++) trainingOutputs[i] = 2;
            for (int i =273; i<367;i++) trainingOutputs[i] = 3;

            await perceptron.Train(trainingInputs, trainingOutputs, Convert.ToInt32(numericUpDown2.Value), progressBar1, Convert.ToDouble(numericUpDown1.Value)); // Обучение на 1000 эпохах
            richTextBox1.AppendText("Обучение завершено\n");


            /*
            richTextBox1.AppendText("Тестовое изображение\n");
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    richTextBox1.AppendText(testInputs[i * 32 + j].ToString());
                }
                richTextBox1.AppendText("\n");
            }


            
            int[] result = perceptron.Predict(testInputs, progressBar1);
            richTextBox1.AppendText("Результат\n");
            for (int i = 0; i < result.Length; i++)
            {
                richTextBox1.AppendText(result[i].ToString());
            }
            richTextBox1.AppendText("\n");
            string klass = "";
            if (result[0] == 1) klass = "Близнецы";
            else if (result[1] == 1) klass = "Козерог";
            else if (result[2] == 1) klass = "Овен";
            else if (result[3] == 1) klass = "Рак";
            richTextBox1.AppendText(klass + "\n");
            */
        }
        private async void button7_Click(object sender, EventArgs e)
        {
            int[][] testInputs = LoadTestData();
            int counter = 0; 
            int[] testOutputs = new int[testInputs.Length];
            for (int i = 0; i < 11; i++) testOutputs[i] = 0;
            for (int i = 11; i < 19; i++) testOutputs[i] = 1;
            for (int i = 19; i < 24; i++) testOutputs[i] = 2;
            for (int i = 24; i < 29; i++) testOutputs[i] = 3;
            int[][] result = new int[testInputs.Length][];
            for (int j = 0; j < 29; j++)
            {
                /*
                for (int i = 0; i < 32; i++)
                {
                    for (int k = 0; k < 32; k++)
                    {
                        richTextBox1.AppendText(testInputs[j][i * 32 + k].ToString());
                    }
                    richTextBox1.AppendText("\n");
                }
                richTextBox1.AppendText("\n");
                */
                result[j]= perceptron.Predict(testInputs[j], progressBar1);
                for (int i = 0; i < 4; i++)
                {
                    richTextBox1.AppendText(result[j][i].ToString());
                    if (result[j][i].ToString() == "1" && testOutputs[j]==i) counter++;
                }
                richTextBox1.AppendText("\n");

               
            }
            double acc = Convert.ToDouble(counter) / 29 * 100;
            int accu = Convert.ToInt32(acc);
            richTextBox1.AppendText($"Тестирование завершено. Точность:{accu}%\n");
        }
        private async void button4_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.Title = "Загрузить веса";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        perceptron.ReadWeightsFromFile(openFileDialog.FileName);
                        MessageBox.Show("Веса загружены успешно.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке весов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
            private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void ReversibleDraw()
        {
            Point p1 = pictureBox1.PointToScreen(startPt),
            p2 = pictureBox1.PointToScreen(movePt);
            if (mode == 1)
                ControlPaint.DrawReversibleLine(p1, p2, Color.Black);
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pen.Width = 10;
            if (startPt == nullPt) return;
            if (e.Button == MouseButtons.Left)
            {
                Graphics g = Graphics.FromImage(pictureBox1.Image);
                g.DrawLine(pen, startPt, e.Location);
                g.Dispose();
                startPt = e.Location;
                pictureBox1.Invalidate();
                pictureBox1.Update();

            }
            
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (startPt == nullPt) return;
            
                //ReversibleDraw();
                Graphics g = Graphics.FromImage(pictureBox1.Image);
                g.DrawLine(pen, startPt, e.Location);
                //g.DrawLine(pen, startPt, movePt);
                g.Dispose();
                pictureBox1.Invalidate();
           
        }
        private void pictureBox1_MouseDown_1(object sender, MouseEventArgs e)
        {
            movePt = startPt = e.Location;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            //pictureBox1.Image = new Bitmap(256, 256);
            g.Clear(Color.White);   
            g.Dispose();
            pictureBox1.Invalidate();
            //pictureBox1.Image = new Bitmap(256,256);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string s = openFileDialog1.FileName;
                try
                {
                    Image im = new Bitmap(s);
                    Graphics g = Graphics.FromImage(im);
                    g.Dispose();
                    if (pictureBox1.Image != null)
                        pictureBox1.Image.Dispose();
                    pictureBox1.Image = im;
                }
                catch //(Exception ex)
                {
                    MessageBox.Show("Файл " + s + "недопустимый формат", "Ошибка");
                }
                using (Bitmap bmp = new Bitmap(s))
                {
                    if (bmp.Width != 32 || bmp.Height != 32)
                        throw new ArgumentException("Image must be 32x32 pixels.");

                    int[] array = new int[bmp.Height * bmp.Width];

                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            Color pixelColor = bmp.GetPixel(x, y);
                            // Предполагаем, что изображение черно-белое
                            // Если цвет близок к черному, запишем 1, если близок к белому - 0
                            int brightness = (pixelColor.R + pixelColor.G + pixelColor.B) / 3; // Средняя яркость

                            // Определяем черный или белый
                            array[y * bmp.Width + x] = brightness < 224 ? 1 : 0; // Черный - 1, белый - 0
                        }
                    }
                    richTextBox1.AppendText($"Array for {s}:\n");

                    for (int y = 0; y < 32; y++)
                    {
                        for (int x = 0; x < 32; x++)
                        {
                            richTextBox1.AppendText(array[y * 32 + x].ToString()); // # - черный цвет, . - белый
                            
                        }
                        richTextBox1.AppendText("\n");
                    }

                    richTextBox1.AppendText("\n"); // Отступ между изображениями
                    int[] result = perceptron.Predict(array, progressBar1);
                    richTextBox1.AppendText("Результат\n");
                    for (int i = 0; i < result.Length; i++)
                    {
                        richTextBox1.AppendText(result[i].ToString());
                    }
                    richTextBox1.AppendText("\n");
                    string klass = "";
                    if (result[0] == 1) klass = "Близнецы";
                    else if (result[1] == 1) klass = "Козерог";
                    else if (result[2] == 1) klass = "Овен";
                    else if (result[3] == 1) klass = "Рак";
                    richTextBox1.AppendText(klass + "\n");

                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // Получаем оригинальное изображение из PictureBox
                Bitmap originalBitmap = new Bitmap(pictureBox1.Image);

                // Создаем новый Bitmap размером 32x32
                Bitmap resizedBitmap = new Bitmap(32, 32);

                // Создаем Graphics объект для рисования
                using (Graphics g = Graphics.FromImage(resizedBitmap))
                {
                    // Настраиваем режим интерполяции для повышения качества
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    // Рисуем изображение на новом Bitmap
                    g.DrawImage(originalBitmap, 0, 0, 32, 32);
                }

                // Освобождаем оригинальное изображение если больше не нужно
                originalBitmap.Dispose();

                // Присваиваем сжатое изображение обратно в PictureBox
                //pictureBox1.Image = resizedBitmap;

                int[] array = new int[resizedBitmap.Height * resizedBitmap.Width];

                for (int y = 0; y < resizedBitmap.Height; y++)
                {
                    for (int x = 0; x < resizedBitmap.Width; x++)
                    {
                        Color pixelColor = resizedBitmap.GetPixel(x, y);
                        // Предполагаем, что изображение черно-белое
                        // Если цвет близок к черному, запишем 1, если близок к белому - 0
                        int brightness = (pixelColor.R + pixelColor.G + pixelColor.B) / 3; // Средняя яркость

                        // Определяем черный или белый
                        array[y * resizedBitmap.Width + x] = brightness < 224 ? 1 : 0; // Черный - 1, белый - 0
                    }
                }
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        richTextBox1.AppendText(array[i * 32 + j].ToString());
                    }
                    richTextBox1.AppendText("\n");
                }
                int[] result = perceptron.Predict(array, progressBar1);
                richTextBox1.AppendText("Результат\n");
                for (int i = 0; i < result.Length; i++)
                {
                    richTextBox1.AppendText(result[i].ToString());
                }
                richTextBox1.AppendText("\n");
                string klass = "";
                if (result[0] == 1) klass = "Близнецы";
                else if (result[1] == 1) klass = "Козерог";
                else if (result[2] == 1) klass = "Овен";
                else if (result[3] == 1) klass = "Рак";
                richTextBox1.AppendText($"Предполагаемый знак - {klass}\n");

            }
            else
            {
                MessageBox.Show("Нет загруженного изображения в PictureBox.");
            }

        }

       
    }
}
