using System;
using System.Collections.Generic;
using System.Collections;


namespace LinAlg
{
    public class Array : IList<double>
    {
        private List<double> data;

        public double this[int index] { get => data[index]; set => data[index] = value; }

        public int Count => data.Count;

        public bool IsReadOnly => ((ICollection<double>)data).IsReadOnly;

        public Array T { get => GetTranspose(); }

        public Array(List<double> data)
        {
            this.data = data;
        }

        public Array()
        {
            data = new List<double>();
        }

        public Array(int size)
        {
            data = new List<double>(size);
        }

        private Array GetTranspose()
        {
            Array result = new Array();

            for (int i = 0; i < data.Count; i++)
            {

            }
            
            return new Array();
        }

        public void SetAll(double value)
        {
            for (int i = 0; i < data.Count; i++)
            {
                data[i] = value;
            }
        }

        public void Add(double item)
        {
            ((ICollection<double>)data).Add(item);
        }

        public void Clear()
        {
            ((ICollection<double>)data).Clear();
        }

        public bool Contains(double item)
        {
            return ((ICollection<double>)data).Contains(item);
        }

        public void CopyTo(double[] array, int arrayIndex)
        {
            ((ICollection<double>)data).CopyTo(array, arrayIndex);
        }

        public IEnumerator<double> GetEnumerator()
        {
            return ((IEnumerable<double>)data).GetEnumerator();
        }

        public int IndexOf(double item)
        {
            return ((IList<double>)data).IndexOf(item);
        }

        public void Insert(int index, double item)
        {
            ((IList<double>)data).Insert(index, item);
        }

        public bool Remove(double item)
        {
            return ((ICollection<double>)data).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<double>)data).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public static Array operator +(Array a, Array b)
        {
            Array result = new Array();
            for (int i = 0; i < a.Count; i++)
            {
                result.Add(a[i]);
            }
            for (int i = 0; i < a.Count; i++)
            {
                result[i] += b[i];
            }
            return result;
        }

        public static Array operator -(Array a, Array b)
        {
            Array result = new Array();
            for (int i = 0; i < a.Count; i++)
            {
                result.Add(a[i]);
            }
            for (int i = 0; i < a.Count; i++)
            {
                result[i] -= b[i];
            }
            return result;
        }

        public static Array operator /(Array a, Array b)
        {
            Array result = new Array();
            for (int i = 0; i < a.Count; i++)
            {
                result.Add(a[i]);
            }
            for (int i = 0; i < a.Count; i++)
            {
                result[i] /= b[i];
            }
            return result;
        }

