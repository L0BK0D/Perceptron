using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perceptron
{
    internal class Perceptron
    {
        private int inputSize;
        private int hiddenSize;   // Количество нейронов в скрытом слое
        private int outputSize;
        private double[,] weightsInputHidden; // Веса между входным и скрытым слоями
        private double[,] weightsHiddenOutput; // Веса между скрытым и выходным слоями
        private double[] biasesHidden; // Смещения для скрытого слоя
        private double[] biasesOutput; // Смещения для выходного слоя
        //private double learningRate;
        Form1 f1;


        public Perceptron(int inputSize, int outputSize, int hiddenSize)
        {
            this.inputSize = inputSize;
            this.hiddenSize = hiddenSize;
            this.outputSize = outputSize;

            //this.learningRate = learningRate;

            // Инициализация весов
            weightsInputHidden = InitializeWeights(inputSize, hiddenSize);
            weightsHiddenOutput = InitializeWeights(hiddenSize, outputSize);

            biasesHidden = new double[hiddenSize];
            biasesOutput = new double[outputSize];
        }
        private double[,] InitializeWeights(int inputSize, int outputSize)
        {
            Random rand = new Random();
            double[,] weights = new double[inputSize, outputSize];
            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < outputSize; j++)
                {
                    weights[i, j] = rand.NextDouble() * 0.01; // Небольшие начальные веса
                    //weights[i, j] = rand.NextDouble() * 2 - 1; // Инициализация случайными значениями
                }
            }
            return weights;

        }

        public int[] Predict(int[] inputs, ProgressBar bar1)
        {
            // Вычисление выходов скрытого слоя
            double[] hiddenOutputs = new double[hiddenSize];
            double[] outputs = new double[outputSize];

            // Вычисление взвешенной суммы для скрытого слоя
            for (int j = 0; j < hiddenSize; j++)
            {
                hiddenOutputs[j] = biasesHidden[j];
                for (int i = 0; i < inputSize; i++)
                {
                    hiddenOutputs[j] += inputs[i] * weightsInputHidden[i, j];
                    bar1.Step += 1 / hiddenSize * 100; // Обновление прогрессбара
                }
                hiddenOutputs[j] = Sigmoid(hiddenOutputs[j]);
            }

            // Вычисление выходов финального слоя
            for (int j = 0; j < outputSize; j++)
            {
                outputs[j] = biasesOutput[j];
                for (int i = 0; i < hiddenSize; i++)
                {
                    outputs[j] += hiddenOutputs[i] * weightsHiddenOutput[i, j];
                    bar1.Step += 1 / outputSize * 100; // Обновление прогрессбара
                }
                outputs[j] = Sigmoid(outputs[j]);
            }

            // Получение предсказания (классов)
            int[] result = new int[outputSize];
            for (int j = 0; j < outputSize; j++)
            {
                result[j] = outputs[j] >= 0.5 ? 1 : 0; // 1 (активный класс) или 0 (неактивный)
            }

            return result;
        }

        public async Task Train(int[][] trainingInputs, int[] trainingOutputs, int epochs, ProgressBar bar1, double learningRate)
        {
            bar1.Maximum = trainingInputs.Length;
            bar1.Step = 1;
            bar1.Value = 0;
            double MCE = 0;
            int[] vectorOut = new int[trainingOutputs.Length];
            
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                for (int i = 0; i < trainingInputs.Length; i++)
                {
                    
                    for (int j = 0;j < outputSize; j++)
                        if (trainingOutputs[i] == j) vectorOut[j] = 1; else vectorOut[j] = 0;
                    
                    // Прямое распространение
                    double[] hiddenOutputs = new double[hiddenSize];
                    double[] finalOutputs = new double[outputSize];
                    // Вход -> Скрытый слой
                    for (int j = 0; j < hiddenSize; j++)
                    {
                        hiddenOutputs[j] = biasesHidden[j];
                        for (int k = 0; k < inputSize; k++)
                        {
                            hiddenOutputs[j] += trainingInputs[i][k] * weightsInputHidden[k, j];
                        }
                        hiddenOutputs[j] = Sigmoid(hiddenOutputs[j]);
                    }
                    // Скрытый слой -> Выход
                    for (int j = 0; j < outputSize; j++)
                    {
                        finalOutputs[j] = biasesOutput[j];
                        for (int k = 0; k < hiddenSize; k++)
                        {
                            finalOutputs[j] += hiddenOutputs[k] * weightsHiddenOutput[k, j];
                        }
                        finalOutputs[j] = Sigmoid(finalOutputs[j]);
                        bar1.Step += 1 / outputSize * 100;
                    }

                    
                    // Вычисление ошибки для выходного слоя и обновление весов
                    for (int j = 0; j < outputSize; j++)
                    {
                        MCE += (-1)*vectorOut[j] * Math.Log(finalOutputs[j]);
                        double target = (trainingOutputs[i] == j) ? 1 : 0;
                        double error = target - finalOutputs[j];

                        for (int k = 0; k < hiddenSize; k++)
                        {
                            weightsHiddenOutput[k, j] += learningRate * error * SigmoidDerivative(finalOutputs[j]) * hiddenOutputs[k];
                        }
                        biasesOutput[j] += learningRate * error * SigmoidDerivative(finalOutputs[j]);
                    }
                    // Вычисление ошибки для скрытого слоя и обновление весов
                    for (int j = 0; j < hiddenSize; j++)
                    {
                        double error = 0;
                        for (int k = 0; k < outputSize; k++)
                        {
                            error += (trainingOutputs[i] == k ? 1 : 0 - finalOutputs[k]) * weightsHiddenOutput[j, k];
                        }

                        for (int k = 0; k < inputSize; k++)
                        {
                            weightsInputHidden[k, j] += learningRate * error * SigmoidDerivative(hiddenOutputs[j]) * trainingInputs[i][k];
                        }
                        biasesHidden[j] += learningRate * error * SigmoidDerivative(hiddenOutputs[j]);
                    }
                }
                MCE = MCE/outputSize; Console.WriteLine(MCE.ToString("F6"));
            }
            WriteWeightsToFile("weights.txt");
            //WriteWeightsToFile1("outputweights.txt", weightsHiddenOutput, biasesOutput);
        }
    

        private double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        private double SigmoidDerivative(double x)
        {
            return x * (1 - x);
        }
        private void WriteWeightsToFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    for (int k = 0; k < inputSize; k++)
                    {
                        writer.Write(weightsInputHidden[k, j].ToString("F6") + " "); // Запись весов с 6 знаками после запятой
                    }
                    writer.WriteLine(); // Перейти на новую строку после записи всех весов для одного выхода
                }

                // Запись смещений
                writer.WriteLine("Biases:");
                for (int j = 0; j < hiddenSize; j++)
                {
                    writer.Write(biasesHidden[j].ToString("F6") + " ");
                }
                writer.WriteLine();

                for (int j = 0; j < outputSize; j++)
                {
                    for (int k = 0; k < hiddenSize; k++)
                    {
                        writer.Write(weightsHiddenOutput[k, j].ToString("F6") + " "); // Запись весов с 6 знаками после запятой
                    }
                    writer.WriteLine(); // Перейти на новую строку после записи всех весов для одного выхода
                }

                // Запись смещений
                writer.WriteLine("Biases:");
                for (int j = 0; j < outputSize; j++)
                {
                    writer.Write(biasesOutput[j].ToString("F6") + " ");
                }
            }
        }
        private void WriteWeightsToFile1(string filePath, double[,] weights, double[] biases)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int j = 0; j < outputSize; j++)
                {
                    for (int k = 0; k < hiddenSize; k++)
                    {
                        writer.Write(weights[k, j].ToString("F6") + " "); // Запись весов с 6 знаками после запятой
                    }
                    writer.WriteLine(); // Перейти на новую строку после записи всех весов для одного выхода
                }

                // Запись смещений
                writer.WriteLine("Biases:");
                for (int j = 0; j < outputSize; j++)
                {
                    writer.Write(biases[j].ToString("F6") + " ");
                }
            }
        }
        public void ReadWeightsFromFile(string filePath)
        {
            // Проверяем существует ли файл
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                // Чтение весов для скрытого слоя
                for (int j = 0; j < hiddenSize; j++)
                {
                    string line = reader.ReadLine();
                    if (line == null) throw new EndOfStreamException("Не хватает данных для определения весов.");

                    string[] weightsString = line.Split(' ');
                    for (int k = 0; k < inputSize; k++)
                    {
                        if (k < weightsInputHidden.GetLength(0))
                        {
                            weightsInputHidden[k, j] = double.Parse(weightsString[k]);
                        }
                    }
                }
                // Чтение смещений для скрытого слоя
                string biasesLine = reader.ReadLine();
                if (biasesLine == null || !biasesLine.StartsWith("Biases:")) throw new FormatException("Не найден заголовок для смещений.");
                string biasesValues = reader.ReadLine();
                string[] biasesString = biasesValues.Split(' ');

                for (int j = 0; j < hiddenSize; j++)
                {
                    biasesHidden[j] = double.Parse(biasesString[j]);
                }
                // Чтение весов для выходного слоя
                for (int j = 0; j < outputSize; j++)
                {
                    string line = reader.ReadLine();
                    if (line == null) throw new EndOfStreamException("Не хватает данных для определения весов.");

                    string[] weightsString = line.Split(' ');
                    for (int k = 0; k < hiddenSize; k++)
                    {
                        if (k < weightsHiddenOutput.GetLength(0))
                        {
                            weightsHiddenOutput[k, j] = double.Parse(weightsString[k]);
                        }
                    }
                }



                // Чтение смещений для выходного слоя
                biasesLine = reader.ReadLine();
                if (biasesLine == null || !biasesLine.StartsWith("Biases:")) throw new FormatException("Не найден заголовок для смещений.");
                biasesValues = reader.ReadLine();
                biasesString = biasesValues.Split(' ');

                for (int j = 0; j < outputSize; j++)
                {
                    biasesOutput[j] = double.Parse(biasesString[j]);
                }
            }
        }
    }
}
