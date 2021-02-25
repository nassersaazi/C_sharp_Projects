﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4016
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 2.0.50727.4016.
// 
#pragma warning disable 1591

namespace ConsoleApplication1.CEMAS {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4016")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="EPaymentSoap", Namespace="http://cemasinterface/")]
    public partial class EPayment : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetServerStatusOperationCompleted;
        
        private System.Threading.SendOrPostCallback SendReconcilliationFileOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetTransactionDetailsOperationCompleted;
        
        private System.Threading.SendOrPostCallback ValidateStudentDetailsOperationCompleted;
        
        private System.Threading.SendOrPostCallback PostTransactionOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public EPayment() {
            this.Url = global::ConsoleApplication1.Properties.Settings.Default.ConsoleApplication1_CEMAS_EPayment;
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
        public event GetServerStatusCompletedEventHandler GetServerStatusCompleted;
        
        /// <remarks/>
        public event SendReconcilliationFileCompletedEventHandler SendReconcilliationFileCompleted;
        
        /// <remarks/>
        public event GetTransactionDetailsCompletedEventHandler GetTransactionDetailsCompleted;
        
        /// <remarks/>
        public event ValidateStudentDetailsCompletedEventHandler ValidateStudentDetailsCompleted;
        
        /// <remarks/>
        public event PostTransactionCompletedEventHandler PostTransactionCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cemasinterface/GetServerStatus", RequestNamespace="http://cemasinterface/", ResponseNamespace="http://cemasinterface/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
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
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cemasinterface/SendReconcilliationFile", RequestNamespace="http://cemasinterface/", ResponseNamespace="http://cemasinterface/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response SendReconcilliationFile(string base64String, string vendorCode, string Password) {
            object[] results = this.Invoke("SendReconcilliationFile", new object[] {
                        base64String,
                        vendorCode,
                        Password});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void SendReconcilliationFileAsync(string base64String, string vendorCode, string Password) {
            this.SendReconcilliationFileAsync(base64String, vendorCode, Password, null);
        }
        
        /// <remarks/>
        public void SendReconcilliationFileAsync(string base64String, string vendorCode, string Password, object userState) {
            if ((this.SendReconcilliationFileOperationCompleted == null)) {
                this.SendReconcilliationFileOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSendReconcilliationFileOperationCompleted);
            }
            this.InvokeAsync("SendReconcilliationFile", new object[] {
                        base64String,
                        vendorCode,
                        Password}, this.SendReconcilliationFileOperationCompleted, userState);
        }
        
