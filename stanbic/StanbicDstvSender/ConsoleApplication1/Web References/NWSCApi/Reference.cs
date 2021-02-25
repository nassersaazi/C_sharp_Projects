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

namespace ConsoleApplication1.NWSCApi {
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
    [System.Web.Services.WebServiceBindingAttribute(Name="NWSCBillingInterfaceSoap", Namespace="http://NWSCBillingInterface/")]
    public partial class NWSCBillingInterface : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback verifyCustomerDetailsOperationCompleted;
        
        private System.Threading.SendOrPostCallback verifyCustomerDetailsWithAreaOperationCompleted;
        
        private System.Threading.SendOrPostCallback postCustomerTransactionsOperationCompleted;
        
        private System.Threading.SendOrPostCallback postCustomerTransactionsWithAreaOperationCompleted;
        
        private System.Threading.SendOrPostCallback postChequeTransactionsOperationCompleted;
        
        private System.Threading.SendOrPostCallback postEftTransactionsOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public NWSCBillingInterface() {
            this.Url = global::ConsoleApplication1.Properties.Settings.Default.ConsoleApplication1_NWSCApi_NWSCBillingInterface;
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
        public event verifyCustomerDetailsCompletedEventHandler verifyCustomerDetailsCompleted;
        
        /// <remarks/>
        public event verifyCustomerDetailsWithAreaCompletedEventHandler verifyCustomerDetailsWithAreaCompleted;
        
        /// <remarks/>
        public event postCustomerTransactionsCompletedEventHandler postCustomerTransactionsCompleted;
        
        /// <remarks/>
        public event postCustomerTransactionsWithAreaCompletedEventHandler postCustomerTransactionsWithAreaCompleted;
        
        /// <remarks/>
        public event postChequeTransactionsCompletedEventHandler postChequeTransactionsCompleted;
        
        /// <remarks/>
        public event postEftTransactionsCompletedEventHandler postEftTransactionsCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://NWSCBillingInterface/verifyCustomerDetails", RequestNamespace="http://NWSCBillingInterface/", ResponseNamespace="http://NWSCBillingInterface/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Customer verifyCustomerDetails(string custref, string vendorCode, string password) {
            object[] results = this.Invoke("verifyCustomerDetails", new object[] {
                        custref,
                        vendorCode,
                        password});
            return ((Customer)(results[0]));
        }
        
        /// <remarks/>
        public void verifyCustomerDetailsAsync(string custref, string vendorCode, string password) {
            this.verifyCustomerDetailsAsync(custref, vendorCode, password, null);
        }
        
