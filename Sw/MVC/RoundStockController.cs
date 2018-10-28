﻿//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.MVC;
using CodeStack.Community.StockFit.Stocks.Cylinder;
using CodeStack.Community.StockFit.Sw.Options;
using CodeStack.Community.StockFit.Sw.Services;
using CodeStack.SwEx.PMPage;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorksTools.File;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Attributes;

namespace CodeStack.Community.StockFit.Sw.MVC
{
    public class RoundStockController
    {
        private RoundStockView m_ActivePage;

        private readonly ISldWorks m_App;
        private readonly RoundStockModel m_Model;
        
        private readonly OptionsStore m_OptsStore;

        private IPartDoc m_CurrentPart;
        private RoundStockFeatureParameters m_CurrentParameters;
        private IFeature m_EditingFeature;
        private RoundStockViewModel m_CurrentViewModel;

        public event Action<RoundStockFeatureParameters, IPartDoc, IFeature, bool> FeatureEditingCompleted;
        public event Action<RoundStockFeatureParameters, IPartDoc, bool> FeatureInsertionCompleted;

        public RoundStockController(ISldWorks app, RoundStockModel model, 
            OptionsStore opts)
        {
            m_App = app;
            m_Model = model;
            m_OptsStore = opts;
        }

        public void ShowPage(RoundStockFeatureParameters parameters, IPartDoc part, IFeature editingFeature)
        {
            m_CurrentParameters = parameters;
            m_CurrentPart = part;
            m_EditingFeature = editingFeature;
            
            if (m_ActivePage != null)
            {
                m_ActivePage.Handler.DataChanged -= OnDataChanged;
                m_ActivePage.Handler.Closing -= OnPageClosing;
                m_ActivePage.Handler.Closed -= OnClosed;
            }

            m_CurrentViewModel = RoundStockViewModel.FromParameters(parameters);

            m_ActivePage = new RoundStockView(m_CurrentViewModel, m_App);

            m_ActivePage.Handler.DataChanged += OnDataChanged;
            m_ActivePage.Handler.Closing += OnPageClosing;
            m_ActivePage.Handler.Closed += OnClosed;

            m_ActivePage.Show();
            
            m_Model.ShowPreview(part, parameters.Direction, parameters.ConcenticWithCylindricalFace, parameters.StockStep);
        }

        private void OnDataChanged()
        {
            m_CurrentViewModel.WriteToParameters(m_CurrentParameters);

            m_Model.ShowPreview(m_CurrentPart, m_CurrentParameters.Direction,
                m_CurrentParameters.ConcenticWithCylindricalFace, m_CurrentParameters.StockStep);
        }

        private void OnPageClosing(swPropertyManagerPageCloseReasons_e reason, SwEx.PMPage.Base.ClosingArg arg)
        {
            if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                IBody2 body = null;

                Exception err = null;

                try
                {
                    var cylParams = m_Model.GetCylinderParameters(m_CurrentPart, m_CurrentParameters.Direction,
                        m_CurrentParameters.ConcenticWithCylindricalFace, m_CurrentParameters.StockStep);

                    body = m_Model.CreateCylindricalStock(cylParams);
                }
                catch(Exception ex)
                {
                    err = ex;
                }
                
                if (body == null)
                {
                    arg.Cancel = true;
                    arg.ErrorTitle = "Cylindrical Stock Error";
                    arg.ErrorMessage = err?.Message;
                }
            }
        }

        private void OnClosed(swPropertyManagerPageCloseReasons_e reason)
        {
            m_Model.HidePreview(m_CurrentPart);

            var isOk = reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay;

            if (isOk)
            {
                m_CurrentParameters.ScopeBody = m_Model.GetScopeBody(
                    m_CurrentPart, m_CurrentParameters.Direction);

                m_OptsStore.Save(m_CurrentParameters);
            }

            if (m_EditingFeature != null)
            {
                FeatureEditingCompleted?.Invoke(m_CurrentParameters, m_CurrentPart, m_EditingFeature,
                    isOk);
            }
            else
            {
                FeatureInsertionCompleted?.Invoke(m_CurrentParameters, m_CurrentPart, isOk);
            }

            m_EditingFeature = null;
            m_CurrentPart = null;
        }
    }
}