        private void OnSendReconcilliationFileOperationCompleted(object arg) {
            if ((this.SendReconcilliationFileCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SendReconcilliationFileCompleted(this, new SendReconcilliationFileCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cemasinterface/GetTransactionDetails", RequestNamespace="http://cemasinterface/", ResponseNamespace="http://cemasinterface/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
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
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cemasinterface/ValidateStudentDetails", RequestNamespace="http://cemasinterface/", ResponseNamespace="http://cemasinterface/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Student ValidateStudentDetails(StudentRequest ValidateRequest) {
            object[] results = this.Invoke("ValidateStudentDetails", new object[] {
                        ValidateRequest});
            return ((Student)(results[0]));
        }
        
        /// <remarks/>
        public void ValidateStudentDetailsAsync(StudentRequest ValidateRequest) {
            this.ValidateStudentDetailsAsync(ValidateRequest, null);
        }
        
        /// <remarks/>
        public void ValidateStudentDetailsAsync(StudentRequest ValidateRequest, object userState) {
            if ((this.ValidateStudentDetailsOperationCompleted == null)) {
                this.ValidateStudentDetailsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnValidateStudentDetailsOperationCompleted);
            }
            this.InvokeAsync("ValidateStudentDetails", new object[] {
                        ValidateRequest}, this.ValidateStudentDetailsOperationCompleted, userState);
        }
        
        private void OnValidateStudentDetailsOperationCompleted(object arg) {
            if ((this.ValidateStudentDetailsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ValidateStudentDetailsCompleted(this, new ValidateStudentDetailsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cemasinterface/PostTransaction", RequestNamespace="http://cemasinterface/", ResponseNamespace="http://cemasinterface/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response PostTransaction(Transaction trans) {
            object[] results = this.Invoke("PostTransaction", new object[] {
                        trans});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void PostTransactionAsync(Transaction trans) {
            this.PostTransactionAsync(trans, null);
        }
        
        /// <remarks/>
        public void PostTransactionAsync(Transaction trans, object userState) {
            if ((this.PostTransactionOperationCompleted == null)) {
                this.PostTransactionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPostTransactionOperationCompleted);
            }
            this.InvokeAsync("PostTransaction", new object[] {
                        trans}, this.PostTransactionOperationCompleted, userState);
        }
        
        private void OnPostTransactionOperationCompleted(object arg) {
            if ((this.PostTransactionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.PostTransactionCompleted(this, new PostTransactionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4016")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://cemasinterface/")]
    public partial class Response {
        
        private string statusCodeField;
        
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4016")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://cemasinterface/")]
    public partial class Transaction {
        
        private string semesterField;
        
        private bool sentToTalismaField;
        
        private string currencyField;
        
        private string accountNumberField;
        
        private string chequeNumberField;
        
        private string yearOfStudyField;
        
        private string offlineField;
        
        private string digitalSignatureField;
        
        private string univCodeField;
        
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
        
        private string studentNumberField;
        
        private string studentNameField;
        
        private string courseField;
        
        private string institutionField;
        
        private string studentTelField;
        
        private string reversalField;
        
        /// <remarks/>
        public string Semester {
            get {
                return this.semesterField;
            }
            set {
                this.semesterField = value;
            }
        }
        
        /// <remarks/>
        public bool SentToTalisma {
            get {
                return this.sentToTalismaField;
            }
            set {
                this.sentToTalismaField = value;
            }
        }
        
        /// <remarks/>
        public string Currency {
            get {
                return this.currencyField;
            }
            set {
                this.currencyField = value;
            }
        }
        
        /// <remarks/>
        public string AccountNumber {
            get {
                return this.accountNumberField;
            }
            set {
                this.accountNumberField = value;
            }
        }
        
        /// <remarks/>
        public string ChequeNumber {
            get {
                return this.chequeNumberField;
            }
            set {
                this.chequeNumberField = value;
            }
        }
        
        /// <remarks/>
        public string YearOfStudy {
            get {
                return this.yearOfStudyField;
            }
            set {
                this.yearOfStudyField = value;
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
        public string UnivCode {
            get {
                return this.univCodeField;
            }
            set {
                this.univCodeField = value;
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
        public string StudentNumber {
            get {
                return this.studentNumberField;
            }
            set {
                this.studentNumberField = value;
            }
        }
        
        /// <remarks/>
        public string StudentName {
            get {
                return this.studentNameField;
            }
            set {
                this.studentNameField = value;
            }
        }
        
        /// <remarks/>
        public string Course {
            get {
                return this.courseField;
            }
            set {
                this.courseField = value;
            }
        }
        
        /// <remarks/>
        public string Institution {
            get {
                return this.institutionField;
            }
            set {
                this.institutionField = value;
            }
        }
        
        /// <remarks/>
        public string StudentTel {
            get {
                return this.studentTelField;
            }
            set {
                this.studentTelField = value;
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
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4016")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://cemasinterface/")]
    public partial class Student {
        
        private string studentNumberField;
        
        private string billRefNumberField;
        
        private string statusDescriptionField;
        
        private string statusCodeField;
        
        private string univCodeField;
        
        private string studentNameField;
        
        private string courseField;
        
        private string institutionField;
        
        private string phoneField;
        
        private string balanceField;
        
        /// <remarks/>
        public string StudentNumber {
            get {
                return this.studentNumberField;
            }
            set {
                this.studentNumberField = value;
            }
        }
        
        /// <remarks/>
        public string BillRefNumber {
            get {
                return this.billRefNumberField;
            }
            set {
                this.billRefNumberField = value;
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
        public string UnivCode {
            get {
                return this.univCodeField;
            }
            set {
                this.univCodeField = value;
            }
        }
        
        /// <remarks/>
        public string StudentName {
            get {
                return this.studentNameField;
            }
            set {
                this.studentNameField = value;
            }
        }
        
        /// <remarks/>
        public string Course {
            get {
                return this.courseField;
            }
            set {
                this.courseField = value;
            }
        }
        
        /// <remarks/>
        public string Institution {
            get {
                return this.institutionField;
            }
            set {
                this.institutionField = value;
            }
        }
        
        /// <remarks/>
        public string Phone {
            get {
                return this.phoneField;
            }
            set {
                this.phoneField = value;
            }
        }
        
        /// <remarks/>
        public string Balance {
            get {
                return this.balanceField;
            }
            set {
                this.balanceField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4016")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://cemasinterface/")]
    public partial class StudentRequest {
        
        private string vendorCodeField;
        
        private string passwordField;
        
        private string univCodeField;
        
        private string paymentTypeField;
        
        private string studentNumberField;
        
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
        public string UnivCode {
            get {
                return this.univCodeField;
            }
            set {
                this.univCodeField = value;
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
        public string StudentNumber {
            get {
                return this.studentNumberField;
            }
            set {
                this.studentNumberField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4016")]
    public delegate void GetServerStatusCompletedEventHandler(object sender, GetServerStatusCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4016")]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4016")]
    public delegate void SendReconcilliationFileCompletedEventHandler(object sender, SendReconcilliationFileCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4016")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SendReconcilliationFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SendReconcilliationFileCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4016")]
    public delegate void GetTransactionDetailsCompletedEventHandler(object sender, GetTransactionDetailsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4016")]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4016")]
    public delegate void ValidateStudentDetailsCompletedEventHandler(object sender, ValidateStudentDetailsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4016")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ValidateStudentDetailsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ValidateStudentDetailsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Student Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Student)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4016")]
    public delegate void PostTransactionCompletedEventHandler(object sender, PostTransactionCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4016")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PostTransactionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal PostTransactionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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