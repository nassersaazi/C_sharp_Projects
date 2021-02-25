﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace ConsoleApplication1.KYUBillingInterface {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="KyuPay", Namespace="https://kyupay.kyu.ac.ug/epay?wsdl")]
    public partial class EPayment : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback ValidateRequestOperationCompleted;
        
        private System.Threading.SendOrPostCallback CommitRequestOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public EPayment() {
            this.Url = global::ConsoleApplication1.Properties.Settings.Default.ConsoleApplication1_ug_ac_kyu_kyupay_EPayment;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event ValidateRequestCompletedEventHandler ValidateRequestCompleted;
        
        /// <remarks/>
        public event CommitRequestCompletedEventHandler CommitRequestCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://kyupay.kyu.ac.ug/epay/ValidateRequest", RequestNamespace="https://kyupay.kyu.ac.ug/epay?wsdl", ResponseNamespace="https://kyupay.kyu.ac.ug/epay?wsdl", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ValidateResponse ValidateRequest(string RequestReferenceNo, string RequestState, string RequestSignature, string RequestSourceId) {
            object[] results = this.Invoke("ValidateRequest", new object[] {
                        RequestReferenceNo,
                        RequestState,
                        RequestSignature,
                        RequestSourceId});
            return ((ValidateResponse)(results[0]));
        }
        
        /// <remarks/>
        public void ValidateRequestAsync(string RequestReferenceNo, string RequestState, string RequestSignature, string RequestSourceId) {
            this.ValidateRequestAsync(RequestReferenceNo, RequestState, RequestSignature, RequestSourceId, null);
        }
        
        /// <remarks/>
        public void ValidateRequestAsync(string RequestReferenceNo, string RequestState, string RequestSignature, string RequestSourceId, object userState) {
            if ((this.ValidateRequestOperationCompleted == null)) {
                this.ValidateRequestOperationCompleted = new System.Threading.SendOrPostCallback(this.OnValidateRequestOperationCompleted);
            }
            this.InvokeAsync("ValidateRequest", new object[] {
                        RequestReferenceNo,
                        RequestState,
                        RequestSignature,
                        RequestSourceId}, this.ValidateRequestOperationCompleted, userState);
        }
        
        private void OnValidateRequestOperationCompleted(object arg) {
            if ((this.ValidateRequestCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ValidateRequestCompleted(this, new ValidateRequestCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://kyupay.kyu.ac.ug/epay/CommitRequest", RequestNamespace="https://kyupay.kyu.ac.ug/epay?wsdl", ResponseNamespace="https://kyupay.kyu.ac.ug/epay?wsdl", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("CommitRequestResponseResult")]
        public CommitResp CommitRequest(string RequestReferenceNo, string RequestState, string RequestTransactionId, string AuthToken, string Amount, string Branch, string PaymentType, string ChequeNo, string ValueDate, string Date) {
            object[] results = this.Invoke("CommitRequest", new object[] {
                        RequestReferenceNo,
                        RequestState,
                        RequestTransactionId,
                        AuthToken,
                        Amount,
                        Branch,
                        PaymentType,
                        ChequeNo,
                        ValueDate,
                        Date});
            return ((CommitResp)(results[0]));
        }
        
        /// <remarks/>
        public void CommitRequestAsync(string RequestReferenceNo, string RequestState, string RequestTransactionId, string AuthToken, string Amount, string Branch, string PaymentType, string ChequeNo, string ValueDate, string Date) {
            this.CommitRequestAsync(RequestReferenceNo, RequestState, RequestTransactionId, AuthToken, Amount, Branch, PaymentType, ChequeNo, ValueDate, Date, null);
        }
        
        /// <remarks/>
        public void CommitRequestAsync(string RequestReferenceNo, string RequestState, string RequestTransactionId, string AuthToken, string Amount, string Branch, string PaymentType, string ChequeNo, string ValueDate, string Date, object userState) {
            if ((this.CommitRequestOperationCompleted == null)) {
                this.CommitRequestOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCommitRequestOperationCompleted);
            }
            this.InvokeAsync("CommitRequest", new object[] {
                        RequestReferenceNo,
                        RequestState,
                        RequestTransactionId,
                        AuthToken,
                        Amount,
                        Branch,
                        PaymentType,
                        ChequeNo,
                        ValueDate,
                        Date}, this.CommitRequestOperationCompleted, userState);
        }
        
        private void OnCommitRequestOperationCompleted(object arg) {
            if ((this.CommitRequestCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CommitRequestCompleted(this, new CommitRequestCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://kyupay.kyu.ac.ug/epay?wsdl")]
    public partial class ValidateResponse {
        
        private string requestReferenceNoField;
        
        private string authTokenField;
        
        private string entityTitleField;
        
        private string serialNoField;
        
        private string amountField;
        
        private string statusField;
        
        private string messageField;
        
        /// <remarks/>
        public string RequestReferenceNo {
            get {
                return this.requestReferenceNoField;
            }
            set {
                this.requestReferenceNoField = value;
            }
        }
        
        /// <remarks/>
        public string AuthToken {
            get {
                return this.authTokenField;
            }
            set {
                this.authTokenField = value;
            }
        }
        
        /// <remarks/>
        public string EntityTitle {
            get {
                return this.entityTitleField;
            }
            set {
                this.entityTitleField = value;
            }
        }
        
        /// <remarks/>
        public string SerialNo {
            get {
                return this.serialNoField;
            }
            set {
                this.serialNoField = value;
            }
        }
        
        /// <remarks/>
        public string Amount {
            get {
                return this.amountField;
            }
            set {
                this.amountField = value;
            }
        }
        
        /// <remarks/>
        public string Status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
            }
        }
        
        /// <remarks/>
        public string Message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://kyupay.kyu.ac.ug/epay?wsdl")]
    public partial class CommitResp {
        
        private string requestReferenceNoField;
        
        private string requestTransactionIdField;
        
        private string statusField;
        
        private string messageField;
        
        /// <remarks/>
        public string RequestReferenceNo {
            get {
                return this.requestReferenceNoField;
            }
            set {
                this.requestReferenceNoField = value;
            }
        }
        
        /// <remarks/>
        public string RequestTransactionId {
            get {
                return this.requestTransactionIdField;
            }
            set {
                this.requestTransactionIdField = value;
            }
        }
        
        /// <remarks/>
        public string Status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
            }
        }
        
        /// <remarks/>
        public string Message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void ValidateRequestCompletedEventHandler(object sender, ValidateRequestCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ValidateRequestCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ValidateRequestCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ValidateResponse Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ValidateResponse)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void CommitRequestCompletedEventHandler(object sender, CommitRequestCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CommitRequestCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CommitRequestCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public CommitResp Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((CommitResp)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591