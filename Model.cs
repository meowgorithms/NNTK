using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using LinAlg;
using Array = LinAlg.Array;
using Matrix = LinAlg.Matrix;
using Layer = NNTK.Layers.Layer;
using LossFunction = NNTK.LossFunctions.LossFunction;
using NNTK.Activations;
using NNTK.Layers;

namespace NNTK
{
    public class Model
    {

    }

    public class Sequential : IList
    {
        public List<Layer> layers = new();
        LossFunction lossFunction;

        public Sequential(LossFunction loss, Tuple<int, int> inputShape)
        {
            lossFunction = loss;
            layers.Add(new InputLayer(inputShape));
        }

        public object this[int index] { get => ((IList)layers)[index]; set => ((IList)layers)[index] = value; }

        public bool IsFixedSize => ((IList)layers).IsFixedSize;

        public bool IsReadOnly => ((IList)layers).IsReadOnly;

        public int Count => ((ICollection)layers).Count;

        public bool IsSynchronized => ((ICollection)layers).IsSynchronized;

        public object SyncRoot => ((ICollection)layers).SyncRoot;

        public int Add(object value)
        {
            return ((IList)layers).Add(value);
        }

        public void Clear()
        {
            ((IList)layers).Clear();
        }

        public bool Contains(object value)
        {
            return ((IList)layers).Contains(value);
        }

