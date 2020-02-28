using Autodesk.Revit.UI;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revon.UI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {

        public UIApplication _uiapp;

        public ShellViewModel(UIApplication uiapp)
        {
            _uiapp = uiapp;

            ActivateItem(new SummaryViewModel(this, uiapp));
            Request = new FamilyRequest();
        }

        public void GoToImportPage()
        {
            ActivateItem(new ImportViewModel(this));
        }

        public void GoToSettingsPage()
        {
            ActivateItem(new SettingsViewModel(this));
        }

        public void GoToFamiliesSummary()
        {
            ActivateItem(new SummaryViewModel(this, _uiapp));
        }

        #region request

        /// <summary>
        /// In this sample, the dialog owns the value of the request but it is not necessary. It may as
        /// well be a static property of the application.
        /// </summary>
        private FamilyRequest m_request;

        /// <summary>
        /// Request property
        /// </summary>
        public FamilyRequest Request
        {
            get
            {
                return m_request;
            }
            private set
            {
                m_request = value;
            }
        }

        /// <summary>
        ///   Control enabler / disabler 
        /// </summary>
        ///
        private void EnableCommands(bool status)
        {
            // ActiveItem.IsEnabled = false;
            //foreach (Control ctrl in this.Content)
            //{
            //    ctrl.Enabled = status;
            //}
            //if (!status)
            //{
            //    // this.btnExit.Enabled = true;
            //}
        }

        /// <summary>
        ///   A private helper method to make a request
        ///   and put the dialog to sleep at the same time.
        /// </summary>
        /// <remarks>
        ///   It is expected that the process which executes the request 
        ///   (the Idling helper in this particular case) will also
        ///   wake the dialog up after finishing the execution.
        /// </remarks>
        ///
        public void MakeRequest(RequestId request)
        {
            Request.Make(request);
            DozeOff();
        }


        /// <summary>
        ///   DozeOff -> disable all controls (but the Exit button)
        /// </summary>
        /// 
        private void DozeOff()
        {
            EnableCommands(false);
        }

        /// <summary>
        ///   WakeUp -> enable all controls
        /// </summary>
        /// 
        public void WakeUp()
        {
            EnableCommands(true);
        }

        #endregion
    }

}
