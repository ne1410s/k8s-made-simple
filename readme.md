# self-disposing image example
 docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management
 docker run -it --rm --name clamav -p 3310:3310 clamav/clamav:1.2

# services exposed internally as follows
http://SERVICENAME.NAMESPACE.svc.cluster.local

# extension for open lens to show pod logs, etc
@alebcay/openlens-node-pod-menu

# upsert everything directly under the manifest folder
kubectl apply -f ./manifest

# check everything in a given namspace
kubectl get all -n fileman

# push to docker hub!!
### build image - cd to Dockerfile, then:
docker build .
### then, find your image id:
docker images
### then apply the tag
docker tag 1eb0144ff168 ne1410s/filemanapi:0.0.1-alpha
### push the image by the tag:
docker push ne1410s/filemanapi:0.0.1-alpha

### ingress example
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml

### cert-manager example
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.1/cert-manager.yaml

### prometheus manifests here
https://github.com/techiescamp/kubernetes-prometheus

### otel operator
kubectl apply -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml

### ssl
# fully-automated ssls via certmanager is achievable for HOSTED cluster - as there's a http01 ACME challenge
# when doing the above, see the Ingress resources - where the hard-coded ssl cert secrets currently live :)
## but local dev, we can add host file entries as follows, with a previously issued cert 
127.0.0.1 portal.local.ne1410s.co.uk

### request size
ingress requests capped at about 3mb by default.
to change, use following in the Ingress > metadata > annotations block:
  nginx.ingress.kubernetes.io/proxy-body-size: "20m"

############# TODO

### OpenTel / Prometheus / Loki / Jaeger / Grafana 
### ServiceMesh
### ConfigMaps 