        public void CopyTo(System.Array array, int index)
        {
            ((ICollection)layers).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)layers).GetEnumerator();
        }

        public int IndexOf(object value)
        {
            return ((IList)layers).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            ((IList)layers).Insert(index, value);
        }

        public void Remove(object value)
        {
            ((IList)layers).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)layers).RemoveAt(index);
        }

        public Matrix Predict(Matrix input)
        {
            Matrix activation = layers[0].Apply(input);
            for (int i = 1; i < layers.Count; i++)
            {
                activation = layers[i].Apply(activation);
            }
            // Final layer's activation is the output, so we just return that
            return activation;
        }


        public void Train(Matrix xTrain, Matrix yTrain, int batchSize, int epochs = 10)
        {
            Console.WriteLine("Training...");
            for (int e = 0; e < epochs; e++)
            {
                List<Matrix> entries = xTrain.GetRows();
                List<Matrix> yTrue = yTrain.GetRows();

                List<Matrix> gradientSumW = new();
                List<Matrix> gradientSumB = new();

                // Initialize gradient sums
                for (int i = 0; i < layers.Count; i++)
                {
                    gradientSumW.Add(new Matrix(layers[i].Weights.Rows, layers[i].Weights.Columns));
                    gradientSumB.Add(new Matrix(layers[i].Bias.Rows, layers[i].Bias.Columns));
                }

                for (int i = 0; i < batchSize; i++)
                {
                    // Calculate the activations of each layer in the step
                    List<Matrix> activations = new();
                    Matrix activation = layers[0].Apply(entries[i]);
                    activations.Add(activation);
                    for (int j = 1; j < layers.Count; j++)
                    {
                        activation = layers[j].Apply(activation);
                        activations.Add(activation);
                    }


                    // Matrix stepError = lossFunction.CalculateStep(activation, yTrue[i]);
                    
                    Matrix activationDerivative = layers[layers.Count - 1].Activation
                        .Derivative(activations[activations.Count - 2] * layers[layers.Count - 1].Weights + layers[layers.Count - 1].Bias);
                    
                    // Calculate output error gradients
                    Matrix deltaLayer = lossFunction.Derivative(activation, yTrue[i]).Hadamard(activationDerivative);
                    
                    gradientSumW[gradientSumW.Count - 1] += (deltaLayer * activations[activations.Count - 2]).T;
                    gradientSumB[gradientSumB.Count - 1] += deltaLayer;
                    
                    for (int a = activations.Count - 2; a >= 1; a--)
                    {
                        // Calculate layer gradients, add them to sum
                        IActivation actFunc = layers[a].Activation;

                        activationDerivative = actFunc.Derivative(activations[a - 1] * layers[a].Weights + layers[a].Bias);

                        deltaLayer = (deltaLayer * layers[a + 1].Weights.T)
                            .Hadamard(activationDerivative);
                        
                        gradientSumW[a] += (activations[a - 1].T * deltaLayer);
                        gradientSumB[a] += deltaLayer;
                    }
                }
                // mean of gradients
                /*
                for (int i = 0; i < gradientSumW.Count; i++)
                    gradientSumW[i] *= (1d / gradientSumW.Count);

                for (int i = 0; i < gradientSumB.Count; i++)
                    gradientSumB[i] *= (1d / gradientSumB.Count);
                */

                for (int i = layers.Count - 1; i >= 0; i--)
                {
                    layers[i].Weights -= gradientSumW[i] * lossFunction.LearningRate;
                    layers[i].Bias -= gradientSumB[i] * lossFunction.LearningRate;
                }
                /*
                Console.WriteLine("Batch Predictions:");
                for (int i = 0; i < batchSize; i++)
                {
                    Console.WriteLine(entries[i].ToString() + (Predict(entries[i])[0, 0]));
                }
                */
            }
        }

        public void Train(List<Matrix> xTrain, List<Matrix> yTrain, int batchSize, int epochs = 10)
        {
            for (int e = 0; e < epochs; e++)
            {
                List<Matrix> entries = xTrain;
                List<Matrix> yTrue = yTrain;

                List<Matrix> gradientSumW = new();
                List<Matrix> gradientSumB = new();

                // Initialize gradient sums
                for (int i = 0; i < layers.Count; i++)
                {
                    gradientSumW.Add(new Matrix(layers[i].Weights.Rows, layers[i].Weights.Columns));
                    gradientSumB.Add(new Matrix(layers[i].Bias.Rows, layers[i].Bias.Columns));
                }

                for (int i = 0; i < batchSize; i++)
                {
                    // Calculate the activations of each layer in the step
                    List<Matrix> activations = new();
                    Matrix activation = layers[0].Apply(entries[i]);
                    activations.Add(activation);
                    for (int j = 1; j < layers.Count; j++)
                    {
                        activation = layers[j].Apply(activation);
                        activations.Add(activation);
                    }


                    Matrix stepError = lossFunction.CalculateStep(activation, yTrue[i]);

                    Matrix activationDerivative = layers[layers.Count - 1].Activation
                        .Derivative(activations[activations.Count - 2] * layers[layers.Count - 1].Weights + layers[layers.Count - 1].Bias);

                    // Calculate output error gradients
                    Matrix deltaLayer = lossFunction.Derivative(activation, yTrue[i]).Hadamard(activationDerivative);

                    gradientSumW[gradientSumW.Count - 1] += (deltaLayer * activations[activations.Count - 2]).T;
                    gradientSumB[gradientSumB.Count - 1] += deltaLayer;

                    for (int a = activations.Count - 2; a >= 1; a--)
                    {
                        // Calculate layer gradients, add them to sum
                        IActivation actFunc = layers[a].Activation;

                        activationDerivative = actFunc.Derivative(activations[a - 1] * layers[a].Weights + layers[a].Bias);


                        deltaLayer = (deltaLayer * layers[a + 1].Weights.T)
                            .Hadamard(activationDerivative);

                        gradientSumW[a] += (activations[a - 1].T * deltaLayer);
                        gradientSumB[a] += deltaLayer;
                    }
                }
                // mean of gradients
                /*
                for (int i = 0; i < gradientSumW.Count; i++)
                    gradientSumW[i] *= (1d / gradientSumW.Count);

                for (int i = 0; i < gradientSumB.Count; i++)
                    gradientSumB[i] *= (1d / gradientSumB.Count);
                */

                for (int i = layers.Count - 1; i >= 0; i--)
                {
                    layers[i].Weights -= gradientSumW[i] * lossFunction.LearningRate;
                    layers[i].Bias -= gradientSumB[i] * lossFunction.LearningRate;
                }
                /*Console.WriteLine("Batch Predictions:");
                for (int i = 0; i < batchSize; i++)
                {
                    Console.WriteLine(inputs[i].ToString() + (Predict(inputs[i])[0, 0]));
                }*/
            }
        }

    }

    namespace Layers
    {
        public abstract class Layer
        {
            public abstract IActivation Activation { get; set; }
            public abstract Matrix Weights { get; set; }
            public abstract Matrix Bias { get; set; }
            public abstract Tuple<int, int> OutputShape { get; set; }
            public abstract Matrix Apply(Matrix input);
        }

        public class Dense : Layer
        {
            public override IActivation Activation { get; set; }
            public override Matrix Weights { get; set; }
            public override Matrix Bias { get; set; }
            public override Tuple<int, int> OutputShape { get; set; }

            public Dense(int size, Tuple<int, int> inputShape, IActivation activation = null)
            {
                if (activation != null)
                    Activation = activation;
                else
                    Activation = new Sigmoid();

                Random random = new();
                Weights = new(inputShape.Item2, size);
                for (int i = 0; i < Weights.Rows; i++)
                    for (int j = 0; j < Weights.Columns; j++)
                        Weights[i, j] = random.NextDouble();

                Bias = new(inputShape.Item1, size);
                for (int i = 0; i < Bias.Rows; i++)
                    for (int j = 0; j < Bias.Columns; j++)
                        Bias[i, j] = random.NextDouble();

                OutputShape = new Tuple<int, int>(Bias.Rows, Bias.Columns);
            }

            public override Matrix Apply(Matrix input)
            {
                Matrix result = input * Weights + Bias;
                return Activation.Activate(result);
            }
        }

        public class InputLayer : Layer
        {
            public override IActivation Activation { get; set; }
            public override Matrix Weights { get; set; }
            public override Matrix Bias { get; set; }
            public override Tuple<int, int> OutputShape { get; set; }

            public InputLayer(Tuple<int, int> shape)
            {
                Weights = new(shape.Item2, shape.Item1);
                Weights.SetAll(1);
                Bias = new(shape.Item1, shape.Item2);
                Activation = new NoActivation();
                OutputShape = shape;
            }

            public override Matrix Apply(Matrix input)
            {
                return input;
            }
        }
    }

    namespace Activations
    {
        public interface IActivation
        {
            public Matrix Activate(Matrix input);
            public Matrix Derivative(Matrix input);
        }

        public class Sigmoid : IActivation
        {
            public Sigmoid()
            {

            }
            
            public Matrix Activate(Matrix input)
            {
                Matrix result = new Matrix(input.Rows, input.Columns);
                for (int i = 0; i < input.Rows; i++)
                {
                    for (int j = 0; j < input.Columns; j++)
                    {
                        double value = 1 / (1 + Math.Pow(Math.E, -input[i, j]));
                        result[i, j] = value > 0 ? value : 0;
                    }
                }
                return result;
            }
            public Matrix Derivative(Matrix input)
            {
                Matrix result = new(input.Rows, input.Columns);
                for (int i = 0; i < input.Rows; i++)
                {
                    for (int j = 0; j < input.Columns; j++)
                    {
                        double value = 1 / (1 + Math.Pow(Math.E, -input[i, j]));
                        result[i, j] = value * (1 - value);
                    }
                }
                return result;
            }

        }
        /*
        public class SoftMax : IActivation
        {
            public Array Activate(Array input)
            {
                throw new NotImplementedException();
            }
        }
        */
        public class ReLu : IActivation
        {
            public Matrix Activate(Matrix input)
            {
                Matrix result = new(input.Rows, input.Columns);
                for (int i = 0; i < result.Rows; i++)
                {
                    for (int j = 0; j < result.Columns; j++)
                    {
                        result[i, j] = input[i, j] >= 0 ? input[i, j] : double.Epsilon;
                    }
                }

                return result;
            }

            public Matrix Derivative(Matrix input)
            {
                Matrix result = new(input.Rows, input.Columns);
                for (int i = 0; i < result.Rows; i++)
                {
                    for (int j = 0; j < result.Columns; j++)
                    {
                        result[i, j] = input[i, j] >= 0 ? 1 : 0.1;
                    }
                }

                return result;
            }
        }
        

        public class NoActivation : IActivation
        {
            public Matrix Activate(Matrix input)
            {
                return input;
            }

            public Matrix Derivative(Matrix input)
            {
                return input;
            }
        }
    }

    namespace LossFunctions
    {
        public abstract class LossFunction
        {
            public abstract Matrix CalculateStep(Matrix predicted, Matrix actual);
            public abstract Matrix CalculateBatch(List<Matrix> stepErrors);
            public abstract Matrix Derivative(Matrix predicted, Matrix actual);
            public abstract double LearningRate { get; set; }
        }

        public class MAE : LossFunction
        {
            public override double LearningRate { get; set; }

            public MAE(double learningRate = 1)
            {
                LearningRate = learningRate;
            }

            public override Matrix CalculateStep(Matrix predicted, Matrix actual)
            {
                Matrix result = new(predicted.Rows, predicted.Columns);
                for (int i = 0; i < predicted.Rows; i++)
                    for (int j = 0; j < predicted.Columns; j++)
                        result[i, j] = Math.Abs(predicted[i, j] - actual[i, j]);

                return result;
            }

            public override Matrix Derivative(Matrix predicted, Matrix actual)
            {
                Matrix result = new(predicted.Rows, predicted.Columns);
                for (int i = 0; i < predicted.Count; i++)
                    for (int j = 0; j < predicted.Columns; j++)
                        result[i, j] = predicted[i, j] > actual[i, j] ? 1 : -1;

                return result;
            }
            /// <summary>
            /// Calculates the MAE of a batch
            /// </summary>
            /// <param name="predictedSteps"></param>
            /// <param name="actual"></param>
            /// <returns></returns>
            public override Matrix CalculateBatch(List<Matrix> stepErrors)
            {
                Matrix sums = new(stepErrors.Count, stepErrors[0].Columns);
                for (int i = 0; i < stepErrors.Count; i++)
                {
                    for (int r = 0; r < stepErrors[i].Rows; r++)
                    {
                        for (int c = 0; c < stepErrors[i].Columns; c++)
                        {
                            sums[i, c] += stepErrors[i][r, c];
                        }
                    }
                }

                return sums * (1d / (double)stepErrors.Count);
            }
        }

        public class BinaryCrossEntropy : LossFunction
        {
            public override double LearningRate { get; set; }

            public BinaryCrossEntropy(double learningRate = 1)
            {
                LearningRate = learningRate;
            }

            public override Matrix CalculateBatch(List<Matrix> stepErrors)
            {
                Matrix sums = new(stepErrors.Count, stepErrors[0].Columns);
                for (int i = 0; i < stepErrors.Count; i++)
                {
                    for (int r = 0; r < stepErrors[i].Rows; r++)
                    {
                        for (int c = 0; c < stepErrors[i].Columns; c++)
                        {
                            sums[i, c] += stepErrors[i][r, c];
                        }
                    }
                }

                return sums * (1d / (double) stepErrors.Count);
            }

            public override Matrix CalculateStep(Matrix predicted, Matrix actual)
            {
                Matrix result = new(predicted.Rows, predicted.Columns);
                for (int i = 0; i < result.Rows; i++)
                {
                    for (int j = 0; j < result.Columns; j++)
                    {
                        result[i, j] = CrossEntropy(actual[i, j], predicted[i, j]);
                    }
                }

                return result;
            }

            public override Matrix Derivative(Matrix predicted, Matrix actual)
            {
                Matrix result = new(predicted.Rows, predicted.Columns);
                for (int i = 0; i < result.Rows; i++)
                    for (int j = 0; j < result.Columns; j++)
                        result[i, j] = CrossEntropyPrime(actual[i, j], predicted[i, j]);

                return result;
            }

            private double CrossEntropy(double yTrue, double y)
            {
                if (yTrue == 1)
                    return -Math.Log(y, Math.E);

                return -Math.Log(1 - y, Math.E);
            }

            private double CrossEntropyPrime(double yTrue, double y)
            {
                double term1 = -yTrue / y;
                double term2 = (1 - yTrue) / (1 - y);
             
                if (yTrue == 1 && y == 1)
                {
                    return -1;
                }
                else if (yTrue == 0 && y == 0)
                {
                    return 1;
                }
                else
                    return term1 + term2;
            }
        }

        public class MSE : LossFunction
        {
            public override double LearningRate { get; set; }

            public MSE(double learningRate = 1)
            {
                LearningRate = learningRate;
            }

            public override Matrix CalculateStep(Matrix predicted, Matrix actual)
            {
                Matrix result = new(predicted.Rows, predicted.Columns);
                for (int i = 0; i < result.Rows; i++)
                {
                    for (int j = 0; j < result.Columns; j++)
                    {
                        result[i, j] = Math.Pow(predicted[i, j] - actual[i, j], 2);
                    }
                }
                return result;
            }

            public override Matrix CalculateBatch(List<Matrix> stepErrors)
            {
                throw new NotImplementedException();
            }

            public override Matrix Derivative(Matrix predicted, Matrix actual)
            {
                Matrix result = new(predicted.Rows, predicted.Columns);
                for (int i = 0; i < result.Rows; i++)
                {
                    for (int j = 0; j < result.Columns; j++)
                    {
                        result[i, j] = 2 * (predicted[i, j] - actual[i, j]);
                    }
                }

                return result;
            }


        }
    }
}
