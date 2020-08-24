﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.8669
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 2.0.50727.8669.
// 
#pragma warning disable 1591

namespace myReversalTester.UmemeApi {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="EPaymentSoap", Namespace="http://UmemeEPayment/")]
    public partial class EPayment : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetTransactionDetailsOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetServerStatusOperationCompleted;
        
        private System.Threading.SendOrPostCallback ValidateCustomerOperationCompleted;
        
        private System.Threading.SendOrPostCallback PostUmemePaymentOperationCompleted;
        
        private System.Threading.SendOrPostCallback PostYakaPaymentOperationCompleted;
        
        private System.Threading.SendOrPostCallback PostBankUmemePaymentOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public EPayment() {
            this.Url = global::myReversalTester.Properties.Settings.Default.myReversalTester_UmemeApi_EPayment;
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
        public event GetTransactionDetailsCompletedEventHandler GetTransactionDetailsCompleted;
        
        /// <remarks/>
        public event GetServerStatusCompletedEventHandler GetServerStatusCompleted;
        
        /// <remarks/>
        public event ValidateCustomerCompletedEventHandler ValidateCustomerCompleted;
        
        /// <remarks/>
        public event PostUmemePaymentCompletedEventHandler PostUmemePaymentCompleted;
        
        /// <remarks/>
        public event PostYakaPaymentCompletedEventHandler PostYakaPaymentCompleted;
        
        /// <remarks/>
        public event PostBankUmemePaymentCompletedEventHandler PostBankUmemePaymentCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://UmemeEPayment/GetTransactionDetails", RequestNamespace="http://UmemeEPayment/", ResponseNamespace="http://UmemeEPayment/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response GetTransactionDetails(string vendorTranID, string vendorCode, string password) {
            object[] results = this.Invoke("GetTransactionDetails", new object[] {
                        vendorTranID,
                        vendorCode,
                        password});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void GetTransactionDetailsAsync(string vendorTranID, string vendorCode, string password) {
            this.GetTransactionDetailsAsync(vendorTranID, vendorCode, password, null);
        }
        
        /// <remarks/>
        public void GetTransactionDetailsAsync(string vendorTranID, string vendorCode, string password, object userState) {
            if ((this.GetTransactionDetailsOperationCompleted == null)) {
                this.GetTransactionDetailsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetTransactionDetailsOperationCompleted);
            }
            this.InvokeAsync("GetTransactionDetails", new object[] {
                        vendorTranID,
                        vendorCode,
                        password}, this.GetTransactionDetailsOperationCompleted, userState);
        }
        
        private void OnGetTransactionDetailsOperationCompleted(object arg) {
            if ((this.GetTransactionDetailsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetTransactionDetailsCompleted(this, new GetTransactionDetailsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://UmemeEPayment/GetServerStatus", RequestNamespace="http://UmemeEPayment/", ResponseNamespace="http://UmemeEPayment/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetServerStatus() {
            object[] results = this.Invoke("GetServerStatus", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetServerStatusAsync() {
            this.GetServerStatusAsync(null);
        }
        
        /// <remarks/>
        public void GetServerStatusAsync(object userState) {
            if ((this.GetServerStatusOperationCompleted == null)) {
                this.GetServerStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetServerStatusOperationCompleted);
            }
            this.InvokeAsync("GetServerStatus", new object[0], this.GetServerStatusOperationCompleted, userState);
        }
        
        private void OnGetServerStatusOperationCompleted(object arg) {
            if ((this.GetServerStatusCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetServerStatusCompleted(this, new GetServerStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://UmemeEPayment/ValidateCustomer", RequestNamespace="http://UmemeEPayment/", ResponseNamespace="http://UmemeEPayment/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Customer ValidateCustomer(string customerRef, string vendorCode, string password) {
            object[] results = this.Invoke("ValidateCustomer", new object[] {
                        customerRef,
                        vendorCode,
                        password});
            return ((Customer)(results[0]));
        }
        
        /// <remarks/>
        public void ValidateCustomerAsync(string customerRef, string vendorCode, string password) {
            this.ValidateCustomerAsync(customerRef, vendorCode, password, null);
        }
        
        /// <remarks/>
        public void ValidateCustomerAsync(string customerRef, string vendorCode, string password, object userState) {
            if ((this.ValidateCustomerOperationCompleted == null)) {
                this.ValidateCustomerOperationCompleted = new System.Threading.SendOrPostCallback(this.OnValidateCustomerOperationCompleted);
            }
            this.InvokeAsync("ValidateCustomer", new object[] {
                        customerRef,
                        vendorCode,
                        password}, this.ValidateCustomerOperationCompleted, userState);
        }
        
        private void OnValidateCustomerOperationCompleted(object arg) {
            if ((this.ValidateCustomerCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ValidateCustomerCompleted(this, new ValidateCustomerCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://UmemeEPayment/PostUmemePayment", RequestNamespace="http://UmemeEPayment/", ResponseNamespace="http://UmemeEPayment/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response PostUmemePayment(Transaction trans) {
            object[] results = this.Invoke("PostUmemePayment", new object[] {
                        trans});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void PostUmemePaymentAsync(Transaction trans) {
            this.PostUmemePaymentAsync(trans, null);
        }
        
        /// <remarks/>
        public void PostUmemePaymentAsync(Transaction trans, object userState) {
            if ((this.PostUmemePaymentOperationCompleted == null)) {
                this.PostUmemePaymentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPostUmemePaymentOperationCompleted);
            }
            this.InvokeAsync("PostUmemePayment", new object[] {
                        trans}, this.PostUmemePaymentOperationCompleted, userState);
        }
        
        private void OnPostUmemePaymentOperationCompleted(object arg) {
            if ((this.PostUmemePaymentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.PostUmemePaymentCompleted(this, new PostUmemePaymentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://UmemeEPayment/PostYakaPayment", RequestNamespace="http://UmemeEPayment/", ResponseNamespace="http://UmemeEPayment/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Token PostYakaPayment(Transaction trans) {
            object[] results = this.Invoke("PostYakaPayment", new object[] {
                        trans});
            return ((Token)(results[0]));
        }
        
        /// <remarks/>
        public void PostYakaPaymentAsync(Transaction trans) {
            this.PostYakaPaymentAsync(trans, null);
        }
        
        /// <remarks/>
        public void PostYakaPaymentAsync(Transaction trans, object userState) {
            if ((this.PostYakaPaymentOperationCompleted == null)) {
                this.PostYakaPaymentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPostYakaPaymentOperationCompleted);
            }
            this.InvokeAsync("PostYakaPayment", new object[] {
                        trans}, this.PostYakaPaymentOperationCompleted, userState);
        }
        
        private void OnPostYakaPaymentOperationCompleted(object arg) {
            if ((this.PostYakaPaymentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.PostYakaPaymentCompleted(this, new PostYakaPaymentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://UmemeEPayment/PostBankUmemePayment", RequestNamespace="http://UmemeEPayment/", ResponseNamespace="http://UmemeEPayment/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response PostBankUmemePayment(Transaction trans) {
            object[] results = this.Invoke("PostBankUmemePayment", new object[] {
                        trans});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void PostBankUmemePaymentAsync(Transaction trans) {
            this.PostBankUmemePaymentAsync(trans, null);
        }
        
        /// <remarks/>
        public void PostBankUmemePaymentAsync(Transaction trans, object userState) {
            if ((this.PostBankUmemePaymentOperationCompleted == null)) {
                this.PostBankUmemePaymentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPostBankUmemePaymentOperationCompleted);
            }
            this.InvokeAsync("PostBankUmemePayment", new object[] {
                        trans}, this.PostBankUmemePaymentOperationCompleted, userState);
        }
        
        private void OnPostBankUmemePaymentOperationCompleted(object arg) {
            if ((this.PostBankUmemePaymentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.PostBankUmemePaymentCompleted(this, new PostBankUmemePaymentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.8662")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://UmemeEPayment/")]
    public partial class Response {
        
        private string statusCodeField;
        
        private string tokenField;
        
        private string statusDescriptionField;
        
        private string receiptNumberField;
        
        /// <remarks/>
        public string StatusCode {
            get {
                return this.statusCodeField;
            }
            set {
                this.statusCodeField = value;
            }
        }
        
        /// <remarks/>
        public string Token {
            get {
                return this.tokenField;
            }
            set {
                this.tokenField = value;
            }
        }
        
        /// <remarks/>
        public string StatusDescription {
            get {
                return this.statusDescriptionField;
            }
            set {
                this.statusDescriptionField = value;
            }
        }
        
        /// <remarks/>
        public string ReceiptNumber {
            get {
                return this.receiptNumberField;
            }
            set {
                this.receiptNumberField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.8662")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://UmemeEPayment/")]
    public partial class Token {
        
        private string remainingCreditField;
        
        private string lifeLineField;
        
        private string serviceFeeField;
        
        private string payAccountField;
        
        private string debtRecoveryField;
        
        private string receiptNumberField;
        
        private string statusDescriptionField;
        
        private string statusCodeField;
        
        private string meterNumberField;
        
        private string unitsField;
        
        private string tokenValueField;
        
        private string inflationField;
        
        private string taxField;
        
        private string fxField;
        
        private string fuelField;
        
        private string totalAmountField;
        
        private string prepaidTokenField;
        
        /// <remarks/>
        public string RemainingCredit {
            get {
                return this.remainingCreditField;
            }
            set {
                this.remainingCreditField = value;
            }
        }
        
        /// <remarks/>
        public string LifeLine {
            get {
                return this.lifeLineField;
            }
            set {
                this.lifeLineField = value;
            }
        }
        
        /// <remarks/>
        public string ServiceFee {
            get {
                return this.serviceFeeField;
            }
            set {
                this.serviceFeeField = value;
            }
        }
        
        /// <remarks/>
        public string PayAccount {
            get {
                return this.payAccountField;
            }
            set {
                this.payAccountField = value;
            }
        }
        
        /// <remarks/>
        public string DebtRecovery {
            get {
                return this.debtRecoveryField;
            }
            set {
                this.debtRecoveryField = value;
            }
        }
        
        /// <remarks/>
        public string ReceiptNumber {
            get {
                return this.receiptNumberField;
            }
            set {
                this.receiptNumberField = value;
            }
        }
        
        /// <remarks/>
        public string StatusDescription {
            get {
                return this.statusDescriptionField;
            }
            set {
                this.statusDescriptionField = value;
            }
        }
        
        /// <remarks/>
        public string StatusCode {
            get {
                return this.statusCodeField;
            }
            set {
                this.statusCodeField = value;
            }
        }
        
        /// <remarks/>
        public string MeterNumber {
            get {
                return this.meterNumberField;
            }
            set {
                this.meterNumberField = value;
            }
        }
        
        /// <remarks/>
        public string Units {
            get {
                return this.unitsField;
            }
            set {
                this.unitsField = value;
            }
        }
        
        /// <remarks/>
        public string TokenValue {
            get {
                return this.tokenValueField;
            }
            set {
                this.tokenValueField = value;
            }
        }
        
        /// <remarks/>
        public string Inflation {
            get {
                return this.inflationField;
            }
            set {
                this.inflationField = value;
            }
        }
        
        /// <remarks/>
        public string Tax {
            get {
                return this.taxField;
            }
            set {
                this.taxField = value;
            }
        }
        
        /// <remarks/>
        public string Fx {
            get {
                return this.fxField;
            }
            set {
                this.fxField = value;
            }
        }
        
        /// <remarks/>
        public string Fuel {
            get {
                return this.fuelField;
            }
            set {
                this.fuelField = value;
            }
        }
        
        /// <remarks/>
        public string TotalAmount {
            get {
                return this.totalAmountField;
            }
            set {
                this.totalAmountField = value;
            }
        }
        
        /// <remarks/>
        public string PrepaidToken {
            get {
                return this.prepaidTokenField;
            }
            set {
                this.prepaidTokenField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.8662")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://UmemeEPayment/")]
    public partial class Transaction {
        
        private string tranIdField;
        
        private string statusDescriptionField;
        
        private string digitalSignatureField;
        
        private string statusCodeField;
        
        private string paymentDateField;
        
        private string passwordField;
        
        private string tranAmountField;
        
        private string tellerField;
        
        private string vendorCodeField;
        
        private string tranNarrationField;
        
        private string vendorTranIdField;
        
        private string tranIdToReverseField;
        
        private string paymentTypeField;
        
        private string tranTypeField;
        
        private string customerRefField;
        
        private string customerNameField;
        
        private string customerTypeField;
        
        private string customerTelField;
        
        private string reversalField;
        
        private string offlineField;
        
        /// <remarks/>
        public string TranId {
            get {
                return this.tranIdField;
            }
            set {
                this.tranIdField = value;
            }
        }
        
        /// <remarks/>
        public string StatusDescription {
            get {
                return this.statusDescriptionField;
            }
            set {
                this.statusDescriptionField = value;
            }
        }
        
        /// <remarks/>
        public string DigitalSignature {
            get {
                return this.digitalSignatureField;
            }
            set {
                this.digitalSignatureField = value;
            }
        }
        
        /// <remarks/>
        public string StatusCode {
            get {
                return this.statusCodeField;
            }
            set {
                this.statusCodeField = value;
            }
        }
        
        /// <remarks/>
        public string PaymentDate {
            get {
                return this.paymentDateField;
            }
            set {
                this.paymentDateField = value;
            }
        }
        
        /// <remarks/>
        public string Password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
            }
        }
        
        /// <remarks/>
        public string TranAmount {
            get {
                return this.tranAmountField;
            }
            set {
                this.tranAmountField = value;
            }
        }
        
        /// <remarks/>
        public string Teller {
            get {
                return this.tellerField;
            }
            set {
                this.tellerField = value;
            }
        }
        
        /// <remarks/>
        public string VendorCode {
            get {
                return this.vendorCodeField;
            }
            set {
                this.vendorCodeField = value;
            }
        }
        
        /// <remarks/>
        public string TranNarration {
            get {
                return this.tranNarrationField;
            }
            set {
                this.tranNarrationField = value;
            }
        }
        
        /// <remarks/>
        public string VendorTranId {
            get {
                return this.vendorTranIdField;
            }
            set {
                this.vendorTranIdField = value;
            }
        }
        
        /// <remarks/>
        public string TranIdToReverse {
            get {
                return this.tranIdToReverseField;
            }
            set {
                this.tranIdToReverseField = value;
            }
        }
        
        /// <remarks/>
        public string PaymentType {
            get {
                return this.paymentTypeField;
            }
            set {
                this.paymentTypeField = value;
            }
        }
        
        /// <remarks/>
        public string TranType {
            get {
                return this.tranTypeField;
            }
            set {
                this.tranTypeField = value;
            }
        }
        
        /// <remarks/>
        public string CustomerRef {
            get {
                return this.customerRefField;
            }
            set {
                this.customerRefField = value;
            }
        }
        
        /// <remarks/>
        public string CustomerName {
            get {
                return this.customerNameField;
            }
            set {
                this.customerNameField = value;
            }
        }
        
        /// <remarks/>
        public string CustomerType {
            get {
                return this.customerTypeField;
            }
            set {
                this.customerTypeField = value;
            }
        }
        
        /// <remarks/>
        public string CustomerTel {
            get {
                return this.customerTelField;
            }
            set {
                this.customerTelField = value;
            }
        }
        
        /// <remarks/>
        public string Reversal {
            get {
                return this.reversalField;
            }
            set {
                this.reversalField = value;
            }
        }
        
        /// <remarks/>
        public string Offline {
            get {
                return this.offlineField;
            }
            set {
                this.offlineField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.8662")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://UmemeEPayment/")]
    public partial class Customer {
        
        private string statusDescriptionField;
        
        private string statusCodeField;
        
        private string customerRefField;
        
        private string customerNameField;
        
        private string customerTypeField;
        
        private double balanceField;
        
        private double creditField;
        
        /// <remarks/>
        public string StatusDescription {
            get {
                return this.statusDescriptionField;
            }
            set {
                this.statusDescriptionField = value;
            }
        }
        
        /// <remarks/>
        public string StatusCode {
            get {
                return this.statusCodeField;
            }
            set {
                this.statusCodeField = value;
            }
        }
        
        /// <remarks/>
        public string CustomerRef {
            get {
                return this.customerRefField;
            }
            set {
                this.customerRefField = value;
            }
        }
        
        /// <remarks/>
        public string CustomerName {
            get {
                return this.customerNameField;
            }
            set {
                this.customerNameField = value;
            }
        }
        
        /// <remarks/>
        public string CustomerType {
            get {
                return this.customerTypeField;
            }
            set {
                this.customerTypeField = value;
            }
        }
        
        /// <remarks/>
        public double Balance {
            get {
                return this.balanceField;
            }
            set {
                this.balanceField = value;
            }
        }
        
        /// <remarks/>
        public double Credit {
            get {
                return this.creditField;
            }
            set {
                this.creditField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    public delegate void GetTransactionDetailsCompletedEventHandler(object sender, GetTransactionDetailsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetTransactionDetailsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetTransactionDetailsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    public delegate void GetServerStatusCompletedEventHandler(object sender, GetServerStatusCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetServerStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetServerStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    public delegate void ValidateCustomerCompletedEventHandler(object sender, ValidateCustomerCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ValidateCustomerCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ValidateCustomerCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    public delegate void PostUmemePaymentCompletedEventHandler(object sender, PostUmemePaymentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PostUmemePaymentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal PostUmemePaymentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    public delegate void PostYakaPaymentCompletedEventHandler(object sender, PostYakaPaymentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PostYakaPaymentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal PostYakaPaymentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Token Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Token)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    public delegate void PostBankUmemePaymentCompletedEventHandler(object sender, PostBankUmemePaymentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.8662")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PostBankUmemePaymentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal PostBankUmemePaymentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591