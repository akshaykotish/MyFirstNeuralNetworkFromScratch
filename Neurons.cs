using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NNP
{
    class Neuron
    {
        public float Value;
        public float rawValue;
        public List<float> Weight = new List<float>();
        public List<float> Gamma = new List<float>();

        public Neuron()
        {

        }
        public Neuron(float value)
        {
            Value = value;
        }
    }


    class Layer
    {
        public List<Neuron> Neurons = new List<Neuron>();

        public Layer()
        {

        }


        public Layer(int size)
        {
            for(int i=0; i<size; i++)
            {
                Neuron neuron = new Neuron();
                Neurons.Add(neuron);
            }
        }
    }

    class Layers
    {
        public List<Layer> layers = new List<Layer>();

        public void DisplayInLine()
        {
            for(int i=0; i<layers.Count;i++)
            {
                Console.WriteLine("Layer: " + (i+1));
                for(int j=0; j < layers[i].Neurons.Count; j++)
                {
                    Console.Write("[" + layers[i].Neurons[j].Value + ", (");
                    for(int k=0; k < layers[i].Neurons[j].Weight.Count; k++)
                    {
                        Console.Write(layers[i].Neurons[j].Weight[k] + " ");
                    }
                    Console.Write(")]");
                }
                Console.WriteLine("");
            }
        }
    }

    class NeuralNetwork{

        public Layers layers;
        public List<float> Target = new List<float>();
        public bool forwardPropogation = true;

        public void InitWeights()
        {
            Random random = new Random();
            for (int l=0; l<layers.layers.Count - 1; l++)
            {
                Layer Layer1 = layers.layers[l];
                Layer Layer2 = layers.layers[l+1];

                for (int i = 0; i < Layer1.Neurons.Count; i++)
                {
                    for (int j = 0; j < Layer2.Neurons.Count; j++)
                    {
                        float randomWeight = random.Next(-1, 1);
                        Layer1.Neurons[i].Weight.Add(randomWeight);
                        //Console.Write(Layer1.Neurons[i].Weight[j] + " ");
                    }
                    //Console.WriteLine("Neural Changed");
                }
                //Console.WriteLine("Layer Changed");
            }



        }

        public float Sigmoid(float x)
        {
            float k = (float)Math.Exp(x);
            return (k / (1 + k));
        }

        public float Relu(float x)
        {
            return x > 0 ? x : 0;
        }

        void Reverse(int currentIndex)
        {
            if (currentIndex > 0)
            {
                for (int j = 0; j < layers.layers[currentIndex - 1].Neurons.Count; j++)
                {
                    for (int i = 0; i < layers.layers[currentIndex].Neurons.Count; i++)
                    {
                        float A = 0;
                        for (int l = 0; l < layers.layers[currentIndex+1].Neurons.Count; l++)
                        {
                            A += layers.layers[currentIndex + 1].Neurons[l].Gamma[i];
                        }

                        float B = layers.layers[currentIndex].Neurons[i].Value * (1 - layers.layers[currentIndex].Neurons[i].Value);

                        float C = layers.layers[currentIndex - 1].Neurons[j].Value;

                        float Gamma = A * B;
                        layers.layers[currentIndex].Neurons[i].Gamma.Add(Gamma);

                        float Change = Gamma * C;

                        float WeightToChange = (float)Math.Round(layers.layers[currentIndex - 1].Neurons[j].Weight[i], 2);

                        //Console.WriteLine(WeightToChange + " -= " + Change);
                        //Console.WriteLine("Weight Before: " + WeightToChange);
                        WeightToChange -= Change;
                        layers.layers[currentIndex - 1].Neurons[j].Weight[i] = WeightToChange;
                        //Console.WriteLine("Weight After: " + WeightToChange);
                    }
                }
            }
        }

        void FinalReverse(int currentIndex)
        {
            for (int j = 0; j < layers.layers[currentIndex - 1].Neurons.Count; j++) {
                for (int i = 0; i < layers.layers[currentIndex].Neurons.Count; i++)
                {
                    float A = -1 * (Target[i] - layers.layers[currentIndex].Neurons[i].Value);

                    float B = layers.layers[currentIndex].Neurons[i].Value * (1 - layers.layers[currentIndex].Neurons[i].Value);

                    float C = layers.layers[currentIndex - 1].Neurons[j].Value;

                    float Gamma = A * B;
                    layers.layers[currentIndex].Neurons[i].Gamma.Add(Gamma);

                    float Change = Gamma * C;

                    float WeightToChange = (float) Math.Round(layers.layers[currentIndex - 1].Neurons[j].Weight[i], 2);

                    //if (Target[i] == 1)
                    //{
                        //Console.WriteLine("--Go Up--------------");
                    //}
                    
                    //Console.WriteLine(WeightToChange + " -= " + Change);
                    //Console.WriteLine("Weight Before: " + WeightToChange);
                    WeightToChange -= Change;
                    layers.layers[currentIndex - 1].Neurons[j].Weight[i] = WeightToChange;
                    //Console.WriteLine("Weight After: " + WeightToChange);

                    //if (Target[i] == 1)
                    //{
                        //Console.WriteLine("\n--------------------");
                    //}
                    
                }
            }
        }

        public void Forward(int currentIndex, bool isReverse)
        {
            if (currentIndex < layers.layers.Count - 1)
            {
                Layer CurrentLayer = layers.layers[currentIndex];
                Layer ToProcess = layers.layers[currentIndex + 1];

                for (int j = 0; j < ToProcess.Neurons.Count; j++)
                {
                    ToProcess.Neurons[j].rawValue = 0;
                    for (int i = 0; i < CurrentLayer.Neurons.Count; i++)
                    {
                        ToProcess.Neurons[j].rawValue += CurrentLayer.Neurons[i].Value * CurrentLayer.Neurons[i].Weight[j];
                    }
                    ToProcess.Neurons[j].Value = Sigmoid(ToProcess.Neurons[j].rawValue);
                }
            }
            if (currentIndex < layers.layers.Count)
            {
                Forward(currentIndex + 1, isReverse);
            }

            if (isReverse == true)
            {
                if (currentIndex == layers.layers.Count - 1)
                {
                    FinalReverse(currentIndex);
                }
                else if (currentIndex < layers.layers.Count - 1)
                {
                    Reverse(currentIndex);
                }
            }
        }

        public void Test(Layer inputLayer)
        {
            for(int i=0; i< inputLayer.Neurons.Count; i++)
            {
                layers.layers[0].Neurons[i].Value = inputLayer.Neurons[i].Value;
            }
            Forward(0, false);
        }
    }

    class Controller
    {
        public static void Main(String[] args)
        {
            //Create Input Layer

            Layer inputLayer = new Layer();
            inputLayer.Neurons.Add(new Neuron(0));
            inputLayer.Neurons.Add(new Neuron(0));
            inputLayer.Neurons.Add(new Neuron(0));
            inputLayer.Neurons.Add(new Neuron(1));

            Layer hiddenLayer1 = new Layer(3);
            Layer hiddenLayer2 = new Layer(3);
            Layer outputLayer = new Layer(2);

            Layers layers = new Layers();
            layers.layers.Add(inputLayer);
            layers.layers.Add(hiddenLayer1);
            layers.layers.Add(hiddenLayer2);
            layers.layers.Add(outputLayer);

            NeuralNetwork neuralNetwork = new NeuralNetwork();
            neuralNetwork.layers = layers;
            neuralNetwork.Target.Add(0);
            neuralNetwork.Target.Add(1);

            neuralNetwork.InitWeights();

            for (int i = 0; i < 5000; i++)
            {
                neuralNetwork.Forward(0, true);
            }

            layers.DisplayInLine();

            Console.WriteLine("\n--Testing-----------------\n");

            Layer testLayer = new Layer();
            testLayer.Neurons.Add(new Neuron(0));
            testLayer.Neurons.Add(new Neuron(0));
            testLayer.Neurons.Add(new Neuron(0));
            testLayer.Neurons.Add(new Neuron(1));

            neuralNetwork.Test(testLayer);
            layers.DisplayInLine();

            Console.ReadKey();
        }
    }
}
