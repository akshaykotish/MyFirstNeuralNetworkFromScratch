using System;
using System.Collections.Generic;
using System.Linq;

namespace NNP
{
    class Neuron
    {
        public float Value;
        public float rawValue;
        public float rawValueBiased;
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
        public List<float> Bias = new List<float>();

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
        public Dictionary<String, int> TargetDict = new Dictionary<String, int>();
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
                        float R = (float)random.NextDouble();
                        float randomWeight = (float)Math.Round(R, 1);
                        Layer1.Neurons[i].Weight.Add(randomWeight);
                        Console.Write(Layer1.Neurons[i].Weight[j] + " ");
                    }
                    Console.WriteLine("Neural Changed");
                }

                for(int i=0; i<Layer2.Neurons.Count; i++)
                {
                    float R = (float)random.NextDouble();
                    float randomWeight = (float)Math.Round(R, 1);
                    Layer1.Bias.Add(randomWeight);
                }
                Console.WriteLine("Bias in layer " + l + " are " + Layer1.Bias.Count);
                Console.WriteLine("Layer Changed");
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
                        layers.layers[currentIndex - 1].Bias[i] = layers.layers[currentIndex].Neurons[i].Value - layers.layers[currentIndex-1].Neurons[j].rawValue;
                        //Console.WriteLine("Weight After: " + WeightToChange);
                    }
                }
            }
        }

        void CaclulateTotalSumError()
        {
            float sum = 0;
            for(int i=0; i < layers.layers[layers.layers.Count-1].Neurons.Count; i++)
            {
                float v = Target[i] - layers.layers[layers.layers.Count - 1].Neurons[i].Value;
                v = v * v;
                sum = sum + v;
            }
            Console.WriteLine("Tota Sum Error: " + sum);
        }

        void FinalReverse(int currentIndex)
        {
            CaclulateTotalSumError();
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

                    if (Target[i] == 1)
                    {
                       // Console.WriteLine("--Go Up--------------");
                    }
                    
                    //Console.WriteLine(WeightToChange + " -= " + Change);
                    //Console.WriteLine("Weight Before: " + WeightToChange);
                    WeightToChange -= Change;
                    layers.layers[currentIndex - 1].Neurons[j].Weight[i] = WeightToChange;
                    //Console.WriteLine("Weight After: " + WeightToChange);
                    layers.layers[currentIndex - 1].Bias[i] =  Target[i] - layers.layers[currentIndex].Neurons[i].rawValue;
                    if (Target[i] == 1)
                    {
                      //  Console.WriteLine("\n--------------------");
                    }
                    
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
                    ToProcess.Neurons[j].rawValueBiased = ToProcess.Neurons[j].rawValue + CurrentLayer.Bias[j];
                    ToProcess.Neurons[j].Value = Sigmoid(ToProcess.Neurons[j].rawValueBiased);
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


        public void ResetTarget()
        {
            for(int i=0; i<Target.Count; i++) {
                Target[i] = 0;
            }
        }

        public void Train(Dictionary<String, String> Data, int Epochs)
        {
            var labels = Data.Values;
            foreach(String label in labels)
            {
                if (TargetDict.ContainsKey(label) == false)
                { 
                    Target.Add(0);
                    TargetDict.Add(label, Target.Count - 1);
                }
            }
            
            InitWeights();
            for (int l = 0; l < Epochs; l++)
            {
                Console.WriteLine("Epoch Count: " + l);
                foreach (var data in Data)
                {

                    String[] values = data.Key.Split(',');
                    String label = data.Value;

                    ResetTarget();
                    Target[TargetDict[label]] = 1;


                    for (int i = 0; i < values.Length; i++)
                    {
                        layers.layers[0].Neurons[i].Value = float.Parse(values[i]);
                    }

                    //Console.WriteLine("\n\nBefore Propogation");
                    //layers.DisplayInLine();
                    Forward(0, true);
                    //Console.WriteLine("\n\nAfter Propogation");
                    //layers.DisplayInLine();

                }
            }
        }

        public void Validate(Dictionary<String, String> Data, int Epochs)
        {
            foreach (var data in Data)
            {
                String[] values = data.Key.Split(',');
                String label = data.Value;

                for (int i = 0; i < values.Length; i++)
                {
                    layers.layers[0].Neurons[i].Value = float.Parse(values[i]);
                }
                Forward(0, false);

                float max = 0;
                int index = 0;

                for (int i = 0; i < layers.layers[layers.layers.Count - 1].Neurons.Count; i++)
                {
                    float n = layers.layers[layers.layers.Count - 1].Neurons[i].Value;
                    if (n > max)
                    {
                        index = i;
                        max = n;
                    }

                    if (TargetDict.Keys.ToList()[i] != label)
                    {
                        float diff = 1 - layers.layers[layers.layers.Count - 1].Neurons[i].Value;
                        Dictionary<String, String> newTrain = new Dictionary<String, String>();
                        newTrain.Add(data.Key, data.Value);
                        Console.Write("Validating....\n");
                        Train(newTrain,  ((int)(Epochs)));
                    }
                }
            }
        }

        public void Test(Dictionary<String, String> Data)
        {
            foreach (var data in Data)
            {
                String[] values = data.Key.Split(',');
                String label = data.Value;

                Console.WriteLine("\n\nExpected out is " + label);
                for (int i = 0; i < values.Length; i++)
                {
                    layers.layers[0].Neurons[i].Value = float.Parse(values[i]);
                }
                Forward(0, false);

                float max = 0;
                int index = 0;

                for(int i=0; i < layers.layers[layers.layers.Count-1].Neurons.Count; i++)
                {
                    float n = layers.layers[layers.layers.Count - 1].Neurons[i].Value;
                    if (n > max) {
                        index = i;
                        max = n;
                    }
                }

                //layers.DisplayInLine(); 

                Console.WriteLine("Prediction is " + TargetDict.Keys.ToList()[index]);
            }
        }

        public void TestSingle(String toTest)
        {
            String[] values = toTest.Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                layers.layers[0].Neurons[i].Value = float.Parse(values[i]);
            }
            Forward(0, false);

            float max = 0;
            int index = 0;

            for (int i = 0; i < layers.layers[layers.layers.Count - 1].Neurons.Count; i++)
            {
                float n = layers.layers[layers.layers.Count - 1].Neurons[i].Value;
                if (n > max)
                {
                    index = i;
                    max = n;
                }
            }
            Console.WriteLine("Prediction is " + TargetDict.Keys.ToList()[index]);

        }
    }

    class Controller
    {
        public static void Main(String[] args)
        {
            //Create Input Layer

            Layer inputLayer = new Layer(2);
            Layer hiddenLayer1 = new Layer(5);
            Layer outputLayer = new Layer(2);

            Layers layers = new Layers();
            layers.layers.Add(inputLayer);
            layers.layers.Add(hiddenLayer1);
            layers.layers.Add(outputLayer);

            NeuralNetwork neuralNetwork = new NeuralNetwork();
            neuralNetwork.layers = layers;

            Dictionary<String, String> data = new Dictionary<String, String>() {
                {"1,1", "0"},
                {"0,1", "1"},
                {"1,0", "1"},
                {"0,0", "0"},
            };

            neuralNetwork.Train(data, 400);
            neuralNetwork.Validate(data, 400);
            //neuralNetwork.Test(data);

            bool isexit = false;
            while (isexit == false)
            {
                Console.WriteLine("\nEnter the test ");
                String input = Console.ReadLine();

                if (input != "exit")
                {
                    
                    neuralNetwork.TestSingle(input);
                }
                else
                {
                    isexit = true;
                }
            }

            Console.ReadKey();
        }
    }
}