        public static Array operator *(Array a, Array b)
        {
            Array result = new Array();
            for (int i = 0; i < a.Count; i++)
            {
                result.Add(a[i]);
            }
            for (int i = 0; i < a.Count; i++)
            {
                result[i] *= b[i];
            }
            return result;
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < data.Count; i++)
            {
                if (i == data.Count - 1)
                {
                    result += data[i].ToString();
                }
                else
                {
                    result += data[i].ToString() + ", ";
                }
            }
            return "{ " + result + " }";
        }

        public double Dot(Array b)
        {
            if (Count != b.Count)
            {
                Console.WriteLine("Arrays must be of the same size");
                return -1d;
            }

            double[] products = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                products[i] = data[i] * b[i];
            }

            double result = 0;
            for (int i = 0; i < products.Length; i++)
            {
                result += products[i];
            }
            return result;
        }
    }

    public class Matrix : ICollection
    {
        private double[,] data;

        public Matrix T { get => GetTranspose(); }

        public int Rows { get; private set; }

        public int Columns { get; private set; }

        public Tuple<int, int> Shape { get => new Tuple<int, int>(Rows, Columns); }

        public double this[int x, int y]
        {
            get => data[x, y];
            set => data[x, y] = value;
        }

        public int Count => ((ICollection)data).Count;

        public bool IsSynchronized => data.IsSynchronized;

        public object SyncRoot => data.SyncRoot;

        public Matrix(int rows, int columns = 1)
        {
            data = new double[rows, columns];
            Rows = rows;
            Columns = columns;
        }

        /// <summary>
        /// Specifically, this returns a matrix whose first row is the input array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Matrix FromArray(Array input)
        {
            Matrix result = new(1, input.Count);
            for (int i = 0; i < input.Count; i++)
                result[0, i] = input[i];
            return result;
        }

        /// <summary>
        /// Returns a list of row vectors as matrices
        /// </summary>
        /// <returns></returns>
        public List<Matrix> GetRows()
        {
            List<Matrix> result = new();
            for (int i = 0; i < Rows; i++)
            {
                Matrix row = new(1, Columns);
                for (int j = 0; j < Columns; j++)
                {
                    row[0, j] = data[i, j];
                }
                result.Add(row);
            }
            return result;
        }

        private Matrix GetTranspose()
        {
            Matrix result = new Matrix(Columns, Rows);

            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    result[y, x] = data[x, y];
                }
            }

            return result;
        }

        public void SetRow(int index, double[] row)
        {
            if (row.Length != Columns)
            {
                Console.WriteLine("Row length does not match.");
            }
            else
            {
                for (int i = 0; i < Columns; i++)
                {
                    data[index, i] = row[i];
                }
            }
        }

        public void SetRow(int index, Array row)
        {
            if (row.Count != Columns)
            {
                Console.WriteLine("Row length does not match.");
            }
            else
            {
                for (int i = 0; i < Columns; i++)
                {
                    data[index, i] = row[i];
                }
            }
        }

        public void SetAll(double value)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    data[i, j] = value;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public void CopyTo(System.Array array, int index)
        {
            data.CopyTo(array, index);
        }

        public Matrix Dot(Matrix b)
        {
            if (Columns != b.Rows)
            {
                Console.WriteLine("Inner dimensions must match.");
                return new Matrix(0, 0);
            }

            List<Array> rows = new List<Array>();
            for (int i = 0; i < Rows; i++)
            {
                Array row = new();
                for (int j = 0; j < Columns; j++)
                {
                    row.Add(data[i, j]);
                }
                rows.Add(row);
            }

            List<Array> cols = new List<Array>();
            for (int i = 0; i < b.Columns; i++)
            {
                Array col = new();
                for (int j = 0; j < b.Rows; j++)
                {
                    col.Add(b[j, i]);
                }
                cols.Add(col);
            }

            Matrix result = new Matrix(Rows, b.Columns);
            for (int i = 0; i < result.Rows; i++)
            {
                for (int j = 0; j < result.Columns; j++)
                {
                    result[i, j] = rows[i].Dot(cols[j]);
                }
            }
            return result;
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result += data[i, j].ToString() + " ";
                }
                result += "\n";
            }
            return result;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.Rows == b.Rows && a.Columns == b.Columns)
            {
                Matrix result = new Matrix(a.Rows, a.Columns);
                for (int i = 0; i < a.Rows; i++)
                {
                    for (int j = 0; j < a.Columns; j++)
                    {
                        result[i, j] = a[i, j] + b[i, j];
                    }
                }
                return result;
            }
            else
            {
                throw new ArgumentException("Matrix dimensions must match");
            }
        }

        public static Matrix operator +(Matrix a, double b)
        {
            Matrix result = new Matrix(a.Rows, a.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    result[i, j] = a[i, j] + b;
                }
            }
            return result;
        }

        public static Matrix operator +(double b, Matrix a)
        {
            Matrix result = new Matrix(a.Rows, a.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    result[i, j] = b + a[i, j];
                }
            }
            return result;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.Rows == b.Rows && a.Columns == b.Columns)
            {
                Matrix result = new Matrix(a.Rows, a.Columns);
                for (int i = 0; i < a.Rows; i++)
                {
                    for (int j = 0; j < a.Columns; j++)
                    {
                        result[i, j] = a[i, j] - b[i, j];
                    }
                }
                return result;
            }
            else
            {
                throw new ArgumentException("Matrix dimensions must match");
            }
        }

        public static Matrix operator -(double a, Matrix b)
        {
            Matrix result = new(b.Rows, b.Columns);
            for (int i = 0; i < b.Rows; i++)
            {
                for (int j = 0; j < b.Columns; j++)
                {
                    result[i, j] = a - b[i, j];
                }
            }
            return result;
        }

        public static Matrix operator -(Matrix b, double a)
        {
            Matrix result = new(b.Rows, b.Columns);
            for (int i = 0; i < b.Rows; i++)
            {
                for (int j = 0; j < b.Columns; j++)
                {
                    result[i, j] = b[i, j] - a;
                }
            }
            return result;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Columns == b.Rows)
            {
                return a.Dot(b);
            }
            else
            {
                throw new ArgumentException("Inner matrix dimensions must match");
            }
        }

        public static Matrix operator *(double a, Matrix b)
        {
            Matrix result = new(b.Rows, b.Columns);
            for (int i = 0; i < b.Rows; i++)
            {
                for (int j = 0; j < b.Columns; j++)
                {
                    result[i, j] = a * b[i, j];
                }
            }
            return result;
        }

        public static Matrix operator *(Matrix b, double a)
        {
            Matrix result = new(b.Rows, b.Columns);
            for (int i = 0; i < b.Rows; i++)
            {
                for (int j = 0; j < b.Columns; j++)
                {
                    result[i, j] = a * b[i, j];
                }
            }
            return result;
        }
        /// <summary>
        /// Element-wise multiplication
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Matrix Hadamard(Matrix b)
        {
            if (Rows != b.Rows || Columns != b.Columns)
                throw new ArgumentException("Matrix dimensions must match.");
            Matrix result = new(Rows, Columns);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result[i, j] = data[i, j] * b[i, j];
                }
            }
            return result;
        }
    }
}
