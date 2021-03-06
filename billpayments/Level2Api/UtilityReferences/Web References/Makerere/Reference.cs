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

namespace UtilityReferences.Makerere {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="EPaymentSoap", Namespace="http://MUKEPayment/")]
    public partial class EPayment : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetServerStatusOperationCompleted;
        
        private System.Threading.SendOrPostCallback QueryMukStudentOperationCompleted;
        
        private System.Threading.SendOrPostCallback PostMukPaymentOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public EPayment() {
            this.Url = global::UtilityReferences.Properties.Settings.Default.UtilityReferences_Makerere_EPayment;
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
        public event QueryMukStudentCompletedEventHandler QueryMukStudentCompleted;
        
        /// <remarks/>
        public event PostMukPaymentCompletedEventHandler PostMukPaymentCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://MUKEPayment/GetServerStatus", RequestNamespace="http://MUKEPayment/", ResponseNamespace="http://MUKEPayment/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
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
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://MUKEPayment/QueryMukStudent", RequestNamespace="http://MUKEPayment/", ResponseNamespace="http://MUKEPayment/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Student QueryMukStudent(string studentNumber, string vendorCode, string password) {
            object[] results = this.Invoke("QueryMukStudent", new object[] {
                        studentNumber,
                        vendorCode,
                        password});
            return ((Student)(results[0]));
        }
        
        /// <remarks/>
        public void QueryMukStudentAsync(string studentNumber, string vendorCode, string password) {
            this.QueryMukStudentAsync(studentNumber, vendorCode, password, null);
        }
        
        /// <remarks/>
        public void QueryMukStudentAsync(string studentNumber, string vendorCode, string password, object userState) {
            if ((this.QueryMukStudentOperationCompleted == null)) {
                this.QueryMukStudentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnQueryMukStudentOperationCompleted);
            }
            this.InvokeAsync("QueryMukStudent", new object[] {
                        studentNumber,
                        vendorCode,
                        password}, this.QueryMukStudentOperationCompleted, userState);
        }
        
        private void OnQueryMukStudentOperationCompleted(object arg) {
            if ((this.QueryMukStudentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.QueryMukStudentCompleted(this, new QueryMukStudentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://MUKEPayment/PostMukPayment", RequestNamespace="http://MUKEPayment/", ResponseNamespace="http://MUKEPayment/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response PostMukPayment(Transaction trans) {
            object[] results = this.Invoke("PostMukPayment", new object[] {
                        trans});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void PostMukPaymentAsync(Transaction trans) {
            this.PostMukPaymentAsync(trans, null);
        }
        
        /// <remarks/>
        public void PostMukPaymentAsync(Transaction trans, object userState) {
            if ((this.PostMukPaymentOperationCompleted == null)) {
                this.PostMukPaymentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPostMukPaymentOperationCompleted);
            }
            this.InvokeAsync("PostMukPayment", new object[] {
                        trans}, this.PostMukPaymentOperationCompleted, userState);
        }
        
        private void OnPostMukPaymentOperationCompleted(object arg) {
            if ((this.PostMukPaymentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.PostMukPaymentCompleted(this, new PostMukPaymentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://MUKEPayment/")]
    public partial class Student {
        
        private string studentNumberField;
        
        private string statusDescriptionField;
        
        private string statusCodeField;
        
        private string studentNameField;
        
        private string courseField;
        
        private string schoolField;
        
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
        public string School {
            get {
                return this.schoolField;
            }
            set {
                this.schoolField = value;
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://MUKEPayment/")]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://MUKEPayment/")]
    public partial class Transaction {
        
        private string semesterField;
        
        private string yearOfStudyField;
        
        private string offlineField;
        
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
        
        private string studentNumberField;
        
        private string studentNameField;
        
        private string courseField;
        
        private string schoolField;
        
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
        public string School {
            get {
                return this.schoolField;
            }
            set {
                this.schoolField = value;
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void GetServerStatusCompletedEventHandler(object sender, GetServerStatusCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void QueryMukStudentCompletedEventHandler(object sender, QueryMukStudentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class QueryMukStudentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal QueryMukStudentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void PostMukPaymentCompletedEventHandler(object sender, PostMukPaymentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PostMukPaymentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal PostMukPaymentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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