﻿using CodeStack.Community.StockFit.Base.Math.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeStack.Community.StockFit.Stocks.Cylinder
{
    public class CylinderParams
    {
        public double Height { get; set; }
        public Point Origin { get; set; }
        public Vector Axis { get; set; }
        public double Radius { get; set; }

        public CylinderParams(double height, Point origin, Vector axis, double radius)
        {
            Height = height;
            Origin = origin;
            Radius = radius;
            Axis = axis;
        }
    }
}
