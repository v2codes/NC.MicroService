using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace NC.MicroService.Gateway
{
    /// <summary>
    /// 忽略SSL证书校验
    /// 包含添加证书代码示例 ^_^
    /// </summary>
    public class IgnoreSSLValidateDelegatingHandler : DelegatingHandler
    {
        private readonly X509CertificateCollection _certificates = new X509CertificateCollection();

        public IgnoreSSLValidateDelegatingHandler()
        {
            // testing...
            //byte[] rawData = GetFileAsBytes("secure.local.pfx");
            //_certificates.Add(new X509Certificate2(rawData, "password-used-to-create-pfx"));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var inner = InnerHandler;
            while (inner is DelegatingHandler)
            {
                inner = ((DelegatingHandler)inner).InnerHandler;
            }
            // inner is HttpClientHandler
            if (inner is HttpClientHandler httpClientHandler)
            {
                //if (httpClientHandler.ClientCertificateOptions != ClientCertificateOption.Automatic)
                //{
                //    httpClientHandler.ClientCertificates.AddRange(_certificates);
                //    httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                //}
                if (httpClientHandler.ServerCertificateCustomValidationCallback == null)
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                    {
                        return true;
                    };
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
        //private static byte[] GetFileAsBytes(string filePath)
        //{
        //    if (!File.Exists(filePath))
        //        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
        //    if (!File.Exists(filePath))
        //        throw new ApplicationException("Path missing: " + filePath);

        //    using (FileStream f = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //    {
        //        int size = (int)f.Length;
        //        byte[] data = new byte[size];
        //        size = f.Read(data, 0, size);
        //        f.Close();
        //        return data;
        //    }
        //}
    }
}
