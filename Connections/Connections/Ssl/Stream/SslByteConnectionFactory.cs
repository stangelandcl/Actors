using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Security.Authentication;
using System.Security;
using System.Diagnostics;
using Cls.Extensions;


namespace Cls.Connections
{
    class SslByteConnectionFactory
    {
        public SslByteConnectionFactory(
            byte[] serverCertificate, SecureString serverPassword,
            byte[] clientCertificate, SecureString clientPassword)
            : this(
                new X509Certificate(serverCertificate, serverPassword),
                new X509Certificate(clientCertificate, clientPassword))
        { }
           
        public SslByteConnectionFactory(X509Certificate serverCertificate, X509Certificate clientCertificate)
        {
            this.clientCertificate = clientCertificate;
            this.serverCertificate = serverCertificate;
        }

        X509Certificate serverCertificate;
        X509Certificate clientCertificate;

        public event Action<Exception> Error;
        public event Action<SslByteConnection> Connected;

        class State
        {
            public TcpClient Client;
            public SslStream Stream;
        }

        public void New(TcpClient c, bool isOutBound)
        {
            var stream = new SslStream(c.GetStream());
            var remote = ((IPEndPoint)c.Client.RemoteEndPoint).Address.ToString();
            var certs = new X509CertificateCollection();
            var state = new State { Client = c, Stream = stream };
            if (isOutBound)
            {
                certs.Add(clientCertificate);
                stream.BeginAuthenticateAsClient(remote, certs, SslProtocols.Tls, false, EndAuthenticateAsClient, state);
            }
            else
            {
                certs.Add(serverCertificate);
                stream.BeginAuthenticateAsServer(serverCertificate, true, SslProtocols.Tls, false, EndAuthenticateAsServer, state);
            }           
        }

        void EndAuthenticateAsClient(IAsyncResult result)
        {
            try
            {
                var s = (State)result.AsyncState;
                s.Stream.EndAuthenticateAsClient(result);
            }
            catch (Exception ex)
            {
                Error.FireEventAsync(ex);
            }
        }

        void EndAuthenticateAsServer(IAsyncResult result)
        {
            try
            {
                var s = (State)result.AsyncState;
                s.Stream.EndAuthenticateAsServer(result);
                Connected.FireEventAsync(new SslByteConnection(s.Client, s.Stream));
            }
            catch (Exception ex)
            {
                Error.FireEventAsync(ex);
            }
        }

        /* From MSDN */
        static void DisplaySecurityLevel(SslStream stream)
        {
            Trace.WriteLine(string.Format("Cipher: {0} strength {1}", stream.CipherAlgorithm, stream.CipherStrength));
            Trace.WriteLine(string.Format("Hash: {0} strength {1}", stream.HashAlgorithm, stream.HashStrength));
            Trace.WriteLine(string.Format("Key exchange: {0} strength {1}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength));
            Trace.WriteLine(string.Format("Protocol: {0}", stream.SslProtocol));
        }
        static void DisplaySecurityServices(SslStream stream)
        {
            Trace.WriteLine(string.Format("Is authenticated: {0} as server? {1}", stream.IsAuthenticated, stream.IsServer));
            Trace.WriteLine(string.Format("IsSigned: {0}", stream.IsSigned));
            Trace.WriteLine(string.Format("Is Encrypted: {0}", stream.IsEncrypted));
        }
        static void DisplayStreamProperties(SslStream stream)
        {
            Trace.WriteLine(string.Format("Can read: {0}, write {1}", stream.CanRead, stream.CanWrite));
            Trace.WriteLine(string.Format("Can timeout: {0}", stream.CanTimeout));
        }
        static void DisplayCertificateInformation(SslStream stream)
        {
            Trace.WriteLine(string.Format("Certificate revocation list checked: {0}", stream.CheckCertRevocationStatus));

            X509Certificate localCertificate = stream.LocalCertificate;
            if (stream.LocalCertificate != null)
            {
                Trace.WriteLine(string.Format("Local cert was issued to {0} and is valid from {1} until {2}.",
                    localCertificate.Subject,
                    localCertificate.GetEffectiveDateString(),
                    localCertificate.GetExpirationDateString()));
            }
            else
            {
                Trace.WriteLine("Local certificate is null.");
            }
            // Display the properties of the client's certificate.
            X509Certificate remoteCertificate = stream.RemoteCertificate;
            if (stream.RemoteCertificate != null)
            {
                Trace.WriteLine(string.Format("Remote cert was issued to {0} and is valid from {1} until {2}.",
                    remoteCertificate.Subject,
                    remoteCertificate.GetEffectiveDateString(),
                    remoteCertificate.GetExpirationDateString()));
            }
            else
            {
                Trace.WriteLine("Remote certificate is null.");
            }
        }
    }
}
