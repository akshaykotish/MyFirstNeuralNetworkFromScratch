using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNP
{
    class Neuron
    {
        public float value = 0;
        public float raw_value = 0;
        public List<float> weight = new List<float>();
        public List<float> gamma = new List<float>();

        public Neuron(float value)
        {
           this.value = value;
        }
        public Neuron(float value, float weight)
        { this.value = value; }
        public Neuron(float value, float raw_value, float weight)
        { this.value = value; this.raw_value = raw_value; }
    }

    class Layer
    {
        int size = 0;
        float bias = 0;
        public Neuron[] layer;

        public Layer(int size)
        {
            this.size = size;
            layer = new Neuron[size];
        }
        public Layer(Neuron[] layer)
        {
            this.layer = layer;
        }

        public float Sigmoid(float x)
        {
            float k = (float)Math.Exp(x);
            return (k / (1 + k));
        }

        public float Relu(float x)
        {
            return Math.Max(0, x);
        }

        public void CalculateNewLayerNeurons(Layer previouslayer)
        {
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                float sum = 0;
                for (int j = 0; j < previouslayer.layer.Length; j++)
                {
                    previouslayer.layer[j].weight.Add((float)Math.Round(random.NextDouble(), 5));
                    sum += (previouslayer.layer[j].value * previouslayer.layer[j].weight[previouslayer.layer[j].weight.Count - 1]);
                }
                bias = (float)Math.Round(random.NextDouble(), 5);

                sum += bias;
                sum = (float)Math.Round(sum, 2);


                layer[i] = new Neuron(Sigmoid(sum), sum, 0);
            }
        }

        public void EditWeight(float[] weights)
        {

        }

        public void Display()
        {
            for (int i = 0; i < layer.Length; i++)
            {
                Console.WriteLine(layer[i].value.ToString());
                
            }
        }

    }

    class Layers
    {
        public List<Layer> layers;
        public int[] Targets = new int[2] { 0, 1 };
        float Error = 0;

        public Layers()
        {
            layers = new List<Layer>();
        }

        public void AddOutputTable(int[] tg)
        {
            this.Targets = tg;
        }

        public void CalculateTotalError(Layer layer)
        {
            float sum = 0;
            for (int i = 0; i < layer.layer.Length; i++)
            {
                float value = layer.layer[i].value;
                float difference = (Targets[i] - value);
                float half = (difference * difference) / 2;
                sum += half;
            }

            //sum = (float)Math.Round(sum, 2);

            Error = sum;
            //            Console.WriteLine(sum);
        }

        public void BackwardPass(Layer previousLayer, Layer currentLayer)
        {
            for (int i = 0; i < currentLayer.layer.Length; i++)
            {
                for (int j = 0; j < previousLayer.layer.Length; j++)
                {
                    float dtotalerrorbycurrentvalue = -1 * (Targets[i] - currentLayer.layer[i].value);
                    float dcurrentvalyebyrawvalue = currentLayer.layer[i].value * (1 - currentLayer.layer[i].value);
                    float drawvaluebypreviousweight = previousLayer.layer[j].value;
                    float derrortotalby = dtotalerrorbycurrentvalue * dcurrentvalyebyrawvalue * drawvaluebypreviousweight;

                    float gamma = dtotalerrorbycurrentvalue * dcurrentvalyebyrawvalue;
                    currentLayer.layer[i].gamma.Add(gamma);

                    previousLayer.layer[j].weight[i] = (float)(previousLayer.layer[j].weight[i] - 2f * derrortotalby);

                    currentLayer.Display();
                }
            }
        }

        public void BackwardPass(Layer previousLayer, Layer currentLayer, Layer nextLayer)
        {
            for (int i = 0; i < currentLayer.layer.Length; i++)
            {
                for (int j = 0; j < previousLayer.layer.Length; j++)
                {
                    float dtotalerrorbycurrentvalue = 0;
                    for (int k = 0; k < currentLayer.layer[i].weight.Count; k++)
                    {
                        dtotalerrorbycurrentvalue += (nextLayer.layer[k].gamma[i] * currentLayer.layer[i].weight[k]);
                    }
                    float dcurrentvalyebyrawvalue = currentLayer.layer[i].value * (1 - currentLayer.layer[i].value);
                    float drawvaluebypreviousweight = previousLayer.layer[j].value;
                    float derrortotalby = dtotalerrorbycurrentvalue * dcurrentvalyebyrawvalue * drawvaluebypreviousweight;

                    float gamma = dtotalerrorbycurrentvalue * dcurrentvalyebyrawvalue;
                    currentLayer.layer[i].gamma.Add(gamma);

                    previousLayer.layer[j].weight[i] = (float)(previousLayer.layer[j].weight[i] - 2f * derrortotalby);
                }
            }
        }

        public void Propogation(Layer previousLayer, int layerIndex)
        {
            if (layerIndex < layers.Count)
            {
                layers[layerIndex].CalculateNewLayerNeurons(previousLayer);
                Propogation(layers[layerIndex], layerIndex + 1);
                if (layerIndex > 0)
                {
                    if (layerIndex == layers.Count - 1)
                    {
                        BackwardPass(previousLayer, layers[layerIndex]);
                    }
                    else
                    {
                        BackwardPass(previousLayer, layers[layerIndex], layers[layerIndex + 1]);
                    }
                }
            }
        }
    }

    class NeuralNetwork
    {
        public Layer InputLayer()
        {
            Neuron[] neurons = new Neuron[3];
            neurons[0] = new Neuron(0, 0);
            neurons[1] = new Neuron(1, 0);
            neurons[2] = new Neuron(0, 0);

            Layer inputLayer = new Layer(neurons);
            return inputLayer;
        }

        public Layer InputLayer0()
        {
            Neuron[] neurons = new Neuron[3];
            neurons[0] = new Neuron(2);
            neurons[1] = new Neuron(2);
            neurons[2] = new Neuron(2);

            Layer inputLayer = new Layer(neurons);
            return inputLayer;
        }

        public Layer InputLayer1()
        {
            Neuron[] neurons = new Neuron[3];
            neurons[0] = new Neuron(4);
            neurons[1] = new Neuron(4);
            neurons[2] = new Neuron(4);

            Layer inputLayer = new Layer(neurons);
            return inputLayer;
        }


        public void Execute()
        {
            Random random = new Random();

            Layer hiddenlayer1 = new Layer(2);
            Layer hiddenlayer2 = new Layer(2);
            Layer outputlayer = new Layer(2);


            Layers layers = new Layers();


            layers.layers.Add(InputLayer());
            layers.layers.Add(hiddenlayer1);
            layers.layers.Add(hiddenlayer2);
            layers.layers.Add(outputlayer);



            for (int i = 0; i < 10000; i++)
            {
                if(i % 2 == 0)
                {
                    layers.layers[0] = InputLayer0();
                }
                else
                {
                    layers.layers[0] = InputLayer1();
                }
                layers.Propogation(layers.layers[0], 1);
            }

            Console.WriteLine("Testing");
            layers.layers[0] = InputLayer0();
            layers.Propogation(layers.layers[0], 1);
            


            //for(int i=0; i < layers.layers.Count; i++)
            //{
            //layers.layers[i].Display();
            //Console.WriteLine();
            //}

            Console.WriteLine(layers.layers[3].layer[0].value);
            Console.WriteLine(layers.layers[3].layer[1].value);

            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            NeuralNetwork net = new NeuralNetwork();
            net.Execute();
        }

    }
}
