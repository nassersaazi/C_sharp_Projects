﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.6387
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 2.0.50727.6387.
// 
#pragma warning disable 1591

namespace UtilReqSender.StartimesConnect {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.6387")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="StarTimesConnectSoap", Namespace="http://tempuri.org/")]
    public partial class StarTimesConnect : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetStarTimesCustomerDetailsOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetCustomerDetailsOperationCompleted;
        
        private System.Threading.SendOrPostCallback ProcessRechargePaymentOperationCompleted;
        
        private System.Threading.SendOrPostCallback ProcessStarTimesPaymentOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public StarTimesConnect() {
            this.Url = global::UtilReqSender.Properties.Settings.Default.UtilReqSender_StartimesConnect_StarTimesConnect;
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
        public event GetStarTimesCustomerDetailsCompletedEventHandler GetStarTimesCustomerDetailsCompleted;
        
        /// <remarks/>
        public event GetCustomerDetailsCompletedEventHandler GetCustomerDetailsCompleted;
        
        /// <remarks/>
        public event ProcessRechargePaymentCompletedEventHandler ProcessRechargePaymentCompleted;
        
        /// <remarks/>
        public event ProcessStarTimesPaymentCompletedEventHandler ProcessStarTimesPaymentCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetStarTimesCustomerDetails", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public SubscriberQueryResult GetStarTimesCustomerDetails(string customerRef, string username, string password) {
            object[] results = this.Invoke("GetStarTimesCustomerDetails", new object[] {
                        customerRef,
                        username,
                        password});
            return ((SubscriberQueryResult)(results[0]));
        }
        
        /// <remarks/>
        public void GetStarTimesCustomerDetailsAsync(string customerRef, string username, string password) {
            this.GetStarTimesCustomerDetailsAsync(customerRef, username, password, null);
        }
        