        /// <remarks/>
        public void verifyCustomerDetailsAsync(string custref, string vendorCode, string password, object userState) {
            if ((this.verifyCustomerDetailsOperationCompleted == null)) {
                this.verifyCustomerDetailsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnverifyCustomerDetailsOperationCompleted);
            }
            this.InvokeAsync("verifyCustomerDetails", new object[] {
                        custref,
                        vendorCode,
                        password}, this.verifyCustomerDetailsOperationCompleted, userState);
        }
        
        private void OnverifyCustomerDetailsOperationCompleted(object arg) {
            if ((this.verifyCustomerDetailsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.verifyCustomerDetailsCompleted(this, new verifyCustomerDetailsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://NWSCBillingInterface/verifyCustomerDetailsWithArea", RequestNamespace="http://NWSCBillingInterface/", ResponseNamespace="http://NWSCBillingInterface/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Customer verifyCustomerDetailsWithArea(string custref, string area, string vendorCode, string password) {
            object[] results = this.Invoke("verifyCustomerDetailsWithArea", new object[] {
                        custref,
                        area,
                        vendorCode,
                        password});
            return ((Customer)(results[0]));
        }
        
        /// <remarks/>
        public void verifyCustomerDetailsWithAreaAsync(string custref, string area, string vendorCode, string password) {
            this.verifyCustomerDetailsWithAreaAsync(custref, area, vendorCode, password, null);
        }
        
        /// <remarks/>
        public void verifyCustomerDetailsWithAreaAsync(string custref, string area, string vendorCode, string password, object userState) {
            if ((this.verifyCustomerDetailsWithAreaOperationCompleted == null)) {
                this.verifyCustomerDetailsWithAreaOperationCompleted = new System.Threading.SendOrPostCallback(this.OnverifyCustomerDetailsWithAreaOperationCompleted);
            }
            this.InvokeAsync("verifyCustomerDetailsWithArea", new object[] {
                        custref,
                        area,
                        vendorCode,
                        password}, this.verifyCustomerDetailsWithAreaOperationCompleted, userState);
        }
        
        private void OnverifyCustomerDetailsWithAreaOperationCompleted(object arg) {
            if ((this.verifyCustomerDetailsWithAreaCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.verifyCustomerDetailsWithAreaCompleted(this, new verifyCustomerDetailsWithAreaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://NWSCBillingInterface/postCustomerTransactions", RequestNamespace="http://NWSCBillingInterface/", ResponseNamespace="http://NWSCBillingInterface/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public PostResponse postCustomerTransactions(string custref, string custName, string customerTel, System.DateTime paymentDate, int transactionAmount, string vendorTransactionRef, string transactionType, string vendorCode, string password) {
            object[] results = this.Invoke("postCustomerTransactions", new object[] {
                        custref,
                        custName,
                        customerTel,
                        paymentDate,
                        transactionAmount,
                        vendorTransactionRef,
                        transactionType,
                        vendorCode,
                        password});
            return ((PostResponse)(results[0]));
        }
        
        /// <remarks/>
        public void postCustomerTransactionsAsync(string custref, string custName, string customerTel, System.DateTime paymentDate, int transactionAmount, string vendorTransactionRef, string transactionType, string vendorCode, string password) {
            this.postCustomerTransactionsAsync(custref, custName, customerTel, paymentDate, transactionAmount, vendorTransactionRef, transactionType, vendorCode, password, null);
        }
        
        /// <remarks/>
        public void postCustomerTransactionsAsync(string custref, string custName, string customerTel, System.DateTime paymentDate, int transactionAmount, string vendorTransactionRef, string transactionType, string vendorCode, string password, object userState) {
            if ((this.postCustomerTransactionsOperationCompleted == null)) {
                this.postCustomerTransactionsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnpostCustomerTransactionsOperationCompleted);
            }
            this.InvokeAsync("postCustomerTransactions", new object[] {
                        custref,
                        custName,
                        customerTel,
                        paymentDate,
                        transactionAmount,
                        vendorTransactionRef,
                        transactionType,
                        vendorCode,
                        password}, this.postCustomerTransactionsOperationCompleted, userState);
        }
        
        private void OnpostCustomerTransactionsOperationCompleted(object arg) {
            if ((this.postCustomerTransactionsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.postCustomerTransactionsCompleted(this, new postCustomerTransactionsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://NWSCBillingInterface/postCustomerTransactionsWithArea", RequestNamespace="http://NWSCBillingInterface/", ResponseNamespace="http://NWSCBillingInterface/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public PostResponse postCustomerTransactionsWithArea(string custref, string custName, string area, string customerTel, System.DateTime paymentDate, int transactionAmount, string vendorTransactionRef, string transactionType, string vendorCode, string password) {
            object[] results = this.Invoke("postCustomerTransactionsWithArea", new object[] {
                        custref,
                        custName,
                        area,
                        customerTel,
                        paymentDate,
                        transactionAmount,
                        vendorTransactionRef,
                        transactionType,
                        vendorCode,
                        password});
            return ((PostResponse)(results[0]));
        }
        
        /// <remarks/>
        public void postCustomerTransactionsWithAreaAsync(string custref, string custName, string area, string customerTel, System.DateTime paymentDate, int transactionAmount, string vendorTransactionRef, string transactionType, string vendorCode, string password) {
            this.postCustomerTransactionsWithAreaAsync(custref, custName, area, customerTel, paymentDate, transactionAmount, vendorTransactionRef, transactionType, vendorCode, password, null);
        }
        
        /// <remarks/>
        public void postCustomerTransactionsWithAreaAsync(string custref, string custName, string area, string customerTel, System.DateTime paymentDate, int transactionAmount, string vendorTransactionRef, string transactionType, string vendorCode, string password, object userState) {
            if ((this.postCustomerTransactionsWithAreaOperationCompleted == null)) {
                this.postCustomerTransactionsWithAreaOperationCompleted = new System.Threading.SendOrPostCallback(this.OnpostCustomerTransactionsWithAreaOperationCompleted);
            }
            this.InvokeAsync("postCustomerTransactionsWithArea", new object[] {
                        custref,
                        custName,
                        area,
                        customerTel,
                        paymentDate,
                        transactionAmount,
                        vendorTransactionRef,
                        transactionType,
                        vendorCode,
                        password}, this.postCustomerTransactionsWithAreaOperationCompleted, userState);
        }
        
        private void OnpostCustomerTransactionsWithAreaOperationCompleted(object arg) {
            if ((this.postCustomerTransactionsWithAreaCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.postCustomerTransactionsWithAreaCompleted(this, new postCustomerTransactionsWithAreaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://NWSCBillingInterface/postChequeTransactions", RequestNamespace="http://NWSCBillingInterface/", ResponseNamespace="http://NWSCBillingInterface/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public PostResponse postChequeTransactions(string custref, string custName, string area, string customerTel, System.DateTime paymentDate, int transactionAmount, string vendorTransactionRef, string chequeNumber, string narrative, string vendorCode, string password) {
            object[] results = this.Invoke("postChequeTransactions", new object[] {
                        custref,
                        custName,
                        area,
                        customerTel,
                        paymentDate,
                        transactionAmount,
                        vendorTransactionRef,
                        chequeNumber,
                        narrative,
                        vendorCode,
                        password});
            return ((PostResponse)(results[0]));
        }
        
        /// <remarks/>
        public void postChequeTransactionsAsync(string custref, string custName, string area, string customerTel, System.DateTime paymentDate, int transactionAmount, string vendorTransactionRef, string chequeNumber, string narrative, string vendorCode, string password) {
            this.postChequeTransactionsAsync(custref, custName, area, customerTel, paymentDate, transactionAmount, vendorTransactionRef, chequeNumber, narrative, vendorCode, password, null);
        }
        
        /// <remarks/>
        public void postChequeTransactionsAsync(string custref, string custName, string area, string customerTel, System.DateTime paymentDate, int transactionAmount, string vendorTransactionRef, string chequeNumber, string narrative, string vendorCode, string password, object userState) {
            if ((this.postChequeTransactionsOperationCompleted == null)) {
                this.postChequeTransactionsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnpostChequeTransactionsOperationCompleted);
            }
            this.InvokeAsync("postChequeTransactions", new object[] {
                        custref,
                        custName,
                        area,
                        customerTel,
                        paymentDate,
                        transactionAmount,
                        vendorTransactionRef,
                        chequeNumber,
                        narrative,
                        vendorCode,
                        password}, this.postChequeTransactionsOperationCompleted, userState);
        }
        
        private void OnpostChequeTransactionsOperationCompleted(object arg) {
            if ((this.postChequeTransactionsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.postChequeTransactionsCompleted(this, new postChequeTransactionsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://NWSCBillingInterface/postEftTransactions", RequestNamespace="http://NWSCBillingInterface/", ResponseNamespace="http://NWSCBillingInterface/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public PostResponse postEftTransactions(string custref, string custName, System.DateTime paymentDate, int transactionAmount, string vendorTransactionRef, string narrative, string vendorCode, string password) {
            object[] results = this.Invoke("postEftTransactions", new object[] {
                        custref,
                        custName,
                        paymentDate,
                        transactionAmount,
                        vendorTransactionRef,
                        narrative,
                        vendorCode,
                        password});
            return ((PostResponse)(results[0]));
        }
        
        /// <remarks/>
        public void postEftTransactionsAsync(string custref, string custName, System.DateTime paymentDate, int transactionAmount, string vendorTransactionRef, string narrative, string vendorCode, string password) {
            this.postEftTransactionsAsync(custref, custName, paymentDate, transactionAmount, vendorTransactionRef, narrative, vendorCode, password, null);
        }
        
        /// <remarks/>
        public void postEftTransactionsAsync(string custref, string custName, System.DateTime paymentDate, int transactionAmount, string vendorTransactionRef, string narrative, string vendorCode, string password, object userState) {
            if ((this.postEftTransactionsOperationCompleted == null)) {
                this.postEftTransactionsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnpostEftTransactionsOperationCompleted);
            }
            this.InvokeAsync("postEftTransactions", new object[] {
                        custref,
                        custName,
                        paymentDate,
                        transactionAmount,
                        vendorTransactionRef,
                        narrative,
                        vendorCode,
                        password}, this.postEftTransactionsOperationCompleted, userState);
        }
        
        private void OnpostEftTransactionsOperationCompleted(object arg) {
            if ((this.postEftTransactionsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.postEftTransactionsCompleted(this, new postEftTransactionsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://NWSCBillingInterface/")]
    public partial class Customer {
        
        private string custRefField;
        
        private string propertyRefField;
        
        private string custNameField;
        
        private string areaField;
        
        private int outstandingBalField;
        
        private string customerErrorField;
        
        /// <remarks/>
        public string CustRef {
            get {
                return this.custRefField;
            }
            set {
                this.custRefField = value;
            }
        }
        
        /// <remarks/>
        public string PropertyRef {
            get {
                return this.propertyRefField;
            }
            set {
                this.propertyRefField = value;
            }
        }
        
        /// <remarks/>
        public string CustName {
            get {
                return this.custNameField;
            }
            set {
                this.custNameField = value;
            }
        }
        
        /// <remarks/>
        public string Area {
            get {
                return this.areaField;
            }
            set {
                this.areaField = value;
            }
        }
        
        /// <remarks/>
        public int OutstandingBal {
            get {
                return this.outstandingBalField;
            }
            set {
                this.outstandingBalField = value;
            }
        }
        
        /// <remarks/>
        public string CustomerError {
            get {
                return this.customerErrorField;
            }
            set {
                this.customerErrorField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://NWSCBillingInterface/")]
    public partial class PostResponse {
        
        private string successfulField;
        
        private string postErrorField;
        
        /// <remarks/>
        public string Successful {
            get {
                return this.successfulField;
            }
            set {
                this.successfulField = value;
            }
        }
        
        /// <remarks/>
        public string PostError {
            get {
                return this.postErrorField;
            }
            set {
                this.postErrorField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void verifyCustomerDetailsCompletedEventHandler(object sender, verifyCustomerDetailsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class verifyCustomerDetailsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal verifyCustomerDetailsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Customer Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Customer)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void verifyCustomerDetailsWithAreaCompletedEventHandler(object sender, verifyCustomerDetailsWithAreaCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class verifyCustomerDetailsWithAreaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal verifyCustomerDetailsWithAreaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Customer Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Customer)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void postCustomerTransactionsCompletedEventHandler(object sender, postCustomerTransactionsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class postCustomerTransactionsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal postCustomerTransactionsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public PostResponse Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((PostResponse)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void postCustomerTransactionsWithAreaCompletedEventHandler(object sender, postCustomerTransactionsWithAreaCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class postCustomerTransactionsWithAreaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal postCustomerTransactionsWithAreaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public PostResponse Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((PostResponse)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void postChequeTransactionsCompletedEventHandler(object sender, postChequeTransactionsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class postChequeTransactionsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal postChequeTransactionsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public PostResponse Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((PostResponse)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void postEftTransactionsCompletedEventHandler(object sender, postEftTransactionsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class postEftTransactionsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal postEftTransactionsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public PostResponse Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((PostResponse)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591