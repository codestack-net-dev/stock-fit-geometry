﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Base.Math.Structures
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point(double[] pt)
        {
            if (pt == null)
            {
                throw new ArgumentNullException(nameof(pt));
            }

            if (pt.Length != 3)
            {
                throw new ArgumentException("The size of input array must be equal to 3 (coordinate)");
            }

            X = pt[0];
            Y = pt[1];
            Z = pt[2];
        }

        public double[] ToArray()
        {
            return new double[] { X, Y, Z };
        }

        public bool IsSame(Point pt)
        {
            if (pt == null)
            {
                throw new ArgumentNullException(nameof(pt));
            }

            return IsSame(pt.X, pt.Y, pt.Z);
        }

        public bool IsSame(double x, double y, double z)
        {
            return X == x && Y == y && Z == z;
        }
    }
}