        /// <remarks/>
        public void GetStarTimesCustomerDetailsAsync(string customerRef, string username, string password, object userState) {
            if ((this.GetStarTimesCustomerDetailsOperationCompleted == null)) {
                this.GetStarTimesCustomerDetailsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetStarTimesCustomerDetailsOperationCompleted);
            }
            this.InvokeAsync("GetStarTimesCustomerDetails", new object[] {
                        customerRef,
                        username,
                        password}, this.GetStarTimesCustomerDetailsOperationCompleted, userState);
        }
        
        private void OnGetStarTimesCustomerDetailsOperationCompleted(object arg) {
            if ((this.GetStarTimesCustomerDetailsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetStarTimesCustomerDetailsCompleted(this, new GetStarTimesCustomerDetailsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetCustomerDetails", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public BalanceInfo GetCustomerDetails(string customerRef, string queryNumber, string username, string password) {
            object[] results = this.Invoke("GetCustomerDetails", new object[] {
                        customerRef,
                        queryNumber,
                        username,
                        password});
            return ((BalanceInfo)(results[0]));
        }
        
        /// <remarks/>
        public void GetCustomerDetailsAsync(string customerRef, string queryNumber, string username, string password) {
            this.GetCustomerDetailsAsync(customerRef, queryNumber, username, password, null);
        }
        
        /// <remarks/>
        public void GetCustomerDetailsAsync(string customerRef, string queryNumber, string username, string password, object userState) {
            if ((this.GetCustomerDetailsOperationCompleted == null)) {
                this.GetCustomerDetailsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetCustomerDetailsOperationCompleted);
            }
            this.InvokeAsync("GetCustomerDetails", new object[] {
                        customerRef,
                        queryNumber,
                        username,
                        password}, this.GetCustomerDetailsOperationCompleted, userState);
        }
        
        private void OnGetCustomerDetailsOperationCompleted(object arg) {
            if ((this.GetCustomerDetailsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetCustomerDetailsCompleted(this, new GetCustomerDetailsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ProcessRechargePayment", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ThirdPartnerRes ProcessRechargePayment(StarTimesTransaction transaction) {
            object[] results = this.Invoke("ProcessRechargePayment", new object[] {
                        transaction});
            return ((ThirdPartnerRes)(results[0]));
        }
        
        /// <remarks/>
        public void ProcessRechargePaymentAsync(StarTimesTransaction transaction) {
            this.ProcessRechargePaymentAsync(transaction, null);
        }
        
        /// <remarks/>
        public void ProcessRechargePaymentAsync(StarTimesTransaction transaction, object userState) {
            if ((this.ProcessRechargePaymentOperationCompleted == null)) {
                this.ProcessRechargePaymentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnProcessRechargePaymentOperationCompleted);
            }
            this.InvokeAsync("ProcessRechargePayment", new object[] {
                        transaction}, this.ProcessRechargePaymentOperationCompleted, userState);
        }
        
        private void OnProcessRechargePaymentOperationCompleted(object arg) {
            if ((this.ProcessRechargePaymentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ProcessRechargePaymentCompleted(this, new ProcessRechargePaymentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ProcessStarTimesPayment", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public PaymentResponse ProcessStarTimesPayment(StarTimesTransaction transaction) {
            object[] results = this.Invoke("ProcessStarTimesPayment", new object[] {
                        transaction});
            return ((PaymentResponse)(results[0]));
        }
        
        /// <remarks/>
        public void ProcessStarTimesPaymentAsync(StarTimesTransaction transaction) {
            this.ProcessStarTimesPaymentAsync(transaction, null);
        }
        
        /// <remarks/>
        public void ProcessStarTimesPaymentAsync(StarTimesTransaction transaction, object userState) {
            if ((this.ProcessStarTimesPaymentOperationCompleted == null)) {
                this.ProcessStarTimesPaymentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnProcessStarTimesPaymentOperationCompleted);
            }
            this.InvokeAsync("ProcessStarTimesPayment", new object[] {
                        transaction}, this.ProcessStarTimesPaymentOperationCompleted, userState);
        }
        
        private void OnProcessStarTimesPaymentOperationCompleted(object arg) {
            if ((this.ProcessStarTimesPaymentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ProcessStarTimesPaymentCompleted(this, new ProcessStarTimesPaymentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.6387")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://haiwai.model.sms.star.com")]
    public partial class SubscriberQueryResult {
        
        private System.Nullable<double> balanceField;
        
        private bool balanceFieldSpecified;
        
        private ProductInfo[] canOrderProductInfosField;
        
        private string customerNameField;
        
        private string orderedProductsDescField;
        
        private string returnCodeField;
        
        private string returnMsgField;
        
        private string smartCardCodeField;
        
        private System.Nullable<int> subscriberStatusField;
        
        private bool subscriberStatusFieldSpecified;
        
        private string transactionlNoField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<double> balance {
            get {
                return this.balanceField;
            }
            set {
                this.balanceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool balanceSpecified {
            get {
                return this.balanceFieldSpecified;
            }
            set {
                this.balanceFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)]
        public ProductInfo[] canOrderProductInfos {
            get {
                return this.canOrderProductInfosField;
            }
            set {
                this.canOrderProductInfosField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string customerName {
            get {
                return this.customerNameField;
            }
            set {
                this.customerNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string orderedProductsDesc {
            get {
                return this.orderedProductsDescField;
            }
            set {
                this.orderedProductsDescField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string returnCode {
            get {
                return this.returnCodeField;
            }
            set {
                this.returnCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string returnMsg {
            get {
                return this.returnMsgField;
            }
            set {
                this.returnMsgField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string smartCardCode {
            get {
                return this.smartCardCodeField;
            }
            set {
                this.smartCardCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> subscriberStatus {
            get {
                return this.subscriberStatusField;
            }
            set {
                this.subscriberStatusField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool subscriberStatusSpecified {
            get {
                return this.subscriberStatusFieldSpecified;
            }
            set {
                this.subscriberStatusFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string transactionlNo {
            get {
                return this.transactionlNoField;
            }
            set {
                this.transactionlNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.6387")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://haiwai.model.sms.star.com")]
    public partial class ProductInfo {
        
        private string productDescField;
        
        private string productNoField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string productDesc {
            get {
                return this.productDescField;
            }
            set {
                this.productDescField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string productNo {
            get {
                return this.productNoField;
            }
            set {
                this.productNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.6387")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class PaymentResponse {
        
        private string statusCodeField;
        
        private string statusDescriptionField;
        
        private string transactionIdField;
        
        private string requestIdField;
        
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
        public string StatusDescription {
            get {
                return this.statusDescriptionField;
            }
            set {
                this.statusDescriptionField = value;
            }
        }
        
        /// <remarks/>
        public string TransactionId {
            get {
                return this.transactionIdField;
            }
            set {
                this.transactionIdField = value;
            }
        }
        
        /// <remarks/>
        public string RequestId {
            get {
                return this.requestIdField;
            }
            set {
                this.requestIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.6387")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://haiwai.model.sms.star.com")]
    public partial class ThirdPartnerRes {
        
        private string returnCodeField;
        
        private string returnMsgField;
        
        private string transactionlNoField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string returnCode {
            get {
                return this.returnCodeField;
            }
            set {
                this.returnCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string returnMsg {
            get {
                return this.returnMsgField;
            }
            set {
                this.returnMsgField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string transactionlNo {
            get {
                return this.transactionlNoField;
            }
            set {
                this.transactionlNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.6387")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class StarTimesTransaction {
        
        private string bouquetField;
        
        private string utilityCodeField;
        
        private string feeSpecified1Field;
        
        private string custRefField;
        
        private string reversalField;
        
        private string tranIdToReverseField;
        
        private string tellerField;
        
        private string narrationField;
        
        private string custNameField;
        
        private string customerTelField;
        
        private string vendorTransactionRefField;
        
        private string transactionTypeField;
        
        private string vendorCodeField;
        
        private string passwordField;
        
        private string paymentDateField;
        
        private string transactionAmountField;
        
        /// <remarks/>
        public string Bouquet {
            get {
                return this.bouquetField;
            }
            set {
                this.bouquetField = value;
            }
        }
        
        /// <remarks/>
        public string UtilityCode {
            get {
                return this.utilityCodeField;
            }
            set {
                this.utilityCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FeeSpecified")]
        public string FeeSpecified1 {
            get {
                return this.feeSpecified1Field;
            }
            set {
                this.feeSpecified1Field = value;
            }
        }
        
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
        public string Reversal {
            get {
                return this.reversalField;
            }
            set {
                this.reversalField = value;
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
        public string Teller {
            get {
                return this.tellerField;
            }
            set {
                this.tellerField = value;
            }
        }
        
        /// <remarks/>
        public string Narration {
            get {
                return this.narrationField;
            }
            set {
                this.narrationField = value;
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
        public string CustomerTel {
            get {
                return this.customerTelField;
            }
            set {
                this.customerTelField = value;
            }
        }
        
        /// <remarks/>
        public string VendorTransactionRef {
            get {
                return this.vendorTransactionRefField;
            }
            set {
                this.vendorTransactionRefField = value;
            }
        }
        
        /// <remarks/>
        public string TransactionType {
            get {
                return this.transactionTypeField;
            }
            set {
                this.transactionTypeField = value;
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
        public string Password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
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
        public string TransactionAmount {
            get {
                return this.transactionAmountField;
            }
            set {
                this.transactionAmountField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.6387")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://model.service.sms.star.com")]
    public partial class BalanceInfo {
        
        private string tELDealIDField;
        
        private System.Nullable<double> balanceField;
        
        private bool balanceFieldSpecified;
        
        private System.Nullable<double> billAmountField;
        
        private bool billAmountFieldSpecified;
        
        private string customerCodeField;
        
        private string customerNameField;
        
        private System.Nullable<int> payTypeField;
        
        private bool payTypeFieldSpecified;
        
        private string returnCodeField;
        
        private string returnMsgField;
        
        private string smartCardCodeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string TELDealID {
            get {
                return this.tELDealIDField;
            }
            set {
                this.tELDealIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<double> balance {
            get {
                return this.balanceField;
            }
            set {
                this.balanceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool balanceSpecified {
            get {
                return this.balanceFieldSpecified;
            }
            set {
                this.balanceFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<double> billAmount {
            get {
                return this.billAmountField;
            }
            set {
                this.billAmountField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool billAmountSpecified {
            get {
                return this.billAmountFieldSpecified;
            }
            set {
                this.billAmountFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string customerCode {
            get {
                return this.customerCodeField;
            }
            set {
                this.customerCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string customerName {
            get {
                return this.customerNameField;
            }
            set {
                this.customerNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> payType {
            get {
                return this.payTypeField;
            }
            set {
                this.payTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool payTypeSpecified {
            get {
                return this.payTypeFieldSpecified;
            }
            set {
                this.payTypeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string returnCode {
            get {
                return this.returnCodeField;
            }
            set {
                this.returnCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string returnMsg {
            get {
                return this.returnMsgField;
            }
            set {
                this.returnMsgField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string smartCardCode {
            get {
                return this.smartCardCodeField;
            }
            set {
                this.smartCardCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.6387")]
    public delegate void GetStarTimesCustomerDetailsCompletedEventHandler(object sender, GetStarTimesCustomerDetailsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.6387")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetStarTimesCustomerDetailsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetStarTimesCustomerDetailsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public SubscriberQueryResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((SubscriberQueryResult)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.6387")]
    public delegate void GetCustomerDetailsCompletedEventHandler(object sender, GetCustomerDetailsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.6387")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetCustomerDetailsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetCustomerDetailsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public BalanceInfo Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((BalanceInfo)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.6387")]
    public delegate void ProcessRechargePaymentCompletedEventHandler(object sender, ProcessRechargePaymentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.6387")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ProcessRechargePaymentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ProcessRechargePaymentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ThirdPartnerRes Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ThirdPartnerRes)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.6387")]
    public delegate void ProcessStarTimesPaymentCompletedEventHandler(object sender, ProcessStarTimesPaymentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.6387")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ProcessStarTimesPaymentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ProcessStarTimesPaymentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public PaymentResponse Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((PaymentResponse)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591