# Access

Create secrets in each ingress namespace (portal, mq, fileman):
  - Firstly, cd to a directory containing the cert files
  - Then add 1 secret per ingress namespace
    - `kubectl create secret tls NAMESPACE-tls-cert -n NAMESPACE --key=tls.key --cert=tls.crt`

Add hosts file entries as follows:
  - 127.0.0.1 portal.local.ne1410s.co.uk
  - 127.0.0.1 rabbit.local.ne1410s.co.uk
  - 127.0.0.1 fileman.local.ne1410s.co.uk

Apps are then accessible on:
  - https://portal.local.ne1410s.co.uk
  - https://rabbit.local.ne1410s.co.uk
  - https://fileman.local.ne1410s.co.uk