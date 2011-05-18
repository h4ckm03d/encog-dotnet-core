using System;
using Encog.MathUtil.Matrices;
using Encog.ML;
using Encog.Neural.Networks;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Provides basic functionality that most randomizers will need.
    /// </summary>
    ///
    public abstract class BasicRandomizer : IRandomizer
    {
        /// <summary>
        /// The random number generator.
        /// </summary>
        ///
        private Random random;

        /// <summary>
        /// Construct a random number generator with a random(current time) seed. If
        /// you want to set your own seed, just call "getRandom().setSeed".
        /// </summary>
        ///
        public BasicRandomizer()
        {
            random = new Random((int) (DateTime.Now.Ticks*100));
        }


        /// <value>the random to set</value>
        public Random Random
        {
            /// <returns>The random number generator in use. Use this to set the seed, if
            /// desired.</returns>
            get { return random; }
            /// <param name="theRandom">the random to set</param>
            set { random = value; }
        }

        #region IRandomizer Members

        /// <summary>
        /// Randomize the array based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        ///
        /// <param name="d">An array to randomize.</param>
        public virtual void Randomize(double[] d)
        {
            Randomize(d, 0, d.Length);
        }

        /// <summary>
        /// Randomize the array based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        ///
        /// <param name="d">An array to randomize.</param>
        /// <param name="begin">The beginning element of the array.</param>
        /// <param name="size">The size of the array to copy.</param>
        public virtual void Randomize(double[] d, int begin, int size)
        {
            for (int i = 0; i < size; i++)
            {
                d[begin + i] = Randomize(d[begin + i]);
            }
        }

        /// <summary>
        /// Randomize the 2d array based on an array, modify the array. Previous
        /// values may be used, or they may be discarded, depending on the
        /// randomizer.
        /// </summary>
        ///
        /// <param name="d">An array to randomize.</param>
        public virtual void Randomize(double[][] d)
        {
            for (int r = 0; r < d.Length; r++)
            {
                for (int c = 0; c < d[0].Length; c++)
                {
                    d[r][c] = Randomize(d[r][c]);
                }
            }
        }

        /// <summary>
        /// Randomize the matrix based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        ///
        /// <param name="m">A matrix to randomize.</param>
        public virtual void Randomize(Matrix m)
        {
            double[][] d = m.Data;
            for (int r = 0; r < m.Rows; r++)
            {
                for (int c = 0; c < m.Cols; c++)
                {
                    d[r][c] = Randomize(d[r][c]);
                }
            }
        }

        /// <summary>
        /// Randomize the synapses and biases in the basic network based on an array,
        /// modify the array. Previous values may be used, or they may be discarded,
        /// depending on the randomizer.
        /// </summary>
        ///
        /// <param name="method">A network to randomize.</param>
        public virtual void Randomize(MLMethod method)
        {
            if (method is BasicNetwork)
            {
                var network = (BasicNetwork) method;
                for (int i = 0; i < network.LayerCount - 1; i++)
                {
                    Randomize(network, i);
                }
            }
            else if (method is MLEncodable)
            {
                var encode = (MLEncodable) method;
                var encoded = new double[encode.EncodedArrayLength()];
                encode.EncodeToArray(encoded);
                Randomize(encoded);
                encode.DecodeFromArray(encoded);
            }
        }

        /// <summary>
        /// from Encog.mathutil.randomize.Randomizer
        /// </summary>
        ///
        public abstract double Randomize(double d);

        #endregion

        /// <returns>The next double.</returns>
        public double NextDouble()
        {
            return random.NextDouble();
        }

        /// <summary>
        /// Generate a random number in the specified range.
        /// </summary>
        ///
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A random number.</returns>
        public double NextDouble(double min, double max)
        {
            double range = max - min;
            return (range*random.NextDouble()) + min;
        }

        /// <summary>
        /// Randomize one level of a neural network.
        /// </summary>
        ///
        /// <param name="network">The network to randomize</param>
        /// <param name="fromLayer">The from level to randomize.</param>
        public virtual void Randomize(BasicNetwork network, int fromLayer)
        {
            int fromCount = network.GetLayerTotalNeuronCount(fromLayer);
            int toCount = network.GetLayerNeuronCount(fromLayer + 1);

            for (int fromNeuron = 0; fromNeuron < fromCount; fromNeuron++)
            {
                for (int toNeuron = 0; toNeuron < toCount; toNeuron++)
                {
                    double v = network.GetWeight(fromLayer, fromNeuron, toNeuron);
                    v = Randomize(v);
                    network.SetWeight(fromLayer, fromNeuron, toNeuron, v);
                }
            }
        }
    }
}