﻿//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class EnumExtension
    {
        public static TAtt GetAttribute<TAtt>(this Enum enumer)
            where TAtt : Attribute
        {
            var enumType = enumer.GetType();
            var enumField = enumType.GetMember(enumer.ToString()).FirstOrDefault();
            var atts = enumField.GetCustomAttributes(typeof(TAtt), false);

            if (atts != null && atts.Any())
            {
                return atts.First() as TAtt;
            }
            else
            {
                throw new NullReferenceException($"Attribute of type {typeof(TAtt)} is not fond on {enumer}");
            }
        }
    }
}
