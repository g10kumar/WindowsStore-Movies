﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17626
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.VisualStudio.ServiceReference.Platforms, version 11.0.50522.1
// 
namespace QuotesOfWisdom.ServiceReference1 {
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="QuoteFilter", Namespace="http://schemas.datacontract.org/2004/07/QuotesService")]
    public partial class QuoteFilter : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string authorField;
        
        private string categoryField;
        
        private string keywordField;
        
        private int pageField;
        
        private int quoteidField;
        
        private int quotesPerPageField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string author {
            get {
                return this.authorField;
            }
            set {
                if ((object.ReferenceEquals(this.authorField, value) != true)) {
                    this.authorField = value;
                    this.RaisePropertyChanged("author");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string category {
            get {
                return this.categoryField;
            }
            set {
                if ((object.ReferenceEquals(this.categoryField, value) != true)) {
                    this.categoryField = value;
                    this.RaisePropertyChanged("category");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string keyword {
            get {
                return this.keywordField;
            }
            set {
                if ((object.ReferenceEquals(this.keywordField, value) != true)) {
                    this.keywordField = value;
                    this.RaisePropertyChanged("keyword");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int page {
            get {
                return this.pageField;
            }
            set {
                if ((this.pageField.Equals(value) != true)) {
                    this.pageField = value;
                    this.RaisePropertyChanged("page");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int quoteid {
            get {
                return this.quoteidField;
            }
            set {
                if ((this.quoteidField.Equals(value) != true)) {
                    this.quoteidField = value;
                    this.RaisePropertyChanged("quoteid");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int quotesPerPage {
            get {
                return this.quotesPerPageField;
            }
            set {
                if ((this.quotesPerPageField.Equals(value) != true)) {
                    this.quotesPerPageField = value;
                    this.RaisePropertyChanged("quotesPerPage");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IQService")]
    public interface IQService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IQService/getQuotes", ReplyAction="http://tempuri.org/IQService/getQuotesResponse")]
        System.Threading.Tasks.Task<string> getQuotesAsync(QuotesOfWisdom.ServiceReference1.QuoteFilter qf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IQService/getQuotesWith100Characters", ReplyAction="http://tempuri.org/IQService/getQuotesWith100CharactersResponse")]
        System.Threading.Tasks.Task<string> getQuotesWith100CharactersAsync(QuotesOfWisdom.ServiceReference1.QuoteFilter qf);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IQServiceChannel : QuotesOfWisdom.ServiceReference1.IQService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class QServiceClient : System.ServiceModel.ClientBase<QuotesOfWisdom.ServiceReference1.IQService>, QuotesOfWisdom.ServiceReference1.IQService {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public QServiceClient() : 
                base(QServiceClient.GetDefaultBinding(), QServiceClient.GetDefaultEndpointAddress()) {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_IQService.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public QServiceClient(EndpointConfiguration endpointConfiguration) : 
                base(QServiceClient.GetBindingForEndpoint(endpointConfiguration), QServiceClient.GetEndpointAddress(endpointConfiguration)) {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public QServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(QServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress)) {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public QServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(QServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress) {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public QServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Threading.Tasks.Task<string> getQuotesAsync(QuotesOfWisdom.ServiceReference1.QuoteFilter qf) {
            return base.Channel.getQuotesAsync(qf);
        }
        
        public System.Threading.Tasks.Task<string> getQuotesWith100CharactersAsync(QuotesOfWisdom.ServiceReference1.QuoteFilter qf) {
            return base.Channel.getQuotesWith100CharactersAsync(qf);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync() {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync() {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration) {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IQService)) {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration) {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IQService)) {
                return new System.ServiceModel.EndpointAddress("http://apps.daksatech.com/quoteService/QService.svc");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding() {
            return QServiceClient.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_IQService);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress() {
            return QServiceClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_IQService);
        }
        
        public enum EndpointConfiguration {
            
            BasicHttpBinding_IQService,
        }
    }
}
