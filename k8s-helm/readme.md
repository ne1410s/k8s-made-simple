# Helm
OpenLens - set the following prometheus config:
monitoring/prometheus-kube-prometheus-prometheus:9090

## pre-reqs
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.1/cert-manager.crds.yaml
kubectl apply -f "~\k8s-made-simple\k8s-helm\01_pre\"
kubectl create secret tls ingress-tls-cert -n NAMESPACE --key=tls.key --cert=tls.crt

## helm charts
helm install -n cert-manager cert-manager oci://registry-1.docker.io/bitnamicharts/cert-manager
helm install -n ingress-nginx ingress-nginx oci://registry-1.docker.io/bitnamicharts/nginx-ingress-controller
helm install -n monitoring prometheus oci://registry-1.docker.io/bitnamicharts/kube-prometheus
helm install -n monitoring kube-state-metrics oci://registry-1.docker.io/bitnamicharts/kube-state-metrics
helm install -n monitoring loki --values ".\02_helm-values\loki-helm-values.yaml" grafana/loki
helm install -n monitoring grafana grafana/grafana
helm install -n monitoring opentelemetry-operator open-telemetry/opentelemetry-operator
helm install -n monitoring opentelemetry-collector open-telemetry/opentelemetry-collector --set mode=deployment
helm install -n monitoring jaeger oci://registry-1.docker.io/bitnamicharts/jaeger

## post-helm
kubectl apply -f "~\k8s-made-simple\k8s-helm\03_post\"
kubectl apply -f "~\k8s-made-simple\k8s-helm\04_apps\"



$ helm repo add open-telemetry https://open-telemetry.github.io/opentelemetry-helm-charts
