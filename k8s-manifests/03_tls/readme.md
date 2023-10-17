# k8s
Run, wait about 10s in between:
  - kubectl apply -f "<REPO>\k8s-manifests\03_tls\stage01"
  - kubectl apply -f "<REPO>\k8s-manifests\03_tls\stage02"
  
# 'Static' tls
This is the current method, and useful for local cluster scenarios
(Technically cert-manager and the cluster issuer are not needed for the static method).
This mode basically means tls certs have been obtained already, and need to be set (per namespace):

  - Firstly, cd to a directory containing the cert files
  - Then, for each namespace that needs ingress: (portal, mq, fileman)
    - `kubectl create secret tls ingress-tls-cert -n NAMESPACE --key=tls.key --cert=tls.crt`

Add hosts file entries as follows:
  - 127.0.0.1 portal.local.ne1410s.co.uk
  - 127.0.0.1 rabbit.local.ne1410s.co.uk
  - 127.0.0.1 fileman.local.ne1410s.co.uk

# 'Dynamic' tls-cert
In this mode, cert-manager resources will automatically obtain a free SSL cert from letsencrypt.
Secrets will be automatically provided based on the names specified in the Ingress resources.
To enable, do the following:
  - The cluster must be hosted and reachable from acme servers
  - Uncomment the cert-manager annotations on the Ingress resources

# Access
UIs accessible on:
  - https://portal.local.ne1410s.co.uk
  - https://rabbit.local.ne1410s.co.uk
