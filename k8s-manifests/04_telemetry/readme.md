# Pre-k8s (or anytime)
Add hosts file entries as follows:
  - 127.0.0.1 prometheus.local.ne1410s.co.uk
  - 127.0.0.1 grafana.local.ne1410s.co.uk
  - 127.0.0.1 rabbit.local.ne1410s.co.uk
  - 127.0.0.1 fileman.local.ne1410s.co.uk
  - 127.0.0.1 portal.local.ne1410s.co.uk

Apps are then accessible on:
  - https://prometheus.local.ne1410s.co.uk
  - https://grafana.local.ne1410s.co.uk
  - https://rabbit.local.ne1410s.co.uk
  - https://fileman.local.ne1410s.co.uk
  - https://portal.local.ne1410s.co.uk
  
In OpenLens, go to (Cluster) > Settings > Metrics and set:
  - PROMETHEUS: Prometheus Operator
  - PROMETHEUS SERVICE ADDRESS: monitoring/prometheus-clusterip:8080

With the above, the CPU and Memory dashboards should show up in OpenLens on the pods

Install helm and add grafana repo:
  - helm repo add grafana https://grafana.github.io/helm-charts
  - helm repo update

# Deploy k8s
Run the following in order. Wait 20s or so between each:
  - kubectl apply -f "<REPO>\k8s-manifests\04_telemetry\stage01"
  - kubectl apply -f "<REPO>\k8s-manifests\04_telemetry\stage02"
  - kubectl apply -f "<REPO>\k8s-manifests\04_telemetry\stage03"

# Post-k8s
The following changes require k8s app namespaces to be deployed.

## Apply ssl certificate payloads as secrets
  - cd to directory containing SSL cert files
  - add a secret for each ingress namespace
    - `kubectl create secret tls NAMESPACE-tls-cert -n NAMESPACE --key=tls.key --cert=tls.crt`
  - current ingress namespaces:
    - `portal | mq | fileman | monitoring`
*NB: prometheus and grafana are both configured on the monitoring namespace, so don't require separate secrets).*

## Configure grafana
First and foremost, make sure the prometheus connection is using the *correct* url, e.g.:
NB: This is Home > Connections > Data sources > prometheus > server url:
  - http://prometheus-clusterip.monitoring.svc:8080
  
Then, can import / create dashboards til your heart's content :dashboards
This is + icon, > Import dashboard

  - Starter dashboard id: 8588

## Loki and promtail
We're going to need helm for this, with the grafana repo as added above.
Then, install loki and promtail as follows:
  - helm install loki --namespace monitoring --values "<REPO>\k8s-manifests\04_telemetry\stage04_helm\loki-helm-values.yaml" grafana/loki
  - helm install promtail --namespace monitoring grafana/promtail